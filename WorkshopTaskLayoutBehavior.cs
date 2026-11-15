namespace HomeMaintenanceMAUI {
  /// <summary>Adapts the task form's presentation to the available window width and height.</summary>
  public class WorkshopTaskLayoutBehavior : Behavior<ScrollView> {
    private string? _currentState;

    /// <summary>Starts observing the form's measured size without accessing any task data.</summary>
    protected override void OnAttachedTo(ScrollView scroll) {
      base.OnAttachedTo(scroll);
      scroll.SizeChanged += OnViewportSizeChanged;
      UpdateLayoutState(scroll);
    }

    /// <summary>Releases the resize handler when the form leaves its layout.</summary>
    protected override void OnDetachingFrom(ScrollView scroll) {
      scroll.SizeChanged -= OnViewportSizeChanged;
      _currentState = null;
      base.OnDetachingFrom(scroll);
    }

    /// <summary>Reevaluates the layout after maximizing, restoring, or resizing the window.</summary>
    private void OnViewportSizeChanged(object? sender, EventArgs e) {
      if (sender is ScrollView scroll) {
        UpdateLayoutState(scroll);
      }
    }

    /// <summary>Uses side-by-side panels on desktops and stacked panels in small windows.</summary>
    private void UpdateLayoutState(ScrollView scroll) {
      if (scroll.Parent is not Grid grid || grid.Width <= 0 || grid.Height <= 0) {
        return;
      }

      // Fill the viewport on desktop while allowing stacked fields to scroll on small screens.
      if (scroll.Content is View content && scroll.Height > 0) {
        content.MinimumHeightRequest = scroll.Height;
      }

      string state = grid.Width >= 900 && grid.Height >= 620 ? "Wide" : "Compact";
      if (state == _currentState) {
        return;
      }

      // Measured size responds reliably to both native and programmatic window changes.
      if (VisualStateManager.GoToState(grid, state)) {
        _currentState = state;
      }
    }
  }
}
