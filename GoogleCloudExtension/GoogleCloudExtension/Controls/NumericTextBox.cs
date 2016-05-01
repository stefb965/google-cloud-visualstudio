using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoogleCloudExtension.Controls
{
    public class NumericTextBox : TextBox
    {
        static NumericTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericTextBox),
                new FrameworkPropertyMetadata(typeof(NumericTextBox)));
        }

        public NumericTextBox()
        {
            DataObject.AddPastingHandler(this, NumericTextBox_Paste);
            PreviewTextInput += NumericTextBox_PreviewTextInput;
            PreviewKeyDown += NumericTextBox_PreviewKeyDown;
        }

        private void NumericTextBox_Paste(object sender, DataObjectPastingEventArgs e)
        {
            var txt = e.DataObject.GetData(typeof(string)) as string;
            if (string.IsNullOrWhiteSpace(txt)) return;

            if (!char.IsDigit(txt, txt.Length - 1))
            {
                e.CancelCommand();
            }
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
