using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.OAuth;
using Newtonsoft.Json;

namespace Glow.Core.Authentication
{
    public static class PasswordFlow
    {
        public static async Task<AccessTokenResponse> GetMsAccessTokentWithUsernamePassword(
            this HttpClient httpClient,
            string clientSecret,
            string username,
            string password,
            string clientId,
            string tenantId,
            string scope = "User.Read"
        )
        {
            var grant_type = "password";

            var dict = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "scope", scope },
                { "grant_type", grant_type },
                { "client_secret", clientSecret },
                { "username", username },
                { "password", password },
                { "client_id", clientId },
            };

            HttpResponseMessage response = await httpClient.PostAsync($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token",
                new FormUrlEncodedContent(dict)
            );

            var responseMessage = await response.Content.ReadAsStringAsync();

            var accessTokenResponse = new AccessTokenResponse();

            JsonConvert.PopulateObject(responseMessage, accessTokenResponse);

            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(accessTokenResponse.access_token))
            {
                return accessTokenResponse;
            }
            else
            {
                throw new Exception("Error while getting access token: " + responseMessage);
            }
        }

        public class AccessTokenResponse
        {
            // ReSharper disable once InconsistentNaming
            public string token_type { get; set; }
            // ReSharper disable once InconsistentNaming
            public string scope { get; set; }
            // ReSharper disable once InconsistentNaming
            public int expires_in { get; set; }
            // ReSharper disable once InconsistentNaming
            public int ext_expires_in { get; set; }
            // ReSharper disable once InconsistentNaming
            public string access_token { get; set; }
            public string Message { get; set; }
        }
    }
}
