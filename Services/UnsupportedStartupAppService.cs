namespace HomeMaintenanceMAUI.Services;

public sealed class UnsupportedStartupAppService : IStartupAppService
{
    public bool IsEnabled => false;

    public bool SetEnabled(bool enabled) => false;
}
