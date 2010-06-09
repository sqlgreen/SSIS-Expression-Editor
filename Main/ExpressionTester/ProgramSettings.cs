//-------------------------------------------------------------------------------------------------
// <copyright file="ProgramSettings.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.Tools.ExpressionTester
{
    using System.ComponentModel;
    using System.Drawing;

    /// <summary>
    /// Program setttings class.
    /// </summary>
    internal class ProgramSettings
    {
        /// <summary>
        /// Private member for property.
        /// </summary>
        private Color expressionColor;

        /// <summary>
        /// Private member for property.
        /// </summary>
        private Font expressionFont;

        /// <summary>
        /// Private member for property.
        /// </summary>
        private Color resultColor;

        /// <summary>
        /// Private member for property.
        /// </summary>
        private Font resultFont;
        
        /// <summary>
        /// Private member for property.
        /// </summary>
        private bool findNotFound;
        
        /// <summary>
        /// Private member for property.
        /// </summary>
        private bool replaceAllCount;
        
        /// <summary>
        /// Private member for property.
        /// </summary>
        private bool matchCase;

        /// <summary>
        /// Private member for property.
        /// </summary>
        private bool searchUp;        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramSettings"/> class.
        /// </summary>
        public ProgramSettings()
        {
            this.expressionColor = SystemColors.WindowText;
            this.expressionFont = SystemFonts.DefaultFont;
            this.resultColor = SystemColors.WindowText;
            this.resultFont = SystemFonts.DefaultFont;
            this.findNotFound = true;
            this.replaceAllCount = true;
        }

        /// <summary>
        /// Gets or sets the color of the expression text.
        /// </summary>
        /// <value>The color of the expression text.</value>
        [Category("Appearance")]
        [DisplayName("Expression Color")]
        [Description("The color used for the expression text.")]
        public Color ExpressionColor
        {
            get { return this.expressionColor; }
            set { this.expressionColor = value; }
        }

        /// <summary>
        /// Gets or sets the expression text font.
        /// </summary>
        /// <value>The expression text font.</value>
        [Category("Appearance")]
        [DisplayName("Expression Font")]
        [Description("The font used for the expression text.")]
        public Font ExpressionFont
        {
            get { return this.expressionFont; }
            set { this.expressionFont = value; }
        }

        /// <summary>
        /// Gets or sets the color of the result text.
        /// </summary>
        /// <value>The color of the result text.</value>
        [Category("Appearance")]
        [DisplayName("Result Color")]
        [Description("The color used for the result text.")]
        public Color ResultColor
        {
            get { return this.resultColor; }
            set { this.resultColor = value; }
        }

        /// <summary>
        /// Gets or sets the result text font.
        /// </summary>
        /// <value>The result text font.</value>
        [Category("Appearance")]
        [DisplayName("Result Font")]
        [Description("The font used for the result text.")]
        public Font ResultFont
        {
            get { return this.resultFont; }
            set { this.resultFont = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display the find not found message.
        /// </summary>
        /// <value><c>true</c> to show the message; otherwise, <c>false</c>.</value>
        [Category("Find and Replace")]
        [DisplayName("Display Find Not Found")]
        [Description("Display message box when the search text cannot be found.")]
        public bool FindNotFound
        {
            get { return this.findNotFound; }
            set { this.findNotFound = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display the replace all count.
        /// </summary>
        /// <value><c>true</c> to show the message; otherwise, <c>false</c>.</value>
        [Category("Find and Replace")]
        [DisplayName("Display Replace All Count")]
        [Description("Display message box with replacement count.")]
        public bool ReplaceAllCount
        {
            get { return this.replaceAllCount; }
            set { this.replaceAllCount = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [match case].
        /// </summary>
        /// <value><c>true</c> if [match case]; otherwise, <c>false</c>.</value>
        [Category("Find and Replace")]
        [DisplayName("Match Case")]
        [Description("Default find and replace match case setting.")]
        public bool MatchCase
        {
            get { return this.matchCase; }
            set { this.matchCase = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [search up].
        /// </summary>
        /// <value><c>true</c> if [search up]; otherwise, <c>false</c>.</value>
        [Category("Find and Replace")]
        [DisplayName("Search Up")]
        [Description("Default find and replace search up option setting.")]
        public bool SearchUp
        {
            get { return this.searchUp; }
            set { this.searchUp = value; }
        }

        /*
        private string template;
         *
        [Category("Expressions")]
        [DisplayName("Template File")]
        [Description("The template file used when creating a new expression file.")]
        [Editor(typeof(ExpressionFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TemplateFilename
        {
            get { return template; }
            set { template = value; }
        }
        */

        /// <summary>
        /// Create a deep clone of the current settings object.
        /// </summary>
        /// <returns>A new Settings object.</returns>
        public ProgramSettings Clone()
        {
            ProgramSettings result = new ProgramSettings();
            result.ExpressionColor = this.expressionColor;
            result.ExpressionFont = this.expressionFont;
            result.ResultColor = this.resultColor;
            result.ResultFont = this.resultFont;
            result.FindNotFound = this.findNotFound;
            result.ReplaceAllCount = this.replaceAllCount;
            result.MatchCase = this.matchCase;
            result.SearchUp = this.searchUp;

            ////result.TemplateFilename = template;
            return result;
        }
    }
}
