namespace WebApi.Extensions;

public static class ProcessHelper
{
    private const string EfTool = "ef";
    private const string OpenApiTool = "GetDocument.Insider";
    private const string GenerationArgName = "codegen";

    public static bool IsRunningGeneration()
    {
        return Environment.GetCommandLineArgs()
            .Contains(GenerationArgName);
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