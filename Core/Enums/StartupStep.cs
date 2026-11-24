namespace BTB.Core.Enums;

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