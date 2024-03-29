using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glow.AzdoAuthentication
{
    /// <summary>
    /// Allows to get access tokens
    /// </summary>
    public class AzdoAuthenticationService
    {
        private static readonly Dictionary<Guid, AzdoToken> authorizationRequests = new Dictionary<Guid, AzdoToken>();
        private readonly AzdoConfig config;
        private readonly IHttpClientFactory clientFactory;

        public AzdoAuthenticationService(AzdoConfig config, IHttpClientFactory clientFactory,
            ActiveUsersCache tokenCache)
        {
            this.config = config;
            this.clientFactory = clientFactory;
        }

        public string CreateAuthorizationUrl()
        {
            var state = Guid.NewGuid();
            authorizationRequests[state] = new AzdoToken() { IsPending = true };

            var uriBuilder = new UriBuilder(config.AuthUrl);
            NameValueCollection queryParams = HttpUtility.ParseQueryString(uriBuilder.Query ?? string.Empty);

            queryParams["client_id"] = config.ClientId;
            queryParams["response_type"] = "Assertion";
            queryParams["state"] = state.ToString();
            queryParams["scope"] = config.Scope;
            queryParams["redirect_uri"] = config.RedirectUri;

            uriBuilder.Query = queryParams.ToString();

            return uriBuilder.ToString();
        }

        public async Task<AzdoToken> GetAccessToken(string code, Guid state)
        {
            if (ValidateCallbackValues(code, state.ToString(), out var error))
            {
                var form = new Dictionary<string, string>()
                {
                    {"client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"},
                    {"client_assertion", config.ClientSecret},
                    {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                    {"assertion", code},
                    {"redirect_uri", config.RedirectUri}
                };

                HttpClient httpClient = clientFactory.CreateClient();

                HttpResponseMessage responseMessage = await httpClient.PostAsync(
                    "https://app.vssps.visualstudio.com/oauth2/token" ?? config.TokenUrl,
                    new FormUrlEncodedContent(form)
                );
                if (responseMessage.IsSuccessStatusCode)
                {
                    var body = await responseMessage.Content.ReadAsStringAsync();

                    AzdoToken tokenModel = authorizationRequests[state];
                    JsonConvert.PopulateObject(body, tokenModel);

                    return tokenModel;
                }
                else
                {
                    error = responseMessage.ReasonPhrase;
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    throw new Exception(
                        $"{responseMessage.ReasonPhrase} {(string.IsNullOrEmpty(content) ? "" : $"({content})")}");
                }
            }

            throw new Exception(error);
        }

        private bool ValidateCallbackValues(string code, string state, out string error)
        {
            error = null;
            // TODO: test if code 'granted'
            if (string.IsNullOrEmpty(code))
            {
                error = "Invalid auth code";
            }
            else
            {
                if (!Guid.TryParse(state, out Guid authorizationRequestKey))
                {
                    error = "Invalid authorization request key";
                }
                else
                {
                    if (!authorizationRequests.TryGetValue(authorizationRequestKey, out AzdoToken tokenModel))
                    {
                        error = "Unknown authorization request key";
                    }
                    else if (!tokenModel.IsPending)
                    {
                        error = "Authorization request key already used";
                    }
                    else
                    {
                        authorizationRequests[authorizationRequestKey].IsPending =
                            false; // mark the state value as used so it can't be reused
                    }
                }
            }

            return error == null;
        }

        public async Task<AzdoToken> RefreshToken(string refreshToken)
        {
            string error = null;
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, config.TokenUrl);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var form = new Dictionary<string, string>()
                {
                    {"client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"},
                    {"client_assertion", config.ClientSecret},
                    {"grant_type", "refresh_token"},
                    {"assertion", refreshToken},
                    {"redirect_uri", config.RedirectUri}
                };
                requestMessage.Content = new FormUrlEncodedContent(form);

                HttpClient httpClient = clientFactory.CreateClient();
                HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var body = await responseMessage.Content.ReadAsStringAsync();
                    return JObject.Parse(body).ToObject<AzdoToken>();
                }
                else
                {
                    error = responseMessage.ReasonPhrase;
                    throw new Exception(responseMessage.ReasonPhrase);
                }
            }
            else
            {
                error = "Invalid refresh token";
            }

            throw new Exception(error);
        }
    }
}