using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (TextBox.InvokeRequired)
            {
                TextBox.Invoke((MethodInvoker)delegate { TextBox.AppendText(value); });
            }
            else
            {
                TextBox.AppendText(value);
            }
        }

        public override void WriteLine(string value)
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

    }
}
