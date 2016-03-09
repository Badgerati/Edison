namespace Edison.GUI
{
    partial class EdisonForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EdisonForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TestThreadNumericBox = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.DisableTestCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.DisableConsoleCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FixtureThreadNumericBox = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.TestProgressBar = new System.Windows.Forms.ProgressBar();
            this.RunTestsButton = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.TestTree = new System.Windows.Forms.TreeView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.IncludedCategoriesCheckList = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ExcludedCategoriesCheckList = new System.Windows.Forms.CheckedListBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.SuiteCheckList = new System.Windows.Forms.CheckedListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.OutputRichText = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.FailedTestListBox = new System.Windows.Forms.ListBox();
            this.FailedTestDetails = new System.Windows.Forms.RichTextBox();
            this.TestTreeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.checkboxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RecentlyOpenedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TestThreadNumericBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FixtureThreadNumericBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.TestTreeContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1128, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.RecentlyOpenedMenuItem,
            this.toolStripSeparator1,
            this.saveToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(166, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitContainer1.Panel1.Controls.Add(this.TestThreadNumericBox);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.DisableTestCheckBox);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.DisableConsoleCheckBox);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.FixtureThreadNumericBox);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.TestProgressBar);
            this.splitContainer1.Panel1.Controls.Add(this.RunTestsButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1128, 610);
            this.splitContainer1.SplitterDistance = 60;
            this.splitContainer1.TabIndex = 1000;
            this.splitContainer1.TabStop = false;
            // 
            // TestThreadNumericBox
            // 
            this.TestThreadNumericBox.Location = new System.Drawing.Point(323, 34);
            this.TestThreadNumericBox.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.TestThreadNumericBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TestThreadNumericBox.Name = "TestThreadNumericBox";
            this.TestThreadNumericBox.Size = new System.Drawing.Size(40, 20);
            this.TestThreadNumericBox.TabIndex = 23;
            this.TestThreadNumericBox.TabStop = false;
            this.TestThreadNumericBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TestThreadNumericBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(244, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Test Threads:";
            // 
            // DisableTestCheckBox
            // 
            this.DisableTestCheckBox.AutoSize = true;
            this.DisableTestCheckBox.Location = new System.Drawing.Point(627, 36);
            this.DisableTestCheckBox.Name = "DisableTestCheckBox";
            this.DisableTestCheckBox.Size = new System.Drawing.Size(15, 14);
            this.DisableTestCheckBox.TabIndex = 17;
            this.DisableTestCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(517, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Disable Test Output:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // DisableConsoleCheckBox
            // 
            this.DisableConsoleCheckBox.AutoSize = true;
            this.DisableConsoleCheckBox.Location = new System.Drawing.Point(473, 36);
            this.DisableConsoleCheckBox.Name = "DisableConsoleCheckBox";
            this.DisableConsoleCheckBox.Size = new System.Drawing.Size(15, 14);
            this.DisableConsoleCheckBox.TabIndex = 15;
            this.DisableConsoleCheckBox.TabStop = false;
            this.DisableConsoleCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DisableConsoleCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(387, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Disable Output:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // FixtureThreadNumericBox
            // 
            this.FixtureThreadNumericBox.Location = new System.Drawing.Point(182, 34);
            this.FixtureThreadNumericBox.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.FixtureThreadNumericBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FixtureThreadNumericBox.Name = "FixtureThreadNumericBox";
            this.FixtureThreadNumericBox.Size = new System.Drawing.Size(40, 20);
            this.FixtureThreadNumericBox.TabIndex = 13;
            this.FixtureThreadNumericBox.TabStop = false;
            this.FixtureThreadNumericBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.FixtureThreadNumericBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Fixture Threads:";
            // 
            // TestProgressBar
            // 
            this.TestProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TestProgressBar.Location = new System.Drawing.Point(93, 7);
            this.TestProgressBar.Name = "TestProgressBar";
            this.TestProgressBar.Size = new System.Drawing.Size(1023, 23);
            this.TestProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.TestProgressBar.TabIndex = 11;
            // 
            // RunTestsButton
            // 
            this.RunTestsButton.Location = new System.Drawing.Point(12, 7);
            this.RunTestsButton.Name = "RunTestsButton";
            this.RunTestsButton.Size = new System.Drawing.Size(75, 45);
            this.RunTestsButton.TabIndex = 10;
            this.RunTestsButton.Text = "Run";
            this.RunTestsButton.UseVisualStyleBackColor = true;
            this.RunTestsButton.Click += new System.EventHandler(this.RunTestsButton_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(1128, 546);
            this.splitContainer2.SplitterDistance = 307;
            this.splitContainer2.TabIndex = 0;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(307, 546);
            this.tabControl2.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.TestTree);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(299, 520);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Tests";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // TestTree
            // 
            this.TestTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TestTree.Location = new System.Drawing.Point(3, 3);
            this.TestTree.Name = "TestTree";
            this.TestTree.Size = new System.Drawing.Size(293, 514);
            this.TestTree.TabIndex = 1;
            this.TestTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TestTree_AfterCheck);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(299, 520);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Categories";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(3, 3);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer4.Size = new System.Drawing.Size(293, 514);
            this.splitContainer4.SplitterDistance = 229;
            this.splitContainer4.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.IncludedCategoriesCheckList);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(293, 229);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Included Categories";
            // 
            // IncludedCategoriesCheckList
            // 
            this.IncludedCategoriesCheckList.CheckOnClick = true;
            this.IncludedCategoriesCheckList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IncludedCategoriesCheckList.FormattingEnabled = true;
            this.IncludedCategoriesCheckList.Location = new System.Drawing.Point(3, 16);
            this.IncludedCategoriesCheckList.Name = "IncludedCategoriesCheckList";
            this.IncludedCategoriesCheckList.Size = new System.Drawing.Size(287, 210);
            this.IncludedCategoriesCheckList.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ExcludedCategoriesCheckList);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(293, 281);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Excluded Categories";
            // 
            // ExcludedCategoriesCheckList
            // 
            this.ExcludedCategoriesCheckList.CheckOnClick = true;
            this.ExcludedCategoriesCheckList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExcludedCategoriesCheckList.FormattingEnabled = true;
            this.ExcludedCategoriesCheckList.Location = new System.Drawing.Point(3, 16);
            this.ExcludedCategoriesCheckList.Name = "ExcludedCategoriesCheckList";
            this.ExcludedCategoriesCheckList.Size = new System.Drawing.Size(287, 262);
            this.ExcludedCategoriesCheckList.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.SuiteCheckList);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(299, 520);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Suites";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // SuiteCheckList
            // 
            this.SuiteCheckList.CheckOnClick = true;
            this.SuiteCheckList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SuiteCheckList.FormattingEnabled = true;
            this.SuiteCheckList.Location = new System.Drawing.Point(3, 3);
            this.SuiteCheckList.Name = "SuiteCheckList";
            this.SuiteCheckList.Size = new System.Drawing.Size(293, 514);
            this.SuiteCheckList.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(817, 546);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.OutputRichText);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(809, 520);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Output";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // OutputRichText
            // 
            this.OutputRichText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputRichText.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputRichText.Location = new System.Drawing.Point(3, 3);
            this.OutputRichText.Name = "OutputRichText";
            this.OutputRichText.ReadOnly = true;
            this.OutputRichText.Size = new System.Drawing.Size(803, 514);
            this.OutputRichText.TabIndex = 0;
            this.OutputRichText.Text = "";
            this.OutputRichText.WordWrap = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(809, 520);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Failed Tests";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.FailedTestListBox);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.FailedTestDetails);
            this.splitContainer3.Size = new System.Drawing.Size(803, 514);
            this.splitContainer3.SplitterDistance = 168;
            this.splitContainer3.TabIndex = 0;
            // 
            // FailedTestListBox
            // 
            this.FailedTestListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FailedTestListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.FailedTestListBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FailedTestListBox.FormattingEnabled = true;
            this.FailedTestListBox.Location = new System.Drawing.Point(0, 0);
            this.FailedTestListBox.Name = "FailedTestListBox";
            this.FailedTestListBox.Size = new System.Drawing.Size(803, 168);
            this.FailedTestListBox.TabIndex = 0;
            this.FailedTestListBox.SelectedIndexChanged += new System.EventHandler(this.FailedTestListBox_SelectedIndexChanged);
            // 
            // FailedTestDetails
            // 
            this.FailedTestDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FailedTestDetails.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FailedTestDetails.Location = new System.Drawing.Point(0, 0);
            this.FailedTestDetails.Name = "FailedTestDetails";
            this.FailedTestDetails.ReadOnly = true;
            this.FailedTestDetails.Size = new System.Drawing.Size(803, 342);
            this.FailedTestDetails.TabIndex = 0;
            this.FailedTestDetails.Text = "";
            this.FailedTestDetails.WordWrap = false;
            // 
            // TestTreeContextMenu
            // 
            this.TestTreeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandToolStripMenuItem,
            this.collapseToolStripMenuItem,
            this.toolStripSeparator3,
            this.checkboxesToolStripMenuItem,
            this.uncheckAllToolStripMenuItem});
            this.TestTreeContextMenu.Name = "TestTreeContextMenu";
            this.TestTreeContextMenu.Size = new System.Drawing.Size(145, 98);
            // 
            // expandToolStripMenuItem
            // 
            this.expandToolStripMenuItem.Name = "expandToolStripMenuItem";
            this.expandToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.expandToolStripMenuItem.Text = "Expand";
            this.expandToolStripMenuItem.Click += new System.EventHandler(this.expandToolStripMenuItem_Click);
            // 
            // collapseToolStripMenuItem
            // 
            this.collapseToolStripMenuItem.Name = "collapseToolStripMenuItem";
            this.collapseToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.collapseToolStripMenuItem.Text = "Collapse";
            this.collapseToolStripMenuItem.Click += new System.EventHandler(this.collapseToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
            // 
            // checkboxesToolStripMenuItem
            // 
            this.checkboxesToolStripMenuItem.Name = "checkboxesToolStripMenuItem";
            this.checkboxesToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.checkboxesToolStripMenuItem.Text = "Checkboxes";
            this.checkboxesToolStripMenuItem.Click += new System.EventHandler(this.checkboxesToolStripMenuItem_Click);
            // 
            // uncheckAllToolStripMenuItem
            // 
            this.uncheckAllToolStripMenuItem.Name = "uncheckAllToolStripMenuItem";
            this.uncheckAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.uncheckAllToolStripMenuItem.Text = "Uncheck All";
            this.uncheckAllToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllToolStripMenuItem_Click);
            // 
            // RecentlyOpenedMenuItem
            // 
            this.RecentlyOpenedMenuItem.Name = "RecentlyOpenedMenuItem";
            this.RecentlyOpenedMenuItem.Size = new System.Drawing.Size(169, 22);
            this.RecentlyOpenedMenuItem.Text = "Recently Opened";
            // 
            // EdisonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 634);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "EdisonForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edison";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TestThreadNumericBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FixtureThreadNumericBox)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.TestTreeContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView TestTree;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox OutputRichText;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.RichTextBox FailedTestDetails;
        private System.Windows.Forms.ListBox FailedTestListBox;
        private System.Windows.Forms.ContextMenuStrip TestTreeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem expandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkboxesToolStripMenuItem;
        private System.Windows.Forms.Button RunTestsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllToolStripMenuItem;
        private System.Windows.Forms.ProgressBar TestProgressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown FixtureThreadNumericBox;
        private System.Windows.Forms.CheckBox DisableTestCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox DisableConsoleCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown TestThreadNumericBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox IncludedCategoriesCheckList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox ExcludedCategoriesCheckList;
        private System.Windows.Forms.CheckedListBox SuiteCheckList;
        private System.Windows.Forms.ToolStripMenuItem RecentlyOpenedMenuItem;
    }
}

