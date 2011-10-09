//-------------------------------------------------------------------------------------------------
// <copyright file="OptionsDialog.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.Tools.ExpressionTester
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Options dialog form.
    /// </summary>
    internal partial class OptionsDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsDialog"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        internal OptionsDialog(ProgramSettings settings)
        {
            this.InitializeComponent();
            this.propertyGridSettings.SelectedObject = settings;

            this.Text = Konesans.Dts.Tools.ExpressionTester.Properties.Resources.DialogTitleProgramOptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsDialog"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        internal OptionsDialog(ExpressionFileSettings settings)
        {
            this.InitializeComponent();
            this.propertyGridSettings.SelectedObject = settings;

            this.Text = Konesans.Dts.Tools.ExpressionTester.Properties.Resources.DialogTitleExpressionProperties;
        }

        /// <summary>
        /// Gets the expression file settings.
        /// </summary>
        /// <value>The expression file settings.</value>
        internal ExpressionFileSettings ExpressionFileSettings
        {
            get { return (ExpressionFileSettings)this.propertyGridSettings.SelectedObject; }
        }

        /// <summary>
        /// Gets the program settings.
        /// </summary>
        /// <value>The program settings.</value>
        internal ProgramSettings ProgramSettings
        {
            get { return (ProgramSettings)this.propertyGridSettings.SelectedObject; }
        }

        /// <summary>
        /// Handles the Click event of the buttonDefaults control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonDefaults_Click(object sender, EventArgs e)
        {
            if (this.propertyGridSettings.SelectedObject.GetType().Equals(typeof(ExpressionFileSettings)))
            {
                this.propertyGridSettings.SelectedObject = new ExpressionFileSettings();
            }
            else
            {
                this.propertyGridSettings.SelectedObject = new ProgramSettings();
            }
        }
    }
}
