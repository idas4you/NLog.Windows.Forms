# NLog.Windows.Forms
[![Version](https://badge.fury.io/nu/NLog.Windows.Forms.svg)](https://www.nuget.org/packages/NLog.Windows.Forms)
[![AppVeyor](https://img.shields.io/appveyor/ci/nlog/nlog-windows-forms/master.svg)](https://ci.appveyor.com/project/nlog/nlog-windows-forms/branch/master)
[![codecov.io](https://codecov.io/github/NLog/NLog.Windows.Forms/coverage.svg?branch=master)](https://codecov.io/github/NLog/NLog.Windows.Forms?branch=master)

This package provides NLog Targets that redirect logging output to Windows Forms:

- [RichTextBoxTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/RichTextBoxTarget)
- `AsyncRichTextBoxTarget` (batched background processing for RichTextBox updates)
- [MessageBoxTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/MessageBoxTarget)
- [FormControlTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/FormControlTarget)
- [ToolStripItemTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/ToolStripItemTarget)

See [list](https://nlog-project.org/config/?tab=targets&search=package:nlog.windows.forms)

## Register Extension
Install the [NLog.Windows.Forms](https://www.nuget.org/packages/NLog.Windows.Forms/) NuGet package and [register the extension-assembly](https://github.com/NLog/NLog/wiki/Register-your-custom-component).

NLog will only recognize the extensions when loading from NLog.config-file, by adding the extension to NLog.config-file:
```xml
<extensions>
  <add assembly="NLog.Windows.Forms"/>
</extensions>
```

Alternative register from code using [fluent configuration API](https://github.com/NLog/NLog/wiki/Fluent-Configuration-API):
```csharp
NLog.LogManager.Setup().RegisterWindowsForms();
```

## AsyncRichTextBoxTarget
`AsyncRichTextBoxTarget` extends `RichTextBoxTarget` and processes queued log events on a background thread.
It can reduce UI update overhead by batching messages before writing them to the target control.

Example (programmatic setup):
```csharp
var richTextTarget = new NLog.Windows.Forms.AsyncRichTextBoxTarget
{
    Name = "rtb",
    FormName = "MainForm",
    ControlName = "richTextBoxLog",
    Layout = "${longdate}|${level:uppercase=true}|${message}",
    Interval = 500
};
```

- `Interval` is the processing interval in milliseconds.
- Use a smaller value for more immediate UI updates.
- Use a larger value to reduce update frequency under high log volume.
