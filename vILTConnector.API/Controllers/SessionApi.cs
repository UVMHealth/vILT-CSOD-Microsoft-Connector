/*
 * VILT Connector
 *
 * Edge virtual Instructor Led Training (vILT) API 
 * 
 * OpenAPI spec version: 1.0.0
 * 
 * Modified from template Generated from https://app.swaggerhub.com/apis/csodedge/vILT-Connector/1.0.0 by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using vILT.Domain;
using System.Threading.Tasks;
using System.Linq;
using CornerstoneTeamsScheduler.Attributes;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace vILTConnector.API
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
    [ApiController]
    [EnableCors("CornerStoneOnlyOrigin")]
    public class SessionApiController : ControllerBase
    {
        private readonly IViltSessionService _sessionService;
        /// <summary>
        /// Controller constructor
        /// </summary>
        /// <param name="sessionService">injected Session Service to handle meeting provider calls</param>
        public SessionApiController(IViltSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        /// <summary>
        /// This endpoint and operation is used to create a new virtual meeting session in the provider&#39;s system.
        /// </summary>

        /// <param name="body"></param>
        /// <param name="correlationId">Unique identifier for each request sent by Cornerstone. Used for debugging purposes. You can include this value in the response body.</param>
        /// <param name="debug">Default value is &#39;false&#39;. Cornerstone will only set this parameter to &#39;true&#39; to aid in debugging any issues with the virtual meeting provider or partner.</param>
        /// <response code="200">Returns a response indicating whether the request was successful.</response>
        /// <response code="500">Returns a response indicating whether the error code and error description.</response>
        [HttpPost]
        [Route("/session")]
        [ValidateModelState]
        [SwaggerOperation("CreateSession")]
        [SwaggerResponse(statusCode: 200, type: typeof(ViltConnectorSuccessResponse), description: "Returns a response indicating whether the request was successful.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ViltConnectorErrorResponse), description: "Returns a response indicating whether the error code and error description.")]
        public async virtual Task<IActionResult> CreateSession([FromBody] SessionRequest body, [FromHeader] string correlationId, [FromHeader] bool? debug)
        {
            try
            {
                await _sessionService.CreateSessionAsync(body);
                return Ok(new ViltConnectorSuccessResponse { CorrelationId = correlationId, Status = "Success", Timestamp = DateTimeOffset.Now.ToString() });
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.ToViltError());
            }

        }

        /// <summary>
        /// This endpoint and operation is used to get the list of attendees for a virtual meeting session.
        /// </summary>

        /// <param name="sessionId">The SessionId originally sent in the Create Session request is sent as a URI parameter in the Update Session request.</param>
        /// <param name="debug">Default value is &#39;false&#39;. Cornerstone will only set this parameter to &#39;true&#39; to aid in debugging any issues with the virtual meeting provider or partner.</param>
        /// <response code="200">Returns a response with the list of attendees for the virtual meeting.</response>
        /// <response code="500">Returns a response indicating whether the error code and error description.</response>
        [HttpGet]
        [Route("/session/{SessionId}/attendees")]
        [ValidateModelState]
        [SwaggerOperation("GetAttendees")]
        [SwaggerResponse(statusCode: 200, type: typeof(ViltConnectorGetAttendanceSuccessResponse), description: "Returns a response with the list of attendees for the virtual meeting.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ViltConnectorErrorResponse), description: "Returns a response indicating whether the error code and error description.")]
        public async virtual Task<IActionResult> GetAttendees([FromRoute][Required] string sessionId, [FromHeader] bool? debug)
        {
            try
            {
                var session = await _sessionService.GetSessionAsync(sessionId);
                return Ok(new ViltConnectorGetAttendanceSuccessResponse
                {
                    Status = "Success",
                    Timestamp = DateTimeOffset.Now.ToString(),
                    Data =
                            new ViltConnectorGetAttendanceData
                            {
                                Attendees = session.Attendees.Select(
                                    a => new ViltConnectorGetAttendanceDataAttendees
                                    {
                                        Email = a.Email
                                    }
                                    ).ToList()
                            }
                }
                );
            }
            catch (SessionNotFoundException exc)
            {
                return NotFound(exc.ToViltError());
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.ToViltError());
            }
        }

        /// <summary>
        /// This endpoint and operation is used to get the virtual meeting&#39;s launch URL from the provider&#39;s system.
        /// </summary>

        /// <param name="sessionId">The SessionId originally sent in the Create Session request is sent as a URI parameter in the Update Session request.</param>
        /// <param name="base64EncodedEmail">The email address of the user requesting the launch URL is included in the URI as a base64 encoded value. If your system generates unique meeting launch URLs based on the user&#39;s role (instructor vs. attendee), on receiving the request from Cornerstone, you must decode the email to identify the user in your system.</param>
        /// <param name="debug">Default value is &#39;false&#39;. Cornerstone will only set this parameter to &#39;true&#39; to aid in debugging any issues with the virtual meeting provider or partner.</param>
        /// <response code="200">Returns a response with the launch URL for the virtual meeting.</response>
        /// <response code="500">Returns a response indicating whether the error code and error description.</response>
        [HttpGet]
        [Route("/session/{SessionId}/user/{base64EncodedEmail}/url")]
        [ValidateModelState]
        [SwaggerOperation("LaunchSession")]
        [SwaggerResponse(statusCode: 200, type: typeof(ViltConnectorLaunchSessionSuccessResponse), description: "Returns a response with the launch URL for the virtual meeting.")]
        [SwaggerResponse(statusCode: 0, type: typeof(ViltConnectorErrorResponse), description: "Returns a response indicating whether the error code and error description.")]
        public async virtual Task<IActionResult> LaunchSession([FromRoute][Required] string sessionId, [FromRoute][Required] string base64EncodedEmail, [FromHeader] bool? debug)
        {
            try
            {
                //Too much noise: await _sessionService.AddAttendeeAsync(sessionId, base64EncodedEmail.ToDecodedBase64String());
                var session = await _sessionService.GetSessionAsync(sessionId);
                
                return Ok(new ViltConnectorLaunchSessionSuccessResponse
                {
                    Timestamp = DateTimeOffset.Now.ToString(),
                    Status = "Success",
                    Data = new ViltConnectorLaunchSessionData
                    {
                        JoinUrl = session.JoinUrl
                    }
                });

            }
            catch (SessionNotFoundException exc)
            {
                return NotFound(exc.ToViltError());
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.ToViltError());
            }

        }

        /// <summary>
        /// This endpoint and operation is used to update an existing virtual meeting session in the provider&#39;s system.
        /// </summary>

        /// <param name="sessionId">The SessionId originally sent in the Create Session request is sent as a URI parameter in the Update Session request.</param>
        /// <param name="body"></param>
        /// <param name="correlationId">Unique identifier for each request sent by Cornerstone. Used for debugging purposes. You can include this value in the response body.</param>
        /// <param name="debug">Default value is &#39;false&#39;. Cornerstone will only set this parameter to &#39;true&#39; to aid in debugging any issues with the virtual meeting provider or partner.</param>
        /// <response code="200">Returns a response indicating whether the request was successful.</response>
        /// <response code="500">Returns a response indicating whether the error code and error description.</response>
        [HttpPut]
        [Route("/session/{SessionId}")]
        [ValidateModelState]
        [SwaggerOperation("UpdateSession")]
        [SwaggerResponse(statusCode: 200, type: typeof(ViltConnectorSuccessResponse), description: "Returns a response indicating whether the request was successful.")]
        [SwaggerResponse(statusCode: 0, type: typeof(ViltConnectorErrorResponse), description: "Returns a response indicating whether the error code and error description.")]
        public async virtual Task<IActionResult> UpdateSession([FromRoute][Required] string sessionId, [FromBody] SessionRequest body, [FromHeader] string correlationId, [FromHeader] bool? debug)
        {
            try
            {
                await _sessionService.UpdateSessionAsync(sessionId, body);
                return Ok(new ViltConnectorSuccessResponse { CorrelationId = correlationId, Status = "Success", Timestamp = DateTimeOffset.Now.ToString() });
            }
            catch (KeyNotFoundException exc)
            {
                return NotFound(exc.ToViltError());
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.ToViltError());
            }
        }
    }
}
