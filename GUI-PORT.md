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
  Task rows open their read-only details when clicked. Selecting a row also enables
  the View details footer button for keyboard navigation. The row template uses
  compiled bindings to the existing record without changing its data.
- `TaskDetailsPage`: displays the selected task's name, notes, due date, status,
  type, and repeat value. Empty notes show an explicit fallback. The page uses the
  same responsive panels and fixed footer as the task form. Close returns to the
  list; Edit task opens a populated editor for that same task. A successful save
  refreshes the details and supplies the newest record for subsequent edits.
- `TaskEditorPage`: the existing task form in paper panels with navy headings,
  outlined inputs, and a gold Save button. The desktop form fills the available
  window: details and notes expand beside a 340-unit schedule panel. Smaller or
  shorter windows stack those same panels in a scrollable area. Save and Cancel
  remain visible in a fixed footer. `WorkshopTaskLayoutBehavior` responds to the
  measured viewport size when the window is resized, maximized, or restored.
  The original control names/types, optional notes, Add defaults, and validation
  are preserved. `WorkshopLightThemeBehavior`
  gives native Windows fields readable text and icons on the paper background.
  The existing constructors still select the caption. The new record overload
  loads the selected task; Save updates its existing ID instead of adding a copy.
- `WorkshopSign`, `WorkshopRuler`, and `Resources/Styles/Workshop.xaml`: shared
  branding, typography, and explicit styles using the existing palette.
  `Resources/Images/workshop_sign.png` is a transparent, high-resolution export
  of the original WinForms sign, preserving its condensed lettering.

## Behavior boundary

The user approved two task-editing changes after reviewing the GUI-only details
page: `TaskEditorPage.xaml.cs` now loads and saves an existing task, and
`Services/AccountStore.cs` adds `UpdateTask`. Comments identify the exact old
save expressions and their replacements, and identify new code with no prior
equivalent. The original AddTask implementation remains unchanged.

The details page receives the immutable record already displayed by the list.
It does not load or write account files, and closing it does not save anything.
The details page opens the editor with the selected record and refreshes through
a save notification. The store replaces one matching ID under the existing file
lock and temporary-file write pattern. Only name, description, type, due date,
and repeat change; task count/order, status, recurrence history, completion,
archive state, other tasks, and account credentials are retained. Missing or
externally changed tasks are rejected instead of appended or overwritten.
The existing title and future-date validation remains in place, including when
editing an overdue task. Cancel does not save. The Add route still creates a new ID.

The new Renovations handler only selects its placeholder view. Existing task
loading and sign-out code in `DashboardPage` are unchanged. The task filter and
other task actions retain their merged behavior.

Except for the approved new AccountStore method, services remain unchanged.
Models, authentication, routing, startup, platform code, and project targets/packages
are unchanged from the merged baseline. After local review, the user approved
publishing the details/editor changes on the separate `murdock-edit-task` branch.

## Coding convention

New C# follows the [Google C# guide](https://google.github.io/styleguide/csharpguide.html):
two spaces, same-line braces, descriptive names, and a 100-column limit.
Every new class, constructor, and method has XML documentation. XAML comments
explain layout groups and intentionally unbound controls. `.editorconfig`
records the formatting conventions without reformatting the team's existing code.

## Validation

The GUI builds successfully for Windows with zero warnings and zero errors.
The task row template now declares its data type for compiled bindings.
Build using:

```powershell
dotnet build HomeMaintenanceMAUI.csproj `
  -f net9.0-windows10.0.19041.0 `
  -p:TargetFrameworks=net9.0-windows10.0.19041.0
```

Diff checks confirm that business changes are limited to the approved editor and
account-store files. Named task controls retain their types and Add defaults.
Android/iOS/MacCatalyst builds have not been run.
The approved edit path passes an isolated storage harness (9 checks / 29 assertions)
covering replacement, metadata preservation, stale/missing/duplicate IDs, file locking,
unrelated changes, and the unchanged Add path. A Windows GUI integration preview
verifies click-to-details, populated editing, two successive saves without duplicates,
rapid Save protection, Cancel, refreshed details/dashboard, stale-edit feedback,
and normal Add. It renders regular, maximized, and narrow editor layouts. Test account
files live only in a separate workspace test directory, never the user's account directory.
An isolated sample-data preview checks details navigation from Home and Tasks,
the selected task's displayed values, empty notes, list selection reset, repeated
click protection, and reopening the same row after Close. Narrow and maximized
details layouts keep the footer visible. A native Windows click on the sample
task name also opens its matching details. No saved account data is used or changed.
An isolated Windows preview renders Add and Edit captions, checks the form at
1180- and 480-unit window widths, and verifies native field contrast and unchanged
enum/date defaults. It also checks Save remains enabled, Cancel returns to the
dashboard, and Home / Tasks / Renovations select the expected view. The compact
navigation layout is rendered separately. The preview does not save any tasks.
A follow-up task-form preview checks desktop, large, narrow, and short windows,
then maximizes and restores the native Windows window. It verifies that details
fill the viewport, controls stay within its width, Save and Cancel remain visible,
and draft text, date, and picker selections survive each layout change. Both
Add and Edit captions are rendered in the maximized window.
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
