//-------------------------------------------------------------------------------------------------
// <copyright file="DTSInfoEvents.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor
{
    using System.Collections.ObjectModel;
    using System.Text;
#if YUKON
    using IDTSInfoEventsXX = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSInfoEvents90;
#else // KATMAI, DENALI, SQL2014, SQL2016 
    using IDTSInfoEventsXX = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSInfoEvents100;
#endif

    /// <summary>
    /// Implementation of IDTSInfoEventsXX used to capture and manage a list of events.
    /// </summary>
    internal sealed class DtsInfoEvents : IDTSInfoEventsXX
    {
        /// <summary>
        /// Private member for errors collection.
        /// </summary>
        private Collection<string> errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="DtsInfoEvents"/> class.
        /// </summary>
        public DtsInfoEvents()
        {
            this.errors = new Collection<string>();
        }

        /// <summary>
        /// Gets the number of events in the list.
        /// </summary>
        public int Count
        {
            get { return this.errors.Count; }
        }

        /// <summary>
        /// Gets the error message list collection.
        /// </summary>
        public Collection<string> Errors
        {
            get { return this.errors; }
        }

        /// <summary>
        /// Clear the error list.
        /// </summary>
        public void ClearErrors()
        {
            this.errors.Clear();
        }

        /// <summary>
        /// Captures an error.
        /// </summary>
        /// <param name="errorCode">An Integer that identifies the error message</param>
        /// <param name="subComponent">An arbitrary string that identifies the sub-module within a source. For example, the transform in a Pipeline task.</param>
        /// <param name="description">Text of the message.</param>
        /// <param name="helpFile">The path to the Help file that contains detailed information.</param>
        /// <param name="helpContext">Identifier of the topic in the Help file.</param>
        /// <param name="cancel">Specifies whether execution is cancelled.</param>
        public void FireError(int errorCode, string subComponent, string description, string helpFile, int helpContext, out bool cancel)
        {
            cancel = false;
            this.errors.Add(description);

            // TODO: Catch HResult codes and add as new error string to collection. e.g. 0xC0047080 The data types "%1!s!" and "%2!s!" are incompatible for binary operator "%3!s!". The operand types could not be implicitly cast into compatible types for the operation.
        }

        /// <summary>
        /// Captures an information event.
        /// </summary>
        /// <param name="informationCode">An Integer that identifies the information message.</param>
        /// <param name="subComponent">An arbitrary string that identifies the sub-module within a source. For example, the transform in a Pipeline task.</param>
        /// <param name="description">Text of the message.</param>
        /// <param name="helpFile">The path to the Help file that contains detailed information.</param>
        /// <param name="helpContext">Identifier of the topic in the Help file.</param>
        /// <param name="fireAgain">Specifies wheter the event should be raised again during the current execution.</param>
        public void FireInformation(int informationCode, string subComponent, string description, string helpFile, int helpContext, ref bool fireAgain)
        {
        }

        /// <summary>
        /// Captures a warning event.
        /// </summary>
        /// <param name="warningCode">An Integer that identifies the information message.</param>
        /// <param name="subComponent">An arbitrary string that identifies the sub-module within a source. For example, the transform in a Pipeline task.</param>
        /// <param name="description">Text of the message.</param>
        /// <param name="helpFile">The path to the Help file that contains detailed information.</param>
        /// <param name="helpContext">Identifier of the topic in the Help file.</param>
        public void FireWarning(int warningCode, string subComponent, string description, string helpFile, int helpContext)
        {
            this.errors.Add(description);
        }

        /// <summary>
        /// Returns the total combined error and warning messages raised.
        /// </summary>
        /// <returns>A string of all error and warning messaages combined.</returns>
        public string GetMessage()
        {
            StringBuilder messages = new StringBuilder();
            foreach (string message in this.errors)
            {
                messages.AppendLine(message);
            }

            return messages.ToString();
        }
    }
}
