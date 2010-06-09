//-------------------------------------------------------------------------------------------------
// <copyright file="ExpressionException.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// The exception that is thrown when an expression validation or evaluation fails.
    /// </summary>
    [Serializable]
    public sealed class ExpressionException : Exception
    {
        /// <summary>
        /// Private member for error message collection.
        /// </summary>
        private Collection<string> errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="errors">Collection of detailed error messages.</param>
        public ExpressionException(string message, Collection<string> errors) : base(message)
        {
            this.errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionException"/> class.
        /// </summary>
        public ExpressionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ExpressionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="exception">The exception.</param>
        public ExpressionException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        private ExpressionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #region ErrorCount
        /// <summary>
        /// Gets the number of individual expression errors.
        /// </summary>
        public int ErrorCount
        {
            get { return this.errors.Count; }
        }
        #endregion

        #region Errors
        /// <summary>
        /// Gets the error message list collection.
        /// </summary>
        public Collection<string> Errors
        {
            get { return this.errors; }
        }
        #endregion

        #region ISerialization Implementation

        /// <summary>
        /// Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <PermissionSet>
        ///     <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        ///     <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_errors", this.errors);
        }

        #endregion
    }
}
