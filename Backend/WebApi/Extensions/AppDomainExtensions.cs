namespace WebApi.Extensions;

public static class AppDomainExtensions
{
    private const string EfTool = "ef";
    private const string OpenApiTool = "GetDocument.Insider";

    extension(AppDomain appDomain)
    {
        public bool IsOpenApiGeneration()
        {
            return appDomain.FriendlyName == OpenApiTool;
        }

        public bool IsRunningByEfTool()
        {
            return appDomain.FriendlyName == EfTool;
        }

        public bool IsDesignTimeProcess()
        {
            return appDomain.IsRunningByEfTool()
                   || appDomain.IsOpenApiGeneration();
        }
    }
}