namespace HomeMaintenanceMAUI {
  /// <summary>Keeps native controls readable on a page with the Workshop paper palette.</summary>
  public class WorkshopLightThemeBehavior : Behavior<ContentPage> {
    /// <summary>Applies the page theme when its native handler becomes available.</summary>
    protected override void OnAttachedTo(ContentPage page) {
      base.OnAttachedTo(page);
      page.HandlerChanged += OnPageHandlerChanged;
      ApplyTheme(page);
    }

    /// <summary>Releases the handler subscription when the behavior leaves the page.</summary>
    protected override void OnDetachingFrom(ContentPage page) {
      page.HandlerChanged -= OnPageHandlerChanged;
      base.OnDetachingFrom(page);
    }

    /// <summary>Reapplies contrast settings if the native page handler is recreated.</summary>
    private void OnPageHandlerChanged(object? sender, EventArgs e) {
      if (sender is ContentPage page) {
        ApplyTheme(page);
      }
    }

    /// <summary>Scopes Windows text, dropdown arrows, and calendar chrome to a light page.</summary>
    private static void ApplyTheme(ContentPage page) {
#if WINDOWS
      if (page.Handler?.PlatformView is Microsoft.UI.Xaml.FrameworkElement nativePage) {
        nativePage.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
      }
#endif
    }
  }
}
