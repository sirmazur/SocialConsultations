using AutoMapper;
using SocialConsultations.Controllers;
using SocialConsultations.Entities;
using SocialConsultations.Filters;
using SocialConsultations.Helpers;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SocialConsultations.Services.UserServices
{
    public class UserService : BasicService<UserDto, User, UserFullDto, UserForCreationDto, UserForUpdateDto>, IUserService
    {

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly EmailSender _emailSender;

        public UserService(IMapper mapper, IConfiguration configuration, IBasicRepository<User> basicRepository, IUserRepository userRepository, EmailSender emailSender) : base(mapper, basicRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _emailSender=emailSender;
        }
        public async Task<UserFullDto> AuthenticateUser(UserParams userParams)
        {
            var account = await _userRepository.GetUserByEmail(userParams.Email);
            if(account == null || account.Password!=userParams.Password)
            {
                throw new Exception("Wrong username or password");
            }
            else
            {
                return _mapper.Map<UserFullDto>(account);
            }

        }

        public async Task<UserFullDto> ActivateUser(Guid code)
        {
            var user = await _basicRepository.GetQueryableAll().SingleOrDefaultAsync(c => c.ConfirmationCode == code);
            if (user == null)
            {
                throw new Exception("Activation code is not vaild");
            }
            else
            if(user.Confirmed == true)
            {
                throw new Exception("Email is already confirmed");
            }
            else
            {
                user.Confirmed = true;
                await _basicRepository.SaveChangesAsync();
                return _mapper.Map<UserFullDto>(user);
            }
        }

        public async override Task<PagedList<UserFullDto>> GetFullAllWithEagerLoadingAsync(IEnumerable<IFilter>? filters,
            ResourceParameters parameters,
            params Expression<Func<User,
                object>>[] includeProperties)
        {
            var listToReturn = _basicRepository.GetQueryableAllWithEagerLoadingAsync(includeProperties);            
            foreach (var filter in filters)
            {
                if (filter.FieldName == "Ids")
                {
                    List<int> values = filter.Value as List<int>;
                    listToReturn = listToReturn.Where(c => values.Any(id => c.Id == id));
                }              
                else
                {
                        listToReturn = FilterEntity(listToReturn, filter);
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                listToReturn = SearchEntityByProperty(listToReturn, parameters.SearchQuery);
            }

            listToReturn = ApplyOrdering(listToReturn, parameters.OrderBy);

            var finalList = await PagedList<User>
                .CreateAsync(listToReturn,
                parameters.PageNumber,
                parameters.PageSize);
            var finalListToReturn = _mapper.Map<PagedList<UserFullDto>>(finalList);
            return finalListToReturn;
        }

        public async override Task<PagedList<UserDto>> GetAllWithEagerLoadingAsync(IEnumerable<IFilter>? filters,
            ResourceParameters parameters,
            params Expression<Func<User,
                object>>[] includeProperties)
        {
            var listToReturn = _basicRepository.GetQueryableAllWithEagerLoadingAsync(includeProperties);

            foreach (var filter in filters)
            {
                if (filter.FieldName == "Ids")
                {
                    List<int> values = filter.Value as List<int>;
                    listToReturn = listToReturn.Where(c => values.Any(id => c.Id == id));
                }
                else
                if (filter.FieldName != "CategoryIds")
                    listToReturn = FilterEntity(listToReturn, filter);
            }

            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                listToReturn = SearchEntityByProperty(listToReturn, parameters.SearchQuery);
            }

            listToReturn = ApplyOrdering(listToReturn, parameters.OrderBy);

            var finalList = await PagedList<User>
                .CreateAsync(listToReturn,
                parameters.PageNumber,
                parameters.PageSize);
            var finalListToReturn = _mapper.Map<PagedList<UserDto>>(finalList);
            return finalListToReturn;
        }

        public string GenerateToken(UserFullDto user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
        
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("username", user.Email)
            };

            var token = new JwtSecurityToken(
                        issuer: _configuration["Authentication:Issuer"],
                        audience: _configuration["Authentication:Audience"],
                        claims: claimsForToken,
                        expires: DateTime.Now.AddDays(30),
                        signingCredentials: credentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return tokenToReturn;
       
        }


        public async Task<UserDto> CreateUser(UserForClientCreation user)
        {
            var nameAvailable = await _userRepository.IsEmailAvailable(user.Email);
            if(!nameAvailable)
            {
                throw new Exception("Email already taken");
            }

            var userToCreate = new UserForCreationDto
            {
                Name = user.Name,
                Surname = user.Surname,
                Password = user.Password,
                Email = user.Email,
            };

            var createdUser = _mapper.Map<User>(userToCreate);
            await _basicRepository.AddAsync(createdUser);
            await _basicRepository.SaveChangesAsync();
            var url = $"{_configuration["SiteData:FrontendUrl"]}/signup/email-confirmed?code={createdUser.ConfirmationCode}";
            await _emailSender.SendEmailAsync(user.Email, "Confirm your account",
                $"Please confirm your account by clicking <a href='{url}'>here</a>");

            return _mapper.Map<UserDto>(createdUser);
        }
    }
}
