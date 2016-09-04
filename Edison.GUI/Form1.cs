/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using Edison.Engine.Contexts;
using Edison.Engine.Core.Enums;
using Edison.Engine;
using System.Threading;
using Edison.Framework;
using Edison.Framework.Enums;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using Edison.Engine.Core.Exceptions;

namespace Edison.GUI
{
    public partial class EdisonForm : Form
    {

        #region Repositories

        private IAssemblyRepository AssemblyRepository
        {
            get { return DIContainer.Instance.Get<IAssemblyRepository>(); }
        }

        private IPathRepository PathRepository
        {
            get { return DIContainer.Instance.Get<IPathRepository>(); }
        }

        private IReflectionRepository ReflectionRepository
        {
            get { return DIContainer.Instance.Get<IReflectionRepository>(); }
        }

        private IFileRepository FileRepository
        {
            get { return DIContainer.Instance.Get<IFileRepository>(); }
        }

        #endregion

        #region Properties

        public const string MainTitle = "Edison";
        public const char Separator = '.';

        private Assembly Assembly = default(Assembly);
        private int TotalNumberOfTestsRunning = 0;
        private int CurrentNumberOfTestsRun = 0;
        private string FileName = string.Empty;
        private string FilePath = string.Empty;

        private List<string> CheckedTests = new List<string>();
        private List<string> CheckedFixtures = new List<string>();

        private EdisonContext EdisonContext = default(EdisonContext);
        private Thread MainThread = default(Thread);

        #endregion

        #region Constructor

        public EdisonForm()
        {
            InitializeComponent();
            RefreshRecentlyOpened();

            FormClosing += EdisonForm_FormClosing;
            TestTree.NodeMouseDoubleClick += TestTree_NodeMouseDoubleClick;
            TestTree.KeyDown += TestTree_KeyDown;
            FailedTestListBox.MeasureItem += FailedTestListBox_MeasureItem;
            FailedTestListBox.DrawItem += FailedTestListBox_DrawItem;
            SuiteCheckList.ItemCheck += SuiteCheckList_ItemCheck;
            RecentlyOpenedMenuItem.DropDownItemClicked += RecentlyOpenedMenuItem_DropDownItemClicked;

            FixtureThreadNumericBox.Value = 1;
            TestThreadNumericBox.Value = 1;
            TestProgressBar.Value = 0;
        }

        #endregion

        #region Events

        private void EdisonForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var abortThread = new Thread(() => AbortThreads());
            abortThread.Start();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            DisableConsoleCheckBox.Checked = !DisableConsoleCheckBox.Checked;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            DisableTestCheckBox.Checked = !DisableTestCheckBox.Checked;
        }

        private void FailedTestListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            e.DrawBackground();
            var item = (TestResult)(FailedTestListBox.Items[e.Index]);
            e.Graphics.DrawString(item.FullName + Environment.NewLine + item.ErrorMessage, e.Font, new SolidBrush(e.ForeColor), e.Bounds);
        }

        private void FailedTestListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 65;
        }

        private void FailedTestListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FailedTestDetails.Clear();
            FailedTestDetails.AppendText("StackTrace:" + Environment.NewLine + ((TestResult)(((ListBox)sender).SelectedItem)).StackTrace);
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestTree.ExpandAll();

            if (TestTree.Nodes.Count > 0)
            {
                TestTree.Nodes[0].EnsureVisible();
            }
        }

        private void collapseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestTree.CollapseAll();
        }

        private void checkboxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestTree.CheckBoxes = !TestTree.CheckBoxes;
        }

        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleCheckBoxes(TestTree.Nodes, false);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = "*.dll";
            dialog.Multiselect = false;
            dialog.Title = "Edison - Select a DLL";
            dialog.Filter = "dll (*.dll)|*.dll";

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                OpenFile(dialog.FileName);
            }
            else if (result != DialogResult.Cancel)
            {
                MessageBox.Show("Failed to open the selected file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OutputRichText.Text))
            {
                MessageBox.Show("Nothing to save.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.CheckPathExists = true;
            dialog.DefaultExt = "*.txt";
            dialog.Title = "Edison - Save Test Result Output";
            dialog.Filter = "txt (*.txt)|*.txt";

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, OutputRichText.Text);
            }
            else if (result != DialogResult.Cancel)
            {
                MessageBox.Show("Failed to save test result output.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Author: Matthew Kelly\nVersion: " + Logger.Instance.GetVersion(),
                "Edison",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void TestTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            DoTestTreeSelect(e.Node);
        }

        private void TestTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DoTestTreeSelect(TestTree.SelectedNode);
            }
        }

        private void TestTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;

            if (node.Checked)
            {
                var tests = node.GetFullPaths(FileName, Separator);
                CheckedTests.AddRange(tests.Item1);
                CheckedFixtures.AddRange(tests.Item2);
            }
            else
            {
                var tests = node.GetFullPaths(FileName, Separator);

                foreach (var test in tests.Item1)
                {
                    CheckedTests.Remove(test);
                }

                foreach (var fixture in tests.Item2)
                {
                    CheckedFixtures.Remove(fixture);
                }
            }
        }

        private void RunTestsButton_Click(object sender, EventArgs e)
        {
            switch (RunTestsButton.Text.ToLower())
            {
                case "run":
                    if (TestTree.Nodes.Count != 0)
                    {
                        DoTestTreeSelect(null);
                    }
                    break;

                case "stop":
                    RunTestsButton.Text = "Run";
                    var abortThread = new Thread(() => AbortThreads());
                    abortThread.Start();
                    break;
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            switch (e.TabPageIndex)
            {
                case 0:
                    OutputRichText.Focus();
                    break;

                case 1:
                    FailedTestListBox.Focus();
                    break;
            }
        }

        private void EdisonContext_OnTestResult(TestResult result)
        {
            CurrentNumberOfTestsRun++;
            AddFailedTestResult(result);
            UpdateProgress();
        }

        private void SuiteCheckList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue != CheckState.Checked)
            {
                return;
            }

            var selectedItems = SuiteCheckList.CheckedIndices;
            if (selectedItems.Count > 0)
            {
                SuiteCheckList.SetItemCheckState(selectedItems[0], CheckState.Unchecked);
            }
        }

        private void RecentlyOpenedMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var name = e.ClickedItem.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (name.Equals("Clear Entries", StringComparison.InvariantCultureIgnoreCase))
            {
                RecentlyOpened.Clear();
                RefreshRecentlyOpened();
            }
            else if (FileRepository.Exists(name))
            {
                OpenFile(name);
            }
        }

        #endregion

        #region Helpers

        private void OpenFile(string path)
        {
            if (!FileRepository.Exists(path))
            {
                MessageBox.Show(string.Format("File doesn't exist: '{0}'", path), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RecentlyOpened.Add(path);
            RefreshRecentlyOpened();

            FilePath = path;
            FileName = PathRepository.GetFileName(FilePath);
            Text = MainTitle + " - " + FileName;
            Assembly = AssemblyRepository.LoadFile(FilePath);
            PopulateForm();
        }

        private void RunTests(List<string> tests, List<string> fixtures)
        {
            if (Assembly == default(Assembly) || (MainThread != default(Thread) && MainThread.ThreadState == ThreadState.Running))
            {
                return;
            }

            OutputRichText.Clear();
            TestProgressBar.Value = 0;
            FailedTestListBox.Items.Clear();
            FailedTestDetails.Clear();

            SetupContext(tests, fixtures);
            var logger = new OutputLogger(OutputRichText);
            Logger.Instance.SetOutput(logger);
            Logger.Instance.SetConsoleOutput(logger);

            CurrentNumberOfTestsRun = 0;
            TotalNumberOfTestsRunning = AssemblyRepository.GetTestCount(
                Assembly,
                EdisonContext.IncludedCategories,
                EdisonContext.ExcludedCategories,
                EdisonContext.Fixtures,
                EdisonContext.Tests,
                EdisonContext.Suite);

            MainThread = new Thread(() => Run(EdisonContext));
            MainThread.Start();

            OutputRichText.Focus();
            RunTestsButton.Text = "Stop";
        }

        private void Run(EdisonContext context)
        {
            try
            {
                context.Run();
            }
            catch (ValidationException vex)
            {
                MessageBox.Show(vex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}{2}{2}{1}", ex.Message, ex.StackTrace, Environment.NewLine), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetRunTestButton("Run");
            SetProgress(100);
        }

        private void UpdateProgress()
        {
            if (CurrentNumberOfTestsRun == 0 || TotalNumberOfTestsRunning == 0)
            {
                return;
            }

            var progress = (int)(((double)CurrentNumberOfTestsRun / (double)TotalNumberOfTestsRunning) * 100.0);
            SetProgress(Math.Max(0, Math.Min(97, progress)));
        }

        private void SetProgress(int progress)
        {
            if (TestProgressBar.IsDisposed)
            {
                return;
            }

            try
            {
                if (TestProgressBar.InvokeRequired)
                {
                    TestProgressBar.Invoke((MethodInvoker)delegate { TestProgressBar.Value = progress; });
                }
                else
                {
                    TestProgressBar.Value = progress;
                }
            }
            catch (ObjectDisposedException) { }
        }

        private void AddFailedTestResult(TestResult result)
        {
            if (result.State == TestResultState.Success)
            {
                return;
            }

            if (FailedTestListBox.IsDisposed)
            {
                return;
            }

            try
            {
                if (FailedTestListBox.InvokeRequired)
                {
                    FailedTestListBox.BeginInvoke((MethodInvoker)delegate { FailedTestListBox.Items.Add(result); });
                }
                else
                {
                    FailedTestListBox.Items.Add(result);
                }
            }
            catch (ObjectDisposedException) { }
        }

        private void SetRunTestButton(string value)
        {
            if (RunTestsButton.IsDisposed)
            {
                return;
            }

            try
            {
                if (RunTestsButton.InvokeRequired)
                {
                    RunTestsButton.BeginInvoke((MethodInvoker)delegate { RunTestsButton.Text = value; });
                }
                else
                {
                    RunTestsButton.Text = value;
                }
            }
            catch (ObjectDisposedException) { }
        }

        private void SetupContext(List<string> tests, List<string> fixtures)
        {
            EdisonContext = EdisonContext.Create();
            EdisonContext.Assemblies.Add(FilePath);
            EdisonContext.NumberOfFixtureThreads = (int)FixtureThreadNumericBox.Value;
            EdisonContext.NumberOfTestThreads = (int)TestThreadNumericBox.Value;
            EdisonContext.DisableConsoleOutput = DisableConsoleCheckBox.Checked;
            EdisonContext.DisableTestOutput = DisableTestCheckBox.Checked;
            EdisonContext.IncludedCategories.AddRange(IncludedCategoriesCheckList.CheckedItems.Cast<string>());
            EdisonContext.ExcludedCategories.AddRange(ExcludedCategoriesCheckList.CheckedItems.Cast<string>());
            EdisonContext.Suite = SuiteCheckList.CheckedItems.Cast<string>().FirstOrDefault();

            if (tests != default(List<string>))
            {
                EdisonContext.Tests.AddRange(tests);
            }

            if (fixtures != default(List<string>))
            {
                EdisonContext.Fixtures.AddRange(fixtures);
            }

            EdisonContext.DisableFileOutput = true;
            EdisonContext.ConsoleOutputType = OutputType.Txt;
            EdisonContext.OnTestResult += EdisonContext_OnTestResult;
        }

        private void DoTestTreeSelect(TreeNode node)
        {
            if (node == default(TreeNode))
            {
                if (CheckedTests.Any() || CheckedFixtures.Any())
                {
                    RunTests(CheckedTests, CheckedFixtures);
                    return;
                }

                node = TestTree.SelectedNode;
            }

            if (node != default(TreeNode))
            {
                var tests = node.GetFullPaths(FileName, Separator);
                RunTests(tests.Item1, tests.Item2);
            }
        }

        private void ToggleCheckBoxes(TreeNodeCollection nodes, bool flag)
        {
            foreach (TreeNode node in nodes)
            {
                ToggleCheckBoxes(node.Nodes, flag);
                node.Checked = flag;
            }
        }

        private void PopulateForm()
        {
            TestTree.Nodes.Clear();
            IncludedCategoriesCheckList.Items.Clear();
            ExcludedCategoriesCheckList.Items.Clear();
            SuiteCheckList.Items.Clear();

            var tests = AssemblyRepository.GetTests(Assembly, default(IList<string>), default(IList<string>), default(IList<string>), default(IList<string>), null);
            PopulateTests(tests.Item1);

            var categories = AssemblyRepository.GetCategories(Assembly, tests.Item1, tests.Item2);
            PopulateCategories(categories);

            var suites = AssemblyRepository.GetSuites(Assembly, tests.Item2);
            PopulateSuites(suites);
        }

        private void PopulateSuites(IEnumerable<string> suites)
        {
            if (!suites.Any())
            {
                return;
            }

            foreach (var suite in suites)
            {
                SuiteCheckList.Items.Add(suite, CheckState.Unchecked);
            }
        }

        private void PopulateCategories(IEnumerable<string> categories)
        {
            if (!categories.Any())
            {
                return;
            }

            foreach (var category in categories)
            {
                IncludedCategoriesCheckList.Items.Add(category, CheckState.Unchecked);
                ExcludedCategoriesCheckList.Items.Add(category, CheckState.Unchecked);
            }
        }

        private void PopulateTests(IEnumerable<MethodInfo> tests)
        {
            var rootNode = new TreeNode(FileName);

            foreach (var test in tests)
            {
                var currentNode = rootNode;
                var pathItems = ReflectionRepository.GetFullNamespace(test).Split(Separator);

                foreach (var item in pathItems)
                {
                    var temp = currentNode.Nodes.Cast<TreeNode>().Where(x => x.Text.Equals(item));
                    currentNode = temp.Any() ? temp.Single() : currentNode.Nodes.Add(item);
                }
            }

            TestTree.Nodes.Add(rootNode);
            TestTree.PathSeparator = Separator.ToString();
            TestTree.ContextMenuStrip = TestTreeContextMenu;
            TestTree.Focus();
        }

        private void AbortThreads()
        {
            if (MainThread != default(Thread))
            {
                MainThread.Abort();
            }

            if (EdisonContext != default(EdisonContext) && EdisonContext.IsRunning)
            {
                EdisonContext.Interrupt();
            }
        }

        private void RefreshRecentlyOpened()
        {
            RecentlyOpened.Populate(RecentlyOpenedMenuItem);
        }

        #endregion
    }
}
