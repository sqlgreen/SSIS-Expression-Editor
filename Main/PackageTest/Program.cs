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

            // Add an invalid variable expression, BIDSHelper issue 30851
            Variable variableValid = package.Variables.Add("GoodExpression", false, "User", string.Empty);
            variableValid.EvaluateAsExpression = true;
            variableValid.Expression = "@PackageName + \" Extra\"";

            Variable variableInvalid = package.Variables.Add("BadExpression", false, "User", string.Empty);
            variableInvalid.EvaluateAsExpression = true;
            variableInvalid.Expression = "@PackageNameInvalid + \" Extra\"";
            variableInvalid.Expression = "\"Package Name: \" + @[System::PackageName] +\" was executed at: \" + (DT_WSTR, 40) @[System::StartTime] + \" by user: \" + @[System::UserName] + \" on Server Name \" + @[System::MachineName]\"";

            Forms.Application.Run(new Konesans.Dts.ExpressionEditor.ExpressionEditorPublic(package.Variables, package.VariableDispenser, taskHost.Description.GetType(), "Operation", string.Empty));

            Forms.Application.Run(new Konesans.Dts.ExpressionEditor.ExpressionEditorPublic(package.Variables, package.VariableDispenser, variable));
        }
    }
}
