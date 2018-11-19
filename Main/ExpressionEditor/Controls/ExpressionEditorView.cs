//-------------------------------------------------------------------------------------------------
// <copyright file="ExpressionEditorView.cs" company="Konesans Limited">
// Copyright (C) 2018 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Collections.Generic;
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
        /// Private member for result colour, preserve and restore colour following errors.
        /// </summary>
        private Color resultColour = SystemColors.WindowText;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEditorView"/> class.
        /// </summary>
        public ExpressionEditorView()
        {
            this.InitializeComponent();

            this.expressionEvaluator = new ExpressionEvaluator();

            this.findReplace = new FindReplace(this.ExpressionTextBox);
            this.findReplace.FindNotFound += this.FindReplaceInternalFindNotFound;
            this.findReplace.ReplaceAllComplete += this.FindReplaceInternalReplaceAllComplete;

            this.expressionEditorViewEditorPanel.ExpressionEditorView = this;

            this.imageListIcons.Images.Add(Resources.Folder);
            this.imageListIcons.Images.Add(Resources.FolderOpen);
            this.imageListIcons.Images.Add(Resources.Variable);
            this.imageListIcons.Images.Add(Resources.SystemVariable);
            this.imageListIcons.Images.Add(Resources.Expression);
#if (!YUKON  && !KATAMAI)
            this.imageListIcons.Images.Add(Resources.Parameter);
#endif

            this.ExpressionTextBox.EnableAutoDragDrop = true;
            this.ExpressionTextBox.AllowDrop = true;
            this.ExpressionTextBox.DragEnter += this.ExpressionTextBox_DragEnter;
        }

        /// <summary>
        /// Occurs when cursor position changes.
        /// </summary>
        public event EventHandler<CursorPositionChangedEventArgs> CursorPositionChanged;

        /// <summary>
        /// Occurs when the variable selection has changed.
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
        /// Gets the tool strip.
        /// </summary>
        /// <value>The tool strip.</value>
        public ToolStrip ToolStrip
        {
            get { return this.toolStrip; }
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
            get
            {
                return resultColour;
            }

            set
            {
                resultColour = value;
                this.richTextResult.ForeColor = value;
            }
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

                this.toolStripButtonFind.Enabled = !value;
                this.toolStripButtonReplace.Enabled = !value;

                this.editFunctionsMenuItem.Checked = value;
                this.editFunctionsToolStripButton.Checked = value;

                this.addVariableMenuItem.Enabled = !value;
                this.addVariableToolStripButton.Enabled = !value;
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
            using (VariableEditor editor = new VariableEditor(this.variables))
            {
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
                        ExceptionMessageBox.Show(this, ex, "Add Variable Failed");
                    }

                    this.InvalidateSaved();
                }
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
                ExceptionMessageBox.Show(this, ex, "Add Variable Failed");
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
                            if (node.Tag != null && node.Tag.Equals(string.Format(CultureInfo.CurrentCulture, VariablesDragTextFormat, editor.Variable.QualifiedName)))
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
                ExceptionMessageBox.Show(this, ex, "Edit Variable Failed");
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
            RuntimeHelper.SaveExpression(file, this.host, this.ExpressionTextBox.Text, this.ResultType, this.ResultTypeValidate);

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
            if (textBox != null && !string.IsNullOrEmpty(textBox.SelectedText))
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
            // Start by clearing any existing result
            this.richTextResult.ForeColor = resultColour;
            this.richTextResult.Text = string.Empty;

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
                    this.richTextResult.ForeColor = Color.Red;
                    this.richTextResult.Text = string.Format(CultureInfo.CurrentCulture, "Cannot convert expression value type ({0}) to result type ({1}).\r\nThe expression is valid, but value type conflicts with the result type. Try using the Cast operators to convert the value to match the result type. You can also change the result type from the Expression Properties menu item.", result.GetType().Name, propertyType.Name);
                }
            }
            catch (ExpressionException ex)
            {
                this.richTextResult.ForeColor = Color.Red;
                this.richTextResult.Text = "Expression cannot be evaluated\r\n" + ex.Message;                
            }
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
            if (!string.IsNullOrEmpty(this.functionsFileName))
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
            
            if (treeNode.Parent.Parent == null)
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
                MessageBox.Show(this, "Expression functions saved successfully.", "Expression Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ExceptionMessageBox.Show(this, ex);
            }
        }

        /// <summary>
        /// Raises the <see cref="VariableSelectionChanged"></see> event.
        /// </summary>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.VariableSelectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnVariableSelectionChanged(VariableSelectionChangedEventArgs e)
        {
            // Update UI
            this.editVariableMenuItem.Enabled = e.VariableSelected;
            this.editVariableToolStripButton.Enabled = e.VariableSelected;
            this.deleteVariableMenuItem.Enabled = e.VariableSelected;
            this.deleteVariableToolStripButton.Enabled = e.VariableSelected;

            if (this.VariableSelectionChanged != null)
            {
                this.VariableSelectionChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="CursorPositionChanged"/> event.
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
        /// Raises the <see cref="TitleChanged"/> event.
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
        /// Raises the <see cref="ResultTypeChanged"/> event.
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
        /// Raises the <see cref="UndoOrRedoCountChanged"/> event.
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
        /// Raises the <see cref="EditModeChanged"/> event.
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
        /// Raises the <see cref="FindNotFound"/> event.
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
        /// Raises the <see cref="ReplaceAllComplete"/> event.
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
                node.Tag = string.Format(CultureInfo.CurrentCulture, VariablesDragTextFormat, var.QualifiedName);
            }
            else
            {
                // Check for Parameter ($Package::ParamaterName or $Project::ParamaterName), now available in SQL 2012
                if (var.Namespace.StartsWith("$"))
                {
                    node = parent.Nodes.Add(var.QualifiedName, var.QualifiedName, 5, 5);
                }
                else
                {
                    node = parent.Nodes.Add(var.QualifiedName, var.QualifiedName, 2, 2);
                }

                node.Tag = string.Format(CultureInfo.CurrentCulture, VariablesDragTextFormat, var.QualifiedName);
            }

            node.ToolTipText = string.Format(CultureInfo.CurrentCulture, "@[{0}], Data Type: {1}, Path: {2},  Value: {3}  {4}", var.QualifiedName, var.DataType == TypeCode.UInt16 ? TypeCode.Char.ToString() : var.DataType.ToString(), RuntimeHelper.GetVariableScope(var), RuntimeHelper.GetVariableStringValue(var), var.ReadOnly ? "[Read Only]" : string.Empty);
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
                // Filter variables to exclude parent and above variables which
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
                    System.Diagnostics.Debug.WriteLine(string.Format("Variable: {0}", var.Name));
                    System.Diagnostics.Debug.WriteLine(var.QualifiedName);
                    System.Diagnostics.Debug.WriteLine(var.Namespace);
                    System.Diagnostics.Debug.WriteLine(var.Site);
                    System.Diagnostics.Debug.WriteLine(var.CreationName);
                    System.Diagnostics.Debug.WriteLine(".");
                }
            }

            Variable[] outputVariables = new Variable[names.Count];
            names.Values.CopyTo(outputVariables, 0);
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

            // Load functions document
            this.SetDefaultFunctionsDocument();
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

            string tempFileName = System.IO.Path.GetTempFileName();
            File.WriteAllText(tempFileName, document.SelectSingleNode("/ExpressionProject/Host").FirstChild.Value);

            // SSDT 17 install doesn't include v14 of Microsoft.SqlServer.Msxml6_interop.dll, so we cannot use DtsContainer.LoadFromXML method anymore, due to the missing dependency
            // Changed to use Application.LoadPackage instead
            Microsoft.SqlServer.Dts.Runtime.Application application = new Microsoft.SqlServer.Dts.Runtime.Application();
            this.host = application.LoadPackage(tempFileName, null);
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
                    System.Diagnostics.Debug.WriteLine("Loading functions from exe path: " + this.functionsFileName);
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
            // This is hardcoded here to ensure we always use the same folder regardless if it is hosted 
            // by a custom tasks, the editor tool or BIDS Helper. BIDSHelper uses ILMerge which confuses means 
            // we loose the normal assembly attributes hence we code a consistent path
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            directory = Path.Combine(directory, @"Konesans Limited\Expression Editor");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            this.functionsFileName = Path.Combine(directory, FunctionsDocumentFileName);
            System.Diagnostics.Debug.WriteLine("4");
            if (File.Exists(this.functionsFileName))
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Loading functions from AppData path: " + this.functionsFileName);
                    document.Load(this.functionsFileName);
                    this.functionsDocument = document;
                    return;
                }
                catch (XmlException ex)
                {
                    ExceptionMessageBox.Show(this, string.Format(CultureInfo.CurrentCulture, "The expression function file is invalid ({0}), the default function list will be used.", this.functionsFileName), ex);
                }
            }

            // Final trap, use embedded resource, but the filename is still set to AppData
            System.Diagnostics.Debug.WriteLine("Loading functions from resource string");
            document.LoadXml(Resources.ExpressionViewFunctions);
            this.functionsDocument = document;
        }

        /// <summary>
        /// Populates the functions tree view.
        /// </summary>
        private void PopulateFunctionsTree()
        {
            this.treeViewVariablesFunctions.Nodes.Clear();

            this.variablesNode = this.treeViewVariablesFunctions.Nodes.Add(VariablesNodeName, Resources.VariablesNodeText, 0, 0);
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
        private void TreeViewVariablesFunctionsAfterExpand(object sender, TreeViewEventArgs e)
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
        private void TreeViewVariablesFunctionsAfterCollapse(object sender, TreeViewEventArgs e)
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
        private void TreeViewVariablesFunctionsAfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            this.labelHelpText.Text = node.ToolTipText;

            if (this.editMode)
            {
                this.expressionEditorViewEditorPanel.SetNode(node);
            }
            else
            {
                // Detect type of node, and enable.disable variable editing
                // Disable for anything of the wrong level, patent node must be of the correct name, not the system variable folder, and also must not be a parameter
                // Maybe look to use the image key or similar going forward, detect variable vs parameters
                if (node.Level == 1 && node.Parent.Name == VariablesNodeName && node.Name != SystemVariablesNodeName && !node.Name.StartsWith("$"))
                {
                    this.OnVariableSelectionChanged(new VariableSelectionChangedEventArgs(true));
                }
                else
                {
                    this.OnVariableSelectionChanged(new VariableSelectionChangedEventArgs(false));
                }
            }
        }

        /// <summary>
        /// Handles the ItemDrag event of the treeViewVariablesFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ItemDragEventArgs"/> instance containing the event data.</param>
        private void TreeViewVariablesFunctionsItemDrag(object sender, ItemDragEventArgs e)
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

            this.DoDragDrop(dragText, DragDropEffects.All);
        }

        /// <summary>
        /// Handles the NodeMouseDoubleClick event of the treeViewVariablesFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeNodeMouseClickEventArgs"/> instance containing the event data.</param>
        private void TreeViewVariablesFunctionsNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
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
        private void TreeViewVariablesFunctionsNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
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
        /// Handles the TextChanged event of the richTxtExpression control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RichTxtExpressionTextChanged(object sender, EventArgs e)
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
        private void LinkEvaluateClick(object sender, EventArgs e)
        {
            this.Run();
        }

        /// <summary>
        /// Handles the Load event of the ExpressionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExpressionEditorLoad(object sender, EventArgs e)
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
        private void TextControlsEnter(object sender, EventArgs e)
        {
            this.activeTextControl = sender as TextBoxBase;
        }

        /// <summary>
        /// Handles the Leave event of the TextControls control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TextControlsLeave(object sender, EventArgs e)
        {
            this.activeTextControl = null;
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
        private void FindReplaceInternalFindNotFound(object sender, FindOrReplaceEventArgs e)
        {
            this.OnFindNotFound(e);
        }

        /// <summary>
        /// Handles the ReplaceAllComplete event of the FindReplaceInternal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.FindOrReplaceEventArgs"/> instance containing the event data.</param>
        private void FindReplaceInternalReplaceAllComplete(object sender, FindOrReplaceEventArgs e)
        {
            this.OnReplaceAllComplete(e);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ExpressionTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExpressionTextBoxSelectionChanged(object sender, EventArgs e)
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

            e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Handles the Opening event of the contextMenuStripTextPanes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void ContextMenuStripTextPanesOpening(object sender, CancelEventArgs e)
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
            this.undoMenuItem.Visible = textIsExpression;
            this.redoMenuItem.Visible = textIsExpression;
            this.toolStripSeparatorUndo.Visible = textIsExpression;

            this.selectAllMenuItem.Enabled = textBox.SelectionLength == textBox.Text.Length;

            bool textIsSelected = textBox.SelectionLength > 0;

            this.cutMenuItem.Enabled = textIsSelected;
            this.copyMenuItem.Enabled = textIsSelected;

            this.pasteMenuItem.Enabled = Clipboard.ContainsText();

            this.undoMenuItem.Enabled = this.undoStack.Count > 0;
            this.redoMenuItem.Enabled = this.redoStack.Count > 0;
        }

        /// <summary>
        /// Handles the Click event of the undoToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UndoToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Undo();
        }

        /// <summary>
        /// Handles the Click event of the redoToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RedoToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Redo();
        }

        /// <summary>
        /// Handles the Click event of the cutToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CutToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Cut();
        }

        /// <summary>
        /// Handles the Click event of the copyToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Copy();
        }

        /// <summary>
        /// Handles the Click event of the pasteToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PasteToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Paste();
        }

        /// <summary>
        /// Handles the Click event of the selectAllToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.SelectAllText();
        }

        /// <summary>
        /// Handles the KeyDown event of the ExpressionTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void ExpressionTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            // Capture key presses for the rich text box that we wish to handle 
            // ourselves or just supress because we do not want the functionality
            if (e.Control)
            {
                if (e.KeyCode == Keys.E)
                {
                    // Evaluate
                    this.Run();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.R || e.KeyCode == Keys.L || e.KeyCode == Keys.L)
                {
                    // Supress
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Handles the Click event for one of the Add variable controls
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AddVariableClick(object sender, EventArgs e)
        {
            this.AddVariable();
        }

        /// <summary>
        /// Handles the Click event for one of the Edit variable controls
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EditVariableClick(object sender, EventArgs e)
        {
            this.EditVariable();
        }

        /// <summary>
        /// Handles the Click event for one of the Delete variable controls
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DeleteVariableClick(object sender, EventArgs e)
        {
            this.DeleteVariable();
        }

        /// <summary>
        /// Handles the Click event for one of the Edit Functions controls.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EditFunctionsClick(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            if (menu != null)
            {
                this.EditMode = menu.Checked; 
                return;
            }

            ToolStripButton button = sender as ToolStripButton;
            if (button != null)
            {
                this.EditMode = button.Checked;
            }
        }

        /// <summary>
        /// Handles the Find event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void FindClick(object sender, EventArgs e)
        {
            this.Find();
        }

        /// <summary>
        /// Handles the Replace event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ReplaceClick(object sender, EventArgs e)
        {
            this.Replace();
        }

        /// <summary>
        /// Handles the ItemRemoved event of the toolStrip control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ToolStripItemEventArgs"/> instance containing the event data.</param>
        private void ToolStripItemRemoved(object sender, ToolStripItemEventArgs e)
        {
            // Hides the toolstrip if nothing in it
            // Used when toolstrip is merged into parent for example
            if (this.toolStrip.Items.Count == 0)
            {
                this.toolStrip.Visible = false;
            }
        }
    }
}
