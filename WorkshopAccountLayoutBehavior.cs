namespace HomeMaintenanceMAUI {
  /// <summary>Switches account layouts using the actual available content width.</summary>
  public class WorkshopAccountLayoutBehavior : Behavior<Grid> {
    private string? _currentState;

    /// <summary>Observes layout changes after the account grid is attached.</summary>
    protected override void OnAttachedTo(Grid grid) {
      base.OnAttachedTo(grid);
      grid.SizeChanged += OnGridSizeChanged;
      UpdateLayoutState(grid);
    }

    /// <summary>Releases the resize handler when the behavior leaves its grid.</summary>
    protected override void OnDetachingFrom(Grid grid) {
      grid.SizeChanged -= OnGridSizeChanged;
      _currentState = null;
      base.OnDetachingFrom(grid);
    }

    /// <summary>Reevaluates the layout when the window or content area resizes.</summary>
    private void OnGridSizeChanged(object? sender, EventArgs e) {
      if (sender is Grid grid) {
        UpdateLayoutState(grid);
      }
    }

    /// <summary>Uses a banner below 720 units and full-height side branding otherwise.</summary>
    private void UpdateLayoutState(Grid grid) {
      if (grid.Width <= 0) {
        return;
      }

      string state = grid.Width >= 720 ? "Wide" : "Narrow";
      if (state == _currentState) {
        return;
      }

      // Use measured width so native window resizing reliably updates both account pages.
      if (VisualStateManager.GoToState(grid, state)) {
        _currentState = state;
      }
    }
  }
}
