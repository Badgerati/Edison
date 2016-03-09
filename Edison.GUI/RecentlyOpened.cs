/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Edison.GUI
{
    public static class RecentlyOpened
    {

        #region Repositories

        private static IFileRepository FileRepository
        {
            get { return DIContainer.Instance.Get<IFileRepository>(); }
        }

        private static IPathRepository PathRepository
        {
            get { return DIContainer.Instance.Get<IPathRepository>(); }
        }

        #endregion


        private static string RecentFile
        {
            get { return PathRepository.Combine(Environment.CurrentDirectory, "RecentlyOpened.txt"); }
        }

        private const int MaxEntries = 10;


        public static IList<string> Add(string path)
        {
            var entries = Load();

            if (string.IsNullOrWhiteSpace(path) || entries.Contains(path))
            {
                return entries;
            }

            entries.Insert(0, path);
            return Save(entries);
        }

        public static IList<string> Save(IList<string> values)
        {
            if (values == default(IList<string>))
            {
                return Load();
            }
            
            try
            {
                FileRepository.WriteAllLines(RecentFile, values.ToArray(), Encoding.UTF8);
            }
            catch { }

            return values;
        }

        public static IList<string> Load()
        {
            var entries = new List<string>();

            try
            {
                entries = FileRepository.ReadAllLines(RecentFile, Encoding.UTF8).Take(MaxEntries).ToList();
            }
            catch { }

            return entries;
        }

        public static void Clear()
        {
            var result = MessageBox.Show("Are you sure you wish to clear all Recently Opened entries?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Save(new List<string>());
            }
        }

        public static void Populate(ToolStripMenuItem menu)
        {
            if (menu == default(ToolStripMenuItem))
            {
                return;
            }

            menu.DropDownItems.Clear();
            menu.DropDownItems.AddRange(Load().Select(x => new ToolStripMenuItem(x)).ToArray());
            menu.DropDownItems.Add(new ToolStripSeparator());
            menu.DropDownItems.Add(new ToolStripMenuItem("Clear Entries"));
        }

    }
}
