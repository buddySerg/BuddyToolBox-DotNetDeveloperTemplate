namespace BTB.Features.SplashScreen;

public enum StartupStep
{
    StartingServices,
    LoadingSettings,
    UpdatingDatabase,
    ApplyingLanguage,
    ApplyingTheme,
    LoadingUsers,
    StartingApplication
}

public sealed record SplashStatus(StartupStep Step, double Progress);