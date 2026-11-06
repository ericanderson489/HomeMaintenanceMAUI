# Workshop GUI port

The visual reference is `HomeMaintenanceApp` branch `ui/workshop-theme`, commit
`b8c86919d27264a4100f201dbe73c1ddb893f8ec`. This port starts from
`HomeMaintenanceMAUI` branch `Murdock-ui-Branch`, commit
`5abab2305ce5d851f1461acea97c5e3db0e2fcf3`.

## Screens

- `MainPage` and `CreateAccountPage`: Workshop sign, existing illustration, labeled
  fields, navy/cream panels, and compact layouts. Every existing entry name and
  button handler is preserved, including the MAUI confirmation-password field.
  The wide-screen navy panel fills its column's entire height; proportional rows
  scale the sign and illustration with the window. Narrow windows use a compact
  sign banner above the independently scrollable form. A shared layout behavior
  selects these states from the measured content width whenever the grid resizes.
- `DashboardPage`: a `ContentPage` for the existing Shell login destination,
  with Home, Tasks, and Sign out navigation. The previous placeholder was a
  `ContentView`, which is not a Shell navigation page.
- `WorkshopHomeView`: overview headings, one horizontal summary strip, and a
  filling upcoming-jobs table matching the reference screenshot.
- `TasksView` and `TaskListView`: filter controls and a full-width task table with
  the empty-state message in its footer, matching the supplied screenshots.
  `JobSheetView` remains available for future wiring but is not displayed.
  The filter has one external label, starts on All jobs after its items load,
  and uses a bordered native dropdown. Its Windows theme is scoped to the picker
  so selected text, the arrow, and menu stay readable against the paper background.
- `TaskEditorPage`: title, required-notes label, due date, task type, and repeat
  inputs. `new TaskEditorPage()` displays Add; `new TaskEditorPage(true)` displays
  Edit. Neither constructor accepts or alters a task record.
- `WorkshopSign`, `WorkshopRuler`, and `Resources/Styles/Workshop.xaml`: shared
  branding, typography, and explicit styles using the existing palette.
  `Resources/Images/workshop_sign.png` is a transparent, high-resolution export
  of the original WinForms sign, preserving its condensed lettering.

## Behavior boundary

Both Add task buttons display only the message `keifer work here.` with an OK
button. They do not navigate to the task editor, create tasks, or save anything.
Other new handlers switch screens or close the application from login Cancel.
Save, Mark complete, Edit task, Archive, Close job sheet, and View details are
disabled and have no task handlers or commands. The filter only changes the
picker's displayed choice. Task fields have no validation or persistence.
Counts are static zeroes matching the reference screenshots and the list is
empty; no sample tasks or calculated state are introduced. The disconnected
task-editor layout remains available for future work but Add task cannot open it.

The existing `Models/`, `Services/`, `MainPage.xaml.cs`,
`CreateAccountPage.xaml.cs`, `AppShell.xaml.cs`, application startup, and project
targets/packages are unchanged. Only the Shell's displayed window title changes.
The existing account logic remains connected to
its original controls. No legacy WinForms models, account storage, task adapters,
recurrence calculations, or task mutations are copied.

## Coding convention

New C# follows the [Google C# guide](https://google.github.io/styleguide/csharpguide.html):
two spaces, same-line braces, descriptive names, and a 100-column limit.
Every new class, constructor, and method has XML documentation. XAML comments
explain layout groups and intentionally unbound controls. `.editorconfig`
records the formatting conventions without reformatting the team's existing code.

## Validation

The original branch and the integrated port build successfully for Windows with
zero warnings and zero errors using:

```powershell
dotnet build HomeMaintenanceMAUI.csproj `
  -f net9.0-windows10.0.19041.0 `
  -p:TargetFrameworks=net9.0-windows10.0.19041.0
```

Diff checks confirm that the existing model, service, authentication, startup,
and routing code is unchanged. XAML event names remain matched to the original
authentication handlers. Android/iOS/MacCatalyst builds have not been run.
An isolated Windows preview successfully loads the workspace and its Home
accessibility tree. Account layout was also visually compared with the reference.
An isolated Windows runtime check verifies both account pages at 1180x800,
1900x1050, and 480x800, then restores the original size. The wide branding fills
the page height, its artwork grows, and the narrow banner spans the content width.
The same check confirms the native filter starts on All jobs, has no duplicate
header, and uses its scoped light theme.
Full interactive and mobile QA remains to be completed on the team's target devices.

For manual review: sign in with an existing development account, switch Home /
Tasks, click Add task on each screen and verify `keifer work here.`, dismiss each
popup with OK, resize to a narrow window, and verify disabled
task actions remain inert. Confirm registration still includes password
confirmation and returns to the existing sign-in screen.
