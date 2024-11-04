using SocialConsultations.Models;
using SocialConsultations.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Interfaces;

namespace SocialConsultations.Controllers
{
    
    [ApiController]
    [Route("api/authentication")]
    [HttpCacheIgnore]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStoreKeyAccessor _storeKeyAccessor;
        private readonly IValidatorValueInvalidator _validatorValueInvalidator;
        public AuthenticationController(IUserService userService, IStoreKeyAccessor storeKeyAccessor, IValidatorValueInvalidator validatorValueInvalidator)
        {
            _userService = userService;
            _storeKeyAccessor=storeKeyAccessor;
            _validatorValueInvalidator=validatorValueInvalidator;
        }

        /// <summary>
        /// Gets a token for the user
        /// </summary>
        /// <param name="param">Username and password</param>
        /// <returns>Bearer token</returns>
        [HttpPost(Name = "Authenticate")]
        public async Task<ActionResult<string>> Authenticate(UserParams param)
        {
            UserFullDto result;
            try
            {
                result = await _userService.AuthenticateUser(param);
            }
            catch(Exception e)
            {
                return Unauthorized(e.Message);
            }
            var keys = _storeKeyAccessor.FindByKeyPart("api/users/self").ToBlockingEnumerable();
            await _validatorValueInvalidator.MarkForInvalidation(keys);
            var token = _userService.GenerateToken(result);
            return Ok(token);

        }
    }


    public class UserParams
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
