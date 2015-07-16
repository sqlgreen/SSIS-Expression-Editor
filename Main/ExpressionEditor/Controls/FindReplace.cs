//-------------------------------------------------------------------------------------------------
// <copyright file="FindReplace.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Find and Replace form.
    /// </summary>
    internal partial class FindReplace : Form
    {
        /// <summary>
        /// Text box control for find and replace operation.
        /// </summary>
        private TextBoxBase textBox;

        /// <summary>
        /// Postion for form resize
        /// </summary>
        private int findButtonLeftOffset;

        /// <summary>
        /// Postion for form resize
        /// </summary>
        private int findFormHeight;
        
        /// <summary>
        /// Postion for form resize
        /// </summary>
        private int replaceFormHeight;
        
        /// <summary>
        /// Postion for form resize
        /// </summary>
        private int findOptionsTop;

        /// <summary>
        /// Postion for form resize
        /// </summary>
        private int replaceOptionsTop;
        
        /// <summary>
        /// Form height
        /// </summary>
        private int optionsHeight;
        
        /// <summary>
        /// Current form height
        /// </summary>
        private int currentOptionsHeightOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindReplace"/> class.
        /// </summary>
        /// <param name="textBox">TextBoxBase control upon which to perform the to find or replace.</param>
        public FindReplace(TextBoxBase textBox)
        {
            this.InitializeComponent();

            this.textBox = textBox;

            this.findButtonLeftOffset = this.buttonReplaceAll.Left - this.buttonFind.Left;
            this.replaceFormHeight = this.Height;
            this.findFormHeight = this.Height - (this.comboBoxReplace.Top - this.comboBoxFind.Top);
            this.findOptionsTop = this.groupBoxFindOptions.Top - (this.comboBoxReplace.Top - this.comboBoxFind.Top);
            this.replaceOptionsTop = this.groupBoxFindOptions.Top;
            this.optionsHeight = this.groupBoxFindOptions.Height;

            this.AcceptButton = this.buttonFind;
        }

        /// <summary>
        /// Occurs when the search text is not found].
        /// </summary>
        internal event EventHandler<FindOrReplaceEventArgs> FindNotFound;

        /// <summary>
        /// Occurs when replace all operation is complete.
        /// </summary>
        internal event EventHandler<FindOrReplaceEventArgs> ReplaceAllComplete;

        /// <summary>
        /// Gets or sets a value indicating whether searching is case sensitive.
        /// </summary>
        /// <value><c>true</c> if case sensitive; otherwise, <c>false</c>.</value>
        public bool MatchCase
        {
            get { return this.checkBoxMatchCase.Checked; }
            set { this.checkBoxMatchCase.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to search upwards.
        /// </summary>
        /// <value><c>true</c> if search upwards; otherwise, <c>false</c>.</value>
        public bool SearchUp
        {
            get { return this.checkBoxSearchUp.Checked; }
            set { this.checkBoxSearchUp.Checked = value; }
        }

        /// <summary>
        /// Raises the <see cref="E:FindNotFound"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.FindOrReplaceEventArgs"/> instance containing the event data.</param>
        public virtual void OnFindNotFound(FindOrReplaceEventArgs e)
        {
            if (this.FindNotFound != null)
            {
                this.FindNotFound(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ReplaceAllComplete"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.FindOrReplaceEventArgs"/> instance containing the event data.</param>
        public virtual void OnReplaceAllComplete(FindOrReplaceEventArgs e)
        {
            if (this.ReplaceAllComplete != null)
            {
                this.ReplaceAllComplete(this, e);
            }
        }

        /// <summary>
        /// Shows this form with the specified owner to the user.
        /// </summary>
        /// <param name="owner">Any object that implements <see cref="T:System.Windows.Forms.IWin32Window"/> and represents the top-level window that will own this form.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The form specified in the <paramref name="owner"/> parameter is the same as the form being shown.
        /// </exception>
        /// <PermissionSet>
        ///     <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///     <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///     <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        ///     <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public new void Show(IWin32Window owner)
        {
            base.Show(owner);

            // Force button disable by default
            this.comboBoxFind.Text = string.Empty;

            if (this.textBox.SelectionLength > 0)
            {
                this.comboBoxFind.Text = this.textBox.SelectedText;
            }
        }

        /// <summary>
        /// Setup form for find or replace opertions. Controls layout and enabling of controls as appropriate.
        /// </summary>
        /// <param name="find">True for find operations, or false for replace operations.</param>
        internal void SetFind(bool find)
        {
            this.FindOrReplaceLayout(find);
        }

        /// <summary>
        /// Reverses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The reverse input string.</returns>
        private static string Reverse(string input)
        {
            StringBuilder result = new StringBuilder(input.Length);

            char[] buffer = input.ToCharArray();

            for (int index = buffer.Length - 1; index >= 0; index--)
            {
                result.Append(buffer[index]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Handles the Click event of the buttonFind control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonFind_Click(object sender, EventArgs e)
        {
            string findText = this.comboBoxFind.GetText();

            // Double check as this will cause an infinate loop.
            if (String.IsNullOrEmpty(findText))
            {
                return;
            }

            int result = this.Find(findText, this.checkBoxMatchCase.Checked, this.checkBoxSearchUp.Checked);

            if (result == -1)
            {
                this.OnFindNotFound(new FindOrReplaceEventArgs(findText));
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonReplace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonReplace_Click(object sender, EventArgs e)
        {
            string findText = this.comboBoxFind.GetText();
            string replaceText = this.comboBoxReplace.GetText();

            // Double check as this will cause an infinate loop.
            if (String.IsNullOrEmpty(findText))
            {
                return;
            }

            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;

            if (this.checkBoxMatchCase.Checked)
            {
                comparisonType = StringComparison.InvariantCulture;
            }

            string selection = this.textBox.SelectedText;

            if (selection.Equals(findText, comparisonType))
            {
                int selectionStart = this.textBox.SelectionStart;
                string expression = this.textBox.Text.Remove(selectionStart, findText.Length);
                this.textBox.Text = expression.Insert(selectionStart, replaceText);
                this.textBox.SelectionStart = selectionStart;
                this.textBox.SelectionLength = replaceText.Length;
            }

            int result = this.Find(findText, this.checkBoxMatchCase.Checked, this.checkBoxSearchUp.Checked);

            if (result == -1)
            {
                this.OnFindNotFound(new FindOrReplaceEventArgs(findText));
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonReplaceAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonReplaceAll_Click(object sender, EventArgs e)
        {
            string findText = this.comboBoxFind.GetText();

            // Double check as this will cause an infinate loop.
            if (String.IsNullOrEmpty(findText))
            {
                return;
            }

            int result = this.ReplaceAll(findText, this.comboBoxReplace.GetText(), this.checkBoxMatchCase.Checked, this.checkBoxSearchUp.Checked);

            if (result > 0)
            {
                this.OnReplaceAllComplete(new FindOrReplaceEventArgs(findText, result));
            }
            else
            {
                this.OnFindNotFound(new FindOrReplaceEventArgs(findText));
            }
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonFind control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonFind_Click(object sender, EventArgs e)
        {
            this.FindOrReplaceLayout(true);
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonReplace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonReplace_Click(object sender, EventArgs e)
        {
            this.FindOrReplaceLayout(false);
        }

        /// <summary>
        /// Changes the layout between Find and Replace modes.
        /// </summary>
        /// <param name="find">if set to <c>true</c> find mode; otherwise false.</param>
        private void FindOrReplaceLayout(bool find)
        {
            this.toolStripButtonFind.Checked = find;
            this.toolStripButtonReplace.Checked = !find;

            this.buttonReplace.Visible = !find;
            this.buttonReplaceAll.Visible = !find;
            this.comboBoxReplace.Visible = !find;
            this.labelReplace.Visible = !find;

            int formHeight;
            if (find)
            {
                formHeight = this.findFormHeight + this.currentOptionsHeightOffset;
                this.buttonFind.Location = new Point(this.buttonReplaceAll.Left, this.buttonReplaceAll.Top);
                this.groupBoxFindOptions.Top = this.findOptionsTop;
                this.label1.Top = this.findOptionsTop;
                this.buttonOptionsToggle.Top = this.findOptionsTop;
            }
            else
            {
                formHeight = this.replaceFormHeight + this.currentOptionsHeightOffset;
                this.buttonFind.Location = new Point(this.buttonReplaceAll.Left - this.findButtonLeftOffset, this.buttonReplaceAll.Top);
                this.groupBoxFindOptions.Top = this.replaceOptionsTop;
                this.label1.Top = this.replaceOptionsTop;
                this.buttonOptionsToggle.Top = this.replaceOptionsTop;
            }

            this.MaximumSize = new Size(this.MaximumSize.Width, formHeight);
            this.Size = new Size(this.Width, formHeight);
        }

        /// <summary>
        /// Perform Find operation.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="caseSensitive">if set to <c>true</c> find is case sensitive.</param>
        /// <param name="searchUp">if set to <c>true</c> search upwards.</param>
        /// <returns>The start position of the text found.</returns>
        private int Find(string searchTerm, bool caseSensitive, bool searchUp)
        {
            int startIndex = this.textBox.SelectionStart + this.textBox.SelectionLength;
            string expression = this.textBox.Text;

            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;

            if (caseSensitive)
            {
                comparisonType = StringComparison.InvariantCulture;
            }

            if (searchUp)
            {
                searchTerm = Reverse(searchTerm);
                expression = Reverse(expression);
            }

            int position = startIndex;

            while (position >= 0)
            {
                position = expression.IndexOf(searchTerm, position, comparisonType);

                if (position >= 0)
                {
                    if (searchUp)
                    {
                        this.textBox.SelectionStart = this.textBox.Text.Length - (position + searchTerm.Length);
                        this.textBox.SelectionLength = searchTerm.Length;
                        return this.textBox.Text.Length - position;
                    }
                    else
                    {
                        this.textBox.SelectionStart = position;
                        this.textBox.SelectionLength = searchTerm.Length;
                        return position;
                    }
                }
                else
                {
                    if (startIndex > 0)
                    {
                        position = 0;
                        startIndex = 0;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Perform Replace All operation.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="replaceTerm">The replace term.</param>
        /// <param name="caseSensitive">if set to <c>true</c> case sensitive.</param>
        /// <param name="searchUp">if set to <c>true</c> search upwards.</param>
        /// <returns>THe number of replacements made.</returns>
        private int ReplaceAll(string searchTerm, string replaceTerm, bool caseSensitive, bool searchUp)
        {
            int startIndex = this.textBox.SelectionStart + this.textBox.SelectionLength;
            string expression = this.textBox.Text;
            int trackFullPass = expression.Length;
            if (startIndex > 0)
            {
                trackFullPass = startIndex;
            }

            int replaceDiff = replaceTerm.Length - searchTerm.Length;

            System.Diagnostics.Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "trackFullPass {0}", trackFullPass));

            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;

            if (caseSensitive)
            {
                comparisonType = StringComparison.InvariantCulture;
            }

            int replaceCount = 0;

            if (searchUp)
            {
                searchTerm = Reverse(searchTerm);
                replaceTerm = Reverse(replaceTerm);
                expression = Reverse(expression);
            }

            int position = startIndex;

            while (position >= 0)
            {
                position = expression.IndexOf(searchTerm, position, comparisonType);

                if (startIndex == 0 && (position > trackFullPass))
                {
                    break;
                }

                if (position >= 0)
                {
                    expression = expression.Remove(position, searchTerm.Length);
                    expression = expression.Insert(position, replaceTerm);
                    replaceCount++;

                    position += replaceTerm.Length;

                    if (startIndex == 0)
                    {
                        trackFullPass += replaceDiff;
                    }
                }
                else
                {
                    if (startIndex > 0)
                    {
                        position = 0;
                        startIndex = 0;
                    }
                }
            }

            if (searchUp)
            {
                expression = Reverse(expression);
            }

            if (replaceCount > 0)
            {
                this.textBox.Text = expression;
            }

            return replaceCount;
        }

        /// <summary>
        /// Handles the KeyDown event of the FindReplace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void FindReplace_KeyDown(object sender, KeyEventArgs e)
        {
            // This handler requires Form.KeyPreview to be true.
            if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
            }
        }

        /// <summary>
        /// Handles the FormClosing event of the FindReplace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void FindReplace_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonOptionsToggle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonOptionsToggle_Click(object sender, EventArgs e)
        {
            // Check state, collpased (0) or expanded (>0)
            if (this.groupBoxFindOptions.Height == 0)
            {
                // Expand
                this.groupBoxFindOptions.Height = this.optionsHeight;
                this.buttonOptionsToggle.Text = "-";
                this.label1.Visible = false;
                this.currentOptionsHeightOffset = 0;
            }
            else
            {
                // Collapse
                this.groupBoxFindOptions.Height = 0;
                this.buttonOptionsToggle.Text = "+";
                this.label1.Visible = true;
                this.currentOptionsHeightOffset = (this.optionsHeight * -1) + this.label1.Height;
            }

            this.FindOrReplaceLayout(this.toolStripButtonFind.Checked);
        }

        /// <summary>
        /// Handles the TextChanged event of the comboBoxFind control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ComboBoxFind_TextChanged(object sender, EventArgs e)
        {
            MruComboBox combo = sender as MruComboBox;
            bool enableButtons = !String.IsNullOrEmpty(combo.Text);

            this.buttonFind.Enabled = enableButtons;
            this.buttonReplace.Enabled = enableButtons;
            this.buttonReplaceAll.Enabled = enableButtons;
        }
    }
}
