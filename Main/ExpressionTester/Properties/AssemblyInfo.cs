// ======================================================================================
// <copyright file="AssemblyInfo.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited
// </copyright>
// ======================================================================================

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Permissions;

[assembly: AssemblyTitle("Expression Tester")]
[assembly: AssemblyProduct("Expression Tester Tool")]

#if DENALI
[assembly: AssemblyDescription("SQL Server \"Denali\" Integration Services expression development and testing tool.")]
[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.0.5.0")]
#endif

#if KATMAI
[assembly: AssemblyDescription("SQL Server 2008 Integration Services expression development and testing tool.")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.5.0")]
#endif

#if YUKON
[assembly: AssemblyDescription("SQL Server 2005 Integration Services expression development and testing tool.")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.5.0")]
#endif

[assembly: AssemblyCompany("Konesans Limited")]
[assembly: AssemblyCopyright("Copyright © 2011 Konesans Limited")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: NeutralResourcesLanguage("en-GB")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

#if KATMAI
[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = false)]
#endif
#if YUKON
[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = false)]
#endif

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif