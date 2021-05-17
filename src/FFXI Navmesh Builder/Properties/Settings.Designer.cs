// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 05-14-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-14-2021
// ***********************************************************************
// <copyright file="Settings.Designer.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace FFXI_Navmesh_Builder.Properties {


    /// <summary>
    /// Class Settings. This class cannot be inherited.
    /// Implements the <see cref="System.Configuration.ApplicationSettingsBase" />
    /// </summary>
    /// <seealso cref="System.Configuration.ApplicationSettingsBase" />
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {

        /// <summary>
        /// The default instance
        /// </summary>
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>The default.</value>
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
    }
}
