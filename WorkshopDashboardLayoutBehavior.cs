namespace HomeMaintenanceMAUI {
  // BEFORE: This file and class did not exist. DashboardPage.xaml used
  // <AdaptiveTrigger MinWindowWidth="800" /> and <AdaptiveTrigger MinWindowWidth="0" />.
  // AFTER: This behavior selects those existing layout states from grid.Width on SizeChanged.
  /// <summary>Switches dashboard navigation using its measured viewport width.</summary>
  public class WorkshopDashboardLayoutBehavior : Behavior<Grid> {
    /// <summary>Remembers the active layout to avoid repeating visual-state changes.</summary>
    private string? _currentState;

    /// <summary>Observes the dashboard grid without reading or changing task data.</summary>
    protected override void OnAttachedTo(Grid grid) {
      base.OnAttachedTo(grid);
      grid.SizeChanged += OnGridSizeChanged;
      UpdateLayout(grid);
    }

    /// <summary>Releases the resize subscription when the dashboard layout is removed.</summary>
    protected override void OnDetachingFrom(Grid grid) {
      grid.SizeChanged -= OnGridSizeChanged;
      _currentState = null;
      base.OnDetachingFrom(grid);
    }

    /// <summary>Rechecks navigation after maximizing, restoring, or resizing the window.</summary>
    private void OnGridSizeChanged(object? sender, EventArgs e) {
      if (sender is Grid grid) {
        UpdateLayout(grid);
      }
    }

    /// <summary>Uses compact navigation below 800 device-independent units.</summary>
    private void UpdateLayout(Grid grid) {
      if (grid.Width <= 0) {
        return;
      }

      string state = grid.Width >= 800 ? "Wide" : "Narrow";
      if (state != _currentState && VisualStateManager.GoToState(grid, state)) {
        _currentState = state;
      }
    }
  }
}
