//-------------------------------------------------------------------------------------------------
// <copyright file="ExpressionEditorView.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;
    using Microsoft.SqlServer.Dts.Runtime;

    /// <summary>
    /// Expression editor and builder control.
    /// </summary>
    public partial class ExpressionEditorView : UserControl
    {
        /// <summary>
        /// Default functions document file name.
        /// </summary>
        private const string FunctionsDocumentFileName = "ExpressionEditor.xml";

        /// <summary>
        /// Syntax text token
        /// </summary>
        private const string LeftParameterBoundary1 = "«";

        /// <summary>
        /// Syntax text token
        /// </summary>
        private const string RightParameterBoundary1 = "»";

        /// <summary>
        /// Constant node name for variables node
        /// </summary>
        private const string VariablesNodeName = "Variables";

        /// <summary>
        /// Format string for variable as an expression
        /// </summary>
        private const string VariablesDragTextFormat = "@[{0}]";

        /// <summary>
        /// Constant node name for system variables node
        /// </summary>
        private const string SystemVariablesNodeName = "SystemVariables";

        /// <summary>
        /// Current treeview node
        /// </summary>
        private TreeNode variablesNode;

        /// <summary>
        /// Private property member
        /// </summary>
        private bool editMode;

        /// <summary>
        /// The expression evaluator class
        /// </summary>
        private ExpressionEvaluator expressionEvaluator;

        /// <summary>
        /// Variable dispenser
        /// </summary>
        private VariableDispenser variableDispenser;

        /// <summary>
        /// Variables collection
        /// </summary>
        private Variables variables;

        /// <summary>
        /// Container that hosts the property, used when we own the package as per the Expression Tester
        /// </summary>
        private DtsContainer host;

        /// <summary>
        /// The active text control
        /// </summary>
        private TextBoxBase activeTextControl;

        /// <summary>
        /// The edit Undo stack
        /// </summary>
        private Stack<UndoText> undoStack = new Stack<UndoText>(100);

        /// <summary>
        /// The edit redo stack
        /// </summary>
        private Stack<UndoText> redoStack = new Stack<UndoText>(100);

        /// <summary>
        /// Ignore events flag
        /// </summary>
        private bool ignoreEvents;

        /// <summary>
        /// Undo operation flag
        /// </summary>
        private bool undoFlag;

        /// <summary>
        /// Private property field with default value
        /// </summary>
        private string applicationTitle = "Expression Editor";

        /// <summary>
        /// Find and replace form
        /// </summary>
        private FindReplace findReplace;

        /// <summary>
        /// The current functions file name
        /// </summary>
        private string functionsFileName;

        /// <summary>
        /// The functions XML
        /// </summary>
        private XmlDocument functionsDocument;

        /// <summary>
        /// Private property member.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Private property member.
        /// </summary>
        private bool saved;

        /// <summary>
        /// Private property member.
        /// </summary>
        private TypeCode resultType;

        /// <summary>
        /// Private property member.
        /// </summary>
        private bool resultTypeValidate = true;

        /// <summary>
        /// Private property member.
        /// </summary>
        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEditorView"/> class.
        /// </summary>
        public ExpressionEditorView()
        {
            this.InitializeComponent();

            this.toolTip.SetToolTip(this.linkEvaluate, "Evaluate expression");
            this.expressionEvaluator = new ExpressionEvaluator();

            this.findReplace = new FindReplace(this.ExpressionTextBox);
            this.findReplace.FindNotFound += new EventHandler<FindOrReplaceEventArgs>(this.FindReplaceInternal_FindNotFound);
            this.findReplace.ReplaceAllComplete += new EventHandler<FindOrReplaceEventArgs>(this.FindReplaceInternal_ReplaceAllComplete);

            this.expressionEditorViewEditorPanel.ExpressionEditorView = this;

            this.imageListIcons.Images.Add(Properties.Resources.Folder);
            this.imageListIcons.Images.Add(Properties.Resources.FolderOpen);
            this.imageListIcons.Images.Add(Properties.Resources.Variable);
            this.imageListIcons.Images.Add(Properties.Resources.SystemVariable);
            this.imageListIcons.Images.Add(Properties.Resources.Expression);

            this.ExpressionTextBox.EnableAutoDragDrop = true;
            this.ExpressionTextBox.AllowDrop = true;
            this.ExpressionTextBox.DragEnter += new DragEventHandler(this.ExpressionTextBox_DragEnter);
        }

        /// <summary>
        /// Occurs when cursor position changes.
        /// </summary>
        public event EventHandler<CursorPositionChangedEventArgs> CursorPositionChanged;

        /// <summary>
        /// Occurs when [variable selection changed].
        /// </summary>
        public event EventHandler<VariableSelectionChangedEventArgs> VariableSelectionChanged;

        /// <summary>
        /// Occurs when title should change.
        /// </summary>
        public event EventHandler<TitleChangedEventArgs> TitleChanged;

        /// <summary>
        /// Occurs when result type changes.
        /// </summary>
        public event EventHandler<ResultTypeChangedEventArgs> ResultTypeChanged;

        /// <summary>
        /// Occurs when the undo or redo count changes.
        /// </summary>
        public event EventHandler<UndoOrRedoCountChangedEventArgs> UndoOrRedoCountChanged;

        /// <summary>
        /// Occurs when no matching text can be found during a Find operation.
        /// </summary>
        public event EventHandler<FindOrReplaceEventArgs> FindNotFound;

        /// <summary>
        /// Occurs when areplace all operation completes..
        /// </summary>
        public event EventHandler<FindOrReplaceEventArgs> ReplaceAllComplete;

        /// <summary>
        /// Occurs when EditMode property is changed..
        /// </summary>
        public event EventHandler<EventArgs> EditModeChanged;

        /// <summary>
        /// Gets or sets the application title.
        /// </summary>
        /// <value>The application title.</value>
        public string ApplicationTitle
        {
            get
            {
                return this.applicationTitle;
            }

            set
            {
                this.applicationTitle = value;
            }
        }

        /// <summary>
        /// Gets or sets the editor expression
        /// </summary>
        public string Expression
        {
            get { return this.ExpressionTextBox.Text; }
            set { this.ExpressionTextBox.Text = value; }
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get { return this.fileName; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ExpressionEditorView"/> is saved.
        /// </summary>
        /// <value><c>true</c> if saved; otherwise, <c>false</c>.</value>
        public bool Saved
        {
            get { return this.saved; }
            set { this.saved = value; }
        }

        /// <summary>
        /// Gets or sets the color of the expression text.
        /// </summary>
        /// <value>The color of the expression.</value>
        [Category("Appearance")]
        [DisplayName("Expression Color")]
        [Description("The color used for the expression text.")]
        public Color ExpressionColor
        {
            get 
            { 
                return this.ExpressionTextBox.ForeColor; 
            }

            set
            {
                this.ignoreEvents = true;
                this.ExpressionTextBox.ForeColor = value;
                this.ignoreEvents = false;
            }
        }

        /// <summary>
        /// Gets or sets the expression text font.
        /// </summary>
        /// <value>The expression font.</value>
        [Category("Appearance")]
        [DisplayName("Expression Font")]
        [Description("The font used for the expression text.")]
        public Font ExpressionFont
        {
            get 
            { 
                return this.ExpressionTextBox.Font; 
            }

            set
            {
                this.ignoreEvents = true;
                this.ExpressionTextBox.Font = value;
                this.ignoreEvents = false;
            }
        }

        /// <summary>
        /// Gets or sets the color of the result text.
        /// </summary>
        /// <value>The color of the result.</value>
        [Category("Appearance")]
        [DisplayName("Result Color")]
        [Description("The color used for the result text.")]
        public Color ResultColor
        {
            get { return this.richTextResult.ForeColor; }
            set { this.richTextResult.ForeColor = value; }
        }

        /// <summary>
        /// Gets or sets the result text font.
        /// </summary>
        /// <value>The result font.</value>
        [Category("Appearance")]
        [DisplayName("Result Font")]
        [Description("The font used for the result text.")]
        public Font ResultFont
        {
            get { return this.richTextResult.Font; }
            set { this.richTextResult.Font = value; }
        }

        /// <summary>
        /// Gets or sets the result type.
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
                this.OnResultTypeChanged(new ResultTypeChangedEventArgs(this.resultType, this.resultTypeValidate));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the result type.
        /// </summary>
        /// <value><c>true</c> to validate the result; otherwise, <c>false</c>.</value>
        public bool ResultTypeValidate
        {
            get
            {
                return this.resultTypeValidate;
            }

            set
            {
                this.resultTypeValidate = value;
                this.OnResultTypeChanged(new ResultTypeChangedEventArgs(this.resultType, this.resultTypeValidate));
            }
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return this.title; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether finad and replace is case sensitive.
        /// </summary>
        /// <value>
        ///     <c>true</c> if case sensitive; otherwise, <c>false</c>.
        /// </value>
        public bool FindOrReplaceMatchCase
        {
            get { return this.findReplace.MatchCase; }
            set { this.findReplace.MatchCase = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether find abd replace searches upwards.
        /// </summary>
        /// <value>
        ///     <c>true</c> if search is upwards; otherwise, <c>false</c>.
        /// </value>
        public bool FindOrReplaceSearchUp
        {
            get { return this.findReplace.SearchUp; }
            set { this.findReplace.SearchUp = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether word wrap is on.
        /// </summary>
        /// <value><c>true</c> if word wrap is on; otherwise, <c>false</c>.</value>
        public bool WordWrap
        {
            get { return this.ExpressionTextBox.WordWrap; }
            set { this.ExpressionTextBox.WordWrap = value; }
        }

        /// <summary>
        /// Gets or sets the main splitter location.
        /// </summary>
        /// <value>The splitter distance main.</value>
        public int SplitterDistanceMain
        {
            get { return this.splitContainerMain.SplitterDistance; }
            set { this.splitContainerMain.SplitterDistance = value; }
        }

        /// <summary>
        /// Gets or sets the result splitter location.
        /// </summary>
        /// <value>The splitter distance result.</value>
        public int SplitterDistanceResult
        {
            get { return this.splitContainerExpressionResult.SplitterDistance; }
            set { this.splitContainerExpressionResult.SplitterDistance = value; }
        }

        /// <summary>
        /// Gets or sets the expressions splitter location.
        /// </summary>
        /// <value>The splitter distance expressions.</value>
        public int SplitterDistanceExpressions
        {
            get { return this.splitContainerFunctionExpressions.SplitterDistance; }
            set { this.splitContainerFunctionExpressions.SplitterDistance = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether function edit mode is on.
        /// </summary>
        /// <value><c>true</c> if in function edit mode; otherwise, <c>false</c>.</value>
        public bool EditMode
        {
            get
            {
                return this.editMode;
            }

            set
            {
                if (this.editMode == value)
                {
                    return;
                }

                this.splitContainerExpressionResult.Visible = !value;
                this.expressionEditorViewEditorPanel.Visible = value;
                this.treeViewVariablesFunctions.HideSelection = !value;
                this.treeViewVariablesFunctions.AllowDrop = value;

                if (value)
                {
                    this.variablesNode.Remove();

                    if (this.treeViewVariablesFunctions.SelectedNode == null)
                    {
                        this.treeViewVariablesFunctions.SelectedNode = this.treeViewVariablesFunctions.Nodes[0];
                    }

                    this.expressionEditorViewEditorPanel.SetNode(this.treeViewVariablesFunctions.SelectedNode);
                }
                else
                {
                    this.treeViewVariablesFunctions.Nodes.Insert(0, this.variablesNode);
                }

                this.editMode = value;
                this.OnEditModeChanged(new EventArgs());
            }
        }

        /// <summary>
        /// Initializes the control.
        /// </summary>
        /// <param name="variableDispenser">The variable dispenser.</param>
        /// <param name="variables">The variables.</param>
        public void Initialize(VariableDispenser variableDispenser, Variables variables)
        {
            this.variableDispenser = variableDispenser;
            this.variables = variables;

            this.InitializeSetup();

            this.PopulateFunctionsTree();
            this.treeViewVariablesFunctions.Nodes[0].Expand();
            this.ignoreEvents = false;
        }

        /// <summary>
        /// Initializes the control.
        /// </summary>
        /// <remarks>Used by the Expresion Tester tool.</remarks>
        /// <param name="file">The expression filename.</param>
        /// <param name="isTemplate">if set to <c>true</c> the file specified is a template.</param>
        public void Initialize(string file, bool isTemplate)
        {
            Package package = new Package();
            this.host = package;
            this.variableDispenser = package.VariableDispenser;
            this.variables = package.Variables;
            this.InitializeSetup();

            this.saved = true;

            // Check if we have a filename, if not get the template
            if (file == null)
            {
                file = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "Template.expr");
                if (!File.Exists(file))
                {
                    // Final trap, no template file available, create static template
                    this.ResultType = TypeCode.Object;
                    this.ResultTypeValidate = true;
                    this.variables.Add("StringVar", false, "User", Convert.ChangeType("String Value", TypeCode.String, CultureInfo.CurrentCulture));
                    this.variables.Add("Int32Var", false, "User", Convert.ChangeType("0", TypeCode.Int32, CultureInfo.CurrentCulture));
                    this.variables.Add("DateTimeVar", false, "User", Convert.ChangeType(DateTime.Now, TypeCode.DateTime, CultureInfo.CurrentCulture));

                    // Reset filename as null
                    file = null;
                }
            }

            if (file != null)
            {
                this.LoadExpressionFile(file);
            }

            this.PopulateFunctionsTree();
            this.treeViewVariablesFunctions.Nodes[0].Expand();

            if (isTemplate)
            {
                this.fileName = null;
            }
            else
            {
                this.fileName = file;
                this.ignoreEvents = false;
            }

            this.OnTitleChanged(new TitleChangedEventArgs(Path.GetFileName(this.fileName)));

            this.undoStack.Clear();
            this.redoStack.Clear();
            this.OnUndoOrRedoCountChanged(new UndoOrRedoCountChangedEventArgs(0, 0));
        }

        /// <summary>
        /// Adds a new variable.
        /// </summary>
        public void AddVariable()
        {
            VariableEditor editor = new VariableEditor(this.variables);
            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    this.PopulateFunctionsTree();

                    this.treeViewVariablesFunctions.Nodes[0].Expand();

                    foreach (TreeNode node in this.treeViewVariablesFunctions.Nodes[0].Nodes)
                    {
                        if (node.Tag != null && node.Tag.Equals(String.Format(CultureInfo.CurrentCulture, VariablesDragTextFormat, editor.Variable.QualifiedName)))
                        {
                            this.treeViewVariablesFunctions.SelectedNode = node;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Microsoft.SqlServer.MessageBox.ExceptionMessageBox message = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(ex);
                    message.Caption = "Add Variable Failed";
                    message.Show(this);
                }

                this.InvalidateSaved();
            }
        }

        /// <summary>
        /// Delete a variable.
        /// </summary>
        public void DeleteVariable()
        {
            try
            {
                TreeNode node = this.treeViewVariablesFunctions.SelectedNode;

                if (node != null && node.Level == 1 && node.Parent.Name == VariablesNodeName)
                {
                    this.variables.Remove(node.Text);
                }

                this.PopulateFunctionsTree();
                this.treeViewVariablesFunctions.Nodes[0].Expand();
            }
            catch (Exception ex)
            {
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox message = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(ex);
                message.Caption = "Add Variable Failed";
                message.Show(this);
            }

            this.InvalidateSaved();
        }

        /// <summary>
        /// Edits a variable.
        /// </summary>
        public void EditVariable()
        {
            try
            {
                TreeNode variableNode = this.treeViewVariablesFunctions.SelectedNode;

                if (variableNode != null && variableNode.Level == 1 && variableNode.Parent.Name == VariablesNodeName)
                {
                    VariableEditor editor = new VariableEditor(this.variables, this.variables[variableNode.Name]);
                    if (editor.ShowDialog(this) == DialogResult.OK)
                    {
                        this.PopulateFunctionsTree();

                        this.treeViewVariablesFunctions.Nodes[0].Expand();

                        foreach (TreeNode node in this.treeViewVariablesFunctions.Nodes[0].Nodes)
                        {
                            if (node.Tag != null && node.Tag.Equals(String.Format(CultureInfo.CurrentCulture, VariablesDragTextFormat, editor.Variable.QualifiedName)))
                            {
                                this.treeViewVariablesFunctions.SelectedNode = node;
                            }
                        }

                        this.InvalidateSaved();
                    }
                }
            }
            catch (Exception ex)
            {
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox message = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(ex);
                message.Caption = "Edit Variable Failed";
                message.Show(this);
            }
        }

        /// <summary>
        /// Invalidates the saved.
        /// </summary>
        public void InvalidateSaved()
        {
            if (this.saved && !this.ignoreEvents)
            {
                this.saved = false;
                this.OnTitleChanged(new TitleChangedEventArgs(this.title));
            }
        }

        /// <summary>
        /// Save the expression to a file
        /// </summary>
        public void Save()
        {
            this.Save(this.fileName, false);
        }

        /// <summary>
        /// Saves the specified file.
        /// </summary>
        /// <param name="file">The file name.</param>
        /// <param name="isTemplate">if set to <c>true</c> the file is a template.</param>
        public void Save(string file, bool isTemplate)
        {
            XmlDocument hostDocument = new XmlDocument();
            this.host.SaveToXML(ref hostDocument, null, null);

            XmlDocument exprDocument = new XmlDocument();

            XmlElement root = exprDocument.CreateElement("ExpressionProject");
            exprDocument.AppendChild(root);

            XmlElement expressionXml = exprDocument.CreateElement("Expression");
            XmlCDataSection expressionData = exprDocument.CreateCDataSection(this.ExpressionTextBox.Text);
            expressionXml.AppendChild(expressionData);
            root.AppendChild(expressionXml);

            XmlElement resultTypeXml = exprDocument.CreateElement("ResultType");
            resultTypeXml.InnerText = Convert.ToInt32(this.ResultType, CultureInfo.CurrentCulture).ToString(CultureInfo.CurrentCulture);
            root.AppendChild(resultTypeXml);

            XmlElement resultTypeValidateXml = exprDocument.CreateElement("ResultTypeValidate");
            resultTypeValidateXml.InnerText = this.ResultTypeValidate.ToString(CultureInfo.CurrentCulture);
            root.AppendChild(resultTypeValidateXml);

            XmlElement hostXml = exprDocument.CreateElement("Host");
            XmlCDataSection hostData = exprDocument.CreateCDataSection(hostDocument.InnerXml);
            hostXml.AppendChild(hostData);
            root.AppendChild(hostXml);

            exprDocument.Save(file);

            if (isTemplate)
            {
                return;
            }

            this.saved = true;
            this.fileName = file;
            this.OnTitleChanged(new TitleChangedEventArgs(Path.GetFileName(file)));
        }

        /// <summary>
        /// Text cuts method.
        /// </summary>
        public void Cut()
        {
            RichTextBox textBox = this.activeTextControl as RichTextBox;
            if (textBox != null && !String.IsNullOrEmpty(textBox.SelectedText))
            {
                Clipboard.SetText(textBox.SelectedText);
                int start = textBox.SelectionStart;
                textBox.Text = textBox.Text.Remove(start, textBox.SelectionLength);
                textBox.SelectionStart = start;
            }
        }

        /// <summary>
        /// Text copy method.
        /// </summary>
        public void Copy()
        {
            TextBoxBase textBox = this.activeTextControl;
            if (textBox != null && !String.IsNullOrEmpty(textBox.SelectedText))
            {
                Clipboard.SetText(textBox.SelectedText);
            }
        }

        /// <summary>
        /// Text paste method.
        /// </summary>
        public void Paste()
        {
            RichTextBox textBox = this.activeTextControl as RichTextBox;
            if (textBox != null && Clipboard.ContainsText())
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                textBox.Text = textBox.Text.Remove(start, length).Insert(start, Clipboard.GetText());
                textBox.SelectionStart = start + Clipboard.GetText().Length;
            }
        }

        /// <summary>
        /// Text undo method.
        /// </summary>
        public void Undo()
        {
            RichTextBox control = this.activeTextControl as RichTextBox;
            if (control == null)
            {
                control = this.ExpressionTextBox;
            }

            if (control != null && this.undoStack.Count > 1)
            {
                this.redoStack.Push(this.undoStack.Pop());

                this.undoFlag = true;
                control.Text = this.undoStack.Peek().Text;
                control.SelectionStart = this.undoStack.Peek().SelectionStart;
                control.SelectionLength = this.undoStack.Peek().SelectionLength;
                this.undoFlag = false;

                this.OnUndoOrRedoCountChanged(new UndoOrRedoCountChangedEventArgs(this.undoStack.Count, this.redoStack.Count));
            }
        }

        /// <summary>
        /// Text redo method.
        /// </summary>
        public void Redo()
        {
            RichTextBox control = this.activeTextControl as RichTextBox;
            if (control != null && this.redoStack.Count > 0)
            {
                UndoText reddo = this.redoStack.Pop();
                this.undoStack.Push(reddo);
                this.undoFlag = true;
                control.Text = reddo.Text;
                control.SelectionStart = reddo.SelectionStart;
                control.SelectionLength = reddo.SelectionLength;
                this.undoFlag = false;

                this.OnUndoOrRedoCountChanged(new UndoOrRedoCountChangedEventArgs(this.undoStack.Count, this.redoStack.Count));
            }
        }

        /// <summary>
        /// Selects all text.
        /// </summary>
        public void SelectAllText()
        {
            TextBoxBase textBox = this.activeTextControl;
            if (textBox != null)
            {
                textBox.SelectionStart = 0;
                textBox.SelectionLength = textBox.Text.Length;
            }
        }

        /// <summary>
        /// Evaluate the expression.
        /// </summary>
        public void Run()
        {
            try
            {
                this.expressionEvaluator.Expression = this.ExpressionTextBox.Text;
                object result = this.expressionEvaluator.Validate(this.variableDispenser);

                Type propertyType = RuntimeHelper.GetTypeFromTypeCode(this.ResultType);

                if (propertyType.IsAssignableFrom(result.GetType()) || this.ResultTypeValidate == false)
                {
                    this.richTextResult.Text = result.ToString().Replace("\n", "\r\n");
                }
                else
                {
                    string message = String.Format(CultureInfo.CurrentCulture, "Cannot convert expression value type ({0}) to result type ({1}).", result.GetType().Name, propertyType.Name);
                    string additionalInfo = "The expression is valid, but value type conflicts with the result type. Try using the Cast operators to convert the value to match the result type. You can also change the result type from the Expression Properties menu item.";
                    ApplicationException exceptionMessage = new ApplicationException(String.Format(CultureInfo.CurrentCulture, "Cannot convert {0} to {1}.", result.GetType().FullName, propertyType.FullName));
                    Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(message, this.ApplicationTitle, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.OK, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Warning);
                    messageBox.InnerException = new ApplicationException(additionalInfo, exceptionMessage);
                    messageBox.Show(this);
                }
            }
            catch (ExpressionException ex)
            {
                string message = "Expression cannot be evaluated";
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(message, this.ApplicationTitle, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.OK, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Warning);
                messageBox.InnerException = new ApplicationException(ex.Message);
                messageBox.Show(this);
            }

            ////if (this.evaluatorResultTextBox.Text.Length > 4000)
            ////{
            //    string message = "Expressions cannot be greater than 4000 characters.";
            //    Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(message, ApplicationTitle, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.OK, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Warning);
            //    messageBox.Show(this);
            ////}
        }

        /// <summary>
        /// Find text method.
        /// </summary>
        public void Find()
        {
            this.FindOrReplace(true);
        }

        /// <summary>
        /// Replace text method.
        /// </summary>
        public void Replace()
        {
            this.FindOrReplace(false);
        }

        /// <summary>
        /// Deletes a function.
        /// </summary>
        public void DeleteFunction()
        {
            TreeNode nextNode = this.treeViewVariablesFunctions.SelectedNode.PrevNode;
            if (nextNode == null)
            {
                nextNode = this.treeViewVariablesFunctions.SelectedNode.Parent;
                if (nextNode == null && this.treeViewVariablesFunctions.Nodes.Count > 0)
                {
                    nextNode = this.treeViewVariablesFunctions.Nodes[0];
                }
            }

            this.treeViewVariablesFunctions.Nodes.Remove(this.treeViewVariablesFunctions.SelectedNode);
            this.treeViewVariablesFunctions.SelectedNode = nextNode;

            // Flag as changed
            this.ExpressionEdited();
        }

        /// <summary>
        /// Adds a new function group.
        /// </summary>
        public void NewFunctionGroup()
        {
            TreeNode newNode = new TreeNode("<New Group>");
            newNode.ImageIndex = 1;
            newNode.SelectedImageIndex = 1;

            TreeNode parentNode = this.treeViewVariablesFunctions.SelectedNode;
            if (parentNode == null)
            {
                this.treeViewVariablesFunctions.Nodes.Add(newNode);
            }
            else if (parentNode.ImageIndex == 0 || parentNode.ImageIndex == 1)
            {
                parentNode.Nodes.Add(newNode);
            }
            else
            {
                if (parentNode.Parent == null)
                {
                    this.treeViewVariablesFunctions.Nodes.Add(newNode);
                }
                else
                {
                    parentNode.Parent.Nodes.Add(newNode);
                }
            }

            this.treeViewVariablesFunctions.SelectedNode = newNode;

            // Flag as changed
            this.ExpressionEdited();
        }

        /// <summary>
        /// Adds a new function.
        /// </summary>
        public void NewFunction()
        {
            TreeNode newNode = new TreeNode("<New Function>");
            newNode.ImageIndex = 4;
            newNode.SelectedImageIndex = 4;

            TreeNode parentNode = this.treeViewVariablesFunctions.SelectedNode;
            if (parentNode == null)
            {
                this.treeViewVariablesFunctions.Nodes.Add(newNode);
            }
            else if (parentNode.ImageIndex == 0 || parentNode.ImageIndex == 1)
            {
                parentNode.Nodes.Add(newNode);
            }
            else
            {
                if (parentNode.Parent == null)
                {
                    this.treeViewVariablesFunctions.Nodes.Add(newNode);
                }
                else
                {
                    parentNode.Parent.Nodes.Add(newNode);
                }
            }

            this.treeViewVariablesFunctions.SelectedNode = newNode;

            // Flag as changed
            this.ExpressionEdited();
        }

        /// <summary>
        /// Called to flag the expression functions file as havng been edited, and therefore enable the save button if we can.
        /// </summary>
        internal void ExpressionEdited()
        {
            if (!String.IsNullOrEmpty(this.functionsFileName))
            {
                this.expressionEditorViewEditorPanel.SaveEnabled();
            }
        }

        /// <summary>
        /// Move the function node up.
        /// </summary>
        internal void NodeUp()
        {
            this.NodeMove(-1);
        }

        /// <summary>
        /// Move the function node down.
        /// </summary>
        internal void NodeDown()
        {
            this.NodeMove(+1);
        }

        /// <summary>
        /// Move the function node left.
        /// </summary>
        internal void NodeLeft()
        {
            TreeNode treeNode = this.treeViewVariablesFunctions.SelectedNode;

            if (treeNode == null)
            {
                return;
            }

            int index = treeNode.Index;

            // Get parent node so we can find index to set relative position of current node alongside parent
            TreeNode parent;
            if (treeNode.Parent == null)
            {
                return;
            }
            else
            {
                parent = treeNode.Parent;
            }

            // Need to get grandparent nodes collection to make node a sibling of current parent
            TreeNodeCollection grandParentNodes;

            // Check if parent by TreeView already, cannot move
            if (treeNode.Parent == null)
            {
                return;
            }
            else if (treeNode.Parent.Parent == null)
            {
                // If grandparent is null, then use TreeView
                grandParentNodes = treeNode.Parent.TreeView.Nodes;
            }
            else
            {
                // Use grandparent TreeNode
                grandParentNodes = treeNode.Parent.Parent.Nodes;
            }

            parent.Nodes.RemoveAt(index);
            grandParentNodes.Insert(parent.Index, treeNode);

            treeNode.TreeView.SelectedNode = treeNode;

            // Flag as changed
            this.ExpressionEdited();
        }

        /// <summary>
        /// Move the function node right.
        /// </summary>
        internal void NodeRight()
        {
            TreeNode treeNode = this.treeViewVariablesFunctions.SelectedNode;

            if (treeNode == null)
            {
                return;
            }

            TreeNodeCollection parentNodes;
            if (treeNode.Parent == null)
            {
                parentNodes = treeNode.TreeView.Nodes;
            }
            else
            {
                parentNodes = treeNode.Parent.Nodes;
            }

            int index = treeNode.Index;

            // Start from current position and work down
            int offsetIndex = index;
            for (int i = 0; i < parentNodes.Count; i++)
            {
                offsetIndex++;
                if (offsetIndex == parentNodes.Count)
                {
                    offsetIndex = 0;
                }

                TreeNode newParentNode = parentNodes[offsetIndex];
                if (newParentNode.ImageIndex <= 1 && newParentNode.Index != index)
                {
                    parentNodes.RemoveAt(index);
                    newParentNode.Nodes.Insert(-1, treeNode);
                    break;
                }
            }

            treeNode.TreeView.SelectedNode = treeNode;

            // Flag as changed
            this.ExpressionEdited();
        }

        /// <summary>
        /// Save the functions view XML.
        /// </summary>
        internal void SaveFunctions()
        {
            try
            {
                ExpressionEditorViewSerializer.Serialize(this.treeViewVariablesFunctions, this.functionsFileName);
                ExceptionMessageBox messageBox = new ExceptionMessageBox();
                messageBox.Caption = "Expression Editor";
                messageBox.Text = "Expression functions saved successfully.";
                messageBox.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Information;
                messageBox.Show(this);
            }
            catch (Exception ex)
            {
                ExceptionMessageBox.Show(this, ex);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:VariableSelectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.VariableSelectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnVariableSelectionChanged(VariableSelectionChangedEventArgs e)
        {
            if (this.VariableSelectionChanged != null)
            {
                this.VariableSelectionChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CursorPositionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.CursorPositionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCursorPositionChanged(CursorPositionChangedEventArgs e)
        {
            if (this.CursorPositionChanged != null)
            {
                this.CursorPositionChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:TitleChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.TitleChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTitleChanged(TitleChangedEventArgs e)
        {
            this.title = e.Title;

            if (this.TitleChanged != null)
            {
                this.TitleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ResultTypeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.ResultTypeChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnResultTypeChanged(ResultTypeChangedEventArgs e)
        {
            if (this.ResultTypeChanged != null)
            {
                this.ResultTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:UndoOrRedoCountChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.UndoOrRedoCountChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUndoOrRedoCountChanged(UndoOrRedoCountChangedEventArgs e)
        {
            if (this.UndoOrRedoCountChanged != null)
            {
                this.UndoOrRedoCountChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:EditModeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnEditModeChanged(EventArgs e)
        {
            if (this.EditModeChanged != null)
            {
                this.EditModeChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:FindNotFound"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.FindOrReplaceEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFindNotFound(FindOrReplaceEventArgs e)
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
        protected virtual void OnReplaceAllComplete(FindOrReplaceEventArgs e)
        {
            if (this.ReplaceAllComplete != null)
            {
                this.ReplaceAllComplete(this, e);
            }
        }

        /// <summary>
        /// Adds a variable node to the tree view.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="var">The variable.</param>
        private static void TreeViewAddVariable(TreeNode parent, Variable var)
        {
            TreeNode node;
            if (var.SystemVariable)
            {
                node = parent.Nodes.Add(var.QualifiedName, var.QualifiedName, 3, 3);
                node.Tag = String.Format(CultureInfo.CurrentCulture, VariablesDragTextFormat, var.QualifiedName);
            }
            else
            {
                node = parent.Nodes.Add(var.QualifiedName, var.QualifiedName, 2, 2);
                node.Tag = String.Format(CultureInfo.CurrentCulture, VariablesDragTextFormat, var.QualifiedName);
            }

            node.ToolTipText = String.Format(CultureInfo.CurrentCulture, "@[{0}], Data Type: {1}, Path: {2},  Value: {3}  {4}", var.QualifiedName, var.DataType == TypeCode.UInt16 ? TypeCode.Char.ToString() : var.DataType.ToString(), RuntimeHelper.GetVariableScope(var), RuntimeHelper.GetVariableStringValue(var), var.ReadOnly ? "[Read Only]" : string.Empty);
        }

        /// <summary>
        /// Gets the local variables only.
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <returns>An array of the local variables.</returns>
        private static Variable[] GetLocalVariables(Variables variables)
        {
            Dictionary<string, Variable> names = new Dictionary<string, Variable>();

            foreach (Variable var in variables)
            {
                // Filter variables to exclude patrent and above variables which
                // are obscured by local or lower level variables.
                if (names.ContainsKey(var.QualifiedName))
                {
                    if (var.GetPackagePath().Length > names[var.QualifiedName].GetPackagePath().Length)
                    {
                        names[var.QualifiedName] = var;
                    }
                }
                else
                {
                    names.Add(var.QualifiedName, var);
                }
            }

            Variable[] outputVariables = new Variable[names.Count];
            names.Values.CopyTo(outputVariables, 0);

            ////foreach (KeyValuePair<String, Variable>int index = 0; index < names.Count; index++)
            ////{
            //    outputVariables[index] = names[index];
            ////}
            return outputVariables;
        }

        /// <summary>
        /// Move the function node.
        /// </summary>
        /// <param name="indexOffset">The index offset.</param>
        private void NodeMove(int indexOffset)
        {
            TreeNode treeNode = this.treeViewVariablesFunctions.SelectedNode;

            if (treeNode == null)
            {
                return;
            }

            int index = treeNode.Index;

            TreeNodeCollection parentNodes;
            if (treeNode.Parent == null)
            {
                parentNodes = treeNode.TreeView.Nodes;
            }
            else
            {
                parentNodes = treeNode.Parent.Nodes;
            }

            parentNodes.RemoveAt(index);
            parentNodes.Insert(index + indexOffset, treeNode);

            treeNode.TreeView.SelectedNode = treeNode;

            // Flag as changed
            this.ExpressionEdited();
        }

        /// <summary>
        /// Initialize the control, common setup tasks.
        /// </summary>
        private void InitializeSetup()
        {
            this.ignoreEvents = true;

            this.expressionEvaluator = new ExpressionEvaluator();
            this.treeViewVariablesFunctions.ImageList = this.imageListIcons;
            this.toolTip.SetToolTip(this.linkEvaluate, "Evaluate expression");

            // Load functions document
            try
            {
                this.SetDefaultFunctionsDocument();
            }
            catch
            {
                this.functionsDocument = null;
            }
        }

        /// <summary>
        /// Loads the expression file.
        /// </summary>
        /// <param name="file">The file name.</param>
        private void LoadExpressionFile(string file)
        {
            XmlDocument document = new XmlDocument();
            document.Load(file);

            this.ExpressionTextBox.Text = document.SelectSingleNode("/ExpressionProject/Expression").FirstChild.Value;
            this.ResultType = (TypeCode)Enum.Parse(typeof(TypeCode), document.SelectSingleNode("/ExpressionProject/ResultType").InnerText);
            this.ResultTypeValidate = Convert.ToBoolean(document.SelectSingleNode("/ExpressionProject/ResultTypeValidate").InnerText, CultureInfo.CurrentCulture);

            XmlDocument hostDocument = new XmlDocument();
            hostDocument.LoadXml(document.SelectSingleNode("/ExpressionProject/Host").FirstChild.Value);
            this.host.LoadFromXML(hostDocument, null);
        }

        /// <summary>
        /// Sets the default functions document.
        /// </summary>
        private void SetDefaultFunctionsDocument()
        {
            XmlDocument document = new XmlDocument();

            // Check for executable path file
            this.functionsFileName = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), FunctionsDocumentFileName);
            if (File.Exists(this.functionsFileName))
            {
                try
                {
                    document.Load(this.functionsFileName);
                    this.functionsDocument = document;
                    return;
                }
                catch (XmlException ex)
                {
                    System.Diagnostics.Debug.Assert(false, ex.ToString());
                }
            }

            // Get Common Application Data Folder
            string directory = RuntimeHelper.ApplicationDataFolder(RuntimeHelper.CommonProductName);
            this.functionsFileName = Path.Combine(directory, FunctionsDocumentFileName);

            if (File.Exists(this.functionsFileName))
            {
                try
                {
                    document.Load(this.functionsFileName);
                    this.functionsDocument = document;
                    return;
                }
                catch (XmlException ex)
                {
                    ExceptionMessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "The expression function file is invalid ({0}), the default function list will be used.", this.functionsFileName), ex);
                }
            }

            // Final trap, use embedded resource, but the filename is still set to AppData
            document.LoadXml(Konesans.Dts.ExpressionEditor.Properties.Resources.ExpressionViewFunctions);
            this.functionsDocument = document;
        }

        /// <summary>
        /// Populates the functions tree view.
        /// </summary>
        private void PopulateFunctionsTree()
        {
            this.treeViewVariablesFunctions.Nodes.Clear();

            this.variablesNode = this.treeViewVariablesFunctions.Nodes.Add(VariablesNodeName, "Variables", 0, 0);
            this.variablesNode.Tag = VariablesNodeName;
            TreeNode systemVariablesNode = this.variablesNode.Nodes.Add(SystemVariablesNodeName, "System", 0, 0);

            if (this.variables != null)
            {
                Variable[] localVariables = GetLocalVariables(this.variables);

                foreach (Variable variable in localVariables)
                {
                    if (variable.SystemVariable)
                    {
                        TreeViewAddVariable(systemVariablesNode, variable);
                    }
                    else
                    {
                        TreeViewAddVariable(this.variablesNode, variable);
                    }
                }
            }

            ExpressionEditorViewSerializer.DeSerialize(this.treeViewVariablesFunctions, this.functionsDocument);
        }

        /// <summary>
        /// Handles the AfterExpand event of the treeViewVariablesFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
        private void TreeViewVariablesFunctions_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.IsExpanded && e.Node.ImageIndex == 0)
            {
                e.Node.ImageIndex = 1;
                e.Node.SelectedImageIndex = 1;
            }
        }

        /// <summary>
        /// Handles the AfterCollapse event of the treeViewVariablesFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
        private void TreeViewVariablesFunctions_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (!e.Node.IsExpanded && e.Node.ImageIndex == 1)
            {
                e.Node.ImageIndex = 0;
                e.Node.SelectedImageIndex = 0;
            }
        }

        /// <summary>
        /// Handles the AfterSelect event of the treeViewVariablesFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
        private void TreeViewVariablesFunctions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            this.labelHelpText.Text = node.ToolTipText;

            if (this.editMode)
            {
                this.expressionEditorViewEditorPanel.SetNode(node);
            }
            else
            {
                // node != null &&
                if (node.Level == 1 && node.Parent.Name == VariablesNodeName && node.Name != SystemVariablesNodeName)
                {
                    this.OnVariableSelectionChanged(new VariableSelectionChangedEventArgs(true));
                }
                else
                {
                    this.OnVariableSelectionChanged(new VariableSelectionChangedEventArgs(false));
                }
            }
        }

        ////private void treeViewVariablesFunctions_DragOver(object sender, DragEventArgs e)
        ////{
        //    //if (_editMode)
        //    //    e.Effect = DragDropEffects.Move;
        //    //else
        //        return;
        ////}

        ////private void treeViewVariablesFunctions_DragDrop(object sender, DragEventArgs e)
        ////{
        //    //if (_editMode)
        //    //{
        //    //    TreeNode targetNode = treeViewVariablesFunctions.GetNodeAt(e.X, e.Y);
        //    //    if (e.Data.GetDataPresent(typeof(TreeNode)))
        //    //    {
        //    //        if (e.Effect == DragDropEffects.Move)
        //    //        {
        //    //            TreeNode treeNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
        //    //            if (treeNode.ImageIndex == 0 || treeNode.ImageIndex == 1)
        //    //                targetNode.Nodes.Add(treeNode);
        //    //            else
        //    //                targetNode.Parent.Nodes.Add(treeNode);
        //    //        }
        //    //    }
        //    //}
        //    //else
        //        return;
        ////}

        /// <summary>
        /// Handles the ItemDrag event of the treeViewVariablesFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ItemDragEventArgs"/> instance containing the event data.</param>
        private void TreeViewVariablesFunctions_ItemDrag(object sender, ItemDragEventArgs e)
        {
                TreeNode node = e.Item as TreeNode;
                if (node == null)
                {
                    return;
                }

                string dragText = node.Tag as string;
                if (string.IsNullOrEmpty(dragText))
                {
                    return;
                }

                DoDragDrop(dragText, DragDropEffects.All);
        }

        /// <summary>
        /// Handles the NodeMouseDoubleClick event of the treeViewVariablesFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeNodeMouseClickEventArgs"/> instance containing the event data.</param>
        private void TreeViewVariablesFunctions_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (this.editMode)
            {
                return;
            }

            if (e.Button == MouseButtons.Left && e.Node != null && e.Node.Tag != null)
            {
                this.ExpressionTextBox.SelectedText = e.Node.Tag.ToString();
            }
        }

        /// <summary>
        /// Handles the NodeMouseClick event of the treeViewVariablesFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeNodeMouseClickEventArgs"/> instance containing the event data.</param>
        private void TreeViewVariablesFunctions_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (this.editMode)
            {
                return;
            }
            else
            {
                // Force node selection for reliable context menu behaviour, on any click.
                this.treeViewVariablesFunctions.SelectedNode = e.Node;
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the richTxtExpression control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void RichTxtExpression_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ExpandSelectedText(LeftParameterBoundary1, RightParameterBoundary1, "_");

                this.ExpandSelectedText("@[", "]", ":", "::", "[", "_");
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the richTxtExpression control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RichTxtExpression_TextChanged(object sender, EventArgs e)
        {
            if (!this.ignoreEvents)
            {
                this.InvalidateSaved();
            }

            if (!this.undoFlag)
            {
                UndoText undo = new UndoText();
                undo.Text = this.ExpressionTextBox.Text;
                undo.SelectionStart = this.ExpressionTextBox.SelectionStart;
                undo.SelectionLength = this.ExpressionTextBox.SelectionLength;

                if (this.undoStack.Count > 0 && undo.IsDuplicate(this.undoStack.Peek()))
                {
                    return;
                }

                this.undoStack.Push(undo);
                this.redoStack.Clear();
                this.OnUndoOrRedoCountChanged(new UndoOrRedoCountChangedEventArgs(this.undoStack.Count, this.redoStack.Count));
            }
        }

        /// <summary>
        /// Handles the Click event of the linkEvaluate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LinkEvaluate_Click(object sender, EventArgs e)
        {
            this.Run();
        }

        /// <summary>
        /// Handles the Opening event of the contextMenuStripVariablesFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void ContextMenuStripVariablesFunctions_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            ContextMenuStrip menu = sender as ContextMenuStrip;
            if (menu == null)
            {
                return;
            }

            TreeView treeView = menu.SourceControl as TreeView;
            if (treeView == null)
            {
                return;
            }

            // Set the edit functions menu item checked state to the current EditMode value
            ToolStripMenuItem item = menu.Items[4] as ToolStripMenuItem;
            if (item == null)
            {
                return;
            }

            item.Checked = this.EditMode;

            TreeNode node = treeView.SelectedNode;
            if (node == null)
            {
                return;
            }

            // Check if we have selected an existing variable or not
            if (!this.EditMode && node.Level == 1)
            {
                if (node.Parent.Name == VariablesNodeName && node.Name != SystemVariablesNodeName)
                {
                    // Allow add, edit and delete
                    e.Cancel = false;
                    menu.Items[0].Enabled = true;
                    menu.Items[1].Enabled = true;
                    menu.Items[2].Enabled = true;
                }
            }
            else
            {
                // Just allow adding or variables, except in edit mode
                e.Cancel = false;
                menu.Items[0].Enabled = !this.EditMode;
                menu.Items[1].Enabled = false;
                menu.Items[2].Enabled = false;
            }
        }

        /// <summary>
        /// Handles the ItemClicked event of the contextMenuStripVariablesFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ToolStripItemClickedEventArgs"/> instance containing the event data.</param>
        private void ContextMenuStripVariablesFunctions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip menu = sender as ContextMenuStrip;
            if (menu == null)
            {
                return;
            }

            if (menu.Items[0].Equals(e.ClickedItem))
            {
                this.AddVariable();
            }
            else if (menu.Items[1].Equals(e.ClickedItem))
            {
                this.EditVariable();
            }
            else if (menu.Items[2].Equals(e.ClickedItem))
            {
                this.DeleteVariable();
            }
            else if (menu.Items[4].Equals(e.ClickedItem))
            {
                ToolStripMenuItem item = e.ClickedItem as ToolStripMenuItem;
                if (item == null)
                {
                    return;
                }

                this.EditMode = !item.Checked;
            }
        }

        /// <summary>
        /// Handles the Load event of the ExpressionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExpressionEditor_Load(object sender, EventArgs e)
        {
            ////try
            ////{
            ////    ////this.splitContainerExpressionResult.SplitterDistance
            ////    ////this.Size = Properties.Settings.Default.ExpressionEditorSize;
            ////    ////this.Location = Properties.Settings.Default.ExpressionEditorLocation;
            ////    ////this.WindowState = Properties.Settings.Default.ExpressionEditorState;

            ////    ////this.verticalSplitter.Location = Properties.Settings.Default.ExpressionEditorSplitterV;
            ////    ////this.splitterExpressionEvaluator.Location = Properties.Settings.Default.ExpressionEditorSplitterH1;
            ////    ////this.splitterWorkingDescription.Location = Properties.Settings.Default.ExpressionEditorSplitterH2;
            ////}
            ////catch
            ////{
            ////}
        }
        
        /// <summary>
        /// Handles the Enter event of the TextControls control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TextControls_Enter(object sender, EventArgs e)
        {
            this.activeTextControl = sender as TextBoxBase;
        }

        /// <summary>
        /// Handles the Leave event of the TextControls control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TextControls_Leave(object sender, EventArgs e)
        {
            this.activeTextControl = null;
        }

        /// <summary>
        /// Expands the selected text.
        /// </summary>
        /// <param name="leftBoundary">The left boundary.</param>
        /// <param name="rightBoundary">The right boundary.</param>
        /// <param name="intermediate">The intermediate.</param>
        private void ExpandSelectedText(string leftBoundary, string rightBoundary, params string[] intermediate)
        {
            StringCollection intermediates = new StringCollection();
            intermediates.AddRange(intermediate);

            string text = this.ExpressionTextBox.Text;

            ////string selectedText = this.ExpressionTextBox.SelectedText;
            int start = this.ExpressionTextBox.SelectionStart;
            int end = start + this.ExpressionTextBox.SelectionLength;
            int leftBoundaryLength = leftBoundary.Length;
            int rightBoundaryLength = rightBoundary.Length;

            if (end > start && start > 0 && this.ExpressionTextBox.Text.Length > end)
            {
                if (text.Substring(start - leftBoundaryLength, leftBoundaryLength) == leftBoundary && text.Substring(end, rightBoundaryLength) == rightBoundary)
                {
                    this.ExpressionTextBox.SelectionStart = start - leftBoundaryLength;
                    this.ExpressionTextBox.SelectionLength = 1 + (end - this.ExpressionTextBox.SelectionStart);
                }
                else if (text.Substring(end, 1) == rightBoundary && intermediates.Contains(text.Substring(start - leftBoundaryLength, leftBoundaryLength)))
                {
                    for (int i = start; i > 0; i--)
                    {
                        string x = text.Substring(i, leftBoundaryLength);
                        if (x == leftBoundary)
                        {
                            this.ExpressionTextBox.SelectionStart = i;
                            this.ExpressionTextBox.SelectionLength = rightBoundaryLength + (end - this.ExpressionTextBox.SelectionStart);
                            break;
                        }
                        else if (x == " " || x == "(" || x == "[")
                        {
                            break;
                        }
                    }

                    // Underscore on left boundary
                    ////if (this.richTxtExpression.Text.i
                }
                else if (text.Substring(start - leftBoundaryLength, leftBoundaryLength) == leftBoundary && intermediates.Contains(text.Substring(end, rightBoundaryLength)))
                {
                    int len = text.Length;
                    for (int i = end; i < len; i++)
                    {
                        string x = text.Substring(i, rightBoundaryLength);
                        if (x == rightBoundary)
                        {
                            this.ExpressionTextBox.SelectionStart = start - leftBoundaryLength;
                            this.ExpressionTextBox.SelectionLength = rightBoundaryLength + (i - this.ExpressionTextBox.SelectionStart);
                            break;
                        }
                        else if (x == " " || x == "(" || x == "[")
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Common support method for the Find or Replace event handler methods.
        /// </summary>
        /// <param name="find">Boolean that idicates if this is find (true) or replace (false).</param>
        private void FindOrReplace(bool find)
        {
            this.findReplace.SetFind(find);

            if (this.findReplace.Visible)
            {
                this.findReplace.Activate();
            }
            else
            {
                this.findReplace.Show(this);
            }
        }

        /// <summary>
        /// Handles the FindNotFound event of the FindReplaceInternal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.FindOrReplaceEventArgs"/> instance containing the event data.</param>
        private void FindReplaceInternal_FindNotFound(object sender, FindOrReplaceEventArgs e)
        {
            this.OnFindNotFound(e);
        }

        /// <summary>
        /// Handles the ReplaceAllComplete event of the FindReplaceInternal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.FindOrReplaceEventArgs"/> instance containing the event data.</param>
        private void FindReplaceInternal_ReplaceAllComplete(object sender, FindOrReplaceEventArgs e)
        {
            this.OnReplaceAllComplete(e);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ExpressionTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExpressionTextBox_SelectionChanged(object sender, EventArgs e)
        {
            TextBoxBase textBox = sender as TextBoxBase;
            if (textBox == null)
            {
                return;
            }

            int column = textBox.SelectionStart - textBox.GetFirstCharIndexOfCurrentLine();
            int line = textBox.GetLineFromCharIndex(textBox.SelectionStart);
            this.OnCursorPositionChanged(new CursorPositionChangedEventArgs(line, column));
        }

        /// <summary>
        /// Handles the DragEnter event of the ExpressionTextBox control.
        /// </summary>
        /// <remarks>The EnableAutoDragDrop property is true, but we use this event to exclude inappropriate data formats.</remarks>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        private void ExpressionTextBox_DragEnter(object sender, DragEventArgs e)
        {
            // Check we have string data
            if (e.Data.GetDataPresent(typeof(string)))
            {
                if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }

            // TODO:  Need to add file load support if we want to include this, default action for Auto mode is to embed it.
            //////else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //////{
            //////    string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            //////    if (files.Length > 0)
            //////    {
            //////        string file = files[0];
            //////        if (Path.GetExtension(file) == ".expr")
            //////        {
            //////            e.Effect = DragDropEffects.Copy;
            //////            return;
            //////        }
            //////    }
            //////}

            e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Handles the Opening event of the contextMenuStripTextPanes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void ContextMenuStripTextPanes_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menu = sender as ContextMenuStrip;
            if (menu == null)
            {
                return;
            }

            TextBoxBase textBox = menu.SourceControl as TextBoxBase;
            if (textBox == null)
            {
                return;
            }

            if (!textBox.Equals(this.activeTextControl))
            {
                if (textBox.SelectionStart == 0 && textBox.SelectionLength == 0)
                {
                    textBox.SelectAll();
                }

                textBox.Focus();
            }

            bool textIsExpression = this.ExpressionTextBox.Equals(menu.SourceControl);
            this.undoToolStripMenuItem.Visible = textIsExpression;
            this.redoToolStripMenuItem.Visible = textIsExpression;
            this.toolStripSeparatorUndo.Visible = textIsExpression;

            this.selectAllToolStripMenuItem.Enabled = textBox.SelectionLength == textBox.Text.Length;

            bool textIsSelected = textBox.SelectionLength > 0;

            this.cutToolStripMenuItem.Enabled = textIsSelected;
            this.copyToolStripMenuItem.Enabled = textIsSelected;

            this.pasteToolStripMenuItem.Enabled = Clipboard.ContainsText();

            this.undoToolStripMenuItem.Enabled = this.undoStack.Count > 0;
            this.redoToolStripMenuItem.Enabled = this.redoStack.Count > 0;
        }

        /// <summary>
        /// Handles the Click event of the undoToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Undo();
        }

        /// <summary>
        /// Handles the Click event of the redoToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Redo();
        }

        /// <summary>
        /// Handles the Click event of the cutToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cut();
        }

        /// <summary>
        /// Handles the Click event of the copyToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Copy();
        }

        /// <summary>
        /// Handles the Click event of the pasteToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Paste();
        }

        /// <summary>
        /// Handles the Click event of the selectAllToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectAllText();
        }

        /// <summary>
        /// Undo operation class.
        /// </summary>
        private class UndoText
        {
            /// <summary>
            /// The text changed during the operation
            /// </summary>
            private string text;

            /// <summary>
            /// The start position of the text
            /// </summary>
            private int selectionStart;

            /// <summary>
            /// The length of the text
            /// </summary>
            private int selectionLength;

            /// <summary>
            /// Gets or sets the text changed during the operation
            /// </summary>
            /// <value>The operation text.</value>
            public string Text
            {
                get { return this.text; }
                set { this.text = value; }
            }

            /// <summary>
            /// Gets or sets the start position of the text.
            /// </summary>
            /// <value>The selection start.</value>
            public int SelectionStart
            {
                get { return this.selectionStart; }
                set { this.selectionStart = value; }
            }

            /// <summary>
            /// Gets or sets the length of the text.
            /// </summary>
            /// <value>The length of the selection.</value>
            public int SelectionLength
            {
                get { return this.selectionLength; }
                set { this.selectionLength = value; }
            }

            /// <summary>
            /// Determines whether the undo text is a duplicate.
            /// </summary>
            /// <param name="undoText">The undo text.</param>
            /// <returns>
            ///     <c>true</c> if the specified undo text is duplicate; otherwise, <c>false</c>.
            /// </returns>
            public bool IsDuplicate(UndoText undoText)
            {
                if (undoText.SelectionLength == this.SelectionLength && undoText.SelectionStart == this.SelectionStart && undoText.Text == this.Text)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}