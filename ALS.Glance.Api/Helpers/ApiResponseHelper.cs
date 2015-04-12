using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using System.Web.OData.Extensions;
using ALS.Glance.Api.Properties;
using Microsoft.OData.Core;

namespace ALS.Glance.Api.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApiResponseHelper
    {
        public static SingleResult<T> ToSingleResult<T>(this T result)
        {
            return SingleResult.Create(new[] { result }.AsQueryable());
        }

        #region Not Implemented

        public static HttpResponseMessage NotImplemented(this HttpRequestMessage request)
        {
            return request.CreateErrorResponse(
                HttpStatusCode.NotImplemented,
                new ODataError
                {
                    ErrorCode = HttpStatusCode.NotImplemented.ErrorCode(),
                    Message = Resources.NotImplementedErrorMessage,
                });
        }

        #endregion

        #region Forbidden

        public static HttpResponseMessage Forbidden(this HttpRequestMessage request, string message)
        {
            return request.CreateErrorResponse(
                HttpStatusCode.Forbidden,
                new ODataError
                {
                    ErrorCode = HttpStatusCode.Forbidden.ErrorCode(),
                    Message = message,
                });
        }

        #endregion

        #region Bad Request

        public static HttpResponseMessage BadRequest(this HttpRequestMessage request)
        {
            return request.BadRequest(Resources.BadRequestErrorMessage);
        }

        public static HttpResponseMessage BadRequest(this HttpRequestMessage request, string message)
        {
            return request.CreateErrorResponse(
                HttpStatusCode.BadRequest,
                new ODataError
                {
                    ErrorCode = HttpStatusCode.BadRequest.ErrorCode(),
                    Message = message,
                });
        }

        /// <summary>
        /// Creates a bad request result (400)
        /// </summary>
        /// <param name="request">The bad request</param>
        /// <param name="message">The message to include in the result</param>
        /// <returns>The bad request result</returns>
        public static ResponseMessageResult CreateBadRequestResult(this HttpRequestMessage request, string message)
        {
            return request.CreateBadRequestResult(message, errors: null);
        }

        /// <summary>
        /// Creates a bad request result (400)
        /// </summary>
        /// <param name="request">The bad request</param>
        /// <param name="message">The message to include in the result</param>
        /// <param name="modelState">The model state with error information that will be included in the response</param>
        /// <returns>The bad request result</returns>
        public static IHttpActionResult CreateBadRequestResult(
            this HttpRequestMessage request, string message, ModelStateDictionary modelState)
        {
            if (modelState == null || modelState.IsValid)
                return request.CreateBadRequestResult(message);

            return request.CreateBadRequestResult(message, modelState.ToErrorsDictionary());

        }

        /// <summary>
        /// Creates a bad request result (400)
        /// </summary>
        /// <param name="request">The bad request</param>
        /// <param name="message">The message to include in the result</param>
        /// <param name="errors">Error information that will be included in the response</param>
        /// <returns>The bad request result</returns>
        public static ResponseMessageResult CreateBadRequestResult(
            this HttpRequestMessage request, string message, Dictionary<string, string[]> errors)
        {
            return request.CreateResponseResult(HttpStatusCode.BadRequest, message, errors);
        }

        #endregion

        #region Conflict

        public static HttpResponseMessage Conflict(this HttpRequestMessage request, string message)
        {
            return request.CreateErrorResponse(
                HttpStatusCode.Conflict,
                new ODataError
                {
                    ErrorCode = HttpStatusCode.Conflict.ErrorCode(),
                    Message = message,
                });
        }

        /// <summary>
        /// Creates a conflict result (409)
        /// </summary>
        /// <param name="request">The request that lead to the conflict</param>
        /// <param name="message">The message to include in the result</param>
        /// <returns>The conflict result</returns>
        public static ResponseMessageResult CreateConflictResponse(this HttpRequestMessage request, string message)
        {
            return request.CreateConflictResponse(message, errors: null);
        }

        /// <summary>
        /// Creates a conflict result (409)
        /// </summary>
        /// <param name="request">The request that lead to the conflict</param>
        /// <param name="message">The message to include in the result</param>
        /// <param name="modelState">The model state with error information that will be included in the response</param>
        /// <returns>The conflict result</returns>
        public static IHttpActionResult CreateConflictResponse(
            this HttpRequestMessage request, string message, ModelStateDictionary modelState)
        {
            if (modelState == null || modelState.IsValid)
                return request.CreateConflictResponse(message);

            return request.CreateConflictResponse(message, modelState.ToErrorsDictionary());

        }

        /// <summary>
        /// Creates a conflict result (409)
        /// </summary>
        /// <param name="request">The request that lead to the conflict</param>
        /// <param name="message">The message to include in the result</param>
        /// <param name="errors">Error information that will be included in the response</param>
        /// <returns>The conflict result</returns>
        public static ResponseMessageResult CreateConflictResponse(
            this HttpRequestMessage request, string message, Dictionary<string, string[]> errors)
        {
            return request.CreateResponseResult(HttpStatusCode.Conflict, message, errors);
        }

        #endregion

        #region Deleted

        public static ResponseMessageResult CreateDeletedResponse(
         this HttpRequestMessage request, string message)
        {
            return request.CreateResponseResult(HttpStatusCode.NoContent, message, null);
        }

        public static ResponseMessageResult CreateDeletedResponse(
            this HttpRequestMessage request, string message, Dictionary<string, string[]> errors)
        {
            return request.CreateResponseResult(HttpStatusCode.NoContent, message, errors);
        }

        #endregion

        #region Internal Server Error

        public static HttpResponseMessage InternalServerError(this HttpRequestMessage request, string message)
        {
            return request.CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                new ODataError
                {
                    ErrorCode = HttpStatusCode.InternalServerError.ErrorCode(),
                    Message = message,
                });
        }

        /// <summary>
        /// Creates an internat server error result (500)
        /// </summary>
        /// <param name="request">The request that lead to the conflict</param>
        /// <param name="message">The message to include in the result</param>
        /// <returns>The conflict result</returns>
        public static ResponseMessageResult CreateInternalServerErrorResponse(
            this HttpRequestMessage request, string message)
        {
            return request.CreateInternalServerErrorResponse(message, errors: null);
        }

        /// <summary>
        /// Creates an internat server error result (500)
        /// </summary>
        /// <param name="request">The request that lead to the conflict</param>
        /// <param name="message">The message to include in the result</param>
        /// <param name="modelState">The model state with error information that will be included in the response</param>
        /// <returns>The conflict result</returns>
        public static IHttpActionResult CreateInternalServerErrorResponse(
            this HttpRequestMessage request, string message, ModelStateDictionary modelState)
        {
            if (modelState == null || modelState.IsValid)
                return request.CreateInternalServerErrorResponse(message);

            return request.CreateInternalServerErrorResponse(message, modelState.ToErrorsDictionary());

        }

        /// <summary>
        /// Creates an internat server error result (500)
        /// </summary>
        /// <param name="request">The request that lead to the conflict</param>
        /// <param name="message">The message to include in the result</param>
        /// <param name="errors">Error information that will be included in the response</param>
        /// <returns>The conflict result</returns>
        public static ResponseMessageResult CreateInternalServerErrorResponse(
            this HttpRequestMessage request, string message, Dictionary<string, string[]> errors)
        {
            return request.CreateResponseResult(HttpStatusCode.InternalServerError, message, errors);
        }

        #endregion

        public static HttpResponseMessage CreateResponse(
            this HttpRequestMessage request, HttpStatusCode code, string message,
            IReadOnlyCollection<KeyValuePair<string, string[]>> errors)
        {
            var modelState =
                new
                {
                    Error =
                        new
                        {
                            ErrorCode = code.ErrorCode(),
                            Message = message,
                            ModelState = errors ?? new Dictionary<string, string[]>()
                        }
                };

            //var result = request.CreateResponse(code, modelState);
            var result =
                new HttpResponseMessage(code)
                {
                    Content = new ObjectContent(
                        modelState.GetType(), modelState, new JsonMediaTypeFormatter())
                };
            return result;
        }

        #region Private methods

        private static Dictionary<string, string[]> ToErrorsDictionary(this IEnumerable<KeyValuePair<string, ModelState>> modelState)
        {
            return modelState == null
                ? new Dictionary<string, string[]>(0)
                : modelState.ToDictionary(
                    ms => ms.Key,
                    ms =>
                        ms.Value.Errors
                            .Select(e => e.Exception != null ? e.Exception.Message : e.ErrorMessage)
                            .ToArray());
        }

        private static ResponseMessageResult CreateResponseResult(
            this HttpRequestMessage request, HttpStatusCode code, string message, IReadOnlyCollection<KeyValuePair<string, string[]>> errors)
        {
            var response = request.CreateResponse(code, message, errors);
            var result = new ResponseMessageResult(response);

            return result;
        }
        private static string ErrorCode(this HttpStatusCode code)
        {
            return Convert.ToString((int)code);
        }

        #endregion
    }
}