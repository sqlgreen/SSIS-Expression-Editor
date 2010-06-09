// <auto-generated/>

namespace Konesans.Dts.ExpressionEditor.Controls
{
    partial class FindReplace
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonFind = new System.Windows.Forms.Button();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.comboBoxFind = new Konesans.Dts.ExpressionEditor.Controls.MruComboBox();
            this.labelFind = new System.Windows.Forms.Label();
            this.checkBoxMatchCase = new System.Windows.Forms.CheckBox();
            this.checkBoxSearchUp = new System.Windows.Forms.CheckBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonFind = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonReplace = new System.Windows.Forms.ToolStripButton();
            this.buttonReplaceAll = new System.Windows.Forms.Button();
            this.comboBoxReplace = new Konesans.Dts.ExpressionEditor.Controls.MruComboBox();
            this.labelReplace = new System.Windows.Forms.Label();
            this.groupBoxFindOptions = new System.Windows.Forms.GroupBox();
            this.buttonOptionsToggle = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip.SuspendLayout();
            this.groupBoxFindOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonFind
            // 
            this.buttonFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFind.Location = new System.Drawing.Point(61, 228);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(96, 23);
            this.buttonFind.TabIndex = 8;
            this.buttonFind.Text = "&Find Next";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.ButtonFind_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReplace.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonReplace.Location = new System.Drawing.Point(163, 228);
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(96, 23);
            this.buttonReplace.TabIndex = 9;
            this.buttonReplace.Text = "&Replace";
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.ButtonReplace_Click);
            // 
            // comboBoxFind
            // 
            this.comboBoxFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxFind.Items.AddRange(new object[] {
            string.Empty});
            this.comboBoxFind.Location = new System.Drawing.Point(12, 52);
            this.comboBoxFind.Name = "comboBoxFind";
            this.comboBoxFind.Size = new System.Drawing.Size(349, 21);
            this.comboBoxFind.TabIndex = 2;
            this.comboBoxFind.TextChanged += new System.EventHandler(this.ComboBoxFind_TextChanged);
            // 
            // labelFind
            // 
            this.labelFind.AutoSize = true;
            this.labelFind.Location = new System.Drawing.Point(9, 36);
            this.labelFind.Name = "labelFind";
            this.labelFind.Size = new System.Drawing.Size(56, 13);
            this.labelFind.TabIndex = 1;
            this.labelFind.Text = "Fi&nd what:";
            // 
            // checkBoxMatchCase
            // 
            this.checkBoxMatchCase.AutoSize = true;
            this.checkBoxMatchCase.Location = new System.Drawing.Point(16, 23);
            this.checkBoxMatchCase.Name = "checkBoxMatchCase";
            this.checkBoxMatchCase.Size = new System.Drawing.Size(82, 17);
            this.checkBoxMatchCase.TabIndex = 6;
            this.checkBoxMatchCase.Text = "Match &case";
            this.checkBoxMatchCase.UseVisualStyleBackColor = true;
            // 
            // checkBoxSearchUp
            // 
            this.checkBoxSearchUp.AutoSize = true;
            this.checkBoxSearchUp.Location = new System.Drawing.Point(16, 46);
            this.checkBoxSearchUp.Name = "checkBoxSearchUp";
            this.checkBoxSearchUp.Size = new System.Drawing.Size(75, 17);
            this.checkBoxSearchUp.TabIndex = 7;
            this.checkBoxSearchUp.Text = "Search &up";
            this.checkBoxSearchUp.UseVisualStyleBackColor = true;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonFind,
            this.toolStripSeparator1,
            this.toolStripButtonReplace});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(373, 25);
            this.toolStrip.TabIndex = 17;
            // 
            // toolStripButtonFind
            // 
            this.toolStripButtonFind.CheckOnClick = true;
            this.toolStripButtonFind.Image = global::Konesans.Dts.ExpressionEditor.Properties.Resources.Find;
            this.toolStripButtonFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFind.Name = "toolStripButtonFind";
            this.toolStripButtonFind.Size = new System.Drawing.Size(47, 22);
            this.toolStripButtonFind.Text = "Find";
            this.toolStripButtonFind.Click += new System.EventHandler(this.ToolStripButtonFind_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonReplace
            // 
            this.toolStripButtonReplace.CheckOnClick = true;
            this.toolStripButtonReplace.Image = global::Konesans.Dts.ExpressionEditor.Properties.Resources.Replace;
            this.toolStripButtonReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReplace.Name = "toolStripButtonReplace";
            this.toolStripButtonReplace.Size = new System.Drawing.Size(65, 22);
            this.toolStripButtonReplace.Text = "Replace";
            this.toolStripButtonReplace.Click += new System.EventHandler(this.ToolStripButtonReplace_Click);
            // 
            // buttonReplaceAll
            // 
            this.buttonReplaceAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReplaceAll.Location = new System.Drawing.Point(265, 228);
            this.buttonReplaceAll.Name = "buttonReplaceAll";
            this.buttonReplaceAll.Size = new System.Drawing.Size(96, 23);
            this.buttonReplaceAll.TabIndex = 10;
            this.buttonReplaceAll.Text = "Replace &All";
            this.buttonReplaceAll.UseVisualStyleBackColor = true;
            this.buttonReplaceAll.Click += new System.EventHandler(this.ButtonReplaceAll_Click);
            // 
            // comboBoxReplace
            // 
            this.comboBoxReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxReplace.Location = new System.Drawing.Point(12, 96);
            this.comboBoxReplace.Name = "comboBoxReplace";
            this.comboBoxReplace.Size = new System.Drawing.Size(349, 21);
            this.comboBoxReplace.TabIndex = 4;
            // 
            // labelReplace
            // 
            this.labelReplace.AutoSize = true;
            this.labelReplace.Location = new System.Drawing.Point(9, 80);
            this.labelReplace.Name = "labelReplace";
            this.labelReplace.Size = new System.Drawing.Size(72, 13);
            this.labelReplace.TabIndex = 3;
            this.labelReplace.Text = "R&eplace with:";
            // 
            // groupBoxFindOptions
            // 
            this.groupBoxFindOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFindOptions.Controls.Add(this.checkBoxSearchUp);
            this.groupBoxFindOptions.Controls.Add(this.checkBoxMatchCase);
            this.groupBoxFindOptions.Location = new System.Drawing.Point(12, 131);
            this.groupBoxFindOptions.Name = "groupBoxFindOptions";
            this.groupBoxFindOptions.Size = new System.Drawing.Size(348, 80);
            this.groupBoxFindOptions.TabIndex = 6;
            this.groupBoxFindOptions.TabStop = false;
            this.groupBoxFindOptions.Text = "    Find Options";
            // 
            // buttonOptionsToggle
            // 
            this.buttonOptionsToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOptionsToggle.Location = new System.Drawing.Point(12, 131);
            this.buttonOptionsToggle.Margin = new System.Windows.Forms.Padding(0);
            this.buttonOptionsToggle.Name = "buttonOptionsToggle";
            this.buttonOptionsToggle.Size = new System.Drawing.Size(16, 16);
            this.buttonOptionsToggle.TabIndex = 5;
            this.buttonOptionsToggle.Text = "-";
            this.buttonOptionsToggle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonOptionsToggle.UseVisualStyleBackColor = true;
            this.buttonOptionsToggle.Click += new System.EventHandler(this.ButtonOptionsToggle_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(12, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "      Find Options";
            // 
            // FindReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 263);
            this.Controls.Add(this.buttonOptionsToggle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBoxFindOptions);
            this.Controls.Add(this.comboBoxReplace);
            this.Controls.Add(this.labelReplace);
            this.Controls.Add(this.buttonReplaceAll);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.buttonReplace);
            this.Controls.Add(this.comboBoxFind);
            this.Controls.Add(this.labelFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MaximumSize = new System.Drawing.Size(1000, 289);
            this.Name = "FindReplace";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find and Replace";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindReplace_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindReplace_KeyDown);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.groupBoxFindOptions.ResumeLayout(false);
            this.groupBoxFindOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.Button buttonReplace;
        private MruComboBox comboBoxFind;
        private System.Windows.Forms.Label labelFind;
        private System.Windows.Forms.CheckBox checkBoxMatchCase;
        private System.Windows.Forms.CheckBox checkBoxSearchUp;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonFind;
        private System.Windows.Forms.ToolStripButton toolStripButtonReplace;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button buttonReplaceAll;
        private MruComboBox comboBoxReplace;
        private System.Windows.Forms.Label labelReplace;
        private System.Windows.Forms.GroupBox groupBoxFindOptions;
        private System.Windows.Forms.Button buttonOptionsToggle;
        private System.Windows.Forms.Label label1;
    }
}