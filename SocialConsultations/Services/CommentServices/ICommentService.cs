using SocialConsultations.Entities;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;

namespace SocialConsultations.Services.CommentServices
{
    public interface ICommentService : IBasicService<CommentDto, Comment, CommentFullDto, CommentForCreationDto, CommentForUpdateDto>
    {
        Task<CommentFullDto> ToggleUpvoteComment(int commentId, int userId);
    }
}
