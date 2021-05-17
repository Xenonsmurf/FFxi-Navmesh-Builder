// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 04-26-2021
// ***********************************************************************
// <copyright file="Zone.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Xml.Serialization;

namespace Ffxi_Navmesh_Builder.Common
{
    /// <summary>
    /// Class Zones.
    /// </summary>
    public class Zones
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [XmlAttribute("ID")] public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("Name")] public string Name { get; set; }
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        [XmlAttribute("Path")] public string Path { get; set; }
    }
}