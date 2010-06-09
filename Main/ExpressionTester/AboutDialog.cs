//-------------------------------------------------------------------------------------------------
// <copyright file="AboutDialog.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.Tools.ExpressionTester
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Forms;
    
    /// <summary>
    /// About dialog
    /// </summary>
    public partial class AboutDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutDialog"/> class.
        /// </summary>
        /// <param name="parentForm">The parent form.</param>
        public AboutDialog(Form parentForm)
        {
            this.InitializeComponent();

            // Setup form information
            Assembly assembly = this.GetType().Assembly;

            AssemblyCopyrightAttribute copyrightAttribute = (AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute));
            this.CopyrightLabel.Text = copyrightAttribute.Copyright;
            
            AssemblyDescriptionAttribute descriptionAttribute = (AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute));
            this.DescriptionLabel.Text = descriptionAttribute.Description;
            
            AssemblyProductAttribute productAttribute = (AssemblyProductAttribute)AssemblyProductAttribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute));
            this.NameLabel.Text = productAttribute.Product;

            AssemblyFileVersionAttribute fileVersionAttribute = (AssemblyFileVersionAttribute)AssemblyTitleAttribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute));
            this.VersionLabel.Text = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", fileVersionAttribute.Version, assembly.GetName().Version);

            this.Text = string.Format(CultureInfo.InvariantCulture, "About {0}", productAttribute.Product);

            this.Icon = parentForm.Icon;
            this.picIcon.Image = parentForm.Icon.ToBitmap();
        }

        /// <summary>
        /// Handles the Click event of the OK button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
