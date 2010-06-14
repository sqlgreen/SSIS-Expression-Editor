//-------------------------------------------------------------------------------------------------
// <copyright file="UndoText.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    /// <summary>
    /// Undo operation class.
    /// </summary>
    internal class UndoText
    {
        /// <summary>
        /// The text changed during the operation
        /// </summary>
        private string text;

        /// <summary>
        /// The start position of the text
        /// </summary>
        private int selectionStart;

        /// <summary>
        /// The length of the text
        /// </summary>
        private int selectionLength;

        /// <summary>
        /// Gets or sets the text changed during the operation
        /// </summary>
        /// <value>The operation text.</value>
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        /// <summary>
        /// Gets or sets the start position of the text.
        /// </summary>
        /// <value>The selection start.</value>
        public int SelectionStart
        {
            get { return this.selectionStart; }
            set { this.selectionStart = value; }
        }

        /// <summary>
        /// Gets or sets the length of the text.
        /// </summary>
        /// <value>The length of the selection.</value>
        public int SelectionLength
        {
            get { return this.selectionLength; }
            set { this.selectionLength = value; }
        }

        /// <summary>
        /// Determines whether the undo text is a duplicate.
        /// </summary>
        /// <param name="undoText">The undo text.</param>
        /// <returns>
        ///     <c>true</c> if the specified undo text is duplicate; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDuplicate(UndoText undoText)
        {
            if (undoText.SelectionLength == this.SelectionLength && undoText.SelectionStart == this.SelectionStart && undoText.Text == this.Text)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
