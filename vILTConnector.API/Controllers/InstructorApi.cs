/*
 * VILT Connector
 *
 * Edge virtual Instructor Led Training (vILT) API 
 * 
 * OpenAPI spec version: 1.0.0
 * 
 * Modified from template Generated from https://app.swaggerhub.com/apis/csodedge/vILT-Connector/1.0.0 by: https://github.com/swagger-api/swagger-codegen.git */

using System;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Threading.Tasks;
using vILT.Domain;
using CornerstoneTeamsScheduler.Attributes;
using Microsoft.AspNetCore.Http;

namespace vILTConnector.API
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [EnableCors("CornerStoneOnlyOrigin")]
    [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
    public class InstructorApiController : ControllerBase
    {
        private readonly IViltSessionService _sessionService;
        /// <summary>
        /// Controller Constructor injecting Session Service
        /// </summary>
        /// <param name="sessionService">injected Session Service to handle meeting provider calls</param>
        public InstructorApiController(IViltSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        /// <summary>
        /// This endpoint and operation is used to add a user as a host or instructor in the virtual meeting provider&#39;s system.
        /// </summary>

        /// <param name="body"></param>
        /// <param name="correlationId">Unique identifier for each request sent by Cornerstone. Used for debugging purposes. You can include this value in the response body.</param>
        /// <param name="debug">Default value is &#39;false&#39;. Cornerstone will only set this parameter to &#39;true&#39; to aid in debugging any issues with the virtual meeting provider or partner.</param>
        /// <response code="200">Returns a response indicating whether the request was successful.</response>
        /// <response code="500">Returns a response indicating whether the error code and error description.</response>
        /// <remarks>This method is not particularly useful. For Microsoft Active Directory, the user will exist or not exist. We will not sue this method to add or update a new user.</remarks>

        [HttpPost]
        [Route("/instructor")]
        [ValidateModelState]
        [SwaggerOperation("AddInstructor")]
        [SwaggerResponse(statusCode: 200, type: typeof(ViltConnectorSuccessResponse), description: "Returns a response indicating whether the request was successful.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ViltConnectorErrorResponse), description: "Returns a response indicating whether the error code and error description.")]
        public async virtual Task<IActionResult> AddInstructor([FromBody] AddInstructorRequest body, [FromHeader] string correlationId, [FromHeader] bool? debug)
        {
            try
            {
                if (await _sessionService.AddInstructorAsync(body))
                {
                    return Ok(new ViltConnectorSuccessResponse { CorrelationId = correlationId, Status = "Success", Timestamp = DateTime.Now.ToString() });
                }

                return NotFound(default(ViltConnectorError));

            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.ToViltError());
            }
        }

        /// <summary>
        /// This endpoint and operation is used to update a user who is identified as a host or instructor in the virtual meeting provider&#39;s system.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="correlationId">Unique identifier for each request sent by Cornerstone. Used for debugging purposes. You can include this value in the response body.</param>
        /// <param name="debug">Default value is &#39;false&#39;. Cornerstone will only set this parameter to &#39;true&#39; to aid in debugging any issues with the virtual meeting provider or partner.</param>
        /// <response code="200">Returns a response indicating whether the request was successful.</response>
        /// <response code="500">Returns a response indicating whether the error code and error description.</response>
        /// <remarks>This method is not particularly useful. For Microsoft Active Directory, the user will exist or not exist. We will not sue this method to add or update a new user.</remarks>
        [HttpPut]
        [Route("/instructor")]
        [ValidateModelState]
        [SwaggerOperation("UpdateInstructor")]
        [SwaggerResponse(statusCode: 200, type: typeof(ViltConnectorSuccessResponse), description: "Returns a response indicating whether the request was successful.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ViltConnectorErrorResponse), description: "Returns a response indicating whether the error code and error description.")]
        public async virtual Task<IActionResult> UpdateInstructor([FromBody] UpdateInstructorRequest body, [FromHeader] string correlationId, [FromHeader] bool? debug)
        {
            try
            {
                if (await _sessionService.UpdateInstructorAsync(body))
                {
                    return Ok(new ViltConnectorSuccessResponse { CorrelationId = correlationId, Status = "Success", Timestamp = DateTime.Now.ToString() });
                }
                return NotFound();
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.ToViltError());
            }
        }
    }
}
