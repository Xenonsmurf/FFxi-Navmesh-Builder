// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-28-2021
//
// Last Modified By : xenon
// Last Modified On : 05-17-2021
// ***********************************************************************
// <copyright file="UpdateViewCommand.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using FFXI_Navmesh_Builder.ViewModels;
using FFXI_Navmesh_Builder.Views;
using System;
using System.Windows.Input;

namespace FFXI_Navmesh_Builder.Commands
{
    /// <summary>
    /// Class UpdateViewCommand.
    /// Implements the <see cref="System.Windows.Input.ICommand" />
    /// </summary>
    /// <seealso cref="System.Windows.Input.ICommand" />
    public class UpdateViewCommand : ICommand
    {
        /// <summary>
        /// The view model
        /// </summary>
        private MainViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateViewCommand" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="main">The main.</param>
        public UpdateViewCommand(MainViewModel viewModel, MainWindow main)
        {
            this.viewModel = viewModel;
            MF = main;
            IV = new InitializeView();
            HV = new HomeView();
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets or sets the hv.
        /// </summary>
        /// <value>The hv.</value>
        public HomeView HV { get; set; }

        /// <summary>
        /// Gets or sets the iv.
        /// </summary>
        /// <value>The iv.</value>
        public InitializeView IV { get; set; }

        /// <summary>
        /// Gets or sets the mf.
        /// </summary>
        /// <value>The mf.</value>
        public MainWindow MF { get; set; }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        /// <returns><see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void Execute(object parameter)
        {
            if (parameter.ToString() == "Initialize")
            {
                viewModel.SelectedViewModel = new InitializeViewModel(viewModel, MF, IV);
            }
            else if (parameter.ToString() == "Home")
            {
                viewModel.SelectedViewModel = new HomeViewModel(viewModel, MF, HV);
            }
        }
    }
}