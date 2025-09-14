// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-28-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 05-04-2021
// ***********************************************************************
// <copyright file="BaseViewModel.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;

namespace FFXI_Navmesh_Builder.ViewModels
{
    /// <summary>
    /// Class BaseViewModel.
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The message
        /// </summary>
        private string _message;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the Customer's name.
        /// </summary>
        /// <value>The message.</value>
        public String Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}