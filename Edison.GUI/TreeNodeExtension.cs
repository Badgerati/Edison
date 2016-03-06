/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Edison.GUI
{
    public static class TreeNodeExtension
    {

        public static TreeNodeType GetNodeType(this TreeNode node)
        {
            if (node.Nodes.Count == 0)
            {
                return TreeNodeType.Test;
            }

            if (node.Nodes[0].Nodes.Count == 0)
            {
                return TreeNodeType.Fixture;
            }

            return TreeNodeType.Group;
        }

        public static bool IsTest(this TreeNode node)
        {
            return node.GetNodeType() == TreeNodeType.Test;
        }

        public static bool IsFixture(this TreeNode node)
        {
            return node.GetNodeType() == TreeNodeType.Fixture;
        }

        public static bool IsGroup(this TreeNode node)
        {
            return node.GetNodeType() == TreeNodeType.Group;
        }

        public static Tuple<List<string>, List<string>> GetFullPaths(this TreeNode node, string fileName, char separator)
        {
            var tests = new List<string>();
            var fixtures = new List<string>();

            switch (node.GetNodeType())
            {
                case TreeNodeType.Test:
                    tests.Add(node.GetFullPath(fileName, separator));
                    break;

                case TreeNodeType.Fixture:
                    fixtures.Add(node.GetFullPath(fileName, separator));
                    break;

                case TreeNodeType.Group:
                    foreach (TreeNode innerNode in node.Nodes)
                    {
                        var innerTests = innerNode.GetFullPaths(fileName, separator);
                        fixtures.AddRange(innerTests.Item2);
                    }
                    break;
            }

            return Tuple.Create(tests, fixtures);
        }

        public static string GetFullPath(this TreeNode node, string fileName, char separator)
        {
            return node.FullPath.Replace(fileName + separator, string.Empty);
        }

    }
}
