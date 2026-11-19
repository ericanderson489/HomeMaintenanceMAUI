using HomeMaintenanceMAUI.Models;
using HomeMaintenanceMAUI.Models.Records;

namespace HomeMaintenanceMAUI {
  // BEFORE: This file/class did not exist. TaskDetailsPage used a read-only StatusLabel.
  // AFTER: This empty picker displays the supplied status inside its field without adding
  // choices, a status-change handler, or any task persistence behavior.
  /// <summary>Provides an empty status dropdown for the team's future task actions.</summary>
  public class WorkshopStatusPicker : WorkshopFilterPicker {
    // Parameterless constructor to set ItemsSource of each picker
    public WorkshopStatusPicker() {
      ItemsSource = Enum.GetValues<Status>().ToList();
      // SelectedIndex = Array.IndexOf(Enum.GetValues<Status>(), DisplayStatus);
      SelectedIndexChanged += OnPickerSelectedIndexChanged;
    }
    /// <summary>Stores only the status text displayed by this presentation control.</summary>
    public static readonly BindableProperty DisplayStatusProperty = BindableProperty.Create(
        nameof(DisplayStatus), typeof(Status), typeof(WorkshopStatusPicker), Status.Pending,
        defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnDisplayStatusChanged);

    /// <summary>Gets or sets display text; Pending is the initial empty-dropdown caption.</summary>
    public Status DisplayStatus {
      get { return (Status)GetValue(DisplayStatusProperty); }
      set { SetValue(DisplayStatusProperty, value); }
    }

    /// <summary>Refreshes the visible caption when a different task is displayed.</summary>
    // Commented this iteration of the function out to test
    // private static void OnDisplayStatusChanged(BindableObject bindable, object oldValue,
    // object newValue) {
    //  ((WorkshopStatusPicker)bindable).UpdateDisplayedStatus();
    // }

    // Testing this implementation of OnDisplayStatusChanged instead to display the correct status
    private static void OnDisplayStatusChanged(BindableObject bindable, object oldValue, object newValue) {
      if (bindable is WorkshopStatusPicker picker && newValue is Status newStatus) {
        int targetIndex = Array.IndexOf(Enum.GetValues<Status>(), newStatus);
        if (picker.SelectedIndex != targetIndex) {
          picker.SelectedIndex = targetIndex;
        }
      }
    }

    /// <summary>Applies the caption after the themed native picker has been created.</summary>
    protected override void OnHandlerChanged() {
      base.OnHandlerChanged();
      UpdateDisplayedStatus();
    }

    private void OnPickerSelectedIndexChanged(object? sender, EventArgs e) {
      if (SelectedIndex >= 0 && SelectedItem is Status selectedStatus) {
        if (DisplayStatus != selectedStatus) {
          DisplayStatus = selectedStatus;
        }
      }
    }

    // This is supposed to be an override that populates the picker with the status of the attached task when dashboard page is opened
    protected override void OnBindingContextChanged() {
      base.OnBindingContextChanged();

      // Ensure items exist before setting the index
      if (ItemsSource == null) {
        ItemsSource = Enum.GetValues<Status>().ToList();
      }

      // Read the attached task's status directly when the row binds
      if (BindingContext is TaskRecord task) {
        DisplayStatus = task.Status;
        SelectedIndex = Array.IndexOf(Enum.GetValues<Status>(), task.Status);
      }
    }

    /// <summary>Shows the caption inside the empty field without adding selectable items.</summary>
    private void UpdateDisplayedStatus() {
#if WINDOWS
      if (Handler?.PlatformView is Microsoft.UI.Xaml.Controls.ComboBox picker) {
        // MAUI Title becomes a separate Windows Header. PlaceholderText keeps the status
        // inside the field, with the native dropdown arrow and no duplicate heading.
        picker.Header = null;
        picker.PlaceholderText = null;
      }
#else
      // On the other MAUI platforms Title is the empty picker's visible prompt.
      Title = "Pending";
#endif
    }
  }
}
