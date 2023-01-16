using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Glow.Glue.AspNetCore;

public static class AspNetcoreHttpRequestPipelineExtensions
{
    public const string HttpContextRequestItemName = "MediatR.Request";

    // Name of the action the user wants to execute. The value should be unique within the application among all possible actions users can execute
    // And should not change over time even if action name (for example class or namespace) changes
    public const string UserIntentItemName = "UserIntent";

    public static void AddRequestToItems(this HttpContext httpContext, object request)
    {
        httpContext.Items.TryAdd(HttpContextRequestItemName, request);
    }

    public static void AddUserIntentToItems(this HttpContext httpContext, string userIntent)
    {
        httpContext.Items.TryAdd(UserIntentItemName, userIntent);
    }

    public static object TryGetRequestFromItems(this HttpContext httpContext)
    {
        return httpContext.Items.TryGetValue(HttpContextRequestItemName, out var request) ? request : null;
    }

    public static string TryGetUserIntentFromItems(this HttpContext httpContext)
    {
        return httpContext.Items.TryGetValue(UserIntentItemName, out var userIntent) ? (string)userIntent : null;
    }
}
