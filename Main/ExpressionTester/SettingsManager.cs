//-------------------------------------------------------------------------------------------------
// <copyright file="SettingsManager.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.Tools.Common
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    /// <summary>
    /// Custom settings manager for loading and saving settings.
    /// </summary>
    /// <remarks>
    /// The built in support uses unpredictable file paths and does not share settinsg accross application versions, hence the custom approach.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "The reader and writer lifecycle is explicitly managed in a try catch finally block.")]
    internal class SettingsManager
    {
        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string SettingsElement = "Settings";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string ObjectElement = "Object";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string ObjectIdAttribute = "Id";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string TypeAttribute = "Type";
        
        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string PropertyElement = "Property";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string NameAttribute = "Name";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string IsNullAttribute = "IsNull";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string SeralizeAsAttribute = "SeralizeAs";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string TrueValue = "true";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string FalseValue = "false";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string FormState = "FormState";

        /// <summary>
        /// Xml text writer for saving the settings.
        /// </summary>
        private XmlTextWriter writer;

        /// <summary>
        /// Xml text reader for loading the settings.
        /// </summary>
        private XmlTextReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsManager"/> class.
        /// </summary>
        internal SettingsManager()
        {
        }

        /// <summary>
        /// Enumeration that defines the value type and serialization method.
        /// </summary>
        private enum SeralizeAs
        {
            /// <summary>
            /// The default type.
            /// </summary>
            Default,

            /// <summary>
            /// Serialise as a Font type.
            /// </summary>
            Font,

            /// <summary>
            /// Serialise as a Point type.
            /// </summary>
            Point,

            /// <summary>
            /// Serialise as a Size type.
            /// </summary>
            Size,

            /// <summary>
            /// Serialise as a Color type.
            /// </summary>
            Color,

            /// <summary>
            /// Serialise as a RecentFiles type.
            /// </summary>
            RecentFiles
        }

        /// <summary>
        /// Read the named value from the settings XML file.
        /// </summary>
        /// <param name="name">The value name.</param>
        /// <returns>The return value.</returns>
        internal object LoadValue(string name)
        {
            while (this.reader.Read())
            {
                if (this.reader.NodeType == XmlNodeType.Element)
                {
                    if (this.reader.Name == PropertyElement)
                    {
                        if (this.reader.GetAttribute(NameAttribute) == name)
                        {
                            return this.ReadPropertyValue();
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Read the named value from the settings XML file.
        /// </summary>
        /// <param name="name">The value name.</param>
        /// <param name="defaultValue">The default value in case the value is not found.</param>
        /// <returns>The return value.</returns>
        internal bool LoadValue(string name, bool defaultValue)
        {
            object value = this.LoadValue(name);
            if (value != null)
            {
                if (value is bool)
                {
                    return (bool)value;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Read the named value from the settings XML file.
        /// </summary>
        /// <param name="name">The value name.</param>
        /// <param name="defaultValue">The default value in case the value is not found.</param>
        /// <returns>The return value.</returns>
        internal int LoadValue(string name, int defaultValue)
        {
            object value = this.LoadValue(name);
            if (value != null)
            {
                if (value is int)
                {
                    return (int)value;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="id">The element id.</param>
        internal void LoadObject(object property, string id)
        {
            if (id == null)
            {
                id = String.Empty;
            }

            Type objectType = property.GetType();

            while (this.reader.Read())
            {
                if (this.reader.NodeType == XmlNodeType.Element)
                {
                    if (this.reader.Name == ObjectElement)
                    {
                        if (this.reader.GetAttribute(ObjectIdAttribute) == id)
                        {
                            this.ReadObject(property, objectType);
                            return;
                        }
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Loads the state of the form.
        /// </summary>
        /// <param name="property">The property.</param>
        internal void LoadFormState(object property)
        {
            Type objectType = typeof(Form);

            while (this.reader.Read())
            {
                if (this.reader.NodeType == XmlNodeType.Element)
                {
                    if (this.reader.Name == FormState)
                    {
                        this.ReadObject(property, objectType);
                        return;
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Begins the load.
        /// </summary>
        internal void BeginLoad()
        {
            this.reader = new XmlTextReader(GetPath());
        }

        /// <summary>
        /// Ends the load.
        /// </summary>
        internal void EndLoad()
        {
            this.reader.Close();
        }

        /// <summary>
        /// Begins the save.
        /// </summary>
        internal void BeginSave()
        {
            this.writer = new XmlTextWriter(GetPath(), Encoding.Unicode);

            this.writer.WriteStartDocument();

            this.writer.WriteComment("Settings Manager");

            this.writer.WriteStartElement(SettingsElement);
        }

        /// <summary>
        /// Ends the save.
        /// </summary>
        internal void EndSave()
        {
            this.writer.WriteEndElement(); // Settings

            this.writer.WriteEndDocument();

            this.writer.Close();
        }

        /// <summary>
        /// Saves the object.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="id">The element id.</param>
        internal void SaveObject(object property, string id)
        {
            if (id == null)
            {
                id = String.Empty;
            }

            this.WriteObject(property, id);
        }

        /// <summary>
        /// Saves the value.
        /// </summary>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The value to save.</param>
        internal void SaveValue(string name, object value)
        {
            this.WriteProperty(name, value.GetType(), value);
        }

        /// <summary>
        /// Saves the state of the form.
        /// </summary>
        /// <param name="form">The form to save state from.</param>
        internal void SaveFormState(Form form)
        {
            this.writer.WriteStartElement(FormState);
            this.writer.WriteAttributeString(TypeAttribute, typeof(Form).AssemblyQualifiedName);

            if (form.WindowState == FormWindowState.Normal)
            {
                this.WriteProperty("Size", typeof(Size), form.Size);
                this.WriteProperty("Location", typeof(Point), form.Location);
            }
            else
            {
                this.WriteProperty("Size", typeof(Size), form.RestoreBounds.Size);
                this.WriteProperty("Location", typeof(Point), form.RestoreBounds.Location);
            }

            this.WriteProperty("WindowState", typeof(FormWindowState), form.WindowState.ToString());

            this.writer.WriteEndElement(); // FormState
        }

        /// <summary>
        /// Gets the settings file path.
        /// </summary>
        /// <returns>The path to the Settings.config file.</returns>
        private static string GetPath()
        {
            string folder = Konesans.Dts.ExpressionEditor.RuntimeHelper.ApplicationDataFolder(Application.ProductName);
            return Path.Combine(folder, "Settings.config");
        }

        /// <summary>
        /// Reads the object.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="objectType">Type of the object.</param>
        private void ReadObject(object property, Type objectType)
        {
            if (this.reader.HasAttributes)
            {
                if (this.reader.GetAttribute(TypeAttribute) == objectType.AssemblyQualifiedName)
                {
                    bool readOn = this.reader.ReadToDescendant(PropertyElement);
                    while (readOn)
                    {
                        ////reader.ReadStartElement();
                        if (this.reader.Name == PropertyElement)
                        {
                            this.SetObjectValue(objectType, property);
                        }

                        ////reader.ReadEndElement();
                        readOn = this.reader.ReadToNextSibling(PropertyElement);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the object value.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="property">The property.</param>
        private void SetObjectValue(Type objectType, object property)
        {
            string propertyName = this.reader.GetAttribute(NameAttribute);
            PropertyInfo propertyInfo = objectType.GetProperty(propertyName);
            object value = this.ReadPropertyValue();
            propertyInfo.SetValue(property, value, null);
        }

        /// <summary>
        /// Reads the property value.
        /// </summary>
        /// <returns>The property value.</returns>
        private object ReadPropertyValue()
        {
            SeralizeAs serializeAs = (SeralizeAs)Enum.Parse(typeof(SeralizeAs), this.reader.GetAttribute(SeralizeAsAttribute));

            if (serializeAs == SeralizeAs.Font)
            {
                return FontSerailizer.ReadType(this.reader);
            }
            else if (serializeAs == SeralizeAs.Point)
            {
                return PointSerializer.ReadType(this.reader);
            }
            else if (serializeAs == SeralizeAs.Size)
            {
                return SizeSerializer.ReadType(this.reader);
            }
            else if (serializeAs == SeralizeAs.Color)
            {
                return ColorSerializer.ReadType(this.reader);
            }
            else if (serializeAs == SeralizeAs.RecentFiles)
            {
                return RecentFiles.ReadType(this.reader);
            }
            else 
            {
                // SeralizeAs.Default
                Type type = Type.GetType(this.reader.GetAttribute(TypeAttribute));
                if (type.IsEnum)
                {
                    return Enum.Parse(type, this.reader.ReadString());
                }
                else if (type == typeof(string))
                {
                    return this.reader.ReadString();
                }
                else
                {
                    return Convert.ChangeType(this.reader.ReadString(), type, CultureInfo.CurrentCulture);
                }
            }
        }

        /// <summary>
        /// Writes the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        private void WriteProperty(PropertyInfo property, object value)
        {
            Type propertyType = property.PropertyType;

            this.WriteProperty(property.Name, propertyType, value);
        }

        /// <summary>
        /// Writes the property.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="type">The property type.</param>
        /// <param name="value">The property value.</param>
        private void WriteProperty(string name, Type type, object value)
        {
            this.writer.WriteStartElement(PropertyElement);
            this.writer.WriteAttributeString(NameAttribute, name);
            this.writer.WriteAttributeString(TypeAttribute, type.AssemblyQualifiedName);

            if (value == null)
            {
                this.writer.WriteAttributeString(IsNullAttribute, TrueValue);
            }
            else
            {
                this.writer.WriteAttributeString(IsNullAttribute, FalseValue);

                if (value is Font)
                {
                    FontSerailizer.WriteType(value, this.writer);
                }
                else if (value is Point)
                {
                    PointSerializer.WriteType(value, this.writer);
                }
                else if (value is Size)
                {
                    SizeSerializer.WriteType(value, this.writer);
                }
                else if (value is Color)
                {
                    ColorSerializer.WriteType(value, this.writer);
                }
                else if (value is RecentFiles)
                {
                    RecentFiles.WriteType(value, this.writer);
                }
                else
                {
                    this.writer.WriteAttributeString(SeralizeAsAttribute, SeralizeAs.Default.ToString());
                    this.writer.WriteValue(value);
                }
            }

            this.writer.WriteEndElement(); // Property
        }

        /// <summary>
        /// Writes the object.
        /// </summary>
        /// <param name="propertyObject">The property object.</param>
        /// <param name="id">The element id.</param>
        private void WriteObject(object propertyObject, string id)
        {
            this.writer.WriteStartElement(ObjectElement);
            this.writer.WriteAttributeString(ObjectIdAttribute, id);

            Type objectType = propertyObject.GetType();
            this.writer.WriteAttributeString(TypeAttribute, objectType.AssemblyQualifiedName);

            PropertyInfo[] properties = objectType.GetProperties();
            for (int index = 0; index < properties.Length; index++)
            {
                PropertyInfo property = properties[index];
                if (property.CanWrite)
                {
                    this.WriteProperty(property, property.GetValue(propertyObject, null));
                }
            }

            this.writer.WriteEndElement(); // Object
        }

        /// <summary>
        /// Recent files collection
        /// </summary>
        [System.Xml.Serialization.XmlType("string")]
        internal sealed class RecentFiles : IConvertible
        {
            /// <summary>
            /// Delimited list of files.
            /// </summary>
            private string rawFiles;

            /// <summary>
            /// Initializes a new instance of the <see cref="RecentFiles"/> class.
            /// </summary>
            /// <param name="recentFiles">The recent files.</param>
            internal RecentFiles(string recentFiles)
            {
                this.rawFiles = recentFiles;
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="Konesans.Dts.Tools.Common.SettingsManager.RecentFiles"/> to <see cref="System.String"/>.
            /// </summary>
            /// <param name="recentFiles">The recent files.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator string(RecentFiles recentFiles)
            {
                return recentFiles.rawFiles;
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Konesans.Dts.Tools.Common.SettingsManager.RecentFiles"/>.
            /// </summary>
            /// <param name="files">The files.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator RecentFiles(string files)
            {
                return new RecentFiles(files);
            }

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </returns>
            public override string ToString()
            {
                return this.rawFiles;
            }

            /// <summary>
            /// Returns the <see cref="T:System.TypeCode"/> for this instance.
            /// </summary>
            /// <returns>
            /// The enumerated constant that is the <see cref="T:System.TypeCode"/> of the class or value type that implements this interface.
            /// </returns>
            TypeCode IConvertible.GetTypeCode()
            {
                return TypeCode.String;
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent Boolean value using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// A Boolean value equivalent to the value of this instance.
            /// </returns>
            bool IConvertible.ToBoolean(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// An 8-bit unsigned integer equivalent to the value of this instance.
            /// </returns>
            byte IConvertible.ToByte(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent Unicode character using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// A Unicode character equivalent to the value of this instance.
            /// </returns>
            char IConvertible.ToChar(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent <see cref="T:System.DateTime"/> using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// A <see cref="T:System.DateTime"/> instance equivalent to the value of this instance.
            /// </returns>
            DateTime IConvertible.ToDateTime(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent <see cref="T:System.Decimal"/> number using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// A <see cref="T:System.Decimal"/> number equivalent to the value of this instance.
            /// </returns>
            decimal IConvertible.ToDecimal(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent double-precision floating-point number using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// A double-precision floating-point number equivalent to the value of this instance.
            /// </returns>
            double IConvertible.ToDouble(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent 16-bit signed integer using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// An 16-bit signed integer equivalent to the value of this instance.
            /// </returns>
            short IConvertible.ToInt16(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent 32-bit signed integer using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// An 32-bit signed integer equivalent to the value of this instance.
            /// </returns>
            int IConvertible.ToInt32(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent 64-bit signed integer using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// An 64-bit signed integer equivalent to the value of this instance.
            /// </returns>
            long IConvertible.ToInt64(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// An 8-bit signed integer equivalent to the value of this instance.
            /// </returns>
            sbyte IConvertible.ToSByte(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent single-precision floating-point number using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// A single-precision floating-point number equivalent to the value of this instance.
            /// </returns>
            float IConvertible.ToSingle(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent <see cref="T:System.String"/> using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// A <see cref="T:System.String"/> instance equivalent to the value of this instance.
            /// </returns>
            string IConvertible.ToString(IFormatProvider provider)
            {
                return this.rawFiles;
            }

            /// <summary>
            /// Converts the value of this instance to an <see cref="T:System.Object"/> of the specified <see cref="T:System.Type"/> that has an equivalent value, using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="conversionType">The <see cref="T:System.Type"/> to which the value of this instance is converted.</param>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// An <see cref="T:System.Object"/> instance of type <paramref name="conversionType"/> whose value is equivalent to the value of this instance.
            /// </returns>
            object IConvertible.ToType(Type conversionType, IFormatProvider provider)
            {
                if (conversionType == typeof(string))
                {
                    return this.rawFiles;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// An 16-bit unsigned integer equivalent to the value of this instance.
            /// </returns>
            ushort IConvertible.ToUInt16(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// An 32-bit unsigned integer equivalent to the value of this instance.
            /// </returns>
            uint IConvertible.ToUInt32(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified culture-specific formatting information.
            /// </summary>
            /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
            /// <returns>
            /// An 64-bit unsigned integer equivalent to the value of this instance.
            /// </returns>
            ulong IConvertible.ToUInt64(IFormatProvider provider)
            {
                throw new InvalidCastException();
            }

            /// <summary>
            /// Writes the type to XML.
            /// </summary>
            /// <param name="property">The property to serialize.</param>
            /// <param name="writer">The XML writer.</param>
            internal static void WriteType(object property, XmlTextWriter writer)
            {
                RecentFiles recentFiles = (RecentFiles)property;

                writer.WriteAttributeString(SeralizeAsAttribute, SeralizeAs.RecentFiles.ToString());
                writer.WriteValue(recentFiles);
            }

            /// <summary>
            /// Reads the type from XML.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <returns>The deserialized type object.</returns>
            internal static object ReadType(XmlTextReader reader)
            {
                return new RecentFiles(reader.ReadString());
            }

            /// <summary>
            /// Add a recent file to this list.
            /// </summary>
            /// <param name="filename">The filename.</param>
            internal void AddRecentFile(string filename)
            {
                StringBuilder output = new StringBuilder();
                output.Append(filename);
                output.Append("|");

                string[] files = this.GetFiles();
                for (int index = 0; index < files.Length; index++)
                {
                    string file = files[index];
                    if (file.Length > 0 && file != filename)
                    {
                        output.Append(file);
                        output.Append("|");
                    }
                }

                this.rawFiles = output.ToString();
            }

            /// <summary>
            /// Remove a file from this list.
            /// </summary>
            /// <param name="filename">The filename.</param>
            internal void RemoveRecentFile(string filename)
            {
                StringBuilder output = new StringBuilder();

                string[] files = this.GetFiles();

                for (int index = 0; index < files.Length; index++)
                {
                    string file = files[index];
                    if (file.Length > 0 && file != filename)
                    {
                        output.Append(file);
                        output.Append("|");
                    }
                }

                this.rawFiles = output.ToString();
            }

            /// <summary>
            /// Gets the files as an array.
            /// </summary>
            /// <returns>An array of recent file entries.</returns>
            internal string[] GetFiles()
            {
                return this.rawFiles.Split('|');
            }
        }

        /// <summary>
        /// Serializer class for Font type.
        /// </summary>
        private class FontSerailizer
        {
            /// <summary>
            /// XML node name constant
            /// </summary>
            private const string FontFamilyAttribute = "FontFamily";

            /// <summary>
            /// XML node name constant
            /// </summary>
            private const string FontStyleAttribute = "FontStyle";

            /// <summary>
            /// XML node name constant
            /// </summary>
            private const string FontSizeAttribute = "FontSize";

            /// <summary>
            /// Prevents a default instance of the <see cref="FontSerailizer"/> class from being created.
            /// </summary>
            private FontSerailizer()
            {
            }

            /// <summary>
            /// Writes the type to XML.
            /// </summary>
            /// <param name="property">The property to serialize.</param>
            /// <param name="writer">The XML writer.</param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Single.ToString", Justification = "Code Analysis contradicts with compiler warning, MyObject.ToString(System.IFormatProvider) is obsolete.")]
            internal static void WriteType(object property, XmlTextWriter writer)
            {
                Font font = property as Font;
                if (font == null)
                {
                    return;
                }

                writer.WriteAttributeString(SeralizeAsAttribute, SeralizeAs.Font.ToString());
                writer.WriteAttributeString(FontFamilyAttribute, font.FontFamily.Name);
                writer.WriteAttributeString(FontSizeAttribute, font.Size.ToString());
                writer.WriteAttributeString(FontStyleAttribute, font.Style.ToString());
            }

            /// <summary>
            /// Reads the type.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <returns>The deserialized type object.</returns>
            internal static object ReadType(XmlTextReader reader)
            {
                string fontFamily = reader.GetAttribute(FontFamilyAttribute);
                string size = reader.GetAttribute(FontSizeAttribute);
                string style = reader.GetAttribute(FontStyleAttribute);

                FontStyle fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), style);

                Font font = new Font(fontFamily, Convert.ToSingle(size, CultureInfo.CurrentCulture), fontStyle);

                return font;
            }
        }

        /// <summary>
        /// Serializer class for Point type.
        /// </summary>
        private class PointSerializer
        {
            /// <summary>
            /// XML node name constant
            /// </summary>
            private const string XAttribute = "X";

            /// <summary>
            /// XML node name constant
            /// </summary>
            private const string YAttribute = "Y";

            /// <summary>
            /// Prevents a default instance of the <see cref="PointSerializer"/> class from being created.
            /// </summary>
            private PointSerializer()
            {
            }

            /// <summary>
            /// Writes the type to XML.
            /// </summary>
            /// <param name="property">The property to serialize.</param>
            /// <param name="writer">The XML writer.</param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString", Justification = "Code Analysis contradicts with compiler warning, MyObject.ToString(System.IFormatProvider) is obsolete.")]
            internal static void WriteType(object property, XmlTextWriter writer)
            {
                Point point = (Point)property;

                writer.WriteAttributeString(SeralizeAsAttribute, SeralizeAs.Point.ToString());
                writer.WriteAttributeString(XAttribute, point.X.ToString());
                writer.WriteAttributeString(YAttribute, point.Y.ToString());
            }

            /// <summary>
            /// Reads the type.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <returns>The deserialized type object.</returns>
            internal static object ReadType(XmlTextReader reader)
            {
                string x = reader.GetAttribute(XAttribute);
                string y = reader.GetAttribute(YAttribute);

                return new Point(Convert.ToInt32(x, CultureInfo.CurrentCulture), Convert.ToInt32(y, CultureInfo.CurrentCulture));
            }
        }

        /// <summary>
        /// Serializer class for Size type.
        /// </summary>
        private class SizeSerializer
        {
            /// <summary>
            /// XML node name constant
            /// </summary>
            private const string HeightAttribute = "Height";

            /// <summary>
            /// XML node name constant
            /// </summary>
            private const string WidthAttribute = "Width";

            /// <summary>
            /// Prevents a default instance of the <see cref="SizeSerializer"/> class from being created.
            /// </summary>
            private SizeSerializer()
            {
            }

            /// <summary>
            /// Writes the type to XML.
            /// </summary>
            /// <param name="property">The property to serialize.</param>
            /// <param name="writer">The XML writer.</param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString", Justification = "Code Analysis contradicts with compiler warning, MyObject.ToString(System.IFormatProvider) is obsolete.")]
            internal static void WriteType(object property, XmlTextWriter writer)
            {
                Size size = (Size)property;

                writer.WriteAttributeString(SeralizeAsAttribute, SeralizeAs.Size.ToString());
                writer.WriteAttributeString(HeightAttribute, size.Height.ToString());
                writer.WriteAttributeString(WidthAttribute, size.Width.ToString());
            }

            /// <summary>
            /// Reads the type.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <returns>The deserialized type object.</returns>
            internal static object ReadType(XmlTextReader reader)
            {
                string h = reader.GetAttribute(HeightAttribute);
                string w = reader.GetAttribute(WidthAttribute);

                return new Size(Convert.ToInt32(w, CultureInfo.CurrentCulture), Convert.ToInt32(h, CultureInfo.CurrentCulture));
            }
        }

        /// <summary>
        /// Serializer class for Color type.
        /// </summary>
        private class ColorSerializer
        {
            /// <summary>
            /// XML node name constant
            /// </summary>
            private const string ColorAttribute = "Color";

            /// <summary>
            /// Prevents a default instance of the <see cref="ColorSerializer"/> class from being created.
            /// </summary>
            private ColorSerializer()
            {
            }

            /// <summary>
            /// Writes the type to XML.
            /// </summary>
            /// <param name="property">The property to serialize.</param>
            /// <param name="writer">The XML writer.</param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString", Justification = "Code Analysis contradicts with compiler warning, MyObject.ToString(System.IFormatProvider) is obsolete.")]
            internal static void WriteType(object property, XmlTextWriter writer)
            {
                Color color = (Color)property;

                writer.WriteAttributeString(SeralizeAsAttribute, SeralizeAs.Color.ToString());
                writer.WriteAttributeString(ColorAttribute, color.ToArgb().ToString());
            }

            /// <summary>
            /// Reads the type.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <returns>The deserialized type object.</returns>
            internal static object ReadType(XmlTextReader reader)
            {
                string argb = reader.GetAttribute(ColorAttribute);
                return Color.FromArgb(Convert.ToInt32(argb, CultureInfo.CurrentCulture));
            }
        }
    }
}
