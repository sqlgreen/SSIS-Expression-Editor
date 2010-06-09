//-------------------------------------------------------------------------------------------------
// <copyright file="ExpressionFileSettings.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.Tools.ExpressionTester
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Expression file settings.
    /// </summary>
    internal class ExpressionFileSettings
    {
        /// <summary>
        /// Private property member.
        /// </summary>
        private TypeCode resultType;

        /// <summary>
        /// Private property member.
        /// </summary>
        private bool resultTypeValidate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionFileSettings"/> class.
        /// </summary>
        public ExpressionFileSettings()
        {
            this.resultType = TypeCode.Object;
            this.resultTypeValidate = true;
        }

        /// <summary>
        /// Gets or sets the result type used to validate the result against
        /// </summary>
        /// <value>The type of the result.</value>
        [Category("Settings")]
        [DisplayName("Result Type")]
        [Description("The result type used to validate the result against.")]
        public TypeCode ResultType
        {
            get { return this.resultType; }
            set { this.resultType = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the expression result should be validated against the result type specified.
        /// </summary>
        /// <value><c>true</c> if [result type validate]; otherwise, <c>false</c>.</value>
        [Category("Settings")]
        [DisplayName("Validate Result Type")]
        [Description("Indicates whether the expression result should be validated against the result type specified.")]
        public bool ResultTypeValidate
        {
            get { return this.resultTypeValidate; }
            set { this.resultTypeValidate = value; }
        }

        /// <summary>
        /// Create a deep clone of the current settings object.
        /// </summary>
        /// <returns>A new Settings object.</returns>
        public ExpressionFileSettings Clone()
        {
            ExpressionFileSettings result = new ExpressionFileSettings();
            result.ResultType = this.resultType;
            result.ResultTypeValidate = this.resultTypeValidate;
            return result;
        }
    }
}
