# AsyncRichTextBoxTarget
**English** | [한국어](AsyncRichTextBoxTarget.ko.md)

Writes log messages to a `RichTextBox` control in batches from a background thread.

`AsyncRichTextBoxTarget` extends [RichTextBoxTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/RichTextBoxTarget). Instead of updating the control once per log event, it queues events and writes them to the control at a fixed interval. This keeps the UI responsive when many messages are logged in a short time.

## When to use it
- Use `AsyncRichTextBoxTarget` when log volume is high and per-message updates make the form slow.
- Use `RichTextBoxTarget` when messages must appear immediately, or when each message needs its own row-coloring or link handling.

## Configuration

### NLog.config
The assembly name of this package is `NLog.Windows.Forms.Async`, and the target type is `AsyncRichTextBox`.

```xml
<nlog xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <extensions>
    <add assembly="NLog.Windows.Forms.Async"/>
  </extensions>

  <targets>
    <target name="rtb" xsi:type="AsyncRichTextBox"
            formName="MainForm"
            controlName="richTextBoxLog"
            layout="${longdate}|${level:uppercase=true}|${message}"
            interval="500" />
  </targets>

  <rules>
    <logger name="*" minlevel="Info" writeTo="rtb" />
  </rules>
</nlog>
```

### Code
```csharp
NLog.LogManager.Setup().RegisterWindowsForms();

var richTextTarget = new NLog.Windows.Forms.AsyncRichTextBoxTarget
{
    Name = "rtb",
    FormName = "MainForm",
    ControlName = "richTextBoxLog",
    Layout = "${longdate}|${level:uppercase=true}|${message}",
    Interval = 500
};

NLog.LogManager.Setup().LoadConfiguration(cfg => cfg.ForLogger().WriteTo(richTextTarget));
```

`RegisterWindowsForms()` registers the `AsyncRichTextBox` type name. It is only needed when the target is created from a configuration file.

## Options

| Option | Type | Default | Description |
| --- | --- | --- | --- |
| `Interval` | `int` | `500` | Time in milliseconds between two writes to the control. |

- A smaller `Interval` shows messages sooner.
- A larger `Interval` updates the control less often under high log volume.

All other options (`FormName`, `ControlName`, `Layout`, `MaxLines`, `AutoScroll`, row and word coloring rules, message retention and so on) are inherited from `RichTextBoxTarget`. See the [RichTextBoxTarget wiki page](https://github.com/NLog/NLog.Windows.Forms/wiki/RichTextBoxTarget).

## How it works
1. Each log event is added to a queue. The call returns without touching the control.
2. A background thread starts on the first log event.
3. Every `Interval` milliseconds the thread takes all queued events and renders each one with `Layout`.
4. Consecutive events with the same level are joined with line breaks and written to the control as one block. A change of level starts a new block.

## Differences from RichTextBoxTarget
- **Messages are delayed.** A message can appear up to `Interval` milliseconds after it was logged.
- **Row-coloring rules apply to a block, not to a single event.** A block carries the level and logger name of its last event, and its message is the combined rendered text. Rules based on level work as expected. Rules based on other event data (for example the logger name of each event, the exception or event properties) are evaluated against the block.
- **Queued messages can be lost on exit.** The background thread does not flush the queue when the application closes, so events logged during the last interval may not reach the control.
