/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Edison.GUI
{
    public class OutputLogger : TextWriter
    {

        private RichTextBox TextBox;


        public OutputLogger(RichTextBox textBox)
        {
            TextBox = textBox;
        }


        public override Encoding Encoding
        {
            get
            {
                return null;
            }
        }

        public override void Write(string value)
        {
            if (TextBox.IsDisposed)
            {
                return;
            }

            try
            {
                if (TextBox.InvokeRequired)
                {
                    TextBox.Invoke((MethodInvoker)delegate { TextBox.AppendText(value); });
                }
                else
                {
                    TextBox.AppendText(value);
                }
            }
            catch (ObjectDisposedException) { }
        }

        public override void WriteLine(string value)
        {
            if (TextBox.IsDisposed)
            {
                return;
            }

            try
            {
                if (TextBox.InvokeRequired)
                {
                    TextBox.Invoke((MethodInvoker)delegate { TextBox.AppendText(Environment.NewLine + value); });
                }
                else
                {
                    TextBox.AppendText(Environment.NewLine + value);
                }
            }
            catch (ObjectDisposedException) { }
        }

    }
}
