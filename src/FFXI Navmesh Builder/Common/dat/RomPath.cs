// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 16/02/2023
// ***********************************************************************
// <copyright file="RomPath.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using FFXI_Navmesh_Builder.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Ffxi_Navmesh_Builder.Common.dat
{
    /// <summary>
    /// Class RomPath.
    /// </summary>
    public class RomPath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RomPath"/> class.
        /// </summary>
        /// <param name="ffxipath">The ffxipath.</param>
        /// <param name="l">The l.</param>
        /// <param name="mf">The mf.</param>
        public RomPath(string ffxipath, Log l, HomeView mf)
        {
            Log = l;
            Main = mf;
            InstallPath = ffxipath;

            InitializeTableDirectory();
        }

        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <value>The log.</value>
        public Log Log { get; set; }

        /// <summary>
        /// Gets or sets the main.
        /// </summary>
        /// <value>The main.</value>
        public HomeView Main { get; set; }

        /// <summary>
        /// Gets or sets the table directory.
        /// </summary>
        /// <value>The table directory.</value>
        public (int RomIndex, string Vtable, string Ftable)[] TableDirectory { get; set; }

        /// <summary>
        /// Gets or sets the install path.
        /// </summary>
        /// <value>The install path.</value>
        public string InstallPath { get; set; }

        /// <summary>
        /// Gets the rom path.
        /// </summary>
        /// <param name="fId">The f identifier.</param>
        /// <param name="tableDirectory">The table directory.</param>
        /// <returns>System.String.</returns>
        ///
        public string GetRomPath(int FileID, IList<(int RomIndex, string Vtable, string Ftable)> TableDirectory)
        {
            try
            {
                for (var i = 0; i < TableDirectory.Count; i++)
                {
                    var vData = File.ReadAllBytes($@"{TableDirectory[i].Vtable}").AsSpan();
                    if (FileID > vData.Length) continue;
                    var vTableValue = vData[FileID];
                    var fData = File.ReadAllBytes($@"{TableDirectory[i].Ftable}").AsSpan();
                    var fTableOffset = FileID * 2;
                    var fTableValue = MemoryMarshal.Read<UInt16>(fData[fTableOffset..]);
                    switch (vTableValue)
                    {
                        case 0:
                            continue;
                        case 1:
                            return $@"ROM\{fTableValue >> 7}\{fTableValue & 0x7F}.DAT";

                        default:
                            return $@"ROM{vTableValue}\{fTableValue >> 7}\{fTableValue & 0x7F}.DAT";
                    }
                }
                return "NULL";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "NULL";
            }
        }

        /// <summary>
        /// Initializes the table directory.
        /// </summary>
        public void InitializeTableDirectory()
        {
            var tablePaths = new string[,]
            {
        { "", "VTABLE.DAT", "FTABLE.DAT" },
        { "ROM2/", "VTABLE2.DAT", "FTABLE2.DAT" },
        { "ROM3/", "VTABLE3.DAT", "FTABLE3.DAT" },
        { "ROM4/", "VTABLE4.DAT", "FTABLE4.DAT" },
        { "ROM5/", "VTABLE5.DAT", "FTABLE5.DAT" },
        { "ROM6/", "VTABLE6.DAT", "FTABLE6.DAT" },
        { "ROM7/", "VTABLE7.DAT", "FTABLE7.DAT" },
        { "ROM8/", "VTABLE8.DAT", "FTABLE8.DAT" },
        { "ROM9/", "VTABLE9.DAT", "FTABLE9.DAT" }
            };

            var tableDirectory = new List<(int RomIndex, string Vtable, string Ftable)>();

            for (int i = 0; i < tablePaths.GetLength(0); i++)
            {
                var vtable = $@"{InstallPath}{tablePaths[i, 0]}{tablePaths[i, 1]}";
                var ftable = $@"{InstallPath}{tablePaths[i, 0]}{tablePaths[i, 2]}";
                tableDirectory.Add((i + 1, vtable, ftable));
            }

            TableDirectory = tableDirectory.ToArray();
        }
    }
}