using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialConsultations.Entities;
using SocialConsultations.Filters;
using SocialConsultations.Helpers;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;
using System.Linq.Expressions;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SocialConsultations.Services.UserServices;

namespace SocialConsultations.Services.CommentServices
{
    public class CommentService : BasicService<CommentDto, Comment, CommentFullDto, CommentForCreationDto, CommentForUpdateDto>, ICommentService
    {
        private readonly IBasicRepository<Comment> _basicRepository;
        private readonly IUserService _userService;
        public CommentService(IBasicRepository<Comment> basicRepository, IMapper mapper, IUserService userService) : base(mapper, basicRepository)
        {
            _basicRepository = basicRepository;
            _userService = userService;
        }

        public async Task<CommentFullDto> ToggleUpvoteComment(int commentId, int userId)
        {
            var comment = await _basicRepository.GetQueryableAll().Include(c => c.Upvotes).FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment is null)
            {
                throw new Exception("Comment not found");
            }
            var user = await _userService.GetEntityByIdAsync(userId);
            if (user is null)
            {
                throw new Exception("User not found");
            }
            if (comment.Upvotes.Contains(user))
            {
                comment.Upvotes.Remove(user);
            }
            else
            {
                comment.Upvotes.Add(user);
            }
            await _basicRepository.SaveChangesAsync();
            return _mapper.Map<CommentFullDto>(comment);
        }

        public async override Task<PagedList<CommentFullDto>> GetFullAllWithEagerLoadingAsync(IEnumerable<IFilter>? filters,
           ResourceParameters parameters,
           params Expression<Func<Comment,
               object>>[] includeProperties)
        {
            var listToReturn = _basicRepository.GetQueryableAllWithEagerLoadingAsync(includeProperties).Include(c => c.Author).ThenInclude(d => d.Avatar).AsQueryable();
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

            var finalList = await PagedList<Comment>
                .CreateAsync(listToReturn,
                parameters.PageNumber,
                parameters.PageSize);
            var finalListToReturn = _mapper.Map<PagedList<CommentFullDto>>(finalList);
            
            foreach(var comment in finalListToReturn)
            {
                if(comment.Author is not null && comment.Author.Avatar is not null && comment.Author.Avatar.Data is not null)
                comment.Author.Avatar.Data = CompressAndDownscaleImage(comment.Author.Avatar.Data, 50, 75);
            }

            return finalListToReturn;
        }
        private static byte[] CompressAndDownscaleImage(byte[] imageBytes, double scalePercentage, long quality = 50L)
        {
            using (var inputStream = new MemoryStream(imageBytes))
            {
                using (var originalImage = Image.FromStream(inputStream))
                {
                    int targetWidth = (int)(originalImage.Width * scalePercentage / 100);
                    int targetHeight = (int)(originalImage.Height * scalePercentage / 100);

                    using (var resizedImage = new Bitmap(targetWidth, targetHeight))
                    {
                        using (var graphics = Graphics.FromImage(resizedImage))
                        {
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;

                            graphics.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);
                        }

                        using (var outputStream = new MemoryStream())
                        {
                            var encoder = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                            var encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                            resizedImage.Save(outputStream, encoder, encoderParams);

                            return outputStream.ToArray();
                        }
                    }
                }
            }
        }
    }
    

}
