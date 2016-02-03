using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Edison.Engine.Utilities.Extensions;
using Edison.Engine.Utilities.Helpers;
using System.Reflection;
using System.IO;
using Edison.Engine.Contexts;
using Edison.Engine.Core.Enums;
using Edison.Engine;

namespace Edison.GUI
{
    public partial class EdisonForm : Form
    {

        #region Properties

        public const string MainTitle = "Edison";
        public const char Separator = '.';
        private string FileName = string.Empty;
        private string FilePath = string.Empty;

        #endregion

        #region Constructor

        public EdisonForm()
        {
            InitializeComponent();
            TestTree.NodeMouseDoubleClick += TestTree_NodeMouseDoubleClick;
            TestTree.KeyDown += TestTree_KeyDown;
        }
        
        #endregion

        #region Events

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
                FilePath = dialog.FileName;
                FileName = Path.GetFileName(FilePath);
                Text = MainTitle + " - " + FileName;

                var assembly = AssemblyHelper.GetAssembly(FilePath);
                var tests = assembly.GetTests(null, null, null);
                PopulateTestTree(tests.ToList());
            }
            else
            {
                MessageBox.Show("Failed to open the selected file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
            {
                RunTests(new List<string>() { e.Node.FullPath.Replace(FileName + ".", string.Empty) });
            }
        }

        private void TestTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            var node = TestTree.SelectedNode;
            if (node.Nodes.Count == 0)
            {
                RunTests(new List<string>() { node.FullPath.Replace(FileName + ".", string.Empty) });
            }
        }

        #endregion

        #region Helpers

        private void RunTests(List<string> tests)
        {
            OutputRichText.Clear();
            var context = new EdisonContext();

            context.AssemblyPaths.Add(FilePath);
            context.NumberOfThreads = 1;
            context.Tests.AddRange(tests);
            context.CreateOutput = false;
            context.ConsoleOutputType = OutputType.None;

            var logger = new OutputLogger(OutputRichText);
            Logger.Instance.SetOutput(logger, logger);
            context.Run();
        }

        private void PopulateTestTree(List<MethodInfo> tests)
        {
            if (tests == default(List<MethodInfo>) || !tests.Any())
            {
                return;
            }
            
            var rootNode = new TreeNode(FileName);

            foreach (var test in tests)
            {
                var currentNode = rootNode;
                var pathItems = test.GetFullNamespace().Split(Separator);

                foreach (var item in pathItems)
                {
                    var temp = currentNode.Nodes.Cast<TreeNode>().Where(x => x.Text.Equals(item));
                    currentNode = temp.Any() ? temp.Single() : currentNode.Nodes.Add(item);
                }
            }

            TestTree.Nodes.Add(rootNode);
            TestTree.PathSeparator = Separator.ToString();
            TestTree.Focus();
        }

        #endregion

    }
}
