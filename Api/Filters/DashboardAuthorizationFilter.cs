using Hangfire.Dashboard;

namespace Api.Filters;

public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        //var httpContext = context.GetHttpContext();
        //return httpContext.User.Identity?.IsAuthenticated ?? false;
        return true;
    }
}
