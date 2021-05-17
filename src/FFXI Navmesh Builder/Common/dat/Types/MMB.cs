// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 04-29-2021
// ***********************************************************************
// <copyright file="MMB.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using FFXI_Navmesh_Builder.Views;
using System;

namespace Ffxi_Navmesh_Builder.Common.dat.Types
{
    /// <summary>
    /// Class Mmb.
    /// </summary>
    public class Mmb
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mmb"/> class.
        /// </summary>
        /// <param name="l">The l.</param>
        /// <param name="mf">The mf.</param>
        public Mmb(Log l, HomeView mf)
        {
            Log = l;
            Main = mf;
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
        /// Decodes the MMB.
        /// </summary>
        /// <param name="data">The data.</param>
        public void DecodeMmb(Span<byte> data)
        {
            try
            {
                if (data[3] >= 5)
                {
                    var len = data.Length;
                    var key = (int)KeyTables.KeyTable[data[5] ^ 0xf0];
                    var keyCount = 0;
                    for (var pos = 8; pos < len; ++pos)
                    {
                        var x = ((key & 0xFF) << 8) | (key & 0xFF);
                        key += ++keyCount;

                        data[pos] ^= (byte)(x >> (key & 7));
                        key += ++keyCount;
                    }
                }

                if (data[6] != 0xFF || data[7] != 0xFF) return;
                {
                    var len = data.Length;
                    var key1 = data[5] ^ 0xf0;
                    int key2 = KeyTables.KeyTable2[key1];
                    var len2 = ((len - 8) & ~0xf) >> 1;

                    var offset1 = 8;
                    var offset2 = offset1 + len2;

                    var tmp = new byte[8];

                    for (var i = 0; i < len2; i += 8)
                    {
                        if ((key2 & 1) == 1)
                        {
                            Buffer.BlockCopy(data.ToArray(), offset1, tmp, 0, 8);
                            Buffer.BlockCopy(data.ToArray(), offset2, data.ToArray(), offset1, 8);
                            Buffer.BlockCopy(tmp, 0, data.ToArray(), (int)offset2, 8);
                        }

                        key1 += 9;
                        key2 += key1;
                        offset1 += 8;
                        offset2 += 8;
                    }
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