using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.DataAgents.ALS.Glance.Api.Models;
using ALS.Glance.DataAgents.Core;
using ALS.Glance.DataAgents.Default;
using ALS.Glance.Models;
using ALS.Glance.Models.Core;
using Microsoft.OData.Client;
using Newtonsoft.Json.Converters;
using Authorization = ALS.Glance.DataAgents.ALS.Glance.Api.Models.Authorization;

namespace ALS.Glance.DataAgents
{

    public class WebApiODataContainer : Container, IDisposable
    {
        private static readonly ConcurrentDictionary<string, AuthorizationInfo> AuthorizationsBag =
            new ConcurrentDictionary<string, AuthorizationInfo>();

        private readonly string _applicationId;
        private readonly string _userName;
        private readonly string _password;

        private WebApiODataContainer(Uri serviceRoot, string applicationId, string userName, string password)
            : base(serviceRoot)
        {
            _applicationId = applicationId;
            _userName = userName;
            _password = password;

            SendingRequest2 += (o, requestEventArgs) =>
            {
                AuthorizationInfo authorizationInfo;
                if (AuthorizationsBag.TryGetValue(_userName, out authorizationInfo) &&
                    !requestEventArgs.RequestMessage.Url.ToString().Contains("Authorization"))
                {
                    requestEventArgs.RequestMessage.SetHeader(
                        "Authorization",
                        "Bearer " + authorizationInfo.Authorization.AccessToken);
                }
            };
        }

        public static WebApiODataContainer Using(string url, WebApiCredentials servicesWebApiCredentials)
        {
            return new WebApiODataContainer(
                 new Uri(url),
                 servicesWebApiCredentials.ApplicationId,
                 servicesWebApiCredentials.UserName,
                 servicesWebApiCredentials.Password) { IgnoreResourceNotFoundException = true };
        }

        public void Dispose()
        {

        }

        public async Task<TResult> ExecuteAuthenticated<TResult>(Func<WebApiODataContainer, TResult> action,
            CancellationToken cancellationToken)
        {
            return await ExecuteAuthenticated(action, cancellationToken, e => { });
        }

        public async Task<TResult> ExecuteAuthenticated<TResult>(Func<WebApiODataContainer, TResult> action,
            CancellationToken cancellationToken, Action<Exception> exceptionLogger)
        {
            try
            {
                await AuthenticateAsync(cancellationToken);

                return await Task<TResult>.Factory.StartNew(() => action(this), cancellationToken);
            }
            catch (Exception exception)
            {
                var ex = HandleException(exception, exceptionLogger);

                if (null != ex)
                {
                    throw ex;
                }
                return default(TResult);
            }
        }

        public async Task<T> GetAuthenticatedAsync<T>(Func<WebApiODataContainer, string> pathGetter, Action<Exception> exceptionLogger, CancellationToken cancellationToken)
        {
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters.Add((new StringEnumConverter { CamelCaseText = false }));

            return await Execute<T>(async client => await client.GetAsync(pathGetter(this), cancellationToken), exceptionLogger, cancellationToken);
        }

        public async Task<TResult> ExecuteAuthenticated<TResult>(Func<WebApiODataContainer, Task<TResult>> action,
            CancellationToken cancellationToken)
        {
            return await ExecuteAuthenticated(action, cancellationToken, e => { });
        }

        public async Task<TResult> ExecuteAuthenticated<TResult>(Func<WebApiODataContainer, Task<TResult>> action, CancellationToken cancellationToken, Action<Exception> exceptionLogger)
        {
            try
            {
                await AuthenticateAsync(cancellationToken);

                return await action(this);
            }
            catch (Exception exception)
            {
                var ex = HandleException(exception, exceptionLogger);

                if (null != ex)
                {
                    throw ex;
                }
                return default(TResult);
            }
        }

        public async Task ExecuteAuthenticated(Func<WebApiODataContainer, Task> action,
            CancellationToken cancellationToken)
        {
            await ExecuteAuthenticated(action, cancellationToken, e => { });
        }

        public async Task ExecuteAuthenticated(Func<WebApiODataContainer, Task> action, CancellationToken cancellationToken, Action<Exception> exceptionLogger)
        {
            try
            {
                await AuthenticateAsync(cancellationToken);

                await action(this);
            }
            catch (Exception exception)
            {
                var ex = HandleException(exception, exceptionLogger);

                if (null != ex)
                {
                    throw ex;
                }
            }
        }

        public Task<T> PostAuthenticatedAsync<T>(Func<WebApiODataContainer, string> pathGetter, T data, Action<Exception> exceptionLogger, CancellationToken cancellationToken)
        {
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters.Add((new StringEnumConverter { CamelCaseText = false }));
            return Execute<T>(client => client.PostAsync(pathGetter(this), data, formatter, cancellationToken), exceptionLogger, cancellationToken);
        }

        public Task PutAuthenticatedAsync<T>(Func<WebApiODataContainer, string> pathGetter, T data, Action<Exception> exceptionLogger, CancellationToken cancellationToken)
        {
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters.Add((new StringEnumConverter { CamelCaseText = false }));
            return Execute(client => client.PutAsync(pathGetter(this), data, formatter, cancellationToken), exceptionLogger, cancellationToken);
        }

        public Task PatchAuthenticatedAsync<T>(Func<WebApiODataContainer, string> pathGetter, T data, Action<Exception> exceptionLogger, CancellationToken cancellationToken)
        {
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters.Add((new StringEnumConverter { CamelCaseText = false }));
            return Execute(client => client.PatchAsJsonAsync(pathGetter(this), data, cancellationToken), exceptionLogger, cancellationToken);
        }


        public Task DeleteAuthenticatedAsync(Func<WebApiODataContainer, string> pathGetter, Action<Exception> exceptionLogger, CancellationToken cancellationToken)
        {
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters.Add((new StringEnumConverter { CamelCaseText = false }));
            return Execute(client => client.DeleteAsync(pathGetter(this), cancellationToken), exceptionLogger, cancellationToken);
        }

        #region HttpClientHelpers

        private async Task<TResult> Execute<TResult>(Func<HttpClient, Task<HttpResponseMessage>> funcToExecute, Action<Exception> exceptionLogger, CancellationToken cancellationToken)
        {
            var session = await AuthenticateAsync(cancellationToken);

            var client = new HttpClient();
            try
            {
                client.BaseAddress = BaseUri;
              //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.Authorization.AccessToken);

                var result = await funcToExecute(client);

                if (result.IsSuccessStatusCode)
                {
                    return await result.Content.ReadAsAsync<TResult>(cancellationToken);
                }

                ReturnError error = null;
                try
                {
                    error = await result.Content.ReadAsAsync<ReturnError>(cancellationToken);
                }
                catch
                {
                    if (null != exceptionLogger)
                    {
                        exceptionLogger(new Exception(string.Format("Failed to read \"{0}\" as ReturnError.", result.Content.ReadAsStringAsync().Result)));
                    }
                }

                var exception = HandleErrorCode(result.StatusCode, null, error != null && error.Error != null ? error.Error.ToString() : null);

                if (null != exceptionLogger)
                {
                    exceptionLogger(exception);
                }

                if (null != exception)
                {
                    throw exception;
                }
                else
                {
                    return default(TResult);
                }

            }
            finally
            {
                client.Dispose();
            }
        }

        private async Task Execute(Func<HttpClient, Task<HttpResponseMessage>> funcToExecute, Action<Exception> exceptionLogger, CancellationToken cancellationToken)
        {
            var session = await AuthenticateAsync(cancellationToken);

            var client = new HttpClient();
            try
            {
                client.BaseAddress = BaseUri;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.Authorization.AccessToken);

                var result = await funcToExecute(client);

                if (result.IsSuccessStatusCode)
                {
                    return;
                }

                ReturnError error = null;
                try
                {
                    error = await result.Content.ReadAsAsync<ReturnError>(cancellationToken);
                }
                catch
                {
                    if (null != exceptionLogger)
                    {
                        exceptionLogger(new Exception(string.Format("Failed to read \"{0}\" as ReturnError.", result.Content.ReadAsStringAsync().Result)));
                    }
                }

                var exception = HandleErrorCode(result.StatusCode, null, error != null ? error.Error.ToString() : null);

                if (null != exceptionLogger)
                {
                    exceptionLogger(exception);
                }

                if (null != exception)
                {
                    throw exception;
                }
            }
            finally
            {
                client.Dispose();
            }
        }

        #endregion

        #region ExceptionHandler

        private Exception HandleException(Exception exception, Action<Exception> exceptionLogger)
        {
            if (null != exceptionLogger)
            {
                exceptionLogger(exception);
            }
            var dataServiceQueryException = exception as DataServiceQueryException;
            if (dataServiceQueryException != null)
            {
                string errorMessage = null;
                if (null != dataServiceQueryException.InnerException)
                {
                    try
                    {
                        var error =
                            SerializationHelper.JsonDeserialize<ReturnError>(
                                dataServiceQueryException.InnerException.Message);
                        if (null != error)
                        {
                            errorMessage = error.Error.ToString();
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
                var errorCode = (HttpStatusCode)dataServiceQueryException.Response.StatusCode;
                return HandleErrorCode(errorCode, dataServiceQueryException, errorMessage);
            }

            return exception;
        }

        private static Exception HandleErrorCode(HttpStatusCode errorCode, Exception exception = null, string message = null)
        {
            switch (errorCode)
            {
                //TODO: resources and other error types
                case HttpStatusCode.Unauthorized:
                    return new Exception(message ?? "Unauthorized");
                case HttpStatusCode.Conflict:
                    return new Exception(message ?? "Conflict");
                case HttpStatusCode.BadRequest:
                    return new Exception(message ?? "BadRequest");
                case HttpStatusCode.ServiceUnavailable:
                    return new Exception("Serviço temporariamente indisponível. Por favor aguarde um momento.");
                case HttpStatusCode.NotFound:
                    return null;
                default:
                    return new Exception(errorCode.ToString(), exception);
            }
        }

        internal class ReturnError
        {
            public ErrorDetail Error { get; set; }

            internal class ErrorDetail
            {
                public string ErrorCode { get; set; }
                public string Message { get; set; }
                public IDictionary<string, string[]> ModelState { get; set; }

                public override string ToString()
                {
                    var builder = new StringBuilder();

                    builder.AppendLine(Message);

                    if (null != ModelState)
                    {
                        foreach (var state in ModelState)
                        {
                            builder.AppendLine(state.Key);
                            foreach (var message in state.Value)
                            {
                                builder.AppendLine(string.Format("\t{0}", message));
                            }
                        }
                    }
                    return builder.ToString();
                }
            }
        }

        #endregion

        #region Authentication

        private AuthorizationInfo Authenticate()
        {
            return null;
            return AuthorizationsBag.AddOrUpdate(_userName,
                s =>
                {
                    var authorizationRequest = new AuthorizationRequest
                    {
                        ApplicationId = _applicationId,
                        UserName = _userName,
                        Password = _password
                    };

                    AddToAuthorizationRequest(authorizationRequest);
                    SaveChanges();

                    return new AuthorizationInfo(authorizationRequest.Authorization)
                    {
                        ExpirationDate =
                            DateTimeOffset.Now.AddMinutes(authorizationRequest.Authorization.ExpiresIn - 1)
                    };
                },
                (s, info) =>
                {
                    try
                    {
                        if (info.ExpirationDate > DateTimeOffset.Now)
                            return info;
                        var authorizationRefresh = new AuthorizationRefresh
                        {
                            ApplicationId = _applicationId,
                            RefreshToken = info.Authorization.RefreshToken
                        };

                        AddToAuthorizationRefresh(authorizationRefresh);
                        SaveChanges();

                        return new AuthorizationInfo(authorizationRefresh.Authorization)
                        {
                            ExpirationDate =
                                DateTimeOffset.Now.AddMinutes(authorizationRefresh.Authorization.ExpiresIn - 1)
                        };
                    }
                    catch
                    {
                        info.ExpirationDate = DateTimeOffset.Now.AddMinutes(-1);
                        return info;
                    }
                });
        }

        private async Task<AuthorizationInfo> AuthenticateAsync(CancellationToken ct)
        {
            return await Task<AuthorizationInfo>.Factory.StartNew(Authenticate, ct);
        }

        private class AuthorizationInfo
        {
            private readonly Authorization _authorization;

            public AuthorizationInfo(Authorization authorization)
            {
                _authorization = authorization;

                ExpirationDate = DateTimeOffset.Now.AddMinutes(-1);
            }

            public Authorization Authorization
            {
                get { return _authorization; }
            }

            public DateTimeOffset ExpirationDate { get; set; }
        }

        #endregion
    }

    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string requestUri, T value, CancellationToken cancellationToken)
        {
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters.Add((new StringEnumConverter { CamelCaseText = false }));
            var content = new ObjectContent<T>(value, formatter);
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content };

            return client.SendAsync(request, cancellationToken);
        }
    }
}
