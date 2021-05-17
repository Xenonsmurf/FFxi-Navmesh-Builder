// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-14-2021
// ***********************************************************************
// <copyright file="Entity.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace Ffxi_Navmesh_Builder.Common.dat.Types
{
    /// <summary>
    /// Class Entity.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>The entities.</value>
        public ObservableCollection<Entity> _entities { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; init; }
        /// <summary>
        /// Gets or sets the server identifier.
        /// </summary>
        /// <value>The server identifier.</value>
        public uint ServerId { get; init; }
        /// <summary>
        /// Gets or sets the index of the target.
        /// </summary>
        /// <value>The index of the target.</value>
        public int TargetIndex { get; init; }
        /// <summary>
        /// Gets or sets the zone identifier.
        /// </summary>
        /// <value>The zone identifier.</value>
        public int ZoneId { get; init; }

        /// <summary>
        /// Dumps to XML.
        /// </summary>
        /// <param name="zone">The zone.</param>
        public void DumpToXml(int zone)
        {
            try
            {
                var path = ($@"{AppDomain.CurrentDomain.BaseDirectory}Entities");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!Directory.Exists(path)) return;
                var outFile = File.Create($@"{path}\\ZoneID_{zone}_Entities.xml");
                var formatter = new XmlSerializer(_entities.GetType());
                formatter.Serialize(outFile, _entities);
                outFile.Close();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Parses the NPC dat.
        /// </summary>
        /// <param name="data">The data.</param>
        public void ParseNpcDat(Span<byte> data)
        {
            _entities = new ObservableCollection<Entity>();

            for (var i = 0; i < data.Length;)
            {
                uint serverID;
                int targetIndex;
                int zoneId;
                var name = Encoding.ASCII.GetString(data.Slice(i, 28)).TrimEnd('\0');
                if (name == "none")
                {
                    serverID = 0;
                    targetIndex = 0;
                    zoneId = (int)((serverID >> 12) & 0xFFF);
                    _entities.Add(new Entity { Name = name, ServerId = serverID, TargetIndex = targetIndex, ZoneId = zoneId });
                    i += 32;
                }
                if (name != "none")
                {
                    serverID = MemoryMarshal.Read<uint>(data[(i + 28)..]);
                    targetIndex = (int)(serverID & 0xFFF);
                    zoneId = (int)((serverID >> 12) & 0xFFF);
                    _entities.Add(new Entity { Name = name, ServerId = serverID, TargetIndex = targetIndex, ZoneId = zoneId });
                    i += 32;
                }
            }
            data.Clear();
        }
    }
}