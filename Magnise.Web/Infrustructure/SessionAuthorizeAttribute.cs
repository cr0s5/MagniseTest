using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Magnise.Web.Infrustructure;

[AttributeUsage(AttributeTargets.Class)]
public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private const string SessionKey = "AccessToken";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;

        var sessionValue = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(sessionValue))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}