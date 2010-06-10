//-------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace PackageTest
{
    using System;
    using Microsoft.SqlServer.Dts.Runtime;
    using Forms = System.Windows.Forms;

    /// <summary>
    /// Simplified test harness for the editor control
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application. Create a dummy package and launch the editor.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Forms.Application.EnableVisualStyles();
            Forms.Application.SetCompatibleTextRenderingDefault(false);

            Package package = new Package();
            Executable exec = package.Executables.Add("STOCK:FileSystemTask");
            TaskHost taskHost = exec as TaskHost;

            Variable variable = package.Variables.Add("MyVariable", false, "User", string.Empty);

            Forms.Application.Run(new Konesans.Dts.ExpressionEditor.ExpressionEditorPublic(package.Variables, package.VariableDispenser, variable));

            Forms.Application.Run(new Konesans.Dts.ExpressionEditor.ExpressionEditorPublic(package.Variables, package.VariableDispenser, taskHost.Description.GetType(), "Operation", string.Empty));
        }
    }
}
