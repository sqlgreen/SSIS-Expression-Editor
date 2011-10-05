//-------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.Tools.ExpressionTester
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Program entry point class, including basic assembly dependency checks.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Support text for assembly check failures.
        /// </summary>
        private const string SupportText = "http://www.konesans.com/";

#if DENALI
        /// <summary>
        /// SQL Server "Denali" version text
        /// </summary>
        private const string SqlVersionText = "\"Denali\"";

        /// <summary>
        /// Assembly name for SQL Server "Denali" Interation Services runtime wrapper
        /// </summary>
        private const string RuntimeWrapperAssemblyName = "Microsoft.SqlServer.DTSRuntimeWrap, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

        /// <summary>
        /// Assembly name for SQL Server "Denali" Interation Services managed assembly
        /// </summary>
        private const string ManagedDTSAssemblyName = "Microsoft.SqlServer.ManagedDTS, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
        
        /// <summary>
        /// Assembly name for SQL Server "Denali" Interation Services compatible expression editor control library
        /// </summary>
        private const string ExpressionEditorAssemblyName = "ExpressionEditor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b2ab4a111192992b";
#endif

#if KATMAI
        /// <summary>
        /// SQL Server 2008 version text
        /// </summary>
        private const string SqlVersionText = "2008";

        /// <summary>
        /// Assembly name for SQL Server 2008 Interation Services runtime wrapper
        /// </summary>
        private const string RuntimeWrapperAssemblyName = "Microsoft.SqlServer.DTSRuntimeWrap, Version=10.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

        /// <summary>
        /// Assembly name for SQL Server 2008 Interation Services managed assembly
        /// </summary>
        private const string ManagedDTSAssemblyName = "Microsoft.SqlServer.ManagedDTS, Version=10.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
        
        /// <summary>
        /// Assembly name for SQL Server 2008 Interation Services compatible expression editor control library
        /// </summary>
        private const string ExpressionEditorAssemblyName = "ExpressionEditor, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b2ab4a111192992b";
#endif

#if YUKON
        /// <summary>
        /// SQL Server 2005 version text
        /// </summary>
        private const string SqlVersionText = "2005";

        /// <summary>
        /// Assembly name for SQL Server 2005 Interation Services runtime wrapper
        /// </summary>
        private const string RuntimeWrapperAssemblyName = "Microsoft.SqlServer.DTSRuntimeWrap, Version=9.0.242.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

        /// <summary>
        /// Assembly name for SQL Server 2005 Interation Services managed assembly
        /// </summary>
        private const string ManagedDTSAssemblyName = "Microsoft.SqlServer.ManagedDTS, Version=9.0.242.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
        
        /// <summary>
        /// Assembly name for SQL Server 2005 Interation Services compatible expression editor control library
        /// </summary>
       private const string ExpressionEditorAssemblyName = "ExpressionEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b2ab4a111192992b";
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            try
            {
                if (CheckForSupportAssemblies())
                {
                    Application.ThreadException += new ThreadExceptionEventHandler(GlobalThreadException);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Main());
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Checks for support assemblies.
        /// </summary>
        /// <remarks>
        /// The explicit check is used as tools are expected to be copied around just as executables, rather than full MSI installations.
        /// </remarks>
        /// <returns>A boolean flag indicating whether the checks were successful.</returns>
        private static bool CheckForSupportAssemblies()
        {
            try
            {
                Assembly.ReflectionOnlyLoad(ExpressionEditorAssemblyName);
            }
            catch
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, "The expression editor assembly cannot be found (ExpressionEditor.dll), please check the application is correctly installed.\r\n\r\n{0}\r\n\r\n{1}", ExpressionEditorAssemblyName, SupportText), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            try
            {
                Assembly.ReflectionOnlyLoad(ManagedDTSAssemblyName);
            }
            catch
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, "A valid SQL Server {0} Integration Services installation cannot be detected. Please check the installtion is complete and includes SSIS.\r\n\r\nMissing Assembly Name:\r\n\t{1}\r\n\r\n{2}", SqlVersionText, ManagedDTSAssemblyName, SupportText), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            try
            {
                Assembly.ReflectionOnlyLoad(RuntimeWrapperAssemblyName);
            }
            catch
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, "A valid SQL Server {0} Integration Services installation cannot be detected. Please check the installtion is complete and includes SSIS.\r\n\r\nMissing Assembly Name:\r\n\t{1}\r\n\r\n{2}", SqlVersionText, RuntimeWrapperAssemblyName, SupportText), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Catches a global thread exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Threading.ThreadExceptionEventArgs"/> instance containing the event data.</param>
        private static void GlobalThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Only a test harness.")]
        private static void HandleException(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception.ToString());

            try
            {
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox messagebox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(exception);
                messagebox.Show(null);
            }
            catch
            {
                MessageBox.Show(exception.ToString());
            }
        }
    }
}
