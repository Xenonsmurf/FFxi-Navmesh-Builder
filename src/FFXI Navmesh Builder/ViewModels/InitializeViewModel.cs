// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-28-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-13-2021
// ***********************************************************************
// <copyright file="InitializeViewModel.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using FFXI_Navmesh_Builder.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace FFXI_Navmesh_Builder.ViewModels
{
    /// <summary>
    /// Class InitializeViewModel.
    /// Implements the <see cref="FFXI_Navmesh_Builder.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="FFXI_Navmesh_Builder.ViewModels.BaseViewModel" />
    public class InitializeViewModel : BaseViewModel
    {
        /// <summary>
        /// The required folders
        /// </summary>
        private readonly List<string> _requiredFolders = new List<string>
        { "Map Collision obj files", "Map Collision obj files","Dumped NavMeshes", "Sub Region Info", "Entities"};

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeViewModel"/> class.
        /// </summary>
        /// <param name="mf">The mf.</param>
        /// <param name="main">The main.</param>
        /// <param name="iv">The iv.</param>
        public InitializeViewModel(MainViewModel mf, MainWindow main, InitializeView iv)
        {
            MF = main;
            _ = InitializeAsync();
            Iv = iv;
        }

        /// <summary>
        /// Gets or sets the mf.
        /// </summary>
        /// <value>The mf.</value>
        public MainWindow MF { get; set; }

        /// <summary>
        /// Gets or sets the iv.
        /// </summary>
        /// <value>The iv.</value>
        private InitializeView Iv { get; set; }

        /// <summary>
        /// Checks the files.
        /// </summary>
        private async Task CheckFiles()
        {
            UpdateLabel("Hello!.");
            await Task.Delay(1000);
            UpdateLabel("Just checking we have all required folders and files.");
            await Task.Delay(1000);

            foreach (var folder in _requiredFolders)
            {
                string folderPath = $@"{Directory.GetCurrentDirectory()}\\{folder}";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    UpdateLabel(@$"Creating Directory {folder}.");
                    await Task.Delay(500);
                }
                else
                {
                    UpdateLabel(@$"Directory found {folder}.");
                }

                await Task.Delay(500);
            }

            string dllFilePath = $@"{Directory.GetCurrentDirectory()}\\FFXINAV.dll";

            if (!File.Exists(dllFilePath))
            {
                UpdateLabel("Missing FFXINAV.dll.");
                await Task.Delay(500);
                await DownloadFile("https://github.com/xenonsmurf/Ffxi_Navmesh_Builder/raw/main/FFXINAV.dll", dllFilePath);
            }

            if (File.Exists(dllFilePath))
            {
                string version = FileVersionInfo.GetVersionInfo(dllFilePath).ProductVersion;
                UpdateLabel(@$"Found FFXINAV.dll {version}.");
                await Task.Delay(2000);
            }

            UpdateLabel("Enjoy!...");
            await Task.Delay(500);
        }

        /// <summary>
        /// Completeds the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AsyncCompletedEventArgs"/> instance containing the event data.</param>
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            UpdateLabel(@$"Download complete.");
            Task.Delay(500).Wait();
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="location">The location.</param>
        private async Task DownloadFile(string address, string location) => await Task.Run(() =>
                {
                    var uri = new Uri(address);
                    UpdateLabel(@$"Downloading {{FFXINAV.dll}} from {address}.");
                    Task.Delay(500).Wait();
                    using var client = new WebClient();
                    client.DownloadFileAsync(uri, location);
                    client.DownloadFileCompleted += Completed;
                });

        /// <summary>
        /// initialize as an asynchronous operation.
        /// </summary>
        private async Task InitializeAsync()
        {
            await CheckFiles();
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                MF.ChangeView(1, null, null);
            });
        }

        /// <summary>
        /// Updates the label.
        /// </summary>
        /// <param name="text">The text.</param>
        private void UpdateLabel(string text)
        {
            Message = text;
        }
    }
}