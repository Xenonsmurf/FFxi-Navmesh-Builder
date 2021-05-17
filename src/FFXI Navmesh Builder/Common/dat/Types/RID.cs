// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-12-2021
// ***********************************************************************
// <copyright file="RID.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using FFXI_Navmesh_Builder.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace Ffxi_Navmesh_Builder.Common.dat.Types
{
    /// <summary>
    /// Class Rid.
    /// </summary>
    public class Rid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rid"/> class.
        /// </summary>
        /// <param name="l">The l.</param>
        /// <param name="mf">The mf.</param>
        /// <param name="ffxipath">The ffxipath.</param>
        /// <param name="rm">The rm.</param>
        public Rid(Log l, HomeView mf, string ffxipath, RomPath rm)
        {
            Log = l;
            Main = mf;
            InstallPath = ffxipath;
            RomPath = rm;
            SubRegions = new BindingList<SubRegion>();
            SrSubRegions = new List<SubRegion>();
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
        /// Gets or sets the sub region model.
        /// </summary>
        /// <value>The sub region model.</value>
        public int SubRegionModel { get; set; }
        /// <summary>
        /// Gets or sets the sub regions.
        /// </summary>
        /// <value>The sub regions.</value>
        public BindingList<SubRegion> SubRegions { get; set; }
        /// <summary>
        /// Gets or sets the install path.
        /// </summary>
        /// <value>The install path.</value>
        private string InstallPath { get; set; }
        /// <summary>
        /// Gets or sets the rom path.
        /// </summary>
        /// <value>The rom path.</value>
        private RomPath RomPath { get; set; }
        /// <summary>
        /// Gets or sets the sr sub regions.
        /// </summary>
        /// <value>The sr sub regions.</value>
        private List<SubRegion> SrSubRegions { get; set; }

        /// <summary>
        /// Dumps to XML.
        /// </summary>
        /// <param name="zone">The zone.</param>
        public void DumpToXml(int zone)
        {
            try
            {
                var path = ($@"{AppDomain.CurrentDomain.BaseDirectory}Sub Region Info");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!Directory.Exists(path)) return;
                var outFile = File.Create($@"{path}\\ZoneID_{zone}_SubRegions.xml");
                var formatter = new XmlSerializer(SubRegions.GetType());
                formatter.Serialize(outFile, SubRegions);
                outFile.Close();
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(ParseZoneModelDat));
                Log.AddDebugText(Main.RtbDebug, $@"{ex} > {nameof(ParseZoneModelDat)}");
            }
        }

        /// <summary>
        /// Parses the rid.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="id">The identifier.</param>
        public void ParseRid(Span<byte> block, int id)
        {
            try
            {
                var count = MemoryMarshal.Read<int>(block[0x30..]);
                for (var i = 0; i < count; i++)
                {
                    var temp = new SubRegion
                    {
                        X = MemoryMarshal.Read<Single>(block[(0x40 + i * 0x40 + 0x00)..]),
                        Y = MemoryMarshal.Read<Single>(block[(0x40 + i * 0x40 + 0x04)..]),
                        Z = MemoryMarshal.Read<Single>(block[(0x40 + i * 0x40 + 0x08)..]),
                        RotationX = MemoryMarshal.Read<Single>(block[(0x40 + i * 0x40 + 0x0c)..]),
                        RotationY = MemoryMarshal.Read<Single>(block[(0x40 + i * 0x40 + 0x10)..]),
                        RotationZ = MemoryMarshal.Read<Single>(block[(0x40 + i * 0x40 + 0x14)..]),
                        ScaleX = MemoryMarshal.Read<Single>(block[(0x40 + i * 0x40 + 0x18)..]),
                        ScaleY = MemoryMarshal.Read<Single>(block[(0x40 + i * 0x40 + 0x1c)..]),
                        ScaleZ = MemoryMarshal.Read<Single>(block[(0x40 + i * 0x40 + 0x20)..])
                    };
                    var namebytes = new byte[8];
                    Buffer.BlockCopy(block.ToArray(), 0x40 + i * 0x40 + 0x24, namebytes, 0, 8);
                    var len = 0;
                    while (len < namebytes.Length && namebytes[len] != 0) len++;
                    len = 8;
                    temp.Identifier = Encoding.ASCII.GetString(namebytes, 0, len).Trim('\0');
                    if (temp.Identifier.Length > 0)
                    {
                        var sRtype = temp.Identifier.Substring(0, 1);
                        switch (sRtype)
                        {
                            case "Z":
                            case "z":
                                temp.Type = "ZoneLine";
                                break;

                            case "_":
                                temp.Type = "Door or Object";
                                break;

                            case "F":
                            case "f":
                                temp.Type = "Fishing area";
                                break;

                            case "@":
                                temp.Type = "Elevators";
                                break;

                            case "E":
                            case "e":
                                temp.Type = "Event";
                                break;

                            case "M":
                            case "m":

                                temp.Type = "Model";
                                temp.FileId = MemoryMarshal.Read<int>(block[(0x40 + i * 0x40 + 0x2c)..]);
                                var fileId = temp.FileId + 100;
                                temp.RomPath = ($@"{InstallPath}{RomPath.GetRomPath(fileId, RomPath.TableDirectory)}");
                                SubRegionModel++;
                                break;

                            default:
                                temp.Type = sRtype;
                                break;
                        }
                    }
                    temp.Unknown = MemoryMarshal.Read<int>(block[(0x40 + i * 0x40 + 0x30)..]);
                    {
                        var vx = temp.ScaleX / 2;
                        var vy = temp.ScaleY / 2;
                        var vz = temp.ScaleZ / 2;
                        var ry = -temp.RotationY;
                    }
                    SubRegions.Add(temp);
                }
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(ParseZoneModelDat));
                Log.AddDebugText(Main.RtbDebug, $@"{ex} > {nameof(ParseZoneModelDat)}");
            }
        }
    }
}