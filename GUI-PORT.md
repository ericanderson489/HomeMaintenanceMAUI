# Workshop GUI port

The visual reference is `HomeMaintenanceApp` branch `ui/workshop-theme`, commit
`b8c86919d27264a4100f201dbe73c1ddb893f8ec`. This port starts from
`HomeMaintenanceMAUI` branch `Murdock-ui-Branch`, commit
`5abab2305ce5d851f1461acea97c5e3db0e2fcf3`.

## Screens

- `MainPage` and `CreateAccountPage`: Workshop sign, existing illustration, labeled
  fields, navy/cream panels, and compact layouts. Every existing entry name and
  button handler is preserved, including the MAUI confirmation-password field.
- `DashboardPage`: a `ContentPage` for the existing Shell login destination,
  with Home, Tasks, and Sign out navigation. The previous placeholder was a
  `ContentView`, which is not a Shell navigation page.
- `WorkshopHomeView`: overview headings, unbound summary cards, and upcoming jobs.
- `TasksView`, `TaskListView`, and `JobSheetView`: filter controls, empty task table,
  job-sheet fields, and responsive side-by-side/stacked layouts.
- `TaskEditorPage`: title, required-notes label, due date, task type, and repeat
  inputs. `new TaskEditorPage()` displays Add; `new TaskEditorPage(true)` displays
  Edit. Neither constructor accepts or alters a task record.
- `WorkshopSign`, `WorkshopRuler`, and `Resources/Styles/Workshop.xaml`: shared
  branding, typography, and explicit styles using the existing palette.

## Behavior boundary

New handlers only switch screens, open/cancel the task form, and adjust layout.
Save, Mark complete, Edit task, Archive, Close job sheet, and View details are
disabled and have no task handlers or commands. The filter only changes the
picker's displayed choice. Task fields have no validation or persistence.
Counts use dashes and the list is empty; no sample tasks or calculated state are
introduced. The edit-form layout is ready for future selection wiring.

The existing `Models/`, `Services/`, `MainPage.xaml.cs`,
`CreateAccountPage.xaml.cs`, `AppShell.xaml.cs`, application startup, and project
targets/packages are unchanged. The existing account logic remains connected to
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
Native preview accessibility was inspected, but full visual and navigation QA
remains to be completed on the team's target devices.

For manual review: sign in with an existing development account, switch Home /
Tasks, open Add task and Cancel, resize to a narrow window, and verify disabled
task actions remain inert. Confirm registration still includes password
confirmation and returns to the existing sign-in screen.
