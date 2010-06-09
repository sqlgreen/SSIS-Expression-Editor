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
    public sealed class ExceptionMessageBox : Microsoft.SqlServer.MessageBox.ExceptionMessageBox
    {
        /// <summary>
        /// Shows the exception message box.
        /// </summary>
        /// <param name="owner">The owner for the message box.</param>
        /// <param name="exception">The exception to show.</param>
        /// <param name="buttons">The buttons to show.</param>
        /// <param name="symbol">The symbol to show.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns>The dialog result from the message box.</returns>
        public static DialogResult Show(IWin32Window owner, Exception exception, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons buttons, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol symbol, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton defaultButton)
        {
            Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(exception, buttons, symbol, defaultButton);
            return messageBox.Show(owner);
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
            Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons buttons = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.OK;
            Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
            Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton defaultButton = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton.Button1;

            return Show(owner, text, exception, buttons, symbol, defaultButton);
        }

        /// <summary>
        /// Shows the exception message box.
        /// </summary>
        /// <param name="owner">The owner for the message box.</param>
        /// <param name="exception">The exception to show.</param>
        /// <returns>The dialog result from the message box.</returns>
        public static DialogResult Show(IWin32Window owner, Exception exception)
        {
            Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons buttons = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.OK;
            Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
            Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton defaultButton = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton.Button1;

            return Show(owner, exception, buttons, symbol, defaultButton);
        }

        /// <summary>
        /// Shows the exception message box.
        /// </summary>
        /// <param name="owner">The owner for the message box.</param>
        /// <param name="text">The message text.</param>
        /// <param name="exception">The exception to show.</param>
        /// <param name="buttons">The buttons to show.</param>
        /// <param name="symbol">The symbol to show.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns>The dialog result from the message box.</returns>
        public static DialogResult Show(IWin32Window owner, string text, Exception exception, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons buttons, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol symbol, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton defaultButton)
        {
            ApplicationException topException = new ApplicationException(text, exception);
            Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(topException, buttons, symbol, defaultButton);
            return messageBox.Show(owner);
        }
    }
}
