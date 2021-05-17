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
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Ffxi_Navmesh_Builder.Common
{
    /// <summary>
    /// Class Log.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Adds the debug text.
        /// </summary>
        /// <param name="tb">The tb.</param>
        /// <param name="text">The text.</param>
        public void AddDebugText(ListBox tb, string text)
        {
            tb.Dispatcher.Invoke(new Action(() =>
            {
                var time = DateTime.Now;
                var logEntry = $@"{time:HH:mm:ss} {text}";
                tb.Items.Add(logEntry);
                tb.SelectedIndex = tb.Items.Count - 1;
                tb.ScrollIntoView(tb.SelectedItem);
            }));
        }

        /// <summary>
        /// Clears the log.
        /// </summary>
        public void ClearLog()
        {
            try
            {
                if (File.Exists("Log.Bin"))
                {
                    var fi = new FileInfo("Log.Bin");
                    if (fi.LastAccessTime < DateTime.Now.AddDays(-7))
                        using (new StreamWriter(fi.Open(FileMode.Truncate)))
                        {
                        }
                }
                else if (!File.Exists("Log.Bin"))
                {
                    using (File.CreateText("Log.Bin"))
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                using (TextWriter writer = File.CreateText("Log.Bin"))
                {
                    writer.Write(ex);
                }
            }
        }

        /// <summary>
        /// Logs the file.
        /// </summary>
        /// <param name="sExceptionName">Name of the s exception.</param>
        /// <param name="sFormName">Name of the s form.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="caller">The caller.</param>
        public void LogFile(string sExceptionName, string sFormName, [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null)
        {
            try
            {
                if (File.Exists("Log.Bin"))
                {
                    using (TextWriter tw = File.AppendText("Log.Bin"))
                    {
                        tw.WriteLine(
                            $"{DateTime.Now.ToString(CultureInfo.InvariantCulture)}, {sExceptionName}, at line {lineNumber.ToString()},({caller}) ,{sFormName}");
                        tw.Close();
                    }
                }
                else if (!File.Exists("Log.Bin"))
                {
                    using (File.CreateText("Log.Bin"))
                    {
                    }

                    using (TextWriter tw = File.AppendText("Log.Bin"))
                    {
                        tw.WriteLine(
                            $@"{DateTime.Now.ToString(CultureInfo.InvariantCulture)}, {sExceptionName}, at line {lineNumber.ToString()},({caller}) ,{sFormName}");
                        tw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                using (TextWriter tw = File.CreateText("Log.Bin"))
                {
                    tw.WriteLine(
                        $@"{ DateTime.Now.ToString(CultureInfo.InvariantCulture)}, {ex}, at line {lineNumber.ToString()},({caller}) ,{sFormName}");
                    tw.Close();
                }
            }
        }
    }
}