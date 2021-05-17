// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 04-26-2021
// ***********************************************************************
// <copyright file="subRegion.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Ffxi_Navmesh_Builder.Common.dat
{
    /// <summary>
    /// Class SubRegion.
    /// </summary>
    public class SubRegion
    {
        /// <summary>
        /// Gets or sets the file identifier.
        /// </summary>
        /// <value>The file identifier.</value>
        public int FileId { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets or sets the rom path.
        /// </summary>
        /// <value>The rom path.</value>
        public string RomPath { get; set; }
        /// <summary>
        /// Gets or sets the rotation x.
        /// </summary>
        /// <value>The rotation x.</value>
        public float RotationX { get; set; }
        /// <summary>
        /// Gets or sets the rotation y.
        /// </summary>
        /// <value>The rotation y.</value>
        public float RotationY { get; set; }
        /// <summary>
        /// Gets or sets the rotation z.
        /// </summary>
        /// <value>The rotation z.</value>
        public float RotationZ { get; set; }
        /// <summary>
        /// Gets or sets the scale x.
        /// </summary>
        /// <value>The scale x.</value>
        public float ScaleX { get; set; }
        /// <summary>
        /// Gets or sets the scale y.
        /// </summary>
        /// <value>The scale y.</value>
        public float ScaleY { get; set; }
        /// <summary>
        /// Gets or sets the scale z.
        /// </summary>
        /// <value>The scale z.</value>
        public float ScaleZ { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the unknown.
        /// </summary>
        /// <value>The unknown.</value>
        public int Unknown { get; set; }
        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public float X { get; set; }
        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public float Y { get; set; }
        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>The z.</value>
        public float Z { get; set; }
    }
}