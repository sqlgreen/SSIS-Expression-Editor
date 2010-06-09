//-------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.Tools.ExpressionTester
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using Konesans.Dts.ExpressionEditor;
    using Konesans.Dts.ExpressionEditor.Controls;
    using Konesans.Dts.Tools.Common;
    using Konesans.Dts.Tools.ExpressionTester.Properties;
    using Microsoft.SqlServer.MessageBox;

    /// <summary>
    /// Main form class.
    /// </summary>
    internal partial class Main : Form
    {
        /// <summary>
        /// Default template file name
        /// </summary>
        internal const string TemplateFilename = "Template.expr";

        /// <summary>
        /// Open/Save dialog filter for expression files.
        /// </summary>
        internal const string ExpressionFileFilter = "Expression File (*.expr)|*.expr|All Files (*.*)|*.*";

        /// <summary>
        /// Line status format string
        /// </summary>
        private const string LineStatusFormat = "Ln {0}";

        /// <summary>
        /// Column status format string
        /// </summary>
        private const string ColumnStatusFormat = "Col {0}";

        /// <summary>
        /// Settings manager to manager settings objects
        /// </summary>
        private SettingsManager settingsManager;

        /// <summary>
        /// Program settings object
        /// </summary>
        private ProgramSettings programSettings;

        /// <summary>
        /// Application settings object
        /// </summary>
        private ApplicationSettings applicationSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            this.InitializeComponent();
            this.settingsManager = new SettingsManager();
            this.programSettings = new ProgramSettings();
            this.applicationSettings = new ApplicationSettings();
            this.expressionEditor.ApplicationTitle = Application.ProductName;
        }

        /// <summary>
        /// Gets the template filename.
        /// </summary>
        /// <param name="validate">if set to <c>true</c> check that the file exists.</param>
        /// <returns>The template filename.</returns>
        private static string GetTemplateFilename(bool validate)
        {
            string filename = RuntimeHelper.ApplicationDataFolder(RuntimeHelper.CommonProductName);
            if (Directory.Exists(filename))
            {
                filename = Path.Combine(filename, TemplateFilename);
                if (validate && !File.Exists(filename))
                {
                    filename = null;
                }
            }

            return filename;
        }

        /// <summary>
        /// Handles the OpenRecent file event from the menu.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OpenRecentFile(object sender, EventArgs e)
        {
            ToolStripDropDownItem item = sender as ToolStripDropDownItem;
            if (item == null || item.Tag == null)
            {
                return;
            }

            string filename = item.Tag.ToString();

            if (File.Exists(filename))
            {
                this.InitializeExpressionControl(filename);
            }
            else
            {
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(String.Format(CultureInfo.CurrentCulture, "The file '{0}' cannot be found. Do you wish to remove it from the Recent Files list?", filename), Application.ProductName, ExceptionMessageBoxButtons.YesNo, ExceptionMessageBoxSymbol.Warning);
                if (messageBox.Show(this) == DialogResult.Yes)
                {
                    this.applicationSettings.RecentFiles.RemoveRecentFile(filename);
                }
            }
        }

        /// <summary>
        /// Create a new "expression" from the default template.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void New(object sender, EventArgs e)
        {
            // Check if expression should be saved or close canceled?
            if (sender != null && this.CloseExpressionEditor())
            {
                return;
            }

            string filename = GetTemplateFilename(true);
            this.expressionEditor.Initialize(filename, true);
            this.ApplySettings();
        }

        /// <summary>
        /// Initialize the expression control with the file specified, not a template file.
        /// </summary>
        /// <param name="filename">Filename of expression file to load into control.</param>
        private void InitializeExpressionControl(string filename)
        {
            try
            {
                this.expressionEditor.Initialize(filename, false);
                this.applicationSettings.RecentFiles.AddRecentFile(filename);
                this.ApplySettings();
            }
            catch (Exception ex)
            {
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(String.Format(CultureInfo.CurrentCulture, "An error occured whilst loading the expression file. This may be an invalid file."), Application.ProductName, ExceptionMessageBoxButtons.OK, ExceptionMessageBoxSymbol.Error);
                messageBox.InnerException = ex;
                messageBox.Show(this);
            }
        }

        /// <summary>
        /// Closes the expression editor.
        /// </summary>
        /// <returns>Boolean value indicating whether the form can be closed or not.</returns>
        private bool CloseExpressionEditor()
        {
            if (!this.expressionEditor.Saved)
            {
                DialogResult result;

                if (this.expressionEditor.FileName == null || this.expressionEditor.FileName.Length == 0)
                {
                    string message = String.Format(CultureInfo.CurrentCulture, "Do you want to save changes to \"New Expression\"?");
                    Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(message, Application.ProductName, ExceptionMessageBoxButtons.YesNoCancel, ExceptionMessageBoxSymbol.Question);
                    result = messageBox.Show(this);

                    if (result == DialogResult.Yes)
                    {
                        this.SaveFileAs();
                    }
                }
                else
                {
                    string message = String.Format(CultureInfo.CurrentCulture, "Do you want to save changes to \"{0}\"?", Path.GetFileName(this.expressionEditor.FileName));
                    Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(message, Application.ProductName, ExceptionMessageBoxButtons.YesNoCancel, ExceptionMessageBoxSymbol.Question);
                    result = messageBox.Show(this);

                    if (result == DialogResult.Yes)
                    {
                        this.expressionEditor.Save();
                    }
                }

                if (result == DialogResult.Cancel)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Opens the file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = ExpressionFileFilter;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.InitializeExpressionControl(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SaveFile(object sender, EventArgs e)
        {
            if (this.expressionEditor.FileName == null || this.expressionEditor.FileName.Length == 0)
            {
                this.SaveFileAs(sender, e);
            }
            else
            {
                this.expressionEditor.Save();
            }
        }

        /// <summary>
        /// Perform a SaveAs on the current file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SaveFileAs(object sender, EventArgs e)
        {
            this.SaveFileAs();
        }

        /// <summary>
        /// Perform a SaveAs on the current file.
        /// </summary>
        private void SaveFileAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (this.expressionEditor.Saved)
            {
                saveFileDialog.FileName = this.expressionEditor.FileName;
            }
            else
            {
                saveFileDialog.FileName = "New Expression";
            }

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = ExpressionFileFilter;
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                this.expressionEditor.Save(filename, false);
                this.applicationSettings.RecentFiles.AddRecentFile(filename);
            }
        }

        /// <summary>
        /// Handles the Click event of the SaveAsTemplateToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SaveAsTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.expressionEditor.Save(GetTemplateFilename(false), true);
            }
            catch (Exception ex)
            {
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(ex);
                messageBox.Caption = Application.ProductName;
                messageBox.Symbol = ExceptionMessageBoxSymbol.Error;
                messageBox.Show(this);
            }
        }

        /// <summary>
        /// Handles the Click event of the ExitToolsStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handle the Cut event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Cut(object sender, EventArgs e)
        {
            this.expressionEditor.Cut();
        }

        /// <summary>
        /// Handle the Copy event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Copy(object sender, EventArgs e)
        {
            this.expressionEditor.Copy();
        }

        /// <summary>
        /// Handle the Paste event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Paste(object sender, EventArgs e)
        {
            this.expressionEditor.Paste();
        }

        /// <summary>
        /// Handle the Undo event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Undo(object sender, EventArgs e)
        {
             this.expressionEditor.Undo();
        }

        /// <summary>
        /// Handle the Redo event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Redo(object sender, EventArgs e)
        {
            this.expressionEditor.Redo();
        }

        /// <summary>
        /// Handle the Select All event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SelectAllText(object sender, EventArgs e)
        {
            this.expressionEditor.SelectAllText();
        }

        /// <summary>
        /// Handles the Click event of the ToolBarToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.toolStrip.Visible = this.toolBarToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Handles the Click event of the StatusBarToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.statusStrip.Visible = this.statusBarToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Loads the program settings and apply them.
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                this.settingsManager.BeginLoad();

                this.settingsManager.LoadObject(this.programSettings, null);
                this.settingsManager.LoadFormState(this);

                this.toolStrip.Visible = this.settingsManager.LoadValue("toolStrip.Visible", this.toolStrip.Visible);
                this.statusStrip.Visible = this.settingsManager.LoadValue("statusStrip.Visible", this.statusStrip.Visible);
                this.expressionEditor.WordWrap = this.settingsManager.LoadValue("expressionEditor.WordWrap", this.expressionEditor.WordWrap);

                this.expressionEditor.SplitterDistanceExpressions = this.settingsManager.LoadValue("expressionEditor.SplitterDistanceExpressions", this.expressionEditor.SplitterDistanceExpressions);
                this.expressionEditor.SplitterDistanceResult = this.settingsManager.LoadValue("expressionEditor.SplitterDistanceResult", this.expressionEditor.SplitterDistanceResult);
                this.expressionEditor.SplitterDistanceMain = this.settingsManager.LoadValue("expressionEditor.SplitterDistanceMain", this.expressionEditor.SplitterDistanceMain);

                this.settingsManager.LoadObject(this.applicationSettings, null);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.ToString());
            }
            finally
            {
                this.settingsManager.EndLoad();
            }

            //////try
            //////{
            ////    this.Size = Settings.Default.ExpressionTesterSize;
            ////    this.Location = Settings.Default.ExpressionTesterLocation;
            ////    this.WindowState = Settings.Default.ExpressionTesterState;
            ////    settings.ExpressionColor = Settings.Default.ExpressionColor;
            ////    settings.ExpressionFont = Settings.Default.ExpressionFont;
            ////    settings.ResultColor = Settings.Default.ResultColor;
            ////    settings.ResultFont = Settings.Default.ResultFont;
            ////    settings.MatchCase = Settings.Default.FindOrReplaceMatchCase;
            ////    settings.SearchUp = Settings.Default.FindOrReplaceSearchUp;
            ////    settings.FindNotFound = Settings.Default.FindOrReplaceFindNotFound;
            ////    settings.ReplaceAllCount = Settings.Default.FindOrReplaceReplaceAllCount;
            //////}
            //////catch { }
        }

        /// <summary>
        /// Applies the settings to the expression editor control.
        /// </summary>
        private void ApplySettings()
        {
            this.ApplySettings(this.programSettings);
        }

        /// <summary>
        /// Applies the settings to the expression editor control.
        /// </summary>
        /// <param name="settings">The program settings object.</param>
        private void ApplySettings(ProgramSettings settings)
        {
            this.expressionEditor.ExpressionColor = settings.ExpressionColor;
            this.expressionEditor.ExpressionFont = settings.ExpressionFont;
            this.expressionEditor.ResultColor = settings.ResultColor;
            this.expressionEditor.ResultFont = settings.ResultFont;
            this.expressionEditor.FindOrReplaceMatchCase = settings.MatchCase;
            this.expressionEditor.FindOrReplaceSearchUp = settings.SearchUp;
        }

        /// <summary>
        /// Enable or disable the form controls.
        /// </summary>
        /// <param name="status">if set to <c>true</c> enable the controls; otherwise disable the controls..</param>
        private void EnableControls(bool status)
        {
            this.saveToolStripButton.Enabled = status;
            this.saveToolStripMenuItem.Enabled = status;

            this.saveAsToolStripMenuItem.Enabled = status;

            this.addVariabletoolStripButton.Enabled = status;
            this.addVariableToolStripMenuItem.Enabled = status;

            if (status == false)
            {
                this.deleteVariabletoolStripButton.Enabled = status;
                this.deleteVariableToolStripMenuItem.Enabled = status;
                this.editVariabletoolStripButton.Enabled = status;
                this.editVariableToolStripMenuItem.Enabled = status;
            }

            this.runToolStripButton.Enabled = status;
            this.evaluateToolStripMenuItem.Enabled = status;

            this.expressionFilePropertiesToolStripMenuItem.Enabled = status;
            this.expressionFileSettingsToolStripButton.Enabled = status;

            for (int index = 0; index < this.editMenu.DropDownItems.Count; index++)
            {
                this.editMenu.DropDownItems[index].Enabled = status;
            }
        }

        /// <summary>
        /// Handles the FormClosing event of the ExpressionTester control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void ExpressionTester_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if expression shouldd be saved or close canceled?
            e.Cancel = this.CloseExpressionEditor();
            if (e.Cancel)
            {
                return;
            }

            // Save Settings
            try
            {
                this.settingsManager.BeginSave();

                this.settingsManager.SaveObject(this.programSettings, null);
                this.settingsManager.SaveFormState(this);

                this.settingsManager.SaveValue("toolStrip.Visible", this.toolStrip.Visible);
                this.settingsManager.SaveValue("statusStrip.Visible", this.statusStrip.Visible);
                this.settingsManager.SaveValue("expressionEditor.WordWrap", this.expressionEditor.WordWrap);

                this.settingsManager.SaveValue("expressionEditor.SplitterDistanceExpressions", this.expressionEditor.SplitterDistanceExpressions);
                this.settingsManager.SaveValue("expressionEditor.SplitterDistanceResult", this.expressionEditor.SplitterDistanceResult);
                this.settingsManager.SaveValue("expressionEditor.SplitterDistanceMain", this.expressionEditor.SplitterDistanceMain);

                ////toolStrip.Visible = settingsManager.LoadValue("toolStrip.Visible", toolStrip.Visible);
                ////statusStrip.Visible = settingsManager.LoadValue("statusStrip.Visible", statusStrip.Visible);
                ////expressionEditor.WordWrap = settingsManager.LoadValue("expressionEditor.WordWrap", expressionEditor.WordWrap);

                ////expressionEditor.SplitterDistanceExpressions = settingsManager.LoadValue("expressionEditor.SplitterDistanceExpressions", expressionEditor.SplitterDistanceExpressions);
                ////expressionEditor.SplitterDistanceResult = settingsManager.LoadValue("expressionEditor.SplitterDistanceResult", expressionEditor.SplitterDistanceResult);
                ////expressionEditor.SplitterDistanceMain = settingsManager.LoadValue("expressionEditor.SplitterDistanceMain", expressionEditor.SplitterDistanceMain);

                this.settingsManager.SaveObject(this.applicationSettings, null);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.ToString());
            }
            finally
            {
                this.settingsManager.EndSave();
            }

            // TODO: DO we need this or not?
            //////try
            //////{
            ////    Settings.Default.ExpressionTesterState = this.WindowState;
            ////    Settings.Default.ToolStripVisible = this.toolStrip.Visible;
            ////    Settings.Default.StatusBarVisible = this.statusStrip.Visible;
            ////    Settings.Default.WordWrap = this.expressionEditor.WordWrap;
            ////    Settings.Default.SplitterDistanceExpressions = this.expressionEditor.SplitterDistanceExpressions;
            ////    Settings.Default.SplitterDistanceResult = this.expressionEditor.SplitterDistanceResult;
            ////    Settings.Default.SplitterDistanceMain = this.expressionEditor.SplitterDistanceMain;
            ////    if (this.WindowState == FormWindowState.Normal)
            ////    {
            ////        Settings.Default.ExpressionTesterSize = this.Size;
            ////        Settings.Default.ExpressionTesterLocation = this.Location;
            ////    }
            ////    else
            ////    {
            ////        Settings.Default.ExpressionTesterSize = this.RestoreBounds.Size;
            ////        Settings.Default.ExpressionTesterLocation = this.RestoreBounds.Location;
            ////    }
            //////}
            //////catch { }
            //////try
            //////{
            ////    Settings.Default.Save();
            //////}
            //////catch { }
        }

        /// <summary>
        /// Handles the Load event of the ExpressionTester control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExpressionTester_Load(object sender, EventArgs e)
        {
            this.saveToolStripButton.Enabled = false;
            this.saveToolStripMenuItem.Enabled = false;
            this.addVariabletoolStripButton.Enabled = true;
            this.addVariableToolStripMenuItem.Enabled = true;
            this.deleteVariabletoolStripButton.Enabled = false;
            this.deleteVariableToolStripMenuItem.Enabled = false;
            this.editVariabletoolStripButton.Enabled = false;
            this.editVariableToolStripMenuItem.Enabled = false;
            this.runToolStripButton.Enabled = true;
            this.evaluateToolStripMenuItem.Enabled = true;
            this.expressionFilePropertiesToolStripMenuItem.Enabled = true;
            this.expressionFileSettingsToolStripButton.Enabled = true;

            for (int index = 0; index < this.editMenu.DropDownItems.Count; index++)
            {
                this.editMenu.DropDownItems[index].Enabled = true;
            }

            this.LoadSettings();

            try
            {
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1 && args[1] != null && File.Exists(args[1]))
                {
                    this.InitializeExpressionControl(args[1]);
                }
                else
                {
                    this.New(null, null);
                }
            }
            catch (Exception ex)
            {
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(ex);
                messageBox.Caption = Application.ProductName;
                messageBox.Symbol = ExceptionMessageBoxSymbol.Error;
                messageBox.Show(this);
            }
        }

        /// <summary>
        /// Handles the Run event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Run(object sender, EventArgs e)
        {
            this.expressionEditor.Run();
        }

        /// <summary>
        /// Handles the DropDownOpening event of the recentExpressionFileToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RecentExpressionFileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            // Clear Old Entries
            this.recentFilesToolStripMenuItem.DropDownItems.Clear();

            try
            {
                string[] files = this.applicationSettings.RecentFiles.GetFiles();
                EventHandler expressionFileItemClick = new EventHandler(this.OpenRecentFile);

                for (int index = 0; index < files.Length; index++)
                {
                    string file = files[index];
                    if (file.Length > 0)
                    {
                        ToolStripItem item = this.recentFilesToolStripMenuItem.DropDownItems.Add(String.Format(CultureInfo.CurrentCulture, "{0} {1}", index + 1, file));
                        item.Click += expressionFileItemClick;
                        item.Tag = file;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.ToString());
            }

            if (this.recentFilesToolStripMenuItem.DropDownItems.Count == 0)
            {
                ToolStripMenuItem fileListEmptyToolStripMenuItem = new ToolStripMenuItem();
                fileListEmptyToolStripMenuItem.Enabled = false;
                fileListEmptyToolStripMenuItem.Name = "fileListEmptyToolStripMenuItem";
                fileListEmptyToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
                fileListEmptyToolStripMenuItem.Text = Resources.TextFileListEmpty;
                this.recentFilesToolStripMenuItem.DropDownItems.Add(fileListEmptyToolStripMenuItem);
            }
        }

        /// <summary>
        /// Handles the Add variable event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AddVariable(object sender, EventArgs e)
        {
            this.expressionEditor.AddVariable();
        }

        /// <summary>
        /// Handles the Delete the variable event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DeleteVariable(object sender, EventArgs e)
        {
            this.expressionEditor.DeleteVariable();
        }

        /// <summary>
        /// Handles the Edit the variable event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EditVariable(object sender, EventArgs e)
        {
            this.expressionEditor.EditVariable();
        }

        /// <summary>
        /// Shows the about dialog.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ShowAboutDialog(object sender, EventArgs e)
        {
            AboutDialog dialog = new AboutDialog(this);
            dialog.Show(this);
        }

        /// <summary>
        /// Shows the options dialog.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ShowOptions(object sender, EventArgs e)
        {
            ProgramSettings settingsCopy = this.programSettings.Clone();
            OptionsDialog dialog = new OptionsDialog(settingsCopy);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.programSettings = dialog.ProgramSettings;
                this.ApplySettings();
            }
        }

        /// <summary>
        /// Shows the Options dialog.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ShowExpressionFileProperties(object sender, EventArgs e)
        {
            ExpressionFileSettings settings = new ExpressionFileSettings();
            settings.ResultType = this.expressionEditor.ResultType;
            settings.ResultTypeValidate = this.expressionEditor.ResultTypeValidate;

            OptionsDialog dialog = new OptionsDialog(settings);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                // Check if properties have changed. If so set the new values and invalidate Saved status of expression control
                if (this.expressionEditor.ResultType != dialog.ExpressionFileSettings.ResultType)
                {
                    this.expressionEditor.ResultType = dialog.ExpressionFileSettings.ResultType;
                    this.expressionEditor.InvalidateSaved();
                }

                if (this.expressionEditor.ResultTypeValidate != dialog.ExpressionFileSettings.ResultTypeValidate)
                {
                    this.expressionEditor.ResultTypeValidate = dialog.ExpressionFileSettings.ResultTypeValidate;
                    this.expressionEditor.InvalidateSaved();
                }
            }
        }

        /// <summary>
        /// Handles the VariableSelectionChanged event of the expressionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.VariableSelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ExpressionEditor_VariableSelectionChanged(object sender, VariableSelectionChangedEventArgs e)
        {
            this.deleteVariabletoolStripButton.Enabled = e.VariableSelected;
            this.deleteVariableToolStripMenuItem.Enabled = e.VariableSelected;
            this.editVariabletoolStripButton.Enabled = e.VariableSelected;
            this.editVariableToolStripMenuItem.Enabled = e.VariableSelected;
        }

        /// <summary>
        /// Handles the TitleChanged event of the expressionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.TitleChangedEventArgs"/> instance containing the event data.</param>
        private void ExpressionEditor_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            ExpressionEditorView editor = sender as ExpressionEditorView;
            if (editor != null)
            {
                if (e.Title == null)
                {
                    this.Text = Application.ProductName;
                    this.toolStripStatusLabelFilename.Text = "(New Expression)";
                }
                else
                {
                    if (editor.Saved)
                    {
                        this.Text = String.Format(CultureInfo.CurrentCulture, "{0} - {1}", e.Title, Application.ProductName);
                    }
                    else
                    {
                        this.Text = String.Format(CultureInfo.CurrentCulture, "{0}* - {1}", e.Title, Application.ProductName);
                    }

                    this.toolStripStatusLabelFilename.Text = editor.FileName;
                }

                this.saveToolStripButton.Enabled = !editor.Saved;
                this.saveToolStripMenuItem.Enabled = !editor.Saved;
            }
        }

        /// <summary>
        /// Handles the ResultTypeChanged event of the expressionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.ResultTypeChangedEventArgs"/> instance containing the event data.</param>
        private void ExpressionEditor_ResultTypeChanged(object sender, ResultTypeChangedEventArgs e)
        {
            this.toolStripStatusLabelResultType.Text = String.Format(CultureInfo.CurrentCulture, "Type {0}", e.ResultType);
            this.toolStripStatusLabelResultType.Enabled = e.Validate;
        }
        
        /// <summary>
        /// Handles the UndoOrRedoCountChanged event of the expressionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.UndoOrRedoCountChangedEventArgs"/> instance containing the event data.</param>
        private void ExpressionEditor_UndoOrRedoCountChanged(object sender, UndoOrRedoCountChangedEventArgs e)
        {
            this.undoToolStripMenuItem.Enabled = e.UndoCount > 0;
            this.redoToolStripMenuItem.Enabled = e.RedoCount > 0;
        }
        
        /// <summary>
        /// Handles the CursorPositionChanged event of the expressionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.CursorPositionChangedEventArgs"/> instance containing the event data.</param>
        private void ExpressionEditor_CursorPositionChanged(object sender, CursorPositionChangedEventArgs e)
        {
            this.toolStripStatusLabelLine.Text = String.Format(CultureInfo.CurrentCulture, LineStatusFormat, e.Line + 1);
            this.toolStripStatusLabelColumn.Text = String.Format(CultureInfo.CurrentCulture, ColumnStatusFormat, e.Column + 1);
            Debug.WriteLine(this.toolStripStatusLabelLine.Text + " " + this.toolStripStatusLabelColumn.Text + String.Format(CultureInfo.CurrentCulture, "  {0},{1}", e.Line + 1, e.Column + 1));
            this.toolStripStatusLabelColumn.Invalidate();
        }
        
        /// <summary>
        /// Handles the FindNotFound event of the expressionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.FindOrReplaceEventArgs"/> instance containing the event data.</param>
        private void ExpressionEditor_FindNotFound(object sender, FindOrReplaceEventArgs e)
        {
            if (this.programSettings.FindNotFound)
            {
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox();
                messageBox.Caption = Application.ProductName;
                messageBox.Text = String.Format(CultureInfo.CurrentCulture, "{0}\r\n\r\n{1}", Resources.FindOrReplaceFindNotFound, e.SearchTerm);
                messageBox.ShowCheckBox = true;
                messageBox.CheckBoxText = Resources.AlwaysDisplayThisMessage;
                messageBox.IsCheckBoxChecked = this.programSettings.FindNotFound;
                messageBox.Show(this);
                this.programSettings.FindNotFound = messageBox.IsCheckBoxChecked;
            }
        }

        /// <summary>
        /// Handles the ReplaceAllComplete event of the expressionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Konesans.Dts.ExpressionEditor.Controls.FindOrReplaceEventArgs"/> instance containing the event data.</param>
        private void ExpressionEditor_ReplaceAllComplete(object sender, FindOrReplaceEventArgs e)
        {
            if (this.programSettings.ReplaceAllCount)
            {
                Microsoft.SqlServer.MessageBox.ExceptionMessageBox messageBox = new Microsoft.SqlServer.MessageBox.ExceptionMessageBox(String.Format(CultureInfo.CurrentCulture, Resources.FindOrReplaceReplaceAllCount, e.ReplaceCount), Application.ProductName);
                messageBox.Caption = Application.ProductName;
                messageBox.ShowCheckBox = true;
                messageBox.CheckBoxText = Resources.AlwaysDisplayThisMessage;
                messageBox.IsCheckBoxChecked = this.programSettings.ReplaceAllCount;
                messageBox.Show(this);
                this.programSettings.ReplaceAllCount = messageBox.IsCheckBoxChecked;
            }
        }

        /// <summary>
        /// Handles the EditModeChanged event of the expressionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExpressionEditor_EditModeChanged(object sender, EventArgs e)
        {
            bool enabled = this.expressionEditor.EditMode;
            this.expressionEditor.EditMode = enabled;
            this.EnableControls(!enabled);

            // Checked controls
            this.editFunctionsToolStripMenuItem.Checked = enabled;
            this.toolStripButtonEditFunctions.Checked = enabled;

            // Additional controls
            this.newToolStripButton.Enabled = !enabled;
            this.newToolStripMenuItem.Enabled = !enabled;
            this.openToolStripButton.Enabled = !enabled;
            this.openToolStripMenuItem.Enabled = !enabled;
            this.toolStripButtonFind.Enabled = !enabled;
            this.toolStripButtonReplace.Enabled = !enabled;
            this.saveAsTemplateToolStripMenuItem.Enabled = !enabled;
            this.recentFilesToolStripMenuItem.Enabled = !enabled;
        }

        /// <summary>
        /// Handles the Find event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void FindExpression(object sender, EventArgs e)
        {
            this.expressionEditor.Find();
        }

        /// <summary>
        /// Handles the Replace event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ReplaceExpression(object sender, EventArgs e)
        {
            this.expressionEditor.Replace();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonEditFunctions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonEditFunctions_Click(object sender, EventArgs e)
        {
            ToolStripButton button = sender as ToolStripButton;
            if (button == null)
            {
                return;
            }

            // Change edit mode, but leave UI updates until event is raised
            this.expressionEditor.EditMode = button.Checked;
        }

        /// <summary>
        /// Handles the Click event of the WordWrapToolStripMenuItem control.
        /// Occurs when a key is pressed while the form has focus.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void WordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
            {
                return;
                }

            this.expressionEditor.WordWrap = item.Checked;
        }

        /// <summary>
        /// Handles the DropDownOpening event of the viewMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ViewMenu_DropDownOpening(object sender, EventArgs e)
        {
            this.wordWrapToolStripMenuItem.Checked = this.expressionEditor.WordWrap;
        }

        /// <summary>
        /// Handles the Click event of the editFunctionsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EditFunctionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            if (menu == null)
            {
                return;
            }

            // Change edit mode, but leave UI updates until event is raised
            this.expressionEditor.EditMode = menu.Checked;
        }

        /// <summary>
        /// Handles the Click event of the searchToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string CodeplexUrl = "http://expressioneditor.codeplex.com/";
            System.Diagnostics.Process.Start(CodeplexUrl);
        }
    }
}
