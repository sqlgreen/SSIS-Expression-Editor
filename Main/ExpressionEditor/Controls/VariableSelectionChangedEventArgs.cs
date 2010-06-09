//-------------------------------------------------------------------------------------------------
// <copyright file="VariableSelectionChangedEventArgs.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Variable selection changed EventArgs
    /// </summary>
    public class VariableSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Private property field.
        /// </summary>
        private bool variableSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableSelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="variableSelected">if set to <c>true</c> [variable selected].</param>
        public VariableSelectionChangedEventArgs(bool variableSelected)
        {
            this.VariableSelected = variableSelected;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the variable is selected.
        /// </summary>
        /// <value><c>true</c> if the variable is selected; otherwise, <c>false</c>.</value>
        public bool VariableSelected
        {
            get
            {
                return this.variableSelected;
            }

            set
            {
                this.variableSelected = value;
            }
        }
    }
}
