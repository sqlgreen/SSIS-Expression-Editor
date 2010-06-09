//-------------------------------------------------------------------------------------------------
// <copyright file="UndoOrRedoCountChangedEventArgs.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Undo or redo count changed EventArgs
    /// </summary>
    public class UndoOrRedoCountChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Private property field.
        /// </summary>
        private int undoCount;

        /// <summary>
        /// Private property field.
        /// </summary>
        private int redoCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoOrRedoCountChangedEventArgs"/> class.
        /// </summary>
        /// <param name="undoCount">The undo count.</param>
        /// <param name="redoCount">The redo count.</param>
        public UndoOrRedoCountChangedEventArgs(int undoCount, int redoCount)
        {
            this.UndoCount = undoCount;
            this.RedoCount = redoCount;
        }

        /// <summary>
        /// Gets or sets the undo count.
        /// </summary>
        /// <value>The undo count.</value>
        public int UndoCount
        {
            get
            {
                return this.undoCount;
            }

            set
            {
                this.undoCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the redo count.
        /// </summary>
        /// <value>The redo count.</value>
        public int RedoCount
        {
            get
            {
                return this.redoCount;
            }

            set
            {
                this.redoCount = value;
            }
        }
    }
}
