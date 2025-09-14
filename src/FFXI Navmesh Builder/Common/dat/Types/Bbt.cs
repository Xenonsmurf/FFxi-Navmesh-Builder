// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 04-26-2021
// ***********************************************************************
// <copyright file="Bbt.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ffxi_Navmesh_Builder.Common.dat.Types
{
    /// <summary>
    /// Class Bbt.
    /// </summary>
    public class Bbt
    {
        /// <summary>
        /// The children
        /// </summary>
        public List<Bbt> Children = new List<Bbt>();

        /// <summary>
        /// The matches
        /// </summary>
        public List<uint> Matches = new List<uint>();

        /// <summary>
        /// The matchids
        /// </summary>
        public List<int> Matchids = new List<int>();

        /// <summary>
        /// The x1
        /// </summary>
        public float X1 = 99999, Y1 = 99999, Z1 = 99999;

        /// <summary>
        /// The x2
        /// </summary>
        public float X2 = -99999, Y2 = -99999, Z2 = -99999;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bbt"/> class.
        /// </summary>
        /// <param name="f">The f.</param>
        public Bbt(float[] f)
        {
            for (var i = 0; i < 4; i++)
            {
                X1 = Math.Min(X1, f[i * 4 + 0]);
                X2 = Math.Max(X2, f[i * 4 + 0]);
                Y1 = Math.Min(Y1, f[i * 4 + 1]);
                Y2 = Math.Max(Y2, f[i * 4 + 1]);
                Z1 = Math.Min(Z1, f[i * 4 + 2]);
                Z2 = Math.Max(Z2, f[i * 4 + 2]);
            }
        }

        /// <summary>
        /// Finds the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <returns>Bbt.</returns>
        public Bbt Find(float x, float y, float z)
        {
            foreach (var child in Children.Where(child => x >= child.X1 && x <= child.X2 && y >= child.Y1 && y <= child.Y2 && z >= child.Z1 &&
                                                          z <= child.Z2))
                return child.Find(x, y, z);

            if (x >= X1 && x <= X2 && y >= Y1 && y <= Y2 && z >= Z1 && z <= Z2)
                return this;
            else
                return null;
        }
    }
}