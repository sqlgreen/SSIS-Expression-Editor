//-------------------------------------------------------------------------------------------------
// <copyright file="ExpressionEvaluator.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor
{
    using System;
    using Microsoft.SqlServer.Dts.Runtime;
#if YUKON
    using IDTSVariableDispenserXX = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSVariableDispenser90;
#else // KATMAI, DENALI, SQL2014, 2016
    using IDTSVariableDispenserXX = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSVariableDispenser100;
#endif
    using VariableDispenser = Microsoft.SqlServer.Dts.Runtime.VariableDispenser;

    /// <summary>
    /// Wrapper for expression evaluator class.
    /// </summary>
    public sealed class ExpressionEvaluator
    {
        /// <summary>
        /// Microsoft's expression evaluator
        /// </summary>
        private Microsoft.SqlServer.Dts.Runtime.Wrapper.ExpressionEvaluatorClass expressionEvaluator = new Microsoft.SqlServer.Dts.Runtime.Wrapper.ExpressionEvaluatorClass();

        /// <summary>
        /// IDTSInfoEventsXX implementation to capture events during expression evaluation
        /// </summary>
        private DtsInfoEvents errorEvents;

        /// <summary>
        /// Private property field, <see cref="Expression"/>.
        /// </summary>
        private string expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEvaluator"/> class.
        /// </summary>
        public ExpressionEvaluator()
        {
            this.errorEvents = new DtsInfoEvents();
            this.expressionEvaluator.Events = this.errorEvents;
        }

        /// <summary>
        /// Gets or sets the expression
        /// </summary>
        public string Expression
        {
            get { return this.expression; }
            set { this.expression = value; }
        }

        /// <summary>
        /// Evaluate the expression supplied.
        /// </summary>
        /// <param name="variableDispenser">VariableDispenser used during expression evaluation.</param>
        /// <returns>An object containing the expression result.</returns>
        public object Evaluate(VariableDispenser variableDispenser)
        {
            IDTSVariableDispenserXX variableDispenserWrapper = GetVariableDispenserWrapper(variableDispenser);
            return this.Evaluate(variableDispenserWrapper);
        }

        /// <summary>
        /// Evaluate the expression supplied.
        /// </summary>
        /// <param name="variableDispenser">IDTSVariableDispenser100 used during expression evaluation.</param>
        /// <returns>An object containing the expression result.</returns>
        [CLSCompliant(false)]
        public object Evaluate(IDTSVariableDispenserXX variableDispenser)
        {
            object result;
            try
            {
                // Ensure DTSInfoEvents is clear of any previous messages
                this.errorEvents.ClearErrors();

                this.expressionEvaluator.Expression = this.expression;
                this.expressionEvaluator.Evaluate(variableDispenser, out result, true);
            }
            catch
            {
                if (this.errorEvents.Count > 0)
                {
                    string message = this.errorEvents.GetMessage();
                    throw new ExpressionException(message, this.errorEvents.Errors);
                }

                throw;
            }

            return result;
        }

        /// <summary>
        /// Validate the expression supplied.
        /// </summary>
        /// <param name="variableDispenser">VariableDispenser used during expression validation.</param>
        /// <returns>An object containing the expression result.</returns>
        public object Validate(VariableDispenser variableDispenser)
        {
            IDTSVariableDispenserXX variableDispenserWrapper = GetVariableDispenserWrapper(variableDispenser);
            return this.Validate(variableDispenserWrapper);
        }

        /// <summary>
        /// Validate the expression supplied.
        /// </summary>
        /// <param name="variableDispenser">IDTSVariableDispenser100 used during expression validation.</param>
        /// <returns>An object containing the expression result.</returns>
        [CLSCompliant(false)]
        public object Validate(IDTSVariableDispenserXX variableDispenser)
        {
            object result;
            try
            {
                // Ensure DTSInfoEvents is clear of any previous messages
                this.errorEvents.ClearErrors();

                this.expressionEvaluator.Expression = this.expression;
                this.expressionEvaluator.Evaluate(variableDispenser, out result, false);
            }
            catch
            {
                if (this.errorEvents.Count > 0)
                {
                    string message = this.errorEvents.GetMessage();
                    throw new ExpressionException(message, this.errorEvents.Errors);
                }

                throw;
            }

            return result;
        }

        /// <summary>
        /// Evaluate the expression to determine if it returns a boolean result.
        /// </summary>
        /// <param name="variableDispenser">VariableDispenser used during evaluation.</param>
        /// <returns>Boolean for whether the expression result is boolean.</returns>
        public bool IsBoolean(VariableDispenser variableDispenser)
        {
            IDTSVariableDispenserXX variableDispenserWrapper = GetVariableDispenserWrapper(variableDispenser);
            return this.IsBoolean(variableDispenserWrapper);
        }

        /// <summary>
        /// Evaluate the expression to determine if it returns a boolean result.
        /// </summary>
        /// <param name="variableDispenser">IDTSVariableDispenser100 used during evaluation.</param>
        /// <returns>Boolean for whether the expression result is boolean.</returns>
        [CLSCompliant(false)]
        public bool IsBoolean(IDTSVariableDispenserXX variableDispenser)
        {
            bool result;
            try
            {
                this.expressionEvaluator.Expression = this.expression;
                this.expressionEvaluator.IsBooleanExpression(variableDispenser, out result);
            }
            catch
            {
                if (this.errorEvents.Count > 0)
                {
                    string message = this.errorEvents.GetMessage();
                    throw new ExpressionException(message, this.errorEvents.Errors);
                }

                throw;
            }

            return result;
        }

        /// <summary>
        /// Gets the variable dispenser wrapper. This method itself is just an abstraction for the condition compilation code.
        /// </summary>
        /// <param name="variableDispenser">The variable dispenser.</param>
        /// <returns>A version typed variable dispamser wrapper.</returns>
        private static IDTSVariableDispenserXX GetVariableDispenserWrapper(VariableDispenser variableDispenser)
        {
#if YUKON
            IDTSVariableDispenserXX variableDispenserWrapper = DtsConvert.ToVariableDispenser90(variableDispenser);
#else // KATMAI, DENALI, SQL2014, 2016
            IDTSVariableDispenserXX variableDispenserWrapper = DtsConvert.GetExtendedInterface(variableDispenser);
#endif
            return variableDispenserWrapper;
        }
    }
}
