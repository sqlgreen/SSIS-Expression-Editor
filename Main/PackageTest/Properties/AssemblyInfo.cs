// ======================================================================================
// <copyright file="AssemblyInfo.cs" company="Konesans Limited">
// Copyright (C) 2012 Konesans Limited
// </copyright>
// ======================================================================================

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Package Test")]
[assembly: AssemblyProduct("Package Test")]

#if SQL2014
[assembly: AssemblyDescription("SQL Server 2014 Integration Services expression editor test harnessy.")]
[assembly: AssemblyVersion("4.0.0.0")]
[assembly: AssemblyFileVersion("4.0.0.0")]
#endif

#if DENALI
[assembly: AssemblyDescription("SQL Server 2012 Integration Services expression editor test harnessy.")]
[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.0.0.0")]
#endif

#if KATMAI
[assembly: AssemblyDescription("SQL Server 2008 Integration Services expression editor test harness.")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
#endif

#if YUKON
[assembly: AssemblyDescription("SQL Server 2005 Integration Services expression editor test harness.")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
#endif

[assembly: AssemblyCompany("Konesans Limited")]
[assembly: AssemblyCopyright("Copyright © 2011 Konesans Limited")]
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