// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-14-2021
// ***********************************************************************
// <copyright file="dat.cs" company="Xenonsmurf">
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
using System.Xml.Serialization;

namespace Ffxi_Navmesh_Builder.Common.dat
{
    /// <summary>
    /// Class dat.
    /// </summary>
    public class dat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="dat"/> class.
        /// </summary>
        /// <param name="l">The l.</param>
        /// <param name="mf">The mf.</param>
        /// <param name="ffxIpath">The FFX ipath.</param>
       
        public dat(Log l, HomeView mf, string ffxIpath)
        {
            try
            {
                Log = l;
                Main = mf;
                InstallPath = ffxIpath;
                RomPath = new RomPath(ffxIpath, l, mf);
                Dms = new d_ms(l, mf, RomPath);
                Entity = new Entity();
                Zones = new List<Zones>();
                _DatTypes = new List<Zones>();
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(dat));
                Log.AddDebugText(Main.RtbDebug, $@"{ex} > {nameof(dat)}");
            }
        }
        public void ChangePath(string path)
        {
            InstallPath = path;
            RomPath.InstallPath = path;
            RomPath.InitializeTableDirectory();
            Dms.ChangePath(path);
        }
        /// <summary>
        /// Gets or sets the DMS.
        /// </summary>
        /// <value>The DMS.</value>
        public d_ms Dms { get; set; }
        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public Entity Entity { get; set; }
        /// <summary>
        /// Gets or sets the zones.
        /// </summary>
        /// <value>The zones.</value>
        public List<Zones> Zones { get; set; }
        /// <summary>
        /// Gets or sets the dat types.
        /// </summary>
        /// <value>The dat types.</value>
        private List<Zones> _DatTypes { get; set; }
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
        /// Dumps to XML.
        /// </summary>
        public void DumpToXml()
        {
            try
            {
                var path = ($@"{AppDomain.CurrentDomain.BaseDirectory}");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!Directory.Exists(path)) return;
                var outFile = File.Create($@"{path}\\datTypes.xml");
                var formatter = new XmlSerializer(_DatTypes.GetType());
                formatter.Serialize(outFile, _DatTypes);
                outFile.Close();
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(dat));
                Log.AddDebugText(Main.RtbDebug, $@"{ex} > {nameof(dat)}");
            }
        }

        /// <summary>
        /// Parses the dat.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        public void ParseDat(int fileId)
        {
            var datPath = RomPath.GetRomPath(fileId, RomPath.TableDirectory);
            var data = File.ReadAllBytes($@"{InstallPath }{datPath}").AsSpan();
            if (data.Length <= 0) return;
            var value = MemoryMarshal.Read<int>(data[4..]);
            var identifier = Encoding.ASCII.GetString(data.Slice(0, 4)).TrimStart('\0').TrimEnd('\0');
            switch (identifier)
            {
                case "d_ms":
                    //I have only set this up for the English ZoneList.dat.
                    Dms.ParseD_MSG(data);
                    break;

                case "none":
                    Entity.ParseNpcDat(data);
                    break;

                default:
                    break;
            }
        }
    }
}