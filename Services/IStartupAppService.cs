namespace HomeMaintenanceMAUI.Services;

public interface IStartupAppService
{
    bool IsEnabled { get; }

    bool SetEnabled(bool enabled);
}
