// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-13-2021
// ***********************************************************************
// <copyright file="Log.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Ffxi_Navmesh_Builder.Common
{
    /// <summary>
    /// Class Log. Writes a plain-text, per-session log file to "logs\NavmeshBuilder_&lt;date&gt;_&lt;time&gt;.log"
    /// next to the application, and mirrors everything shown in the on-screen debug window into it.
    /// (This replaces the old misleadingly named "Log.Bin", which was plain text as well.)
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Serializes file writes; debug text can arrive from build worker threads.
        /// </summary>
        private static readonly object FileLock = new object();

        /// <summary>
        /// How long old session logs are kept before being pruned at startup.
        /// </summary>
        private const int KeepLogsForDays = 14;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class, creating the logs folder
        /// and a timestamped file for this session.
        /// </summary>
        public Log()
        {
            var logsDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            try
            {
                Directory.CreateDirectory(logsDir);
                PruneOldLogs(logsDir);
            }
            catch
            {
                // If the folder cannot be created we still run; writes will just fail silently.
            }

            LogFilePath = Path.Combine(logsDir, $"NavmeshBuilder_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");
            WriteLine($"=== FFXI Navmesh Builder session started (v{Assembly.GetExecutingAssembly().GetName().Version}) ===");
        }

        /// <summary>
        /// Gets the full path of this session's log file.
        /// </summary>
        /// <value>The log file path.</value>
        public string LogFilePath { get; }

        /// <summary>
        /// Adds the debug text to the on-screen debug window and mirrors it into the session log file.
        /// </summary>
        /// <param name="tb">The tb.</param>
        /// <param name="text">The text.</param>
        public void AddDebugText(ListBox tb, string text)
        {
            WriteLine(text);
            tb?.Dispatcher.Invoke(new Action(() =>
            {
                var time = DateTime.Now;
                var logEntry = $@"{time:HH:mm:ss} {text}";
                tb.Items.Add(logEntry);
                tb.SelectedIndex = tb.Items.Count - 1;
                tb.ScrollIntoView(tb.SelectedItem);
            }));
        }

        /// <summary>
        /// Prunes session logs older than <see cref="KeepLogsForDays"/> days. Kept as a public
        /// method for compatibility with the old ClearLog API.
        /// </summary>
        public void ClearLog()
        {
            try
            {
                PruneOldLogs(Path.Combine(Directory.GetCurrentDirectory(), "logs"));
            }
            catch
            {
                // best effort only
            }
        }

        /// <summary>
        /// Logs a message (typically an exception) with its call site to the session log file.
        /// </summary>
        /// <param name="sExceptionName">Name of the s exception.</param>
        /// <param name="sFormName">Name of the s form.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="caller">The caller.</param>
        public void LogFile(string sExceptionName, string sFormName, [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null)
        {
            WriteLine($"[{sFormName}.{caller}:{lineNumber}] {sExceptionName}");
        }

        /// <summary>
        /// Appends one timestamped line to the session log file.
        /// </summary>
        /// <param name="text">The text.</param>
        private void WriteLine(string text)
        {
            try
            {
                lock (FileLock)
                {
                    File.AppendAllText(LogFilePath,
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {text}{Environment.NewLine}");
                }
            }
            catch
            {
                // Logging must never take the application down.
            }
        }

        /// <summary>
        /// Deletes session logs older than <see cref="KeepLogsForDays"/> days.
        /// </summary>
        /// <param name="logsDir">The logs directory.</param>
        private static void PruneOldLogs(string logsDir)
        {
            if (!Directory.Exists(logsDir)) return;
            foreach (var file in Directory.GetFiles(logsDir, "*.log"))
            {
                try
                {
                    if (File.GetLastWriteTime(file) < DateTime.Now.AddDays(-KeepLogsForDays))
                        File.Delete(file);
                }
                catch
                {
                    // a locked or unreadable file is not worth failing startup over
                }
            }
        }
    }
}
