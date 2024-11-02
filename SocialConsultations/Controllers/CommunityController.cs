using Marvin.Cache.Headers.Interfaces;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SocialConsultations.Models;
using SocialConsultations.Services.CommunityServices;
using SocialConsultations.Services.FieldsValidationServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;
using SocialConsultations.Entities;

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
        /// Creates a user
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

                var keys = _storeKeyAccessor.FindByCurrentResourcePath().ToBlockingEnumerable();
                await _validatorValueInvalidator.MarkForInvalidation(keys);
                return CreatedAtRoute("GetCommunity", new { communityid = result.Id }, result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Updates user by id, requires admin token
        /// </summary>
        /// <param name="toupdateid"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPut("{toupdateid}", Name = "UpdateCommunity")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<IActionResult> UpdateUser(int toupdateid, CommunityForUpdateDto item)
        {
            Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
            var community = await _communityService.GetEntityByIdWithEagerLoadingAsync(toupdateid, includeProperties);
            var adminIds = community.Administrators.Select(a => a.Id).ToList();

            if (!adminIds.Contains(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)))
            {
                return Unauthorized();
            }
            var operationResult = await _communityService.UpdateAsync(toupdateid, item);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        /// <summary>
        /// Partially updates user by id, requires admin token
        /// </summary>
        /// <param name="toupdateid"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [Authorize(Policy = "MustBeLoggedIn")]
        [HttpPatch("{toupdateid}", Name = "PartialUpdateCommunity")]
        public async Task<IActionResult> PartialUpdateUser(int toupdateid, JsonPatchDocument<CommunityForUpdateDto> patchDocument)
        {
            Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators};
            var community = await _communityService.GetEntityByIdWithEagerLoadingAsync(toupdateid, includeProperties);
            var adminIds = community.Administrators.Select(a => a.Id).ToList();

            if (!adminIds.Contains(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)))
            {
                return Unauthorized();
            }
            var operationResult = await _communityService.PartialUpdateAsync(toupdateid, patchDocument);
            if (operationResult.IsSuccess)
            {
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
