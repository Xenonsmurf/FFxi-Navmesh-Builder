// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-13-2021
// ***********************************************************************
// <copyright file="ParseZoneModelDat.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using Ffxi_Navmesh_Builder.Common.dat.Types;
using FFXI_Navmesh_Builder.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Ffxi_Navmesh_Builder.Common.dat
{
    /// <summary>
    /// Class ParseZoneModelDat.
    /// </summary>
    public class ParseZoneModelDat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseZoneModelDat"/> class.
        /// </summary>
        /// <param name="l">The l.</param>
        /// <param name="mf">The mf.</param>
        /// <param name="zid">The zid.</param>
        /// <param name="zname">The zname.</param>
        /// <param name="ffxIpath">The FFX ipath.</param>
        /// <param name="dumpSRinfo">if set to <c>true</c> [dump s rinfo].</param>
        public ParseZoneModelDat(Log l, HomeView mf, int zid, string zname, string ffxIpath, bool dumpSRinfo)
        {
            try
            {
                Log = l;
                Main = mf;
                ZoneId = zid;
                FileName = zname;
                InstallPath = ffxIpath;
                DumpSRtoXml = dumpSRinfo;
                RomPath = new RomPath(ffxIpath, l, mf);
                Mzb = new Mzb(l, mf);
                Mmb = new Mmb(l, mf);
                Rid = new Rid(l, mf, ffxIpath, RomPath);
                Chunks = new List<DatChunk>();
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(ParseZoneModelDat));
                Log.AddDebugText(Main.RtbDebug, $@"{ex} > {nameof(ParseZoneModelDat)}");
            }
        }
        public void ChangePath(string path)
        {
            InstallPath = path;
            RomPath.InstallPath = path;
        }

        /// <summary>
        /// Gets or sets the chunks.
        /// </summary>
        /// <value>The chunks.</value>
        public List<DatChunk> Chunks { get; set; }
        /// <summary>
        /// Gets or sets the MMB.
        /// </summary>
        /// <value>The MMB.</value>
        public Mmb Mmb { get; set; }
        /// <summary>
        /// Gets or sets the MZB.
        /// </summary>
        /// <value>The MZB.</value>
        public Mzb Mzb { get; set; }
        /// <summary>
        /// Gets or sets the rid.
        /// </summary>
        /// <value>The rid.</value>
        public Rid Rid { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [dump s rto XML].
        /// </summary>
        /// <value><c>true</c> if [dump s rto XML]; otherwise, <c>false</c>.</value>
        private bool DumpSRtoXml { get; set; }
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        private string FileName { get; set; }
        /// <summary>
        /// Gets or sets the install path.
        /// </summary>
        /// <value>The install path.</value>
        private string InstallPath { get; set; }
        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <value>The log.</value>
        private Log Log { get; set; }
        /// <summary>
        /// Gets or sets the main.
        /// </summary>
        /// <value>The main.</value>
        private HomeView Main { get; set; }
        /// <summary>
        /// Gets or sets the rom path.
        /// </summary>
        /// <value>The rom path.</value>
        private RomPath RomPath { get; set; }
        /// <summary>
        /// Gets or sets the zone identifier.
        /// </summary>
        /// <value>The zone identifier.</value>
        private int ZoneId { get; set; }

        /// <summary>
        /// Loads the dat.
        /// </summary>
        /// <param name="datPath">The dat path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool LoadDat(string datPath)
        {
            try
            {
                Chunks.Clear();
                var data = File.ReadAllBytes(datPath).AsSpan();
                var position = 0;
                while (position < data.Length)
                {
                    var name = Encoding.UTF8.GetString(data.Slice(position, 4)).TrimEnd('\0');
                    position += 4;
                    var value = MemoryMarshal.Read<uint>(data[position..]);
                    var type = (ResourceType)(value & 0x7F);
                    var size = 16 * ((value >> 7) & 0x7FFFF) - 16;
                    position += 12;
                    var block = data.Slice(position, (int)size);

                    switch (type)
                    {
                        //dont need mmb for collision mesh
                        case ResourceType.Mmb:
                            // Mmb.DecodeMmb(block);
                            break;

                        case ResourceType.Mzb:
                            //testing  code from ida
                            // Mzb.sub_46B7A80(block,1);
                            //testing  code from ida
                            Mzb.DecodeMzb(block);
                            Mzb.ParseMzb(block);
                            break;

                        case ResourceType.Rid:
                            Rid.ParseRid(block, ZoneId);
                            break;
                    }
                    Chunks.Add(new DatChunk { Name = name, Data = block.ToArray(), Type = type, Size = size });
                    position += block.Length;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(ParseZoneModelDat));
                Log.AddDebugText(Main.RtbDebug, $@"{ex} > {nameof(ParseZoneModelDat)}");
                return false;
            }
        }
    }
}