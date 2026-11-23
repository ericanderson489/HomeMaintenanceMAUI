using Microsoft.Win32;
using System.Diagnostics;
using HomeMaintenanceMAUI.Services;

namespace HomeMaintenanceMAUI.Platforms.Windows;

public sealed class WindowsStartupAppService : IStartupAppService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "HomeMaintenanceMAUI";

    public bool IsEnabled
    {
        get
        {
            try
            {
                using RegistryKey? runKey = Registry.CurrentUser.OpenSubKey(RunKeyPath);
                return runKey?.GetValue(AppName) is string;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool SetEnabled(bool enabled)
    {
        try
        {
            using RegistryKey runKey = Registry.CurrentUser.CreateSubKey(RunKeyPath);

            if (enabled)
            {
                string executablePath = Process.GetCurrentProcess().MainModule?.FileName
                    ?? throw new InvalidOperationException("The application executable path is unavailable.");
                runKey.SetValue(AppName, $"\"{executablePath}\"");
            }
            else
            {
                runKey.DeleteValue(AppName, throwOnMissingValue: false);
            }

            return IsEnabled == enabled;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
