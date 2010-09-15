//-------------------------------------------------------------------------------------------------
// <copyright file="ExpressionRichTextBox.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System.Collections.Specialized;
    using System.Windows.Forms;

    /// <summary>
    /// Expression rich text box control.
    /// </summary>
    internal class ExpressionRichTextBox : RichTextBox
    {
        /// <summary>
        /// Syntax text token
        /// </summary>
        private const string LeftParameterBoundary1 = "«";

        /// <summary>
        /// Syntax text token
        /// </summary>
        private const string RightParameterBoundary1 = "»";
        
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDoubleClick"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.Button == MouseButtons.Left)
            {
                this.ExpandSelectedText(LeftParameterBoundary1, RightParameterBoundary1, "_");
                this.ExpandSelectedText("@[", "]", ":", "::", "[", "_");
            }
        }

        /// <summary>
        /// Expands the selected text.
        /// </summary>
        /// <param name="leftBoundary">The left boundary.</param>
        /// <param name="rightBoundary">The right boundary.</param>
        /// <param name="intermediate">The intermediate.</param>
        private void ExpandSelectedText(string leftBoundary, string rightBoundary, params string[] intermediate)
        {
            StringCollection intermediates = new StringCollection();
            intermediates.AddRange(intermediate);
            string text = this.Text;
            int start = this.SelectionStart;
            int end = start + this.SelectionLength;
            int leftBoundaryLength = leftBoundary.Length;
            int rightBoundaryLength = rightBoundary.Length;

            if (end > start && start > 0 && this.Text.Length > end)
            {
                if (text.Substring(start - leftBoundaryLength, leftBoundaryLength) == leftBoundary && text.Substring(end, rightBoundaryLength) == rightBoundary)
                {
                    this.SelectionStart = start - leftBoundaryLength;
                    this.SelectionLength = 1 + (end - this.SelectionStart);
                }
                else if (text.Substring(end, 1) == rightBoundary && intermediates.Contains(text.Substring(start - leftBoundaryLength, leftBoundaryLength)))
                {
                    for (int i = start; i > 0; i--)
                    {
                        string x = text.Substring(i, leftBoundaryLength);
                        if (x == leftBoundary)
                        {
                            this.SelectionStart = i;
                            this.SelectionLength = rightBoundaryLength + (end - this.SelectionStart);
                            break;
                        }
                        else if (x == " " || x == "(" || x == "[")
                        {
                            break;
                        }
                    }
                }
                else if (text.Substring(start - leftBoundaryLength, leftBoundaryLength) == leftBoundary && intermediates.Contains(text.Substring(end, rightBoundaryLength)))
                {
                    int len = text.Length;
                    for (int i = end; i < len; i++)
                    {
                        string x = text.Substring(i, rightBoundaryLength);
                        if (x == rightBoundary)
                        {
                            this.SelectionStart = start - leftBoundaryLength;
                            this.SelectionLength = rightBoundaryLength + (i - this.SelectionStart);
                            break;
                        }
                        else if (x == " " || x == "(" || x == "[")
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
