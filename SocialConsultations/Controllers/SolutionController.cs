using Marvin.Cache.Headers.Interfaces;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SocialConsultations.Services.CommunityServices;
using SocialConsultations.Services.FieldsValidationServices;
using SocialConsultations.Services.IssueServices;
using SocialConsultations.Services.SolutionServices;
using Microsoft.AspNetCore.Authorization;
using SocialConsultations.Entities;
using SocialConsultations.Models;
using System.Linq.Expressions;
using System.Security.Claims;
using Org.BouncyCastle.Tsp;
using SocialConsultations.Filters;
using SocialConsultations.Helpers;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using System.Dynamic;
using Microsoft.Net.Http.Headers;

namespace SocialConsultations.Controllers
{
    [ApiController]
    [Route("api/solutions")]
    public class SolutionController : ControllerBase
    {
        private readonly IIssueService _issueService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IFieldsValidationService _fieldsValidationService;
        private readonly IValidatorValueInvalidator _validatorValueInvalidator;
        private readonly IStoreKeyAccessor _storeKeyAccessor;
        private readonly ICommunityService _communityService;
        private readonly ISolutionService _solutionService;
        public SolutionController(ICommunityService communityService, ProblemDetailsFactory problemDetailsFactory,
            IFieldsValidationService fieldsValidationService, IValidatorValueInvalidator validatorValueInvalidator, IStoreKeyAccessor storeKeyAccessor, IIssueService issueService, ISolutionService solutionService)
        {
            _communityService = communityService;
            _issueService = issueService;
            _problemDetailsFactory = problemDetailsFactory;
            _fieldsValidationService = fieldsValidationService;
            _validatorValueInvalidator = validatorValueInvalidator;
            _storeKeyAccessor = storeKeyAccessor;
            _solutionService = solutionService;
        }

        [Produces("application/json",
          "application/vnd.socialconsultations.hateoas+json",
          "application/vnd.socialconsultations.solution.full+json",
          "application/vnd.socialconsultations.solution.full.hateoas+json",
          "application/vnd.socialconsultations.solution.friendly+json",
          "application/vnd.socialconsultations.solution.friendly.hateoas+json")]
        [HttpGet(Name = "GetSolutions")]
        [HttpHead]
        public async Task<IActionResult> GetSolutions([FromQuery] IEnumerable<int>? ids, [FromQuery] ResourceParameters resourceParameters, int? issueId, [FromHeader(Name = "Accept")] string? mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Accept header media type value is not supported"));
            }
            if (!_fieldsValidationService.TypeHasProperties<SolutionFullDto>(resourceParameters.Fields))
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

            IEnumerable<ExpandoObject> shapedsolutions = new List<ExpandoObject>();
            IEnumerable<IDictionary<string, object?>>? shapedSolutionsToReturn = new List<IDictionary<string, object?>>();
            if (primaryMediaType == "vnd.socialconsultations.solution.full")
            {
                PagedList<SolutionFullDto>? solutions = null;
                try
                {
                    Expression<Func<Solution, object>>[] includeProperties = { c => c.Files, d => d.UserVotes};
                    solutions = await _solutionService.GetFullAllWithEagerLoadingAsync(filters, resourceParameters, includeProperties);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = solutions.TotalCount,
                    pageSize = solutions.PageSize,
                    currentPage = solutions.CurrentPage,
                    totalPages = solutions.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                solutions.HasNext,
                solutions.HasPrevious
                );
                shapedsolutions = solutions.ShapeData(resourceParameters.Fields);
                int solutionsIEnumerator = 0;
                shapedSolutionsToReturn = shapedsolutions
                    .Select(solution =>
                    {
                        var solutionAsDictionary = solution as IDictionary<string, object?>;

                        var solutionLinks = CreateLinks(
                        solutions[solutionsIEnumerator].Id,
                        resourceParameters.Fields);
                        solutionsIEnumerator++;
                        solutionAsDictionary.Add("links", solutionLinks);

                        return solutionAsDictionary;
                    }).ToArray();
            }
            else
            {
                PagedList<SolutionDto>? solutions = null;
                try
                {
                    solutions = await _solutionService.GetAllAsync(filters, resourceParameters);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = solutions.TotalCount,
                    pageSize = solutions.PageSize,
                    currentPage = solutions.CurrentPage,
                    totalPages = solutions.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                solutions.HasNext,
                solutions.HasPrevious
                );
                shapedsolutions = solutions.ShapeData(resourceParameters.Fields);
                int solutionsIEnumerator = 0;
                shapedSolutionsToReturn = shapedsolutions
                    .Select(solution =>
                    {
                        var solutionAsDictionary = solution as IDictionary<string, object?>;
                        if (includeLinks)
                        {
                            var solutionLinks = CreateLinks(
                            solutions[solutionsIEnumerator].Id,
                            resourceParameters.Fields);
                            solutionAsDictionary.Add("links", solutionLinks);
                        }
                        solutionsIEnumerator++;

                        return solutionAsDictionary;
                    }).ToArray();

            }
            if (shapedSolutionsToReturn.Count() == 0)
            {
                return NotFound();
            }

            if (includeLinks)
            {
                var linkedCollectionResource = new
                {
                    value = shapedSolutionsToReturn,
                    links
                };
                return Ok(linkedCollectionResource);
            }
            else
            {
                var CollectionResource = new
                {
                    value = shapedSolutionsToReturn
                };
                return Ok(CollectionResource);
            }

        }

        [HttpPost("{solutionid}/upvotes")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult> ToggleUpvoteSolution(int solutionId)
        {
            try
            {
                var solution = await _solutionService.GetByIdAsync(solutionId);
                var issue = await _issueService.GetByIdAsync(solution.IssueId);

                var isAllowed = await _communityService.ValidateMember(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), issue.CommunityId);
                if (!isAllowed)
                {
                    return Unauthorized();
                }

                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
                var solutionToReturn = await _solutionService.ToggleUpvoteSolution(solutionId, userId);
                return Ok(solutionToReturn);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Produces("application/json",
            "application/vnd.socialconsultations.hateoas+json",
            "application/vnd.socialconsultations.solution.full+json",
            "application/vnd.socialconsultations.solution.full.hateoas+json",
            "application/vnd.socialconsultations.solution.friendly+json",
            "application/vnd.socialconsultations.solution.friendly.hateoas+json")]
        [HttpGet("{solutionid}", Name = "GetSolution")]
        public async Task<IActionResult> GetSolution(
            int solutionid,
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

            if (!_fieldsValidationService.TypeHasProperties<IssueFullDto>(fields))
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
                links = CreateLinks(solutionid, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(
                0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.socialconsultations.solution.full")
            {
                Expression<Func<Solution, object>>[] includeProperties = { c => c.Files, d => d.UserVotes };
                var fullItem = await _solutionService.GetExtendedByIdWithEagerLoadingAsync(solutionid, includeProperties);
                var fullResourceToReturn = fullItem.ShapeDataForObject(fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }
                return Ok(fullResourceToReturn);
            }
            var item = await _solutionService.GetExtendedByIdWithEagerLoadingAsync(solutionid);

            var lightResourceToReturn = item.ShapeDataForObject(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                lightResourceToReturn.Add("links", links);
            }
            return Ok(lightResourceToReturn);
        }

        [HttpPost()]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult<SolutionDto>> CreateSolution(SolutionForCreationDto solution)
        {
            try
            {
                var issue = await _issueService.GetByIdAsync(solution.IssueId);
                Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
                var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), issue.CommunityId);
                if (!isAllowed)
                {
                    return Unauthorized();
                }
                if(issue.IssueStatus != IssueStatus.GatheringInformation)
                {
                    return BadRequest("Issue is not in GatheringInformation status");
                }
                var result = await _solutionService.CreateAsync(solution);

                var keys = _storeKeyAccessor.FindByKeyPart("api/solutions").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                return CreatedAtRoute("GetSolution", new { solutionid = result.Id }, result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{toupdateid}", Name = "UpdateSolution")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<IActionResult> UpdateSolution(int toupdateid, SolutionForUpdateDto item)
        {
            var solution = await _solutionService.GetByIdAsync(toupdateid);
            var issue = await _issueService.GetByIdAsync(solution.IssueId);
            Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
            var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), issue.CommunityId);
            if (!isAllowed)
            {
                return Unauthorized();
            }
            var operationResult = await _solutionService.UpdateAsync(toupdateid, item);
            if (operationResult.IsSuccess)
            {
                var keys = _storeKeyAccessor.FindByKeyPart("api/solutions").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [Authorize(Policy = "MustBeLoggedIn")]
        [HttpPatch("{toupdateid}", Name = "PartialUpdateSolution")]
        public async Task<IActionResult> PartialUpdateSolution(int toupdateid, JsonPatchDocument<SolutionForUpdateDto> patchDocument)
        {
            var solution = await _solutionService.GetByIdAsync(toupdateid);
            var issue = await _issueService.GetByIdAsync(solution.IssueId);
            Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
            var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), issue.CommunityId);
            if (!isAllowed)
            {
                return Unauthorized();
            }
            var operationResult = await _solutionService.PartialUpdateAsync(toupdateid, patchDocument);
            if (operationResult.IsSuccess)
            {
                var keys = _storeKeyAccessor.FindByKeyPart("api/solutions").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [HttpDelete("{todeleteid}", Name = "DeleteSolution")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult> DeleteSolution(int todeleteid)
        {
            var solution = await _solutionService.GetByIdAsync(todeleteid);
            var issue = await _issueService.GetByIdAsync(solution.IssueId);
            Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
            var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), issue.CommunityId);
            if (!isAllowed)
            {
                return Unauthorized();
            }
            var operationResult = await _solutionService.DeleteByIdAsync(todeleteid);
            if (operationResult.IsSuccess)
            {
                var keys = _storeKeyAccessor.FindByKeyPart("api/solutions").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        private IEnumerable<LinkDto> CreateLinks(
            int solutionid,
            string? fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new(Url.Link("GetSolution", new { solutionid }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new(Url.Link("GetSolution", new { solutionid, fields }),
                    "self",
                    "GET"));
            }

            return links;
        }

        private string? CreateSolutionsResourceUri(
          ResourceParameters resourceParameters,
          List<IFilter> filters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetSolutions",
                     new
                     {
                         fields = resourceParameters.Fields,
                         pageNumber = resourceParameters.PageNumber - 1,
                         pageSize = resourceParameters.PageSize,
                         searchQuery = resourceParameters.SearchQuery,
                         orderBy = resourceParameters.OrderBy
                     });
                case ResourceUriType.NextPage:
                    return Url.Link("GetSolutions",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy
                    });
                default:
                    return Url.Link("GetSolutions",
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

        private IEnumerable<LinkDto> CreateLinksForCollection(
            ResourceParameters resourceParameters,
            List<IFilter> filters,
            bool hasNext,
            bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(
            new(CreateSolutionsResourceUri(
            resourceParameters,
            filters,
            ResourceUriType.Current),
            "self",
            "GET"));

            if (hasNext)
            {
                links.Add(
                    new(CreateSolutionsResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.NextPage),
                        "nextPage",
                        "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new(CreateSolutionsResourceUri(
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
