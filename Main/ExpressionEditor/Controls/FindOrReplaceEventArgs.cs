//-------------------------------------------------------------------------------------------------
// <copyright file="FindOrReplaceEventArgs.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;

    /// <summary>
    /// Find or Replace event arguments
    /// </summary>
    public class FindOrReplaceEventArgs : EventArgs
    {
        /// <summary>
        /// Private property field
        /// </summary>
        private string searchTerm;

        /// <summary>
        /// Private property field
        /// </summary>
        private int replaceCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindOrReplaceEventArgs"/> class.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        public FindOrReplaceEventArgs(string searchTerm)
        {
            this.SearchTerm = searchTerm;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindOrReplaceEventArgs"/> class.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="replaceCount">The replace count.</param>
        public FindOrReplaceEventArgs(string searchTerm, int replaceCount)
        {
            this.SearchTerm = searchTerm;
            this.ReplaceCount = replaceCount;
        }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        /// <value>The search term.</value>
        public string SearchTerm
        {
            get
            {
                return this.searchTerm;
            }

            set
            {
                this.searchTerm = value;
            }
        }

        /// <summary>
        /// Gets or sets the replace count.
        /// </summary>
        /// <value>The replace count.</value>
        public int ReplaceCount
        {
            get
            {
                return this.replaceCount;
            }

            set
            {
                this.replaceCount = value;
            }
        }
    }
}
