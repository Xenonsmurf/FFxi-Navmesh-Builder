// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-28-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 04-29-2021
// ***********************************************************************
// <copyright file="MainViewModel.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using FFXI_Navmesh_Builder.Commands;
using System.Windows.Input;

namespace FFXI_Navmesh_Builder.ViewModels
{
    /// <summary>
    /// Class MainViewModel.
    /// Implements the <see cref="FFXI_Navmesh_Builder.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="FFXI_Navmesh_Builder.ViewModels.BaseViewModel" />
    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// The selected view model
        /// </summary>
        public BaseViewModel _selectedViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="main">The main.</param>
        public MainViewModel(MainWindow main)
        {
            UpdateViewCommand = new UpdateViewCommand(this, main);
        }

        /// <summary>
        /// Gets or sets the selected view model.
        /// </summary>
        /// <value>The selected view model.</value>
        public BaseViewModel SelectedViewModel
        {
            get
            {
                return _selectedViewModel;
            }
            set
            {
                _selectedViewModel = value;
                OnPropertyChanged(nameof(SelectedViewModel));
            }
        }

        /// <summary>
        /// Gets or sets the update view command.
        /// </summary>
        /// <value>The update view command.</value>
        public ICommand UpdateViewCommand { get; set; }
    }
}