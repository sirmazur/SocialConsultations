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

namespace SocialConsultations.Controllers
{
    [ApiController]
    [Route("api/communities")]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _communityService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IFieldsValidationService _fieldsValidationService;
        private readonly IValidatorValueInvalidator _validatorValueInvalidator;
        private readonly IStoreKeyAccessor _storeKeyAccessor;
        public CommunityController(ICommunityService communityService, ProblemDetailsFactory problemDetailsFactory,
            IFieldsValidationService fieldsValidationService, IValidatorValueInvalidator validatorValueInvalidator, IStoreKeyAccessor storeKeyAccessor)
        {
            _communityService = communityService;
            _problemDetailsFactory = problemDetailsFactory;
            _fieldsValidationService = fieldsValidationService;
            _validatorValueInvalidator = validatorValueInvalidator;
            _storeKeyAccessor = storeKeyAccessor;
        }

        /// <summary>
        /// Gets community by id, requires admin token
        /// </summary>
        /// <param name="communityid"></param>
        /// <param name="fields"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        [Produces("application/json",
            "application/vnd.socialconsultations.hateoas+json",
            "application/vnd.socialconsultations.community.full+json",
            "application/vnd.socialconsultations.community.full.hateoas+json",
            "application/vnd.socialconsultations.community.friendly+json",
            "application/vnd.socialconsultations.community.friendly.hateoas+json")]
        [HttpGet("{communityid}", Name = "GetCommunity")]
        public async Task<IActionResult> GetCommunity(
            int communityid,
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

            if (!_fieldsValidationService.TypeHasProperties<CommunityFullDto>(fields))
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
                links = CreateLinks(communityid, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(
                0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.socialconsultations.community.full")
            {
                Expression<Func<Community, object>>[] includeProperties = { c => c.Avatar, d => d.Background, e => e.Administrators, f => f.Members, g => g.Issues, h => h.JoinRequests };
                var fullItem = await _communityService.GetExtendedByIdWithEagerLoadingAsync(communityid, includeProperties);
                var fullResourceToReturn = fullItem.ShapeDataForObject(fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }
                return Ok(fullResourceToReturn);
            }
            var item = await _communityService.GetExtendedByIdWithEagerLoadingAsync(communityid);

            var lightResourceToReturn = item.ShapeDataForObject(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                lightResourceToReturn.Add("links", links);
            }
            return Ok(lightResourceToReturn);
        }

        [HttpPost("{communityid}/joinrequests", Name = "PostJoinRequest")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<IActionResult> CreateJoinRequest(
            int communityid)
        {
            try
            {
                await _communityService.CreateJoinRequest(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), communityid);
            }
            catch
            (Exception e)
            {
                return BadRequest(e.Message);
            }
            var keys = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
            await _validatorValueInvalidator.MarkForInvalidation(keys);
            return NoContent();
        }

        [HttpPost("{communityid}/joinrequests/{joinrequestid}/accept", Name = "AcceptJoinRequest")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<IActionResult> AcceptJoinRequest(
            int communityid, int joinrequestid)
        {
            Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
            var community = await _communityService.GetEntityByIdWithEagerLoadingAsync(communityid, includeProperties);
            var adminIds = community.Administrators.Select(a => a.Id).ToList();

            if (!adminIds.Contains(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)))
            {
                return Unauthorized();
            }
            try
            {
                    await _communityService.AcceptJoinRequest(joinrequestid, communityid);
            }
            catch
            (Exception e)
            {
                return BadRequest(e.Message);
            }
            var keys = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
            await _validatorValueInvalidator.MarkForInvalidation(keys);
            return NoContent();
        }

        [HttpPost("{communityid}/joinrequests/{joinrequestid}/reject", Name = "RejectJoinRequest")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<IActionResult> RejectJoinRequest(
            int communityid, int joinrequestid)
        {
            Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
            var community = await _communityService.GetEntityByIdWithEagerLoadingAsync(communityid, includeProperties);
            var adminIds = community.Administrators.Select(a => a.Id).ToList();

            if (!adminIds.Contains(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)))
            {
                return Unauthorized();
            }

            try
            {
                await _communityService.RejectJoinRequest(joinrequestid, communityid);
            }
            catch
            (Exception e)
            {
                return BadRequest(e.Message);
            }
            var keys = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
            await _validatorValueInvalidator.MarkForInvalidation(keys);
            return NoContent();
        }

        [HttpPost("/closest/{amount}", Name = "GetClosest")]
        public async Task<IActionResult> GetClosestCommunities(Location location, int amount)
        {
            var communities = await _communityService.GetClosestCommunities(location, amount);
            return Ok(communities);
        }

        

        /// <summary>
        /// Gets communitys, requires admin token
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="resourceParameters"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        [Produces("application/json",
          "application/vnd.socialconsultations.hateoas+json",
          "application/vnd.socialconsultations.community.full+json",
          "application/vnd.socialconsultations.community.full.hateoas+json",
          "application/vnd.socialconsultations.community.friendly+json",
          "application/vnd.socialconsultations.community.friendly.hateoas+json")]
        [HttpGet(Name = "Getcommunities")]
        [HttpHead]
        public async Task<IActionResult> GetCommunities([FromQuery] IEnumerable<int>? ids, [FromQuery] ResourceParameters resourceParameters, [FromHeader(Name = "Accept")] string? mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Accept header media type value is not supported"));
            }
            if (!_fieldsValidationService.TypeHasProperties<CommunityFullDto>(resourceParameters.Fields))
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

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<LinkDto> links = new List<LinkDto>();

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix
                .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            IEnumerable<ExpandoObject> shapedcommunities = new List<ExpandoObject>();
            IEnumerable<IDictionary<string, object?>>? shapedcommunitiesToReturn = new List<IDictionary<string, object?>>();
            if (primaryMediaType == "vnd.socialconsultations.community.full")
            {
                PagedList<CommunityFullDto>? communities = null;
                try
                {
                    Expression<Func<Community, object>>[] includeProperties = { c => c.Avatar, d => d.Background, e => e.Administrators, f => f.Members, g => g.Issues, h => h.JoinRequests };
                    communities = await _communityService.GetFullAllWithEagerLoadingAsync(filters, resourceParameters, includeProperties);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = communities.TotalCount,
                    pageSize = communities.PageSize,
                    currentPage = communities.CurrentPage,
                    totalPages = communities.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                communities.HasNext,
                communities.HasPrevious
                );
                shapedcommunities = communities.ShapeData(resourceParameters.Fields);
                int communitiesIenumerator = 0;
                shapedcommunitiesToReturn = shapedcommunities
                    .Select(community =>
                    {
                        var communityAsDictionary = community as IDictionary<string, object?>;

                        var communityLinks = CreateLinks(
                        communities[communitiesIenumerator].Id,
                        resourceParameters.Fields);
                        communitiesIenumerator++;
                        communityAsDictionary.Add("links", communityLinks);

                        return communityAsDictionary;
                    }).ToArray();
            }
            else
            {
                PagedList<CommunityDto>? communities = null;
                try
                {
                    communities = await _communityService.GetAllAsync(filters, resourceParameters);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = communities.TotalCount,
                    pageSize = communities.PageSize,
                    currentPage = communities.CurrentPage,
                    totalPages = communities.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                communities.HasNext,
                communities.HasPrevious
                );
                shapedcommunities = communities.ShapeData(resourceParameters.Fields);
                int communitiesIenumerator = 0;
                shapedcommunitiesToReturn = shapedcommunities
                    .Select(community =>
                    {
                        var communityAsDictionary = community as IDictionary<string, object?>;
                        if (includeLinks)
                        {
                            var communityLinks = CreateLinks(
                            communities[communitiesIenumerator].Id,
                            resourceParameters.Fields);
                            communityAsDictionary.Add("links", communityLinks);
                        }
                        communitiesIenumerator++;

                        return communityAsDictionary;
                    }).ToArray();

            }
            if (shapedcommunitiesToReturn.Count() == 0)
            {
                return NotFound();
            }

            if (includeLinks)
            {
                var linkedCollectionResource = new
                {
                    value = shapedcommunitiesToReturn,
                    links
                };
                return Ok(linkedCollectionResource);
            }
            else
            {
                var CollectionResource = new
                {
                    value = shapedcommunitiesToReturn
                };
                return Ok(CollectionResource);
            }

        }

        private string? CreatecommunitysResourceUri(
          ResourceParameters resourceParameters,
          List<IFilter> filters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCommunities",
                     new
                     {
                         fields = resourceParameters.Fields,
                         pageNumber = resourceParameters.PageNumber - 1,
                         pageSize = resourceParameters.PageSize,
                         searchQuery = resourceParameters.SearchQuery,
                         orderBy = resourceParameters.OrderBy
                     });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCommunities",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy
                    });
                default:
                    return Url.Link("GetCommunities",
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
            int communityid,
            string? fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new(Url.Link("GetCommunity", new { communityid }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new(Url.Link("GetCommunity", new { communityid, fields }),
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
            new(CreatecommunitysResourceUri(
            resourceParameters,
            filters,
            ResourceUriType.Current),
            "self",
            "GET"));

            if (hasNext)
            {
                links.Add(
                    new(CreatecommunitysResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.NextPage),
                        "nextPage",
                        "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new(CreatecommunitysResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.PreviousPage),
                        "previousPage",
                        "GET"));
            }
            return links;
        }

        /// <summary>
        /// Creates a community
        /// </summary>
        /// <param name="community"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult<CommunityDto>> CreateCommunity(CommunityForClientCreationDto community)
        {
            try
            {
                var communityForCreation = await _communityService.GetCommunityForCreationDto(community, int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
                var result = await _communityService.CreateAsync(communityForCreation);

                var keys = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                return CreatedAtRoute("GetCommunity", new { communityid = result.Id }, result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Updates community by id, requires admin token
        /// </summary>
        /// <param name="toupdateid"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPut("{toupdateid}", Name = "UpdateCommunity")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<IActionResult> UpdateCommunity(int toupdateid, CommunityForUpdateDto item)
        {
            Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
            var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), toupdateid);
            if (!isAllowed)
            {
                return Unauthorized();
            }
            var operationResult = await _communityService.UpdateAsync(toupdateid, item);
            if (operationResult.IsSuccess)
            {
                var keys = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        /// <summary>
        /// Partially updates community by id, requires admin token
        /// </summary>
        /// <param name="toupdateid"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [Authorize(Policy = "MustBeLoggedIn")]
        [HttpPatch("{toupdateid}", Name = "PartialUpdateCommunity")]
        public async Task<IActionResult> PartialUpdateCommunity(int toupdateid, JsonPatchDocument<CommunityForUpdateDto> patchDocument)
        {
            var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), toupdateid);
            if (!isAllowed)
            {
                return Unauthorized();
            }
            var operationResult = await _communityService.PartialUpdateAsync(toupdateid, patchDocument);
            if (operationResult.IsSuccess)
            {
                var keys = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        /// <summary>
        /// Deletes user by id, requires admin token
        /// </summary>
        /// <param name="todeleteid"></param>
        /// <returns></returns>
        [HttpDelete("{todeleteid}", Name = "DeleteCommunity")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult> DeleteUser(int todeleteid)
        {
            var isAllowed = await _communityService.ValidateAdmin(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), todeleteid);
            if (!isAllowed)
            {
                return Unauthorized();
            }
            var operationResult = await _communityService.DeleteByIdAsync(todeleteid);
            if (operationResult.IsSuccess)
            {
                var keys = _storeKeyAccessor.FindByKeyPart("api/communities").ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [HttpOptions()]
        public IActionResult GetCommunitiesOptions()
        {
            Response.Headers.Add("Allow", "GET,HEAD,POST,PUT,PATCH,DELETE,OPTIONS");
            return Ok();
        }
    }
}
