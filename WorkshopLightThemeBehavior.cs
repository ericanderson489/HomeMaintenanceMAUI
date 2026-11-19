namespace HomeMaintenanceMAUI {
  /// <summary>Keeps native controls consistent with the user's Workshop appearance.</summary>
  public class WorkshopLightThemeBehavior : Behavior<ContentPage> {
    // BEFORE: No page field or appearance-change subscription existed in this behavior.
    // AFTER: Retain the attached page and use a weak theme event to refresh its native chrome.
    /// <summary>The attached page; cleared when the behavior is explicitly detached.</summary>
    private ContentPage? _page;

    /// <summary>Applies the page theme when its native handler becomes available.</summary>
    protected override void OnAttachedTo(ContentPage page) {
      // BEFORE:
      // base.OnAttachedTo(page);
      // page.HandlerChanged += OnPageHandlerChanged;
      // ApplyTheme(page);
      // AFTER: Keep those calls and initialize/observe the saved appearance before applying it.
      base.OnAttachedTo(page);
      _page = page;
      page.HandlerChanged += OnPageHandlerChanged;
      // BEFORE: Only HandlerChanged was observed; the window's Back template might not
      // exist at that point. AFTER: Loaded also applies our native Back-button contrast.
      page.Loaded += OnPageLoaded;
      WorkshopTheme.Initialize();
      WorkshopTheme.ThemeChanged += OnThemeChanged;
      ApplyTheme(page);
    }

    /// <summary>Releases the handler subscription when the behavior leaves the page.</summary>
    protected override void OnDetachingFrom(ContentPage page) {
      // BEFORE: Only the HandlerChanged subscription was removed before the base call.
      // AFTER: Release the new theme subscription and page reference as well.
      page.HandlerChanged -= OnPageHandlerChanged;
      // BEFORE: No Loaded subscription existed. AFTER: Release our new GUI observer too.
      page.Loaded -= OnPageLoaded;
      WorkshopTheme.ThemeChanged -= OnThemeChanged;
      _page = null;
      base.OnDetachingFrom(page);
    }

    /// <summary>Reapplies contrast settings if the native page handler is recreated.</summary>
    private void OnPageHandlerChanged(object? sender, EventArgs e) {
      if (sender is ContentPage page) {
        ApplyTheme(page);
      }
    }

    /// <summary>Applies appearance once the window's native Back control is available.</summary>
    private void OnPageLoaded(object? sender, EventArgs e) {
      // BEFORE: No OnPageLoaded method existed.
      // AFTER: Reuse our appearance update after the native page joins the window.
      if (sender is ContentPage page) {
        ApplyTheme(page);
      }
    }

    /// <summary>Refreshes already displayed controls when Settings changes the appearance.</summary>
    private void OnThemeChanged(object? sender, EventArgs e) {
      // BEFORE: No live appearance-change callback existed.
      // AFTER: Reuse the same native theme application for the currently attached page.
      if (_page is not null) {
        ApplyTheme(_page);
      }
    }

    /// <summary>Matches Windows text, arrows, and calendar chrome to the selected appearance.</summary>
    private static void ApplyTheme(ContentPage page) {
#if WINDOWS
      if (page.Handler?.PlatformView is Microsoft.UI.Xaml.FrameworkElement nativePage) {
        // BEFORE: nativePage.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
        // AFTER: Use the saved dark/light choice instead of always forcing a light page.
        nativePage.RequestedTheme = WorkshopTheme.IsDarkMode
            ? Microsoft.UI.Xaml.ElementTheme.Dark
            : Microsoft.UI.Xaml.ElementTheme.Light;
      }
#endif
      // BEFORE: ApplyTheme ended after the native page's RequestedTheme assignment.
      // AFTER: Style the existing title-bar Back control without changing its command.
      WorkshopBackButtonAppearance.Apply(page);
    }
  }
}
