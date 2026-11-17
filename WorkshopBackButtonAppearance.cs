#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using NativeBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;
using NativeButton = Microsoft.UI.Xaml.Controls.Button;
using NativeNavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
#endif

namespace HomeMaintenanceMAUI {
  // BEFORE: No custom Back-button appearance existed. Windows supplied a dark arrow
  // against the dark title bar, making the existing navigation control difficult to see.
  // AFTER: Give only that native button a gold surface and contrasting dark arrow, with
  // distinct hover/pressed fills. Navigation, dimensions, tooltip, and focus behavior stay
  // native; no task, authentication, application-startup, or team platform code is changed.
  /// <summary>Makes the existing Windows title-bar Back button easy to identify.</summary>
  internal static class WorkshopBackButtonAppearance {
    /// <summary>Styles the current window's Back control after its template is available.</summary>
    /// <param name="page">Our loaded Workshop page or a page receiving a theme update.</param>
    internal static void Apply(ContentPage page) {
#if WINDOWS
      if (page.Handler?.PlatformView is not FrameworkElement nativePage) {
        return;
      }

      DependencyObject? ancestor = nativePage;
      while (ancestor is not null && ancestor is not Microsoft.Maui.Platform.WindowRootView) {
        ancestor = VisualTreeHelper.GetParent(ancestor);
      }
      if (ancestor is not Microsoft.Maui.Platform.WindowRootView root ||
          root.NavigationViewControl is not NativeNavigationView navigation ||
          FindBackButton(navigation) is not NativeButton button) {
        return;
      }

      // These are Workshop's gold and ink colors. Keep both fixed so the arrow is also
      // readable when Windows' title-bar appearance differs from the app's chosen theme.
      button.Background = CreateBrush(240, 190, 69);
      button.Foreground = CreateBrush(25, 45, 53);
      // Keep a separate native brush per state, matching MAUI's independent foreground
      // resource updates during theme changes.
      button.Resources["NavigationViewButtonBackgroundPointerOver"] = CreateBrush(255, 211, 110);
      button.Resources["NavigationViewButtonForegroundPointerOver"] = CreateBrush(25, 45, 53);
      button.Resources["NavigationViewButtonBackgroundPressed"] = CreateBrush(217, 165, 38);
      button.Resources["NavigationViewButtonForegroundPressed"] = CreateBrush(25, 45, 53);
      button.Resources["NavigationViewButtonForegroundDisabled"] = CreateBrush(82, 103, 113);

      // WinUI's existing template reads the state brushes through ThemeResource. Refresh
      // that one control after inserting local resources, restoring its original theme.
      // No global resource or other navigation/button style is modified.
      ElementTheme previousTheme = button.RequestedTheme;
      button.RequestedTheme = button.ActualTheme == ElementTheme.Dark
          ? ElementTheme.Light : ElementTheme.Dark;
      button.RequestedTheme = previousTheme;
#endif
    }

#if WINDOWS
    /// <summary>Finds the title-bar button without entering nested navigation controls.</summary>
    private static NativeButton? FindBackButton(DependencyObject parent) {
      for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++) {
        DependencyObject child = VisualTreeHelper.GetChild(parent, index);
        if (child is NativeButton button && button.Name == "NavigationViewBackButton") {
          return button;
        }
        if (child is not NativeNavigationView && FindBackButton(child) is NativeButton found) {
          return found;
        }
      }
      return null;
    }

    /// <summary>Creates an opaque native brush for the Back button's local palette.</summary>
    private static NativeBrush CreateBrush(byte red, byte green, byte blue) {
      return new NativeBrush(Windows.UI.Color.FromArgb(255, red, green, blue));
    }
#endif
  }
}
