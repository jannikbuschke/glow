Create the file `/app/Properties/launchSettings.json` with following content:

```json
  "profiles": {
    "app": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "MockExternalSystems": "true",
        "ClientSecret": "*****",
        "OpenIdConnect:ClientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
        "OpenIdConnect:TenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
        "OpenIdConnect:Instance": "https://login.microsoftonline.com/",
        "OpenIdConnect:BaseUrl": "https://localhost:5001",
        "OpenIdConnect:CallbackPath": "/signin-oidc"
      },
      "applicationUrl": "https://localhost:5001;http://localhost:5000"
    }
  }
}
```

ClientId, TenantId and ClientSecret are values from Azure. For this create an appregistration in the Azure portal.
