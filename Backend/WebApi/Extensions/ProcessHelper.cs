using System.Reflection;

namespace WebApi.Extensions;

public static class ProcessHelper
{
    private const string EfTool = "ef";
    private const string OpenApiTool = "GetDocument.Insider";

    public static bool IsRunningGeneration()
    {
        return Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider" ||
               Environment.GetCommandLineArgs().Contains("codegen");
    }

    public static bool IsOpenApiGeneration()
    {
        return AppDomain.CurrentDomain.FriendlyName == OpenApiTool;
    }

    public static bool IsRunningByEfTool()
    {
        return AppDomain.CurrentDomain.FriendlyName == EfTool;
    }

    public static bool IsDesignTimeProcess()
    {
        return
            IsRunningGeneration() ||
            IsRunningByEfTool()
            || IsOpenApiGeneration();
    }
}