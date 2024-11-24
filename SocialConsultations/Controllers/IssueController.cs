using SocialConsultations.Filters;
using SocialConsultations.Helpers;
using SocialConsultations.Models;
using SocialConsultations.Services.FieldsValidationServices;
using SocialConsultations.Services.IssueServices;
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
using SocialConsultations.Services.CommunityServices;

namespace SocialConsultations.Controllers
{
    [ApiController]
    [Route("api/issues")]
    public class IssueController : ControllerBase
    {
        private readonly IIssueService _issueService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IFieldsValidationService _fieldsValidationService;
        private readonly IValidatorValueInvalidator _validatorValueInvalidator;
        private readonly IStoreKeyAccessor _storeKeyAccessor;
        private readonly ICommunityService _communityService;
        public IssueController(ICommunityService communityService, ProblemDetailsFactory problemDetailsFactory,
            IFieldsValidationService fieldsValidationService, IValidatorValueInvalidator validatorValueInvalidator, IStoreKeyAccessor storeKeyAccessor, IIssueService issueService)
        {
            _communityService = communityService;
            _issueService = issueService;
            _problemDetailsFactory = problemDetailsFactory;
            _fieldsValidationService = fieldsValidationService;
            _validatorValueInvalidator = validatorValueInvalidator;
            _storeKeyAccessor = storeKeyAccessor;
        }

        [HttpPost()]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult<IssueDto>> CreateIssue(IssueForCreationDto issue)
        {
            try
            {
                Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
                var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), issue.CommunityId);
                if (!isAllowed)
                {
                    return Unauthorized();
                }
                var result = await _issueService.CreateAsync(issue);

                var keys = _storeKeyAccessor.FindByKeyPart("api/issues").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                var keys2 = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys2);
                return CreatedAtRoute("GetIssue", new { issueid = result.Id }, result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Produces("application/json",
            "application/vnd.socialconsultations.hateoas+json",
            "application/vnd.socialconsultations.issue.full+json",
            "application/vnd.socialconsultations.issue.full.hateoas+json",
            "application/vnd.socialconsultations.issue.friendly+json",
            "application/vnd.socialconsultations.issue.friendly.hateoas+json")]
        [HttpGet("{issueid}", Name = "GetIssue")]
        public async Task<IActionResult> GetIssue(
            int issueid,
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
                links = CreateLinks(issueid, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(
                0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.socialconsultations.issue.full")
            {
                Expression<Func<Issue, object>>[] includeProperties = { c => c.Files, d => d.Solutions, e => e.Comments };
                var fullItem = await _issueService.GetExtendedByIdWithEagerLoadingAsync(issueid, includeProperties);
                var fullResourceToReturn = fullItem.ShapeDataForObject(fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }
                return Ok(fullResourceToReturn);
            }
            var item = await _issueService.GetExtendedByIdWithEagerLoadingAsync(issueid);

            var lightResourceToReturn = item.ShapeDataForObject(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                lightResourceToReturn.Add("links", links);
            }
            return Ok(lightResourceToReturn);
        }

        private string? CreateIssuesResourceUri(
          ResourceParameters resourceParameters,
          List<IFilter> filters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetIssues",
                     new
                     {
                         fields = resourceParameters.Fields,
                         pageNumber = resourceParameters.PageNumber - 1,
                         pageSize = resourceParameters.PageSize,
                         searchQuery = resourceParameters.SearchQuery,
                         orderBy = resourceParameters.OrderBy
                     });
                case ResourceUriType.NextPage:
                    return Url.Link("GetIssues",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy
                    });
                default:
                    return Url.Link("GetIssues",
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


        [Produces("application/json",
          "application/vnd.socialconsultations.hateoas+json",
          "application/vnd.socialconsultations.issue.full+json",
          "application/vnd.socialconsultations.issue.full.hateoas+json",
          "application/vnd.socialconsultations.isue.friendly+json",
          "application/vnd.socialconsultations.issue.friendly.hateoas+json")]
        [HttpGet(Name = "GetIssues")]
        [HttpHead]
        public async Task<IActionResult> GetIssues([FromQuery] IEnumerable<int>? ids, [FromQuery] ResourceParameters resourceParameters, int? communityId, [FromHeader(Name = "Accept")] string? mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Accept header media type value is not supported"));
            }
            if (!_fieldsValidationService.TypeHasProperties<IssueFullDto>(resourceParameters.Fields))
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

            if(communityId is not null)
            {
                filters.Add(new NumericFilter("CommunityId", (int)communityId));
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<LinkDto> links = new List<LinkDto>();

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix
                .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            IEnumerable<ExpandoObject> shapedissues = new List<ExpandoObject>();
            IEnumerable<IDictionary<string, object?>>? shapedissuesToReturn = new List<IDictionary<string, object?>>();
            if (primaryMediaType == "vnd.socialconsultations.issue.full")
            {
                PagedList<IssueFullDto>? issues = null;
                try
                {
                    Expression<Func<Issue, object>>[] includeProperties = { c => c.Files, d => d.Solutions, e => e.Comments, f => f.Community };
                    issues = await _issueService.GetFullAllWithEagerLoadingAsync(filters, resourceParameters, includeProperties);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = issues.TotalCount,
                    pageSize = issues.PageSize,
                    currentPage = issues.CurrentPage,
                    totalPages = issues.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                issues.HasNext,
                issues.HasPrevious
                );
                shapedissues = issues.ShapeData(resourceParameters.Fields);
                int issuesIenumerator = 0;
                shapedissuesToReturn = shapedissues
                    .Select(issue =>
                    {
                        var issueAsDictionary = issue as IDictionary<string, object?>;

                        var issueLinks = CreateLinks(
                        issues[issuesIenumerator].Id,
                        resourceParameters.Fields);
                        issuesIenumerator++;
                        issueAsDictionary.Add("links", issueLinks);

                        return issueAsDictionary;
                    }).ToArray();
            }
            else
            {
                PagedList<IssueDto>? issues = null;
                try
                {
                    issues = await _issueService.GetAllAsync(filters, resourceParameters);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = issues.TotalCount,
                    pageSize = issues.PageSize,
                    currentPage = issues.CurrentPage,
                    totalPages = issues.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                issues.HasNext,
                issues.HasPrevious
                );
                shapedissues = issues.ShapeData(resourceParameters.Fields);
                int issuesIenumerator = 0;
                shapedissuesToReturn = shapedissues
                    .Select(issue =>
                    {
                        var issueAsDictionary = issue as IDictionary<string, object?>;
                        if (includeLinks)
                        {
                            var issueLinks = CreateLinks(
                            issues[issuesIenumerator].Id,
                            resourceParameters.Fields);
                            issueAsDictionary.Add("links", issueLinks);
                        }
                        issuesIenumerator++;

                        return issueAsDictionary;
                    }).ToArray();

            }
            if (shapedissuesToReturn.Count() == 0)
            {
                return NotFound();
            }

            if (includeLinks)
            {
                var linkedCollectionResource = new
                {
                    value = shapedissuesToReturn,
                    links
                };
                return Ok(linkedCollectionResource);
            }
            else
            {
                var CollectionResource = new
                {
                    value = shapedissuesToReturn
                };
                return Ok(CollectionResource);
            }

        }

        [HttpPut("{toupdateid}", Name = "UpdateIssue")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<IActionResult> UpdateIssue(int toupdateid, IssueForUpdateDto item)
        {
            var issue = await _issueService.GetByIdAsync(toupdateid);
            var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), issue.CommunityId);
            if (!isAllowed)
            {
                return Unauthorized();
            }
            var operationResult = await _issueService.UpdateAsync(toupdateid, item);
            if (operationResult.IsSuccess)
            {
                var keys = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                var keys2 = _storeKeyAccessor.FindByKeyPart("api/issues").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys2);
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [Authorize(Policy = "MustBeLoggedIn")]
        [HttpPatch("{toupdateid}", Name = "PartialUpdateIssue")]
        public async Task<IActionResult> PartialUpdateIssue(int toupdateid, JsonPatchDocument<IssueForUpdateDto> patchDocument)
        {
            var issue = await _issueService.GetByIdAsync(toupdateid);
            var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), issue.CommunityId);
            if (!isAllowed)
            {
                return Unauthorized();
            }
            var operationResult = await _issueService.PartialUpdateAsync(toupdateid, patchDocument);
            if (operationResult.IsSuccess)
            {
                var keys = _storeKeyAccessor.FindByKeyPart("api/issues").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                var keys2 = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys2);
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [HttpDelete("{todeleteid}", Name = "DeleteIssue")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult> DeleteIssue(int todeleteid)
        {
            var issue = await _issueService.GetByIdAsync(todeleteid);
            var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), issue.CommunityId);
            if (!isAllowed)
            {
                return Unauthorized();
            }
            var operationResult = await _issueService.DeleteByIdAsync(todeleteid);
            if (operationResult.IsSuccess)
            {
                var keys = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                var keys2 = _storeKeyAccessor.FindByKeyPart("api/issues").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys2);
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        private IEnumerable<LinkDto> CreateLinks(
            int issueid,
            string? fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new(Url.Link("GetIssue", new { issueid }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new(Url.Link("GetIssue", new { issueid, fields }),
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
            new(CreateIssuesResourceUri(
            resourceParameters,
            filters,
            ResourceUriType.Current),
            "self",
            "GET"));

            if (hasNext)
            {
                links.Add(
                    new(CreateIssuesResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.NextPage),
                        "nextPage",
                        "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new(CreateIssuesResourceUri(
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
