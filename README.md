# NLog.Windows.Forms
**English** | [한국어](https://github.com/idas4you/NLog.Windows.Forms/blob/master/README.ko.md)

[![Version](https://badge.fury.io/nu/NLog.Windows.Forms.Async.svg)](https://www.nuget.org/packages/NLog.Windows.Forms.Async)
[![CI](https://github.com/idas4you/NLog.Windows.Forms/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/idas4you/NLog.Windows.Forms/actions/workflows/ci.yml)

This package provides NLog targets for displaying log output in Windows Forms applications.

- [RichTextBoxTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/RichTextBoxTarget): Displays log messages in a RichTextBox control with colorhighlighting.
- [AsyncRichTextBoxTarget](https://github.com/idas4you/NLog.Windows.Forms/blob/master/docs/AsyncRichTextBoxTarget.md): Displays log messages in a RichTextBox control using batched background processing.
- [MessageBoxTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/MessageBoxTarget): Displays log messages in message boxes.
- [FormControlTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/FormControlTarget): Displays log messages in the `Text` property of a Windows Forms control.
- [ToolStripItemTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/ToolStripItemTarget): Displays log messages in the `Text` property of a `ToolStripItem`.

See [list](https://nlog-project.org/config/?tab=targets&search=package:nlog.windows.forms)

## Register Extension
Install the [NLog.Windows.Forms.Async](https://www.nuget.org/packages/NLog.Windows.Forms.Async/) NuGet package and [register the extension-assembly](https://github.com/NLog/NLog/wiki/Register-your-custom-component).

When loading configuration from `NLog.config` file, then register the extension in the <extensions> section:
```xml
<extensions>
  <add assembly="NLog.Windows.Forms.Async"/>
</extensions>
```

Alternative register from code using the [fluent configuration API](https://github.com/NLog/NLog/wiki/Fluent-Configuration-API):
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

See the [AsyncRichTextBoxTarget documentation](https://github.com/idas4you/NLog.Windows.Forms/blob/master/docs/AsyncRichTextBoxTarget.md) for the XML configuration and the differences from `RichTextBoxTarget`.
