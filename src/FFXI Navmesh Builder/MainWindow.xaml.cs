// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-28-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-13-2021
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using FFXI_Navmesh_Builder.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;

namespace FFXI_Navmesh_Builder
{
    /// <summary>
    /// Class MainWindow.
    /// Implements the <see cref="MahApps.Metro.Controls.MetroWindow" />
    /// Implements the <see cref="System.Windows.Markup.IComponentConnector" />
    /// </summary>
    /// <seealso cref="MahApps.Metro.Controls.MetroWindow" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            main = new MainViewModel(this);
            InitializeComponent();

            DataContext = new MainViewModel(this);

            ChangeView(0, this.ToString(), null);
        }

        /// <summary>
        /// Gets or sets the main.
        /// </summary>
        /// <value>The main.</value>
        public MainViewModel main { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [run once].
        /// </summary>
        /// <value><c>true</c> if [run once]; otherwise, <c>false</c>.</value>
        public bool RunOnce { get; set; }

        /// <summary>
        /// Changes the view.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="ex">The ex.</param>
        public void ChangeView(int type, string name, Exception ex)
        {
            if ((!(this.DataContext is MainViewModel vm)) || !vm.UpdateViewCommand.CanExecute(null)) return;
            switch (type)
            {
                case 0:
                    vm.UpdateViewCommand.Execute("Initialize");
                    break;

                case 1:
                    vm.UpdateViewCommand.Execute("Home");
                    break;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public sealed override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Launches the git hub issue site.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LaunchGitHubIssueSite(object sender, RoutedEventArgs e)
        {
            OpenWebPage("https://github.com/xenonsmurf/Ffxi_Navmesh_Builder/issues");
        }

        /// <summary>
        /// Launches the git hub site.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LaunchGitHubSite(object sender, RoutedEventArgs e)
        {
            OpenWebPage("http://github.com/xenonsmurf/Ffxi_Navmesh_Builder");
        }

        /// <summary>
        /// Opens the web page.
        /// </summary>
        /// <param name="url">The URL.</param>
        private void OpenWebPage(string url)
        {
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = url
            };
            Process.Start(psi);
        }
    }
}