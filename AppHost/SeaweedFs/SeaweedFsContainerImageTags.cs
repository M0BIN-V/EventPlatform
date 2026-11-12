namespace AppHost.SeaweedFs;

internal static class SeaweedFsContainerImageTags
{
    internal const string Registry = "docker.io";
    internal const string Image = "chrislusf/seaweedfs";

    // TODO: pin to a specific tested version for reproducible builds.
    internal const string Tag = "latest";
}
