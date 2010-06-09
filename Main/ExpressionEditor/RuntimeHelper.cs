//-------------------------------------------------------------------------------------------------
// <copyright file="RuntimeHelper.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using Microsoft.SqlServer.Dts.Runtime;

    /// <summary>
    /// Runtime helper class, containg static members of helper functions associated with the runtime funtionailty of Integration Services.
    /// </summary>
    public sealed class RuntimeHelper
    {
        /// <summary>
        /// Common product name to allow shared expression functions
        /// </summary>
        public const string CommonProductName = "Expression Editor";

        /// <summary>
        /// Prevents a default instance of the <see cref="RuntimeHelper"/> class from being created.
        /// </summary>
        private RuntimeHelper()
        {
        }

        /// <summary>
        /// Get a Type from a TypeCode
        /// </summary>
        /// <remarks>
        /// SSIS does not support the type UInt16 for variables, since this is actually used to store Char types.
        /// </remarks>
        /// <param name="typeCode">TypeCode to lookup Type</param>
        /// <returns>Type matching TypeCode supplied.</returns>
        public static Type GetTypeFromTypeCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return typeof(bool);
                case TypeCode.Byte:
                    return typeof(byte);
                case TypeCode.Char:
                    return typeof(byte);
                case TypeCode.DateTime:
                    return typeof(DateTime);
                case TypeCode.DBNull:
                    return typeof(DBNull);
                case TypeCode.Decimal:
                    return typeof(decimal);
                case TypeCode.Double:
                    return typeof(double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return typeof(short);
                case TypeCode.Int32:
                    return typeof(int);
                case TypeCode.Int64:
                    return typeof(long);
                case TypeCode.Object:
                    return typeof(object);
                case TypeCode.SByte:
                    return typeof(sbyte);
                case TypeCode.Single:
                    return typeof(float);
                case TypeCode.String:
                    return typeof(string);
                case TypeCode.UInt16:
                    return typeof(char); // Assign a char, get a UInt16 with SSIS variables
                case TypeCode.UInt32:
                    return typeof(uint);
                case TypeCode.UInt64:
                    return typeof(ulong);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the User's Application Data Folder for this application. Creates the folder if it does not exist, specifically the company name and application levels.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <returns>Company and application user application data folder path.</returns>
        /// <remarks>
        /// Function returns base SpecialFolder.ApplicationData folder, 'C:\Documents and Settings\[User Name]\Application Data'
        /// but also includes CompanyFolderName constant and Product Name specified, e.g.
        ///     C:\Documents and Settings\My Name\Application Data\Konesans\ProductName
        /// </remarks>
        public static string ApplicationDataFolder(string productName)
        {
            string common = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (Directory.Exists(common))
            {
                AssemblyCompanyAttribute companyAttribute = (AssemblyCompanyAttribute)AssemblyCompanyAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute));
                string fullPath = Path.Combine(common, companyAttribute.Company);
                fullPath = Path.Combine(fullPath, productName);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                return fullPath;
            }
            else
            {
                throw new DirectoryNotFoundException(
                    String.Format(CultureInfo.CurrentCulture, "Directory for SpecialFolder.CommonApplicationData was not found, '{0}'", common));
            }
        }

        /// <summary>
        /// Get display formatted Variable value, including for specialized formatting by type.
        /// </summary>
        /// <param name="variable">Variable for value to format.</param>
        /// <returns>Formatted string of variable value.</returns>
        internal static string GetVariableStringValue(Variable variable)
        {
            if (variable.DataType == TypeCode.String)
            {
                // Format string with quotes for display
                return String.Format(CultureInfo.CurrentCulture, "'{0}'", variable.Value);
            }
            else if (variable.DataType == TypeCode.UInt16)
            {
                // Char type Variables get stored as UInt16, so convert back to Char text
                return Convert.ToChar(variable.Value, CultureInfo.CurrentCulture).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                // All other types, use plain object.ToString()
                return variable.Value.ToString();
            }
        }

        /// <summary>
        /// Get a variable Scope value Scope is not a defined property
        /// so this will determine one from the parent path.
        /// </summary>
        /// <param name="variable">Variable to examine for scope.</param>
        /// <returns>String that represents the scope of the variable.</returns>
        internal static string GetVariableScope(Variable variable)
        {
            string path = variable.GetPackagePath();
            path = path.Remove(path.LastIndexOf(".Variables[", StringComparison.CurrentCulture));
            return path.Remove(0, 1 + path.LastIndexOf('\\'));
        }
    }
}