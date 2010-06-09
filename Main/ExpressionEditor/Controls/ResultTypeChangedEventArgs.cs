//-------------------------------------------------------------------------------------------------
// <copyright file="ResultTypeChangedEventArgs.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Result type changed EventArgs
    /// </summary>
    public class ResultTypeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Private property field.
        /// </summary>
        private TypeCode resultType;

        /// <summary>
        /// Private property field.
        /// </summary>
        private bool validate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultTypeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="validate">if set to <c>true</c> [validate].</param>
        public ResultTypeChangedEventArgs(TypeCode resultType, bool validate)
        {
            this.ResultType = resultType;
            this.Validate = validate;
        }

        /// <summary>
        /// Gets or sets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public TypeCode ResultType
        {
            get
            {
                return this.resultType;
            }

            set
            {
                this.resultType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ResultTypeChangedEventArgs"/> is validate.
        /// </summary>
        /// <value><c>true</c> if validate; otherwise, <c>false</c>.</value>
        public bool Validate
        {
            get
            {
                return this.validate;
            }

            set
            {
                this.validate = value;
            }
        }
    }
}
