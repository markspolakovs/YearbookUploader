using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace YearbookUploader
{
    internal class TextBoxWriter
    {
        private TextBox _box;
        internal TextBoxWriter(TextBox box)
        {
            _box = box;
        }

        public void WriteLine(string value)
        {
            Debug.Print(value);
            _box.Dispatcher.Invoke(() => { _box.Text = value + Environment.NewLine + _box.Text; });
        }

        public async Task WriteLineAsync(string value)
        {
            Debug.Print(value);
            await _box.Dispatcher.InvokeAsync(() => { _box.Text = value + Environment.NewLine + _box.Text; });
        }
    }
}
