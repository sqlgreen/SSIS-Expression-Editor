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
        /// Initializes a new instance of the <see cref="ExpressionEditorPublic"/> class.
        /// </summary>
        /// <param name="variables">The variables collection for the container that hosts the property.</param>
        /// <param name="variableDispenser">The variable dispenser for the container that hosts the property.</param>
        /// <param name="propertyType">The type of the property which hosts the expression.</param>
        /// <param name="propertyName">The name of the property which hosts the expression.</param>
        /// <param name="expression">The current expression if any.</param>
        public ExpressionEditorPublic(Variables variables, VariableDispenser variableDispenser, Type propertyType, string propertyName, string expression)
        {
            this.InitializeComponent();

            this.Icon = Konesans.Dts.ExpressionEditor.Properties.Resources.Expression;

            this.expressionEditorView.ResultTypeValidate = true;
            this.expressionEditorView.ResultType = Type.GetTypeCode(propertyType);
            this.expressionEditorView.Initialize(variableDispenser, variables);
            this.expressionEditorView.Expression = expression;

            this.labelFormTitle.Text = String.Format(CultureInfo.CurrentCulture, "Expression for property: {0} ({1})", propertyName, propertyType.Name);
        }

        #region Expression
        /// <summary>
        /// Gets or sets the editor expression
        /// </summary>
        public string Expression
        {
            get { return this.expressionEditorView.Expression; }
            set { this.expressionEditorView.Expression = value; }
        }
        #endregion

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

        #region ExpressionEditor_KeyUp - F5 and Ctrl+A
        /// <summary>
        /// Handle the F5 keypress at a form level, to pass down to the evaluator control.
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">KeyEventArgs e</param>
        private void ExpressionEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.expressionEditorView.Run();
            }
            else if (e.Control && e.KeyCode == Keys.A)
            {
                this.expressionEditorView.SelectAllText();
            }
        }
        #endregion
    }
}
