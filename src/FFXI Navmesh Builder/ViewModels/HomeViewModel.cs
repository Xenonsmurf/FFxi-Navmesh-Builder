// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-28-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-13-2021
// ***********************************************************************
// <copyright file="HomeViewModel.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using FFXI_Navmesh_Builder.Views;

namespace FFXI_Navmesh_Builder.ViewModels
{
    /// <summary>
    /// Class HomeViewModel.
    /// Implements the <see cref="FFXI_Navmesh_Builder.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="FFXI_Navmesh_Builder.ViewModels.BaseViewModel" />
    public class HomeViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        /// <param name="mf">The mf.</param>
        /// <param name="main">The main.</param>
        /// <param name="hv">The hv.</param>
        public HomeViewModel(MainViewModel mf, MainWindow main, HomeView hv)
        {
            Mf = mf;
            Main = main;
            Hv = hv;
        }

        /// <summary>
        /// Gets the hv.
        /// </summary>
        /// <value>The hv.</value>
        public HomeView Hv { get; }
        /// <summary>
        /// Gets the main.
        /// </summary>
        /// <value>The main.</value>
        public MainWindow Main { get; }
        /// <summary>
        /// Gets the mf.
        /// </summary>
        /// <value>The mf.</value>
        public MainViewModel Mf { get; }
    }
}