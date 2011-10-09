//-------------------------------------------------------------------------------------------------
// <copyright file="ExpressionEditorPublic.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;
    using Konesans.Dts.ExpressionEditor.Controls;
    using Microsoft.SqlServer.Dts.Runtime;

    /// <summary>
    /// Expression editing form, hosting editor control. Used by Konesans custom tasks and BIDSHelper project.
    /// </summary>
    public partial class ExpressionEditorPublic : Form
    {
        /// <summary>
        /// Private member for the expressionEditorView control.
        /// </summary>
        private ExpressionEditorView expressionEditorView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEditorPublic"/> class for a property
        /// </summary>
        /// <param name="variables">The variables collection for the container that hosts the property.</param>
        /// <param name="variableDispenser">The variable dispenser for the container that hosts the property.</param>
        /// <param name="propertyType">The type of the property which hosts the expression.</param>
        /// <param name="propertyName">The name of the property which hosts the expression.</param>
        /// <param name="expression">The current expression if any.</param>
        public ExpressionEditorPublic(Variables variables, VariableDispenser variableDispenser, Type propertyType, string propertyName, string expression) : this(variables, variableDispenser)
        {
            this.expressionEditorView.ResultType = Type.GetTypeCode(propertyType);
            this.expressionEditorView.Expression = expression;

            this.TitleLabel.Text = "Expression for property:";
            this.TitleValueLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0} ({1})", propertyName, propertyType.Name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEditorPublic"/> class for a variable.
        /// </summary>
        /// <param name="variables">The variables collection for the host container.</param>
        /// <param name="variableDispenser">The variable dispenser for the host container.</param>
        /// <param name="variable">The variable which hosts the expression.</param>
        public ExpressionEditorPublic(Variables variables, VariableDispenser variableDispenser, Variable variable) : this(variables, variableDispenser)
        {
            this.expressionEditorView.ResultTypeValidate = true;
            this.expressionEditorView.Initialize(variableDispenser, variables);
           
            this.expressionEditorView.ResultType = variable.DataType;
            this.expressionEditorView.Expression = variable.Expression;

            this.TitleLabel.Text = "Expression for variable:";
            this.TitleValueLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0} ({1})", variable.QualifiedName, variable.DataType.ToString());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEditorPublic"/> class.
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <param name="variableDispenser">The variable dispenser.</param>
        private ExpressionEditorPublic(Variables variables, VariableDispenser variableDispenser)
        {
            // Common constructor, called by public constructors
            this.InitializeComponent();

            this.Icon = Konesans.Dts.ExpressionEditor.Resources.Expression;

            this.expressionEditorView.ResultTypeValidate = true;
            this.expressionEditorView.Initialize(variableDispenser, variables);

            // Merge the expression editor tool strip with the main form
            int index = 0;
            ToolStrip childToolStrip = this.expressionEditorView.ToolStrip;
            foreach (ToolStripItem item in childToolStrip.Items)
            {
                item.MergeAction = MergeAction.Insert;
                item.MergeIndex = index;
                index++;
            }

            ToolStripManager.Merge(this.expressionEditorView.ToolStrip, this.toolStrip);
        }

        /// <summary>
        /// Gets or sets the editor expression
        /// </summary>
        public string Expression
        {
            get { return this.expressionEditorView.Expression; }
            set { this.expressionEditorView.Expression = value; }
        }

        /// <summary>
        /// Handles the Click event of the OK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the Remove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Remove_Click(object sender, EventArgs e)
        {
            this.expressionEditorView.Expression = null;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Handle the F5 keypress at a form level, to pass down to the evaluator control.
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">KeyEventArgs e</param>
        private void ExpressionEditor_KeyUp(object sender, KeyEventArgs e)
        {
            // F5 or Ctrl+E, Ctrl+A, 
            if (e.KeyCode == Keys.F5 || (e.Control && e.KeyCode == Keys.E))
            {
                this.expressionEditorView.Run();
            }
            else if (e.Control && e.KeyCode == Keys.A)
            {
                this.expressionEditorView.SelectAllText();
            }
        }
    }
}
