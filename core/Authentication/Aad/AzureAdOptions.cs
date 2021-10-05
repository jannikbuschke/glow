namespace Glow.Authentication.Aad
{
    public class AzureAdOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Instance { get; set; } = "https://login.microsoftonline.com/";
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string CallbackPath { get; set; } = "/signin-oidc";
        public string BaseUrl { get; set; }

        public string[] ValidIssuers { get; set; }
        public string[] DefaultScopes { get; set; }
    }
}