using NLog.Common;
using NLog.Targets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace NLog.Windows.Forms
{
    /// <summary>
    /// Log text a Rich Text Box control in an existing or new form.
    /// </summary>
    /// <seealso href="https://github.com/nlog/nlog/wiki/RichTextBox-target">Documentation on NLog Wiki</seealso>
    /// <example>
    /// <p>
    /// To set up the target in the <a href="config.html">configuration file</a>, 
    /// use the following syntax:
    /// </p><code lang="XML" source="examples/targets/Configuration File/RichTextBox/Simple/NLog.config">
    /// </code>
    /// <p>
    /// The result is:
    /// </p><img src="examples/targets/Screenshots/RichTextBox/Simple.gif"/><p>
    /// To set up the target with coloring rules in the <a href="config.html">configuration file</a>, 
    /// use the following syntax:
    /// </p><code lang="XML" source="examples/targets/Configuration File/RichTextBox/RowColoring/NLog.config">
    /// </code>
    /// <code lang="XML" source="examples/targets/Configuration File/RichTextBox/WordColoring/NLog.config">
    /// </code>
    /// <p>
    /// The result is:
    /// </p><img src="examples/targets/Screenshots/RichTextBox/RowColoring.gif"/><img src="examples/targets/Screenshots/RichTextBox/WordColoring.gif"/><p>
    /// To set up the log target programmatically similar to above use code like this:
    /// </p><code lang="C#" source="examples/targets/Configuration API/RichTextBox/Simple/Form1.cs">
    /// </code>
    /// ,
    /// <code lang="C#" source="examples/targets/Configuration API/RichTextBox/RowColoring/Form1.cs">
    /// </code>
    /// for RowColoring,
    /// <code lang="C#" source="examples/targets/Configuration API/RichTextBox/WordColoring/Form1.cs">
    /// </code>
    /// for WordColoring
    /// </example>
    [Target("RichTextBox")]
    public sealed class AsyncRichTextBoxTarget : RichTextBoxTarget
    {
        private ConcurrentQueue<LogEventInfo> _logEventInfoQueue = new ConcurrentQueue<LogEventInfo>();
        private Thread _thread = null;
        private Object _lock = new object();

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }
        private int _interval = 500;

        /// <summary>
        /// Log message to RichTextBox.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            _logEventInfoQueue.Enqueue(logEvent);
            if (_thread == null)
            {
                lock (_lock)
                {
                    if (_thread == null)
                    {
                        _thread = new Thread(() =>
                        {
                            List<LogEventInfo> logMsgs = new List<LogEventInfo>();
                            while (true)
                            {
                                logMsgs.Clear();
                                LogEventInfo lastInfo = null;
                                LogEventInfo info = null;
                                while (_logEventInfoQueue.TryDequeue(out info))
                                {
                                    if (lastInfo != null)
                                    {
                                        if (lastInfo.Level != info.Level)
                                        {
                                            var logEventInfo = new LogEventInfo(lastInfo.Level, lastInfo.LoggerName, string.Join("\n", logMsgs.Select(x => Layout.Render(x)).ToArray()));

                                            Write1(logEventInfo);
                                            logMsgs.Clear();
                                        }
                                    }

                                    logMsgs.Add(info);

                                    lastInfo = info;
                                }

                                if (logMsgs.Count > 0)
                                {
                                    var logEventInfo = new LogEventInfo(lastInfo.Level, lastInfo.LoggerName, string.Join("\n", logMsgs.Select(x => Layout.Render(x)).ToArray()));

                                    Write1(logEventInfo);
                                }

                                Thread.Sleep(Interval);
                            }
                        });
                        _thread.IsBackground = true;
                        _thread.Start();
                    }
                }
            }
        }

        private void Write1(LogEventInfo logEvent)
        {
            RichTextBox textbox = TargetRichTextBox;
            if (textbox == null || textbox.IsDisposed)
            {
                //no last logged textbox
                lastLoggedTextBoxControl = null;
                if (AllowAccessoryFormCreation)
                {
                    CreateAccessoryForm();
                }
                else if (messageRetention == RichTextBoxTargetMessageRetentionStrategy.None)
                {
                    InternalLogger.Trace("Textbox for target {0} is {1}, skipping logging", this.Name, textbox == null ? "null" : "disposed");
                    return;
                }
            }

            //string logMessage = Layout.Render(logEvent);
            string logMessage = logEvent.Message;
            RichTextBoxRowColoringRule matchingRule = FindMatchingRule(logEvent);

            bool messageSent = DoSendMessageToTextbox(logMessage, matchingRule, logEvent);

            if (messageSent)
            {
                //remember last logged text box
                lastLoggedTextBoxControl = textbox;
            }

            switch (messageRetention)
            {
                case RichTextBoxTargetMessageRetentionStrategy.None:
                    break;
                case RichTextBoxTargetMessageRetentionStrategy.All:
                    StoreMessage(logMessage, matchingRule, logEvent);
                    break;
                case RichTextBoxTargetMessageRetentionStrategy.OnlyMissed:
                    if (!messageSent)
                    {
                        StoreMessage(logMessage, matchingRule, logEvent);
                    }
                    break;
                default:
                    HandleError("Unexpected retention strategy {0}", messageRetention);
                    break;
            }
        }
    }
}