# Workshop GUI port

The visual reference is `HomeMaintenanceApp` branch `ui/workshop-theme`, commit
`b8c86919d27264a4100f201dbe73c1ddb893f8ec`. This port starts from
`HomeMaintenanceMAUI` branch `Murdock-ui-Branch`, commit
`5abab2305ce5d851f1461acea97c5e3db0e2fcf3`.
The task-form theme and Renovations tab build on the team's merged task/reminder
work at `b6b8e9315d0f24ac4b0255d06b5857b94f428f12`.

## Screens

- `MainPage` and `CreateAccountPage`: Workshop sign, existing illustration, labeled
  fields, navy/cream panels, and compact layouts. Every existing entry name and
  button handler is preserved, including the MAUI confirmation-password field.
  The wide-screen navy panel fills its column's entire height; proportional rows
  scale the sign and illustration with the window. Narrow windows use a compact
  sign banner above the independently scrollable form. A shared layout behavior
  selects these states from the measured content width whenever the grid resizes.
- `DashboardPage`: a `ContentPage` for the existing Shell login destination,
  with Home, Tasks, Renovations, and Sign out navigation. The desktop sidebar
  and wrapping compact navigation share the same selected-tab styling.
- `RenovationsView`: a Workshop heading, ruler, and placeholder text for future
  renovation tracking. It has no project data, persistence, or task actions.
- `WorkshopHomeView`: overview headings, one horizontal summary strip, and a
  filling upcoming-jobs table matching the reference screenshot.
- `TasksView` and `TaskListView`: filter controls and a full-width task table with
  the empty-state message in its footer, matching the supplied screenshots.
  `JobSheetView` remains available for future wiring but is not displayed.
  The filter has one external label, starts on All jobs after its items load,
  and uses a bordered native dropdown. Its Windows theme is scoped to the picker
  so selected text, the arrow, and menu stay readable against the paper background.
- `TaskEditorPage`: the existing task form in a paper card with a navy heading,
  outlined inputs, and gold Save button. Schedule fields wrap on smaller screens.
  The original names, control types, Save/Cancel handlers, optional notes,
  enum choices, and default values are preserved. `WorkshopLightThemeBehavior`
  gives native Windows fields readable text and icons on the paper background.
  The existing constructor still selects the Add or Edit caption.
- `WorkshopSign`, `WorkshopRuler`, and `Resources/Styles/Workshop.xaml`: shared
  branding, typography, and explicit styles using the existing palette.
  `Resources/Images/workshop_sign.png` is a transparent, high-resolution export
  of the original WinForms sign, preserving its condensed lettering.

## Behavior boundary

The merged team code now opens the task editor, validates and saves tasks,
and reloads the task lists. This GUI update preserves that behavior. In particular,
`TaskEditorPage.xaml.cs` is unchanged, including its existing Add/Edit constructor,
validation, save behavior, and Cancel route. The update does not implement any
additional task-editing behavior.

The new Renovations handler only selects its placeholder view. Existing task
loading and sign-out code in `DashboardPage` are unchanged. The task filter and
other task actions retain their merged behavior.

`Models/`, `Services/`, authentication, routing, application startup, platform
code, and project targets/packages are unchanged from the merged baseline.

## Coding convention

New C# follows the [Google C# guide](https://google.github.io/styleguide/csharpguide.html):
two spaces, same-line braces, descriptive names, and a 100-column limit.
Every new class, constructor, and method has XML documentation. XAML comments
explain layout groups and intentionally unbound controls. `.editorconfig`
records the formatting conventions without reformatting the team's existing code.

## Validation

The themed form and Renovations tab build successfully for Windows with zero
errors. Three compiled-binding warnings remain in the unchanged `TaskListView.xaml`.
Build using:

```powershell
dotnet build HomeMaintenanceMAUI.csproj `
  -f net9.0-windows10.0.19041.0 `
  -p:TargetFrameworks=net9.0-windows10.0.19041.0
```

Diff checks against the merged baseline confirm that task creation/editing,
model, service, authentication, startup, and routing code is unchanged. Named task
controls retain their types and input defaults, and Save/Cancel keep their existing
handlers. Android/iOS/MacCatalyst builds have not been run.
An isolated Windows preview renders Add and Edit captions, checks the form at
1180- and 480-unit window widths, and verifies native field contrast and unchanged
enum/date defaults. It also checks Save remains enabled, Cancel returns to the
dashboard, and Home / Tasks / Renovations select the expected view. The compact
navigation layout is rendered separately. The preview does not save any tasks.
An isolated Windows preview successfully loads the workspace and its Home
accessibility tree. Account layout was also visually compared with the reference.
An isolated Windows runtime check verifies both account pages at 1180x800,
1900x1050, and 480x800, then restores the original size. The wide branding fills
the page height, its artwork grows, and the narrow banner spans the content width.
The same check confirms the native filter starts on All jobs, has no duplicate
header, and uses its scoped light theme.
Full interactive and mobile QA remains to be completed on the team's target devices.

For manual review: sign in with an existing development account, switch Home /
Tasks / Renovations, and confirm the placeholder and selected-tab outline. Open
Add task, resize the window, and check the schedule controls and Save/Cancel
buttons remain reachable. Verify the existing task validation, save, and return
navigation with a development account. Confirm registration still includes
password confirmation and returns to the existing sign-in screen.
