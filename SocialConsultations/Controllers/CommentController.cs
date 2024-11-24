using SocialConsultations.Filters;
using SocialConsultations.Helpers;
using SocialConsultations.Models;
using SocialConsultations.Services.FieldsValidationServices;
using SocialConsultations.Services.CommunityServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Dynamic;
using System.Security.Claims;
using Marvin.Cache.Headers.Interfaces;
using Marvin.Cache.Headers;
using SocialConsultations.Entities;
using System.Linq.Expressions;
using SocialConsultations.Services.IssueServices;
using SocialConsultations.Services.CommentServices;

namespace SocialConsultations.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IIssueService _issueService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IFieldsValidationService _fieldsValidationService;
        private readonly IValidatorValueInvalidator _validatorValueInvalidator;
        private readonly IStoreKeyAccessor _storeKeyAccessor;
        private readonly ICommunityService _communityService;
        private readonly ICommentService _commentService;
        public CommentController(ICommunityService communityService, ProblemDetailsFactory problemDetailsFactory,
            IFieldsValidationService fieldsValidationService, IValidatorValueInvalidator validatorValueInvalidator, IStoreKeyAccessor storeKeyAccessor, IIssueService issueService, ICommentService commentService)
        {
            _communityService = communityService;
            _issueService = issueService;
            _problemDetailsFactory = problemDetailsFactory;
            _fieldsValidationService = fieldsValidationService;
            _validatorValueInvalidator = validatorValueInvalidator;
            _storeKeyAccessor = storeKeyAccessor;
            _commentService=commentService;
        }

        [HttpPost("upvotes/{commentid}")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult> ToggleUpvoteComment(int commentId)
        {
            try
            {
                
                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
                var commentToReturn = await _commentService.ToggleUpvoteComment(commentId, userId);
                var keys = _storeKeyAccessor.FindByKeyPart("api/comments").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                return Ok(commentToReturn);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult<CommentDto>> CreateComment(CommentForCreationDto comment)
        {
            try
            {
                var issue = await _issueService.GetByIdAsync(comment.IssueId);
                Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
                var isAllowed = await _communityService.ValidateMember(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), issue.CommunityId);
                if (!isAllowed || int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value) != comment.AuthorId)
                {
                    return Unauthorized();
                }

                var result = await _commentService.CreateAsync(comment);

                var keys = _storeKeyAccessor.FindByKeyPart("api/issues").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                var keys2 = _storeKeyAccessor.FindByKeyPart("api/comments").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys2);
                return CreatedAtRoute("GetComment", new { commentid = result.Id }, result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Produces("application/json",
         "application/vnd.socialconsultations.hateoas+json",
         "application/vnd.socialconsultations.comment.full+json",
         "application/vnd.socialconsultations.comment.full.hateoas+json",
         "application/vnd.socialconsultations.comment.friendly+json",
         "application/vnd.socialconsultations.comment.friendly.hateoas+json")]
        [HttpGet(Name = "GetComments")]
        [HttpHead]
        public async Task<IActionResult> GetComments([FromQuery] IEnumerable<int>? ids, [FromQuery] ResourceParameters resourceParameters, int? issueId, [FromHeader(Name = "Accept")] string? mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Accept header media type value is not supported"));
            }
            if (!_fieldsValidationService.TypeHasProperties<CommentFullDto>(resourceParameters.Fields))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all provided data shaping fields exist" +
                    $" on the resource: {resourceParameters.Fields}"));
            }
            List<IFilter> filters = new List<IFilter>();

            if (ids is not null && ids.Count()>0)
            {
                filters.Add(new NumericFilter("Ids", ids));
            }

            if (issueId is not null)
            {
                filters.Add(new NumericFilter("IssueId", (int)issueId));
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<LinkDto> links = new List<LinkDto>();

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix
                .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            IEnumerable<ExpandoObject> shapedcomments = new List<ExpandoObject>();
            IEnumerable<IDictionary<string, object?>>? shapedCommentsToReturn = new List<IDictionary<string, object?>>();
            if (primaryMediaType == "vnd.socialconsultations.comment.full")
            {
                PagedList<CommentFullDto>? comments = null;
                try
                {
                    Expression<Func<Comment, object>>[] includeProperties = { a=> a.Upvotes};
                    comments = await _commentService.GetFullAllWithEagerLoadingAsync(filters, resourceParameters, includeProperties);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = comments.TotalCount,
                    pageSize = comments.PageSize,
                    currentPage = comments.CurrentPage,
                    totalPages = comments.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                comments.HasNext,
                comments.HasPrevious
                );
                shapedcomments = comments.ShapeData(resourceParameters.Fields);
                int commentsIenumerator = 0;
                shapedCommentsToReturn = shapedcomments
                    .Select(comment =>
                    {
                        var commentAsDictionary = comment as IDictionary<string, object?>;

                        var commentLinks = CreateLinks(
                        comments[commentsIenumerator].Id,
                        resourceParameters.Fields);
                        commentsIenumerator++;
                        commentAsDictionary.Add("links", commentLinks);

                        return commentAsDictionary;
                    }).ToArray();
            }
            else
            {
                PagedList<CommentDto>? comments = null;
                try
                {
                    comments = await _commentService.GetAllAsync(filters, resourceParameters);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = comments.TotalCount,
                    pageSize = comments.PageSize,
                    currentPage = comments.CurrentPage,
                    totalPages = comments.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                comments.HasNext,
                comments.HasPrevious
                );
                shapedcomments = comments.ShapeData(resourceParameters.Fields);
                int commentsIenumerator = 0;
                shapedCommentsToReturn = shapedcomments
                    .Select(comment =>
                    {
                        var commentAsDictionary = comment as IDictionary<string, object?>;
                        if (includeLinks)
                        {
                            var commentLinks = CreateLinks(
                            comments[commentsIenumerator].Id,
                            resourceParameters.Fields);
                            commentAsDictionary.Add("links", commentLinks);
                        }
                        commentsIenumerator++;

                        return commentAsDictionary;
                    }).ToArray();

            }
            if (shapedCommentsToReturn.Count() == 0)
            {
                return NotFound();
            }

            if (includeLinks)
            {
                var linkedCollectionResource = new
                {
                    value = shapedCommentsToReturn,
                    links
                };
                return Ok(linkedCollectionResource);
            }
            else
            {
                var CollectionResource = new
                {
                    value = shapedCommentsToReturn
                };
                return Ok(CollectionResource);
            }

        }


        [Produces("application/json",
            "application/vnd.socialconsultations.hateoas+json",
            "application/vnd.socialconsultations.comment.full+json",
            "application/vnd.socialconsultations.comment.full.hateoas+json",
            "application/vnd.socialconsultations.comment.friendly+json",
            "application/vnd.socialconsultations.comment.friendly.hateoas+json")]
        [HttpGet("{commentid}", Name = "GetComment")]
        public async Task<IActionResult> GetComment(
            int commentid,
            string? fields,
            [FromHeader(Name = "Accept")] string? mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Accept header media type value is not supported"));
            }

            if (!_fieldsValidationService.TypeHasProperties<UserFullDto>(fields))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all provided data shaping fields exist" +
                    $" on the resource: {fields}"));
            }



            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinks(commentid, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(
                0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.socialconsultations.comment.full")
            {
                Expression<Func<Comment, object>>[] includeProperties = { c => c.Author, d => d.Upvotes };
                var fullItem = await _commentService.GetExtendedByIdWithEagerLoadingAsync(commentid, includeProperties);
                var fullResourceToReturn = fullItem.ShapeDataForObject(fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }
                return Ok(fullResourceToReturn);
            }
            var item = await _commentService.GetExtendedByIdWithEagerLoadingAsync(commentid);

            var lightResourceToReturn = item.ShapeDataForObject(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                lightResourceToReturn.Add("links", links);
            }
            return Ok(lightResourceToReturn);
        }

        private string? CreateCommentsResourceUri(
        ResourceParameters resourceParameters,
        List<IFilter> filters,
        ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetComments",
                     new
                     {
                         fields = resourceParameters.Fields,
                         pageNumber = resourceParameters.PageNumber - 1,
                         pageSize = resourceParameters.PageSize,
                         searchQuery = resourceParameters.SearchQuery,
                         orderBy = resourceParameters.OrderBy
                     });
                case ResourceUriType.NextPage:
                    return Url.Link("GetComments",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy
                    });
                default:
                    return Url.Link("GetComments",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy

                    });
            }

        }

        private IEnumerable<LinkDto> CreateLinks(
            int commentid,
            string? fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new(Url.Link("GetComment", new { commentid }),
                "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new(Url.Link("GetComment", new { commentid, fields }),
                    "self",
                    "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCollection(
            ResourceParameters resourceParameters,
            List<IFilter> filters,
            bool hasNext,
            bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(
            new(CreateCommentsResourceUri(
            resourceParameters,
            filters,
            ResourceUriType.Current),
            "self",
            "GET"));

            if (hasNext)
            {
                links.Add(
                    new(CreateCommentsResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.NextPage),
                        "nextPage",
                        "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new(CreateCommentsResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.PreviousPage),
                        "previousPage",
                        "GET"));
            }
            return links;
        }
    }
}
