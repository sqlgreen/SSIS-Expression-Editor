//-------------------------------------------------------------------------------------------------
// <copyright file="ExpressionEditorViewSerializer.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    /// <summary>
    /// Support class used for loading and saving ExpressionFunction Xml used by <see cref="ExpressionEditorView"/>
    /// </summary>
    internal static class ExpressionEditorViewSerializer
    {
        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string DocumentElementName = "ExpressionFunctions";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string GroupElementName = "Group";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string NameAttributeName = "Name";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string DescriptionElementName = "Description";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string FunctionElementName = "Function";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string SyntaxElementName = "Syntax";

        /// <summary>
        /// XML node name constant
        /// </summary>
        private const string SyntaxLabelElementName = "SyntaxLabel";

        /// <summary>
        /// Serializes the specified tree view ro XML.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="filename">The XML filename.</param>
        internal static void Serialize(TreeView treeView, string filename)
        {
            XmlTextWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteComment(GetTopComment());

            writer.WriteStartElement(DocumentElementName);

            foreach (TreeNode treeNode in treeView.Nodes)
            {
                SaveNode(treeNode, writer);
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        /// <summary>
        /// Deserialize the XML to populate the tree view.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="document">The XML document.</param>
        internal static void DeSerialize(TreeView treeView, XmlDocument document)
        {
            XmlElement root = document.DocumentElement;

            TreeAddNode(treeView, root, null);
        }

        /// <summary>
        /// Add a node to the TreeView.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="xmlNode">The XML node.</param>
        /// <param name="parentTreeNode">The parent tree node.</param>
        private static void TreeAddNode(TreeView treeView, XmlNode xmlNode, TreeNode parentTreeNode)
        {
            if (xmlNode.NodeType == XmlNodeType.Element)
            {
                if (xmlNode.LocalName == DocumentElementName || xmlNode.LocalName == GroupElementName)
                {
                    if (xmlNode.LocalName == GroupElementName)
                    {
                        TreeNode treeNode = new TreeNode(xmlNode.Attributes.Item(0).InnerText, 0, 0);

                        if (parentTreeNode == null)
                        {
                            treeView.Nodes.Add(treeNode);
                        }
                        else
                        {
                            parentTreeNode.Nodes.Add(treeNode);
                        }

                        parentTreeNode = treeNode;
                    }

                    foreach (XmlNode childNode in xmlNode.ChildNodes)
                    {
                        if (childNode.LocalName == "Description")
                        {
                            parentTreeNode.ToolTipText = childNode.InnerText;
                        }
                        else
                        {
                            TreeAddNode(treeView, childNode, parentTreeNode);
                        }
                    }
                }
                else if (xmlNode.LocalName == "Function")
                {
                    TreeNode treeNode = new TreeNode();

                    treeNode.ImageIndex = 4;
                    treeNode.SelectedImageIndex = 4;

                    foreach (XmlNode childNode in xmlNode.ChildNodes)
                    {
                        if (childNode.LocalName == "Syntax")
                        {
                            treeNode.Text = childNode.InnerText;
                            treeNode.Tag = childNode.InnerText;
                        }
                        else if (childNode.LocalName == "SyntaxLabel")
                        {
                            treeNode.Text = childNode.InnerText;
                        }
                        else if (childNode.LocalName == "Description")
                        {
                            treeNode.ToolTipText = childNode.InnerText;
                        }
                    }

                    if (parentTreeNode == null)
                    {
                        treeView.Nodes.Add(treeNode);
                    }
                    else
                    {
                        parentTreeNode.Nodes.Add(treeNode);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the node.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <param name="writer">The XML writer.</param>
        private static void SaveNode(TreeNode treeNode, XmlTextWriter writer)
        {
            if (treeNode.ImageIndex == 0 || treeNode.ImageIndex == 1)
            {
                writer.WriteStartElement(GroupElementName);
                writer.WriteAttributeString(NameAttributeName, treeNode.Text);

                writer.WriteElementString(DescriptionElementName, treeNode.ToolTipText);
            }
            else
            {
                writer.WriteStartElement(FunctionElementName);

                string syntax = treeNode.Tag as string;
                if (syntax == null || syntax.ToString(CultureInfo.CurrentCulture).Length == 0 || syntax == treeNode.Text)
                {
                    writer.WriteElementString(SyntaxElementName, treeNode.Text);
                }
                else
                {
                    writer.WriteElementString(SyntaxElementName, treeNode.Tag.ToString());
                    writer.WriteElementString(SyntaxLabelElementName, treeNode.Text);
                }

                writer.WriteElementString(DescriptionElementName, treeNode.ToolTipText);
            }

            foreach (TreeNode childNode in treeNode.Nodes)
            {
                SaveNode(childNode, writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Produce the XML comment for the top of the file.
        /// </summary>
        /// <returns>The XML comment string</returns>
        private static string GetTopComment()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StringBuilder text = new StringBuilder();
            text.AppendLine();
            text.AppendLine();
            text.Append("Common Expression Functions - ");
            text.AppendLine("ExpressionEditor.xml");

            text.AppendLine("Edit this file to add or remove expression functions used within the common Expression Editor for all");
            text.AppendLine("Konesans SQL Server Integration Services tools and components.");
            text.AppendLine();
            text.Append("Version ");

            text.Append(assembly.GetName().Version.ToString());

            text.Append(" - ");

            AssemblyDescriptionAttribute assemblyDescriptionAttribute = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute));
            if (assemblyDescriptionAttribute != null)
            {
                text.Append(assemblyDescriptionAttribute.Description);
            }
            else
            {
                text.Append("Invalid");
            }

            text.AppendLine();

            AssemblyCopyrightAttribute assemblyCopyrightAttribute = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute));
            if (assemblyCopyrightAttribute != null)
            {
                text.AppendLine(assemblyCopyrightAttribute.Copyright);
            }

            text.AppendLine();

            text.Append("Custom Functions - Saved ");
            text.Append(DateTime.Now.ToString("dd MMM yyyy HH:mm", CultureInfo.CurrentCulture));
            text.Append(" - ");
            text.Append(Environment.UserDomainName);
            text.AppendLine();
            text.AppendLine();

            return text.ToString();
        }
    }
}
