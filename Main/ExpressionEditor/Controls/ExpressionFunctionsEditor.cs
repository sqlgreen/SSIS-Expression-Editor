//-------------------------------------------------------------------------------------------------
// <copyright file="ExpressionEditorViewEditorPanel.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Function editor panel.
    /// </summary>
    internal partial class ExpressionFunctionsEditor : UserControl
    {
        /// <summary>
        /// Parent expression editor control
        /// </summary>
        private ExpressionEditorView expressionEditor;

        /// <summary>
        /// Current tree node.
        /// </summary>
        private TreeNode treeNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEditorViewEditorPanel"/> class.
        /// </summary>
        public ExpressionFunctionsEditor()
        {
            this.InitializeComponent();

            this.toolStripButtonSaveFunctions.Enabled = false;
        }

        /// <summary>
        /// Sets the expression editor view.
        /// </summary>
        /// <value>The expression editor view.</value>
        internal ExpressionEditorView ExpressionEditorView
        {
            set { this.expressionEditor = value; }
        }

        /// <summary>
        /// Sets the state of the current node.
        /// </summary>
        /// <param name="node">The source node.</param>
        internal void SetNode(TreeNode node)
        {
            this.treeNode = node;
            this.textBoxSyntaxLabel.Text = this.treeNode.Text;

            if (this.treeNode.ImageIndex == 0 || this.treeNode.ImageIndex == 1)
            {
                this.textBoxSyntax.Enabled = false;
                this.textBoxSyntax.Text = String.Empty;
                this.labelSyntax.Enabled = false;
            }
            else
            {
                this.textBoxSyntax.Enabled = true;
                this.labelSyntax.Enabled = true;
                if (this.treeNode.Tag != null)
                {
                    this.textBoxSyntax.Text = this.treeNode.Tag.ToString();
                    if (this.textBoxSyntax.Text == this.textBoxSyntaxLabel.Text)
                    {
                        this.textBoxSyntax.Text = String.Empty;
                    }
                }
            }

            this.textBoxDescription.Text = this.treeNode.ToolTipText;

            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;
            int index = this.treeNode.Index;

            if (index > 0)
            {
                up = true;
            }

            TreeNodeCollection parentNodes;
            if (this.treeNode.Parent == null)
            {
                parentNodes = this.treeNode.TreeView.Nodes;
            }
            else
            {
                parentNodes = this.treeNode.Parent.Nodes;
                left = true;
            }

            if (index < parentNodes.Count - 1)
            {
                down = true;
            }

            // Check for sibling Group, excluding current node in case it is a Group
            foreach (TreeNode sibling in parentNodes)
            {
                if (sibling.Index != index && (sibling.ImageIndex == 0 || sibling.ImageIndex == 1))
                {
                    right = true;
                    break;
                }
            }

            this.toolStripButtonUp.Enabled = up;
            this.toolStripButtonDown.Enabled = down;
            this.toolStripButtonLeft.Enabled = left;
            this.toolStripButtonRight.Enabled = right;
        }

        /// <summary>
        /// Sets the Save functions as enabled.
        /// </summary>
        internal void SaveEnabled()
        {
            this.toolStripButtonSaveFunctions.Enabled = true;
        }

        /// <summary>
        /// Handles the Leave event of the textBoxSyntaxLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TextBoxSyntaxLabel_Leave(object sender, EventArgs e)
        {
            // Check for changes, if none, return now.
            if (this.treeNode.Text == this.textBoxSyntaxLabel.Text)
            {
                return;
            }

            // Save new label value
            this.treeNode.Text = this.textBoxSyntaxLabel.Text;

            // Save Label as Syntax, if no Syntax supplied
            if (this.textBoxSyntax.Text.Length == 0)
            {
                this.treeNode.Tag = this.textBoxSyntaxLabel.Text;
            }

            // Flag as changed
            this.expressionEditor.ExpressionEdited();
        }

        /// <summary>
        /// Handles the Leave event of the textBoxSyntax control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TextBoxSyntax_Leave(object sender, EventArgs e)
        {
            // Get syntax, use Syntax first, else use Label
            string syntax = this.textBoxSyntax.Text;
            if (syntax.Length == 0)
            {
                syntax = this.textBoxSyntaxLabel.Text;
            }

            // Check for changes, if none, return now.
            string currentSyntax = this.treeNode.Tag as string;
            if (currentSyntax == syntax)
            {
                return;
            }

            // Save new value and flag as changed
            this.treeNode.Tag = syntax;
            this.expressionEditor.ExpressionEdited();
        }

        /// <summary>
        /// Handles the Leave event of the textBoxDescription control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TextBoxDescription_Leave(object sender, EventArgs e)
        {
            // Check for changes, if none, return now.
            if (this.treeNode.ToolTipText == this.textBoxDescription.Text)
            {
                return;
            }

            // Save new value and flag as changed
            this.treeNode.ToolTipText = this.textBoxDescription.Text;
            this.expressionEditor.ExpressionEdited();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonNewExpressionGroup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonNewExpressionGroup_Click(object sender, EventArgs e)
        {
            this.expressionEditor.NewFunctionGroup();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonNewExpressionFunction control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonNewExpressionFunction_Click(object sender, EventArgs e)
        {
            this.expressionEditor.NewFunction();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonExpressionDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonExpressionDelete_Click(object sender, EventArgs e)
        {
            this.expressionEditor.DeleteFunction();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonUp_Click(object sender, EventArgs e)
        {
            this.expressionEditor.NodeUp();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonDown_Click(object sender, EventArgs e)
        {
            this.expressionEditor.NodeDown();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonRight control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonRight_Click(object sender, EventArgs e)
        {
            this.expressionEditor.NodeRight();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonLeft control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonLeft_Click(object sender, EventArgs e)
        {
            this.expressionEditor.NodeLeft();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonSaveFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonSaveFunctions_Click(object sender, EventArgs e)
        {
            this.expressionEditor.SaveFunctions();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonCloseFunctionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonCloseFunctionEditor_Click(object sender, EventArgs e)
        {
            // Close the functions editor
            this.expressionEditor.EditMode = false;
        }
    }
}
