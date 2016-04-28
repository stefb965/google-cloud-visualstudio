using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Windows
{
    internal class PrefixedTextBox : RichTextBox
    {
        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(
           "Prefix", typeof(string), typeof(PrefixedTextBox), new PropertyMetadata(OnPrefixPropertyChanged));

        public TextPointer InputStartPointer { get; set; }

        public Brush PrefixForeground { get; set; }

        public int MaxLength { get; set; }

        public string Prefix
        {
            get { return (string)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }

        public string Text
        {
            get
            {
                var range = new TextRange(Document.ContentStart, Document.ContentEnd);
                var text = range.Text;

                return text;
            }
        }

        public int TextCaretPosition
        {
            get
            {
                var range = new TextRange(Document.ContentStart, CaretPosition);
                var pos = range.Text.Length;

                return pos;
            }
        }

        public int UserTextLengtn
        {
            get
            {
                var currText = Text.Replace(Environment.NewLine, string.Empty);
                var len = currText.Length - Prefix.Length;

                return len;
            }
        }

        public PrefixedTextBox()
        {
            DataObject.AddPastingHandler(this, PrefixedTextBox_Paste);
            PreviewTextInput += PrefixedTextBox_PreviewTextInput;
            PreviewKeyDown += PrefixedTextBox_PreviewKeyDown;
            TextChanged += PrefixedTextBox_TextChanged;
        }

        private static void OnPrefixPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var inst = obj as PrefixedTextBox;
            inst?.InitPrefixText((string)e.NewValue);
        }

        private void InitPrefixText(string prefix)
        {
            Document.PageWidth = 1500;

            var range = new TextRange(Document.ContentStart, Document.ContentEnd) { Text = prefix };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, PrefixForeground);

            InputStartPointer = GetPointerByOffset(prefix.Length - 1, Document);
            CaretPosition = InputStartPointer;
        }

        private bool IsInputAllowed(Key? key = null)
        {
            var textCaretPosition = TextCaretPosition;

            if (key.HasValue)
            {
                switch (key.Value)
                {
                    case Key.Back:
                        return textCaretPosition > Prefix.Length
                            && Selection.Start.GetOffsetToPosition(InputStartPointer) < 0;
                    case Key.Left:
                        return true;
                    case Key.Right:
                        return true;
                }
            }

            if (MaxLength > 0 && UserTextLengtn >= MaxLength)
            {
                return false;
            }

            return textCaretPosition >= Prefix.Length;
        }

        public static TextPointer GetPointerByOffset(int offset, FlowDocument doc)
        {
            if (offset == 0) return doc.ContentStart;

            var next = doc.ContentStart;
            var counter = 0;

            while (next != null && counter <= offset)
            {
                if (next.CompareTo(doc.ContentEnd) == 0)
                {
                    return next;
                }

                if (next.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    next = next.GetNextInsertionPosition(LogicalDirection.Forward);
                    counter++;
                }
                else
                {
                    next = next.GetNextInsertionPosition(LogicalDirection.Forward);
                }
            }

            return next;
        }

        private void PrefixedTextBox_Paste(object sender, DataObjectPastingEventArgs e)
        {
            var txt = e.DataObject.GetData(typeof(string)) as string;
            if (string.IsNullOrWhiteSpace(txt))
            {
                e.CancelCommand();
                return;
            }


            if (MaxLength > 0 && UserTextLengtn + txt.Length > MaxLength)
            {
                e.CancelCommand();
                return;
            }

            if (!IsInputAllowed())
            {
                e.CancelCommand();
            }
        }

        private void PrefixedTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (!IsInputAllowed(e.Key)) e.Handled = true;
        }

        private void PrefixedTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsInputAllowed()) e.Handled = true;
        }

        private void PrefixedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.All(x => x.AddedLength == x.RemovedLength)) return;
            if (InputStartPointer == null) return;

            if (UserTextLengtn == 0)
            {
                ScrollToHome();
            }

            var range = new TextRange(InputStartPointer, Document.ContentEnd);
            range.ApplyPropertyValue(TextElement.ForegroundProperty, Foreground);
        }
    }
}
