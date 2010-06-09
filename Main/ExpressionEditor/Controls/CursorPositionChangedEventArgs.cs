//-------------------------------------------------------------------------------------------------
// <copyright file="CursorPositionChangedEventArgs.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Cursor position changedEventArgs
    /// </summary>
    public class CursorPositionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Private property field
        /// </summary>
        private int line;

        /// <summary>
        /// Private property field
        /// </summary>
        private int column;

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorPositionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="line">The line nummber.</param>
        /// <param name="column">The column number.</param>
        public CursorPositionChangedEventArgs(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>The line number.</value>
        public int Line
        {
            get
            {
                return this.line;
            }

            set
            {
                this.line = value;
            }
        }

        /// <summary>
        /// Gets or sets the column number.
        /// </summary>
        /// <value>The column number.</value>
        public int Column
        {
            get
            {
                return this.column;
            }

            set
            {
                this.column = value;
            }
        }
    }
}
