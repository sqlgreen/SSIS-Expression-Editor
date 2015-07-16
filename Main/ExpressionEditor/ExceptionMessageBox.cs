//-------------------------------------------------------------------------------------------------
// <copyright file="ExceptionMessageBox.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Exception message box wrapper class.
    /// </summary>
    public sealed class ExceptionMessageBox
    {
        public static DialogResult Show(IWin32Window owner, Exception exception, string caption)
        {
            return MessageBox.Show(owner, exception.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows the exception message box.
        /// </summary>
        /// <param name="owner">The owner for the message box.</param>
        /// <param name="text">The message text.</param>
        /// <param name="exception">The exception to show.</param>
        /// <returns>The dialog result from the message box.</returns>
        public static DialogResult Show(IWin32Window owner, string text, Exception exception)
        {
            return MessageBox.Show(owner, text + "\r\n\r\n" + exception.ToString(), "Expression Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows the exception message box.
        /// </summary>
        /// <param name="owner">The owner for the message box.</param>
        /// <param name="exception">The exception to show.</param>
        /// <returns>The dialog result from the message box.</returns>
        public static DialogResult Show(IWin32Window owner, Exception exception)
        {
            return MessageBox.Show(owner, exception.ToString(), "Expression Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
