namespace HomeMaintenanceMAUI {
  // BEFORE: This file and class did not exist. WorkshopHomeView.xaml used
  // RowDefinitions="Auto,Auto,Auto,*"; TasksView.xaml used RowDefinitions="Auto,Auto,*".
  // AFTER: Both headers get bounded scroll viewports. Home's existing summary cards also
  // change row/column positions by width. TaskListView data behavior is unchanged; its finite
  // layout row moves from Grid.Row="3" (Home) or Grid.Row="2" (Tasks) to Grid.Row="1".
  /// <summary>Reserves room for a task list while its header scrolls when needed.</summary>
  public class WorkshopWorkspaceLayoutBehavior : Behavior<Grid> {
    /// <summary>The name of the header scroll view in the attached grid's namescope.</summary>
    public string HeaderName { get; set; } = "HomeHeaderScroll";

    /// <summary>The optional summary grid; an empty name leaves summary layout untouched.</summary>
    public string SummaryName { get; set; } = "SummaryCards";

    /// <summary>The attached workspace grid, retained only while this behavior is active.</summary>
    private Grid? _layout;

    /// <summary>The measured header content whose height can change as its text wraps.</summary>
    private Grid? _headerContent;

    /// <summary>The current column count, used to avoid unnecessary rearrangement.</summary>
    private int _summaryColumns;

    /// <summary>Prevents recursive notifications while header dimensions are adjusted.</summary>
    private bool _isUpdating;

    /// <summary>Measures the workspace without reading or changing its records.</summary>
    protected override void OnAttachedTo(Grid grid) {
      base.OnAttachedTo(grid);
      _layout = grid;
      grid.SizeChanged += OnLayoutSizeChanged;
      UpdateLayout();
    }

    /// <summary>Releases both size subscriptions when the workspace layout is removed.</summary>
    protected override void OnDetachingFrom(Grid grid) {
      grid.SizeChanged -= OnLayoutSizeChanged;
      if (_headerContent is not null) {
        _headerContent.SizeChanged -= OnLayoutSizeChanged;
      }
      _layout = null;
      _headerContent = null;
      _summaryColumns = 0;
      base.OnDetachingFrom(grid);
    }

    /// <summary>Reevaluates available space after resizing or wrapping summary text.</summary>
    private void OnLayoutSizeChanged(object? sender, EventArgs e) {
      UpdateLayout();
    }

    /// <summary>Bounds the header while keeping the collection outside its scroll view.</summary>
    private void UpdateLayout() {
      if (_isUpdating || _layout is null || _layout.Width <= 0 || _layout.Height <= 0) {
        return;
      }

      var scroll = _layout.FindByName<ScrollView>(HeaderName);
      var summary = string.IsNullOrEmpty(SummaryName)
          ? null : _layout.FindByName<Grid>(SummaryName);
      if (scroll?.Content is not Grid header) {
        return;
      }

      _isUpdating = true;
      try {
        if (_headerContent != header) {
          if (_headerContent is not null) {
            _headerContent.SizeChanged -= OnLayoutSizeChanged;
          }
          _headerContent = header;
          header.SizeChanged += OnLayoutSizeChanged;
        }

        double width = Math.Max(0, _layout.Width - _layout.Padding.HorizontalThickness);
        double height = Math.Max(0, _layout.Height - _layout.Padding.VerticalThickness
            - _layout.RowSpacing);
        if (summary is not null) {
          int columns = width >= 760 ? 4 : width >= 400 ? 2 : 1;
          ArrangeSummary(summary, columns);
        }

        // Reserve up to 300 units for wrapped rows, column headings, and the existing footer.
        // Keep at least 64 units for scrolling a full-height header button into view.
        double listHeight = Math.Min(300, Math.Max(0, height - 64));
        double desiredHeight = ((Microsoft.Maui.IView)header)
            .Measure(width, double.PositiveInfinity).Height;
        double headerHeight = Math.Max(0, Math.Min(desiredHeight, height - listHeight));
        if (Math.Abs(scroll.HeightRequest - headerHeight) > 0.5) {
          scroll.HeightRequest = headerHeight;
        }
      } finally {
        _isUpdating = false;
      }
    }

    /// <summary>Repositions the same four summary cards in four, two, or one columns.</summary>
    private void ArrangeSummary(Grid summary, int columns) {
      if (_summaryColumns == columns) {
        return;
      }

      _summaryColumns = columns;
      summary.ColumnDefinitions.Clear();
      summary.RowDefinitions.Clear();
      for (int column = 0; column < columns; column++) {
        summary.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
      }
      for (int row = 0; row < (summary.Children.Count + columns - 1) / columns; row++) {
        summary.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
      }
      int index = 0;
      foreach (View card in summary.Children.OfType<View>()) {
        Grid.SetColumn(card, index % columns);
        Grid.SetRow(card, index / columns);
        index++;
      }
    }
  }
}
