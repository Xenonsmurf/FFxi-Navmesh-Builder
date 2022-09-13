// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-05-2021
// ***********************************************************************
// <copyright file="d_ms.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using FFXI_Navmesh_Builder.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Ffxi_Navmesh_Builder.Common.dat.Types
{
    /// <summary>
    /// Class d_ms.
    /// </summary>
    public class d_ms
    {
      

        /// <summary>
        /// Initializes a new instance of the <see cref="d_ms"/> class.
        /// </summary>
        /// <param name="l">The l.</param>
        /// <param name="mf">The mf.</param>
        /// <param name="rp">The rp.</param>
        public d_ms(Log l, HomeView mf, RomPath rp)
        {
            Log = l;
            Main = mf;
            romPath = rp;
            _zones = new ObservableCollection<Zones>();
        }

        /// <summary>
        /// Gets or sets the zones.
        /// </summary>
        /// <value>The zones.</value>
        public ObservableCollection<Zones> _zones { get; set; }
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
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        private int position { get; set; } = 0;
        /// <summary>
        /// Gets or sets the rom path.
        /// </summary>
        /// <value>The rom path.</value>
        private RomPath romPath { get; set; }
        public void ChangePath(string path)
        {
            romPath.InstallPath = path;
        }
      

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
                var outFile = File.Create($@"{path}\\ZoneList.xml");
                var formatter = new XmlSerializer(_zones.GetType());
                formatter.Serialize(outFile, _zones);
                outFile.Close();
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(ParseZoneModelDat));
                Log.AddDebugText(Main.RtbDebug, $@"{ex} > {nameof(ParseZoneModelDat)}");
            }
        }
        public unsafe struct d_msg_header_t
        {
            public fixed byte d_msg[8];  //'d_msg' string denoting the file type.
            public ushort HeaderDecoded;  // Flag if the file header is already decoded. (1 = Decoded, Else = Encoded via rotation.)
            public ushort DataEncoded;    // Flag if the string data of the file is bitwise 'NOT' encoded.
            public ushort Unknown0000;    // Unknown flag that deals with how the first entry is handled. (Always 3 in current client.)
            public ushort Unknown0001;    // Unknown flag that works with Unknown0000 if it is not set to 3. (Not used in any DAT file currently.)
            public uint Unknown0002;    // Unknown flag that works with HeaderDecoded if it is not set to 1. (Seems to be unused now, no reference in latest client.)
            public uint FileSize;       // The size of the entire file.
            public uint HeaderSize;     // The size of the header.
            public uint ToCSize;        // The size of the table of contents.
            public uint EntrySize;      // The size of each entry.
            public uint DataSize;       // The size of data in the file. (FileSize - HeaderSize)
            public uint EntryCount;     // The number of entries within the file.
            public uint Unknown0003;    // Unknown flag that works with HeaderDecoded if it is not set to 1.
            public fixed byte Unknown0004[16];// Unknown
        };

        public unsafe struct d_msg_entry_t
        {
            public uint Offset { get; set; }
            public uint Length { get; set; }
        };

        public unsafe struct d_msg_entryheader_t
        {
            public uint Count;
            public uint[] Offset { set; get; }
            public uint[] Flag { get; set; }
        };

        public string? text { get; set; }

        /// <summary>
        /// Parses the d MSG.
        /// </summary>
        /// <param name="data">The data.</param>
        public unsafe void ParseD_MSG(Span<byte> data)
        {
          try
            {
                if (data.Length > sizeof(d_msg_header_t))
                {
                    var header = MemoryMarshal.Read<d_msg_header_t>(data);
                    if (header.DataEncoded == 1)
                    {
                        for (var x = sizeof(d_msg_header_t); x < header.FileSize; x++)
                        {
                            data[x] ^= 0xff;
                        }
                    }
                    for (var x = 0; x < header.EntryCount; x++)
                    {
                        d_msg_entryheader_t entry = new();
                        int entryId = x;
                        if (header.ToCSize > 0)
                        {
                            d_msg_entry_t temp = new();
                            int pos = x * 8;
                            temp.Offset = MemoryMarshal.Read<uint>(data[((int)header.HeaderSize + pos)..]);
                            temp.Length = MemoryMarshal.Read<uint>(data[((int)header.HeaderSize + (pos + 4))..]);

                            if (temp.Length < 0 || temp.Offset < 0 || temp.Offset + temp.Length > header.DataSize)
                            {
                                break;
                            }
                            var position = header.HeaderSize + header.ToCSize + temp.Offset;
                            var tempData = data.Slice((int)position, (int)temp.Length);

                            int index = 0;
                            entry.Count = MemoryMarshal.Read<uint>(tempData[index..]);
                            entry.Offset = new uint[entry.Count];
                            entry.Flag = new uint[entry.Count];
                            index += 4;
                            for (int i = 0; i < entry.Count; i++)
                            {
                                entry.Offset[i] = MemoryMarshal.Read<uint>(tempData[index..]);
                                index += 4;
                                entry.Flag[i] = MemoryMarshal.Read<uint>(tempData[index..]);
                                index += 4;

                                if (entry.Flag[i] > 0 && entry.Offset[i] > 0)
                                {
                                    entryId = MemoryMarshal.Read<int>(tempData[(int)entry.Offset[i]..]);
                                }
                            }
                            for (int y = 0; y < entry.Count; y++)
                            {
                                if (entry.Flag[y] != 0)
                                    continue;
                                uint offset2 = MemoryMarshal.Read<uint>(tempData[((int)entry.Offset[y] + 0x1c)..]);
                                if (entry.Offset[y] == 0 || offset2 == 0)
                                    continue;
                                var start = Convert.ToUInt32(position + (entry.Offset[y] + 0x1c));
                                var textstart = Convert.ToUInt32((entry.Offset[y]) + 0x1c);
                                var end = (tempData.Length - (int)(entry.Offset[y] + 0x1c));
                                var textend = Convert.ToUInt32(position + temp.Length);
                                List<byte> TextBytes = new List<byte>();
                                while (true)
                                {
                                    byte[] FourBytes = tempData.Slice(((int)textstart), 4).ToArray();
                                    textstart += 4;

                                    TextBytes.AddRange(FourBytes);
                                    if (FourBytes[3] == 0)
                                    {
                                        break;
                                    }
                                }
                                text = Encoding.ASCII.GetString(TextBytes.ToArray()).Trim('\0');

                                var fileId = x < 256 ? x + 100 : x + 83635;


                                _zones.Add(new Zones { Id = x, Name = text, Path = romPath.GetRomPath(fileId, romPath.TableDirectory) });
                            }
                        }
                    }

                }

                    var sortedList = new ObservableCollection<Zones>(_zones.OrderBy(x => x.Id).ToList());
                _zones = sortedList;
            }
            catch (Exception ex)
            {
                Log.LogFile(ex.ToString(), nameof(d_ms));
                Log.AddDebugText(Main.RtbDebug, $@"{ex} > {nameof(d_ms)}");
            }
        }
    }
}