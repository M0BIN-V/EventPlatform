namespace WebApi.Extensions;

public static class AppDomainExtensions
{
    public static bool RunByDocumentInsider(this AppDomain  appDomain)
    {
        return appDomain.FriendlyName == "GetDocument.Insider";
    } 
}