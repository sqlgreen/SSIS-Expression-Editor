// ======================================================================================
// <copyright file="AssemblyInfo.cs" company="Konesans Limited">
// Copyright (C) 2018 Konesans Limited
// </copyright>
// ======================================================================================

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

#if SQL2019
[assembly: AssemblyTitle("Expression Tester 2019")]
[assembly: AssemblyProduct("Expression Tester Tool for SQL Server 2019")]
[assembly: AssemblyDescription("SQL Server 2019 Integration Services expression development and testing tool.")]
[assembly: AssemblyVersion("7.0.0.0")]
[assembly: AssemblyFileVersion("7.0.9.0")]
#elif SQL2017
[assembly: AssemblyTitle("Expression Tester 2017")]
[assembly: AssemblyProduct("Expression Tester Tool for SQL Server 2017")]
[assembly: AssemblyDescription("SQL Server 2017 Integration Services expression development and testing tool.")]
[assembly: AssemblyVersion("6.0.0.0")]
[assembly: AssemblyFileVersion("6.0.9.0")]
#elif SQL2016
[assembly: AssemblyTitle("Expression Tester 2016")]
[assembly: AssemblyProduct("Expression Tester Tool for SQL Server 2016")]
[assembly: AssemblyDescription("SQL Server 2016 Integration Services expression development and testing tool.")]
[assembly: AssemblyVersion("5.0.0.0")]
[assembly: AssemblyFileVersion("5.0.9.0")]
#elif SQL2014
[assembly: AssemblyTitle("Expression Tester 2014")]
[assembly: AssemblyProduct("Expression Tester Tool for SQL Server 2014")]
[assembly: AssemblyDescription("SQL Server 2014 Integration Services expression development and testing tool.")]
[assembly: AssemblyVersion("4.0.0.0")]
[assembly: AssemblyFileVersion("4.0.9.0")]
#elif DENALI
[assembly: AssemblyTitle("Expression Tester 2012")]
[assembly: AssemblyProduct("Expression Tester Tool for SQL Server 2012")]
[assembly: AssemblyDescription("SQL Server 2012 Integration Services expression development and testing tool.")]
[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.0.9.0")]
#elif KATMAI
[assembly: AssemblyTitle("Expression Tester 2008")]
[assembly: AssemblyProduct("Expression Tester Tool for SQL Server 2008")]
[assembly: AssemblyDescription("SQL Server 2008 Integration Services expression development and testing tool.")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.9.0")]
#elif YUKON
[assembly: AssemblyTitle("Expression Tester 2005")]
[assembly: AssemblyProduct("Expression Tester Tool for SQL Server 2005")]
[assembly: AssemblyDescription("SQL Server 2005 Integration Services expression development and testing tool.")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.9.0")]
#endif

[assembly: AssemblyCompany("Konesans Limited")]
[assembly: AssemblyCopyright("Copyright © 2018 Konesans Limited")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: NeutralResourcesLanguage("en-GB")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

#if KATMAI
[assembly: System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, UnmanagedCode = false)]
#elif YUKON
[assembly: System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, UnmanagedCode = false)]
#endif

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif