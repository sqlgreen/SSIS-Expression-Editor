//-------------------------------------------------------------------------------------------------
// <copyright file="TitleChangedEventArgs.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Title changed EventArgs.
    /// </summary>
    public class TitleChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Private property field.
        /// </summary>
        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newTitle">The new title.</param>
        public TitleChangedEventArgs(string newTitle)
        {
            this.title = newTitle;
        }

        /// <summary>
        /// Gets the new title.
        /// </summary>
        /// <value>The new title.</value>
        public string Title
        {
            get
            {
                return this.title;
            }
        }
    }
}
