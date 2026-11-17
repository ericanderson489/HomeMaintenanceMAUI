namespace HomeMaintenanceMAUI {
  // BEFORE: This file/class did not exist. TaskDetailsPage used a read-only StatusLabel.
  // AFTER: This empty picker displays the supplied status inside its field without adding
  // choices, a status-change handler, or any task persistence behavior.
  /// <summary>Provides an empty status dropdown for the team's future task actions.</summary>
  public class WorkshopStatusPicker : WorkshopFilterPicker {
    /// <summary>Stores only the status text displayed by this presentation control.</summary>
    public static readonly BindableProperty DisplayStatusProperty = BindableProperty.Create(
        nameof(DisplayStatus), typeof(string), typeof(WorkshopStatusPicker), "Pending",
        propertyChanged: OnDisplayStatusChanged);

    /// <summary>Gets or sets display text; Pending is the initial empty-dropdown caption.</summary>
    public string DisplayStatus {
      get { return (string)GetValue(DisplayStatusProperty); }
      set { SetValue(DisplayStatusProperty, value); }
    }

    /// <summary>Refreshes the visible caption when a different task is displayed.</summary>
    private static void OnDisplayStatusChanged(BindableObject bindable, object oldValue,
        object newValue) {
      ((WorkshopStatusPicker)bindable).UpdateDisplayedStatus();
    }

    /// <summary>Applies the caption after the themed native picker has been created.</summary>
    protected override void OnHandlerChanged() {
      base.OnHandlerChanged();
      UpdateDisplayedStatus();
    }

    /// <summary>Shows the caption inside the empty field without adding selectable items.</summary>
    private void UpdateDisplayedStatus() {
#if WINDOWS
      if (Handler?.PlatformView is Microsoft.UI.Xaml.Controls.ComboBox picker) {
        // MAUI Title becomes a separate Windows Header. PlaceholderText keeps the status
        // inside the field, with the native dropdown arrow and no duplicate heading.
        picker.Header = null;
        picker.PlaceholderText = DisplayStatus;
      }
#else
      // On the other MAUI platforms Title is the empty picker's visible prompt.
      Title = DisplayStatus;
#endif
    }
  }
}
