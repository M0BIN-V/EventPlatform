using Presentation.Endpoints;

namespace Presentation;

public static class MapIdentityEndpointsExtension
{
    public static WebApplication MapIdentityEndpoints(this WebApplication app)
    {
        app.MapUsersEndpoints();

        return app;
    }
}