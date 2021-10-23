// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 04-29-2021
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
        public string GetRomPath(int fId, IList<(int RomIndex, string Vtable, string Ftable)> tableDirectory)
        {
            try
            {
                for (var i = 0; i < tableDirectory.Count; i++)
                {
                    var vreader = new BinaryReader(File.OpenRead(tableDirectory[i].Vtable));
                    if (fId > vreader.BaseStream.Length - 1) continue;
                    var vTableOffset = fId;
                    vreader.BaseStream.Seek(vTableOffset, SeekOrigin.Begin);
                    var vTableValue = vreader.ReadByte();
                    vreader.Close();
                    vreader.Dispose();
                    var freader = new BinaryReader(File.OpenRead(tableDirectory[i].Ftable));
                    //fTable lookup fTableOffset = FileID * 2
                    var fTableOffset = fId * 2;
                    freader.BaseStream.Seek(fTableOffset, SeekOrigin.Begin);
                    Console.ForegroundColor = ConsoleColor.Green;
                    var fvalue = freader.ReadUInt16();
                    freader.Close();
                    freader.Dispose();
                    switch (vTableValue)
                    {
                        case 0:
                            continue;
                        case 1:
                            return $@"ROM/{fvalue >> 7}/{fvalue & 0x7F}.DAT";

                        default:
                            return $@"ROM{vTableValue}/{fvalue >> 7}/{fvalue & 0x7F}.DAT";
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(ParseZoneModelDat));
                Log.AddDebugText(Main.RtbDebug, $@"{ex} > {nameof(ParseZoneModelDat)}");
                return "";
            }
        }

        /// <summary>
        /// Initializes the table directory.
        /// </summary>
        public void InitializeTableDirectory()
        {
            var vtable = $@"{InstallPath}VTABLE.DAT";
            var ftable = $@"{InstallPath}FTABLE.DAT";
            var vtable2 = $@"{InstallPath}ROM2/VTABLE2.DAT";
            var ftable2 = $@"{InstallPath}ROM2/FTABLE2.DAT";
            var vtable3 = $@"{InstallPath}ROM3/VTABLE3.DAT";
            var ftable3 = $@"{InstallPath}ROM3/FTABLE3.DAT";
            var vtable4 = $@"{InstallPath}ROM4/VTABLE4.DAT";
            var ftable4 = $@"{InstallPath}ROM4/FTABLE4.DAT";
            var vtable5 = $@"{InstallPath}ROM5/VTABLE5.DAT";
            var ftable5 = $@"{InstallPath}ROM5/FTABLE5.DAT";
            var vtable6 = $@"{InstallPath}ROM6/VTABLE6.DAT";
            var ftable6 = $@"{InstallPath}ROM6/FTABLE6.DAT";
            var vtable7 = $@"{InstallPath}ROM7/VTABLE7.DAT";
            var ftable7 = $@"{InstallPath}ROM7/FTABLE7.DAT";
            var vtable8 = $@"{InstallPath}ROM8/VTABLE8.DAT";
            var ftable8 = $@"{InstallPath}ROM8/FTABLE8.DAT";
            var vtable9 = $@"{InstallPath}ROM9/VTABLE9.DAT";
            var ftable9 = $@"{InstallPath}ROM9/FTABLE9.DAT";
            TableDirectory = new (int RomIndex, string Vtable, string Ftable)[]
          {
                (1, vtable, ftable),
                (2,  vtable2, ftable2),
                (3,  vtable3, ftable3),
                (4,  vtable4, ftable4),
                (5,  vtable5, ftable5),
                (6,  vtable6, ftable6),
                (7,  vtable7, ftable7),
                (8,  vtable8, ftable8),
                (9,  vtable9, ftable9)
          };
        }
    }
}