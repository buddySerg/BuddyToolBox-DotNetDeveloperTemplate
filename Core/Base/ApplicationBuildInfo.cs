namespace BTB.Core.Base;

/// <summary>
/// Contains build-specific information and enabled project features.
/// </summary>
public sealed record ApplicationBuildInfo
{
    public bool AuthenticationEnabled { get; } =
#if AUTHENTICATION
        true;
#else
        false;
#endif
}