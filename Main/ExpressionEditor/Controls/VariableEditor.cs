//-------------------------------------------------------------------------------------------------
// <copyright file="VariableEditor.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;
    using Microsoft.SqlServer.Dts.Runtime;

    /// <summary>
    /// Variable editor form
    /// </summary>
    internal partial class VariableEditor : Form
    {
        /// <summary>
        /// The parent variables collection.
        /// </summary>
        private Variables variables;
        
        /// <summary>
        /// The variable to edit.
        /// </summary>
        private Variable variable;

        /// <summary>
        /// The active editor control.
        /// </summary>
        private Control activeControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableEditor"/> class.
        /// </summary>
        /// <param name="variables">The variables collection.</param>
        public VariableEditor(Variables variables)
        {
            this.InitializeComponent();

            this.variables = variables;

            this.comboBoxType.Items.AddRange(
                new object[] { TypeCode.Boolean, TypeCode.Byte, TypeCode.Char, TypeCode.DateTime, TypeCode.DBNull, TypeCode.Double, TypeCode.Int16, TypeCode.Int32, TypeCode.Int64, TypeCode.Object, TypeCode.SByte, TypeCode.Single, TypeCode.String, TypeCode.UInt32, TypeCode.UInt64 });

            this.comboBoxType.SelectedItem = TypeCode.String;
            this.comboBoxValue.SelectedIndex = 0;
            this.numericUpDownValue.Value = 0;
            this.dateTimePickerValue.Value = DateTime.Now;
            this.textBoxNamespace.Text = "User";
            this.textBoxName.Focus();

            this.Icon = Konesans.Dts.ExpressionEditor.Resources.Variable;

            this.activeControl = this.textBoxName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableEditor"/> class.
        /// </summary>
        /// <param name="variables">The parent variables collection.</param>
        /// <param name="variable">The variable to edit.</param>
        public VariableEditor(Variables variables, Variable variable) : this(variables)
        {
            this.variable = variable;

            this.textBoxName.Text = variable.Name;
            this.comboBoxType.SelectedItem = variable.DataType;
            this.textBoxNamespace.Text = variable.Namespace;

            if (variable.DataType == TypeCode.Boolean)
            {
                this.comboBoxValue.SelectedIndex = Convert.ToBoolean(variable.Value, CultureInfo.CurrentCulture) ? 1 : 0;
                this.activeControl = this.comboBoxValue;
            }
            else if (variable.DataType == TypeCode.Int16
                || variable.DataType == TypeCode.Int32
                || variable.DataType == TypeCode.Int64
                || variable.DataType == TypeCode.UInt32
                || variable.DataType == TypeCode.UInt64
                || variable.DataType == TypeCode.Single
                || variable.DataType == TypeCode.Decimal
                || variable.DataType == TypeCode.Double)
            {
                this.numericUpDownValue.Value = Convert.ToDecimal(variable.Value, CultureInfo.CurrentCulture);
                this.activeControl = this.numericUpDownValue;
            }
            else if (variable.DataType == TypeCode.DateTime)
            {
                this.dateTimePickerValue.Value = Convert.ToDateTime(variable.Value, CultureInfo.CurrentCulture);
                this.activeControl = this.dateTimePickerValue;
            }
            else if (variable.DataType == TypeCode.Char)
            {
                this.comboBoxType.SelectedItem = TypeCode.Char;
                this.textBoxValue.Text = Convert.ToChar(variable.Value, CultureInfo.CurrentCulture).ToString(CultureInfo.InvariantCulture);
                this.activeControl = this.textBoxValue;
            }
            else
            {
                this.textBoxValue.Text = variable.Value.ToString();
                this.activeControl = this.textBoxValue;
            }
        }

        /// <summary>
        /// Gets the variable.
        /// </summary>
        /// <value>The variable.</value>
        public Variable Variable
        {
            get { return this.variable; }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the comboBoxType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ComboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();

            this.textBoxValue.Visible = false;
            this.comboBoxValue.Visible = false;
            this.numericUpDownValue.Visible = false;
            this.dateTimePickerValue.Visible = false;

            TypeCode type = (TypeCode)this.comboBoxType.SelectedItem;

            if (type == TypeCode.Boolean)
            {
                this.comboBoxValue.Visible = true;
            }
            else if (type == TypeCode.Int16
                || type == TypeCode.Int32
                || type == TypeCode.Int64
                || type == TypeCode.UInt32
                || type == TypeCode.UInt64
                || type == TypeCode.Single
                || type == TypeCode.Decimal
                || type == TypeCode.Double)
            {
                this.numericUpDownValue.Visible = true;
                this.numericUpDownValue.DecimalPlaces = 0;

                if (type == TypeCode.Int16)
                {
                    this.numericUpDownValue.Minimum = Int16.MinValue;
                    this.numericUpDownValue.Maximum = Int16.MaxValue;
                }
                else if (type == TypeCode.Int32)
                {
                    this.numericUpDownValue.Minimum = Int32.MinValue;
                    this.numericUpDownValue.Maximum = Int32.MaxValue;
                }
                else if (type == TypeCode.Int64)
                {
                    this.numericUpDownValue.Minimum = Int64.MinValue;
                    this.numericUpDownValue.Maximum = Int64.MaxValue;
                }
                else if (type == TypeCode.UInt32)
                {
                    this.numericUpDownValue.Minimum = UInt32.MinValue;
                    this.numericUpDownValue.Maximum = UInt32.MaxValue;
                }
                else if (type == TypeCode.UInt64)
                {
                    this.numericUpDownValue.Minimum = UInt64.MinValue;
                    this.numericUpDownValue.Maximum = UInt64.MaxValue;
                }
                else if (type == TypeCode.Single)
                {
                    // Cannot handle Single Min/Max values so make do with decimal.
                    ////this.numericUpDownValue.Minimum = Convert.ToDecimal(Single.MinValue);
                    ////this.numericUpDownValue.Maximum = Convert.ToDecimal(Single.MaxValue);
                    this.numericUpDownValue.Minimum = Decimal.MinValue;
                    this.numericUpDownValue.Maximum = Decimal.MaxValue;
                }
                else if (type == TypeCode.Decimal)
                {
                    this.numericUpDownValue.Minimum = Decimal.MinValue;
                    this.numericUpDownValue.Maximum = Decimal.MaxValue;
                }
                else if (type == TypeCode.Double)
                {
                    // Cannot handle Double Min/Max values so make do with decimal.
                    ////this.numericUpDownValue.Minimum = Convert.ToDecimal(Double.MinValue);
                    ////this.numericUpDownValue.Maximum = Convert.ToDecimal(Double.MaxValue);
                    this.numericUpDownValue.Minimum = Decimal.MinValue;
                    this.numericUpDownValue.Maximum = Decimal.MaxValue;
                }
            }
            else if (type == TypeCode.DateTime)
            {
                this.dateTimePickerValue.Visible = true;
            }
            else
            {
                this.textBoxValue.Enabled = true; 
                this.textBoxValue.Visible = true;
                this.textBoxValue.Multiline = !(type == TypeCode.Char);

                // Issue 31458 
                if (type == TypeCode.DBNull)
                {
                    this.textBoxValue.Text = string.Empty;
                    this.textBoxValue.Enabled = false;
                }
            }

            this.ResumeLayout();
        }

        /// <summary>
        /// Handles the Click event of the buttonOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.None;

                object value;
                TypeCode type = (TypeCode)this.comboBoxType.SelectedItem;

                if ((TypeCode)this.comboBoxType.SelectedItem == TypeCode.Boolean)
                {
                    value = Convert.ToBoolean(this.comboBoxValue.SelectedIndex);
                }
                else if (type == TypeCode.Int16
                    || type == TypeCode.Int32
                    || type == TypeCode.Int64
                    || type == TypeCode.UInt32
                    || type == TypeCode.UInt64
                    || type == TypeCode.Single
                    || type == TypeCode.Decimal
                    || type == TypeCode.Double)
                {
                    value = Convert.ChangeType(this.numericUpDownValue.Value, type, CultureInfo.CurrentCulture);
                }
                else if (type == TypeCode.DateTime)
                {
                    value = Convert.ChangeType(this.dateTimePickerValue.Value, type, CultureInfo.CurrentCulture);
                }
                else
                {
                    value = this.textBoxValue.Text;
                }

                // Issue 
                if (type == TypeCode.DBNull)
                {
                    value = DBNull.Value;
                }

                if (this.variable == null)
                {
                    // Issue 31458
                    if (type == TypeCode.DBNull)
                    {
                        this.variable = this.variables.Add(this.textBoxName.Text, false, this.textBoxNamespace.Text, DBNull.Value);
                    }
                    else
                    {
                        this.variable = this.variables.Add(this.textBoxName.Text, false, this.textBoxNamespace.Text, Convert.ChangeType(value, type, CultureInfo.CurrentCulture));
                    } 
                }
                else
                {
                    this.variable.Namespace = this.textBoxNamespace.Text;
                    this.variable.Name = this.textBoxName.Text;

                    // Issue 31458
                    if (type == TypeCode.DBNull)
                    {
                        this.variable.Value = DBNull.Value;
                    }
                    else
                    {
                        this.variable.Value = Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
                    }                     
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox message = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(ex);
                message.Caption = System.Windows.Forms.Application.ProductName;
                message.Show(this);
            }
        }

        /// <summary>
        /// Handles the key down event for the text controls.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void TextKeyDown(object sender, KeyEventArgs e)
        {
            TextBoxBase textBox = sender as TextBoxBase;
            if (textBox == null)
            {
                return;
            }

            if (e.Control && e.KeyCode == Keys.A)
            {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Handles the variable editor form's Shown event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void VariableEditor_Shown(object sender, EventArgs e)
        {
            this.activeControl.Focus();
        }
    }
}
