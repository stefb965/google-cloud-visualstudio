// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

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
           "Prefix", typeof(string), typeof(PrefixedTextBox),
           new PropertyMetadata((obj, e) => (obj as PrefixedTextBox)?.InitPrefixText((string)e.NewValue)));

        private TextPointer _inputStartPointer;

        public Brush PrefixForeground { get; set; }

        public int MaxLength { get; set; }

        public string Prefix
        {
            get { return (string)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }

        public PrefixedTextBox()
        {
            DataObject.AddPastingHandler(this, PrefixedTextBox_Paste);
            PreviewTextInput += PrefixedTextBox_PreviewTextInput;
            PreviewKeyDown += PrefixedTextBox_PreviewKeyDown;
            TextChanged += PrefixedTextBox_TextChanged;
        }

        private void InitPrefixText(string prefix)
        {
            Document.PageWidth = 3000;

            var range = new TextRange(Document.ContentStart, Document.ContentEnd) { Text = prefix };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, PrefixForeground);

            _inputStartPointer = GetPointerByOffset(prefix.Length - 1, Document);
            CaretPosition = _inputStartPointer;

            //Disable drag&drop of parts of the text. 
            //That prevents prefix changes by dragging.
            AllowDrop = false;

            //Clear undo stack
            IsUndoEnabled = false;
            IsUndoEnabled = true;
        }

        private string GetText()
        {
            var range = new TextRange(Document.ContentStart, Document.ContentEnd);
            return range.Text.Replace(Environment.NewLine, string.Empty);
        }

        private int GetUnprefixedTextLength()
        {
            var text = GetText();
            return text.Length - Prefix.Length;
        }

        private int GetCaretPositionWithinText()
        {
            var range = new TextRange(Document.ContentStart, CaretPosition);
            return range.Text.Length;
        }

        private static TextPointer GetPointerByOffset(int offset, FlowDocument doc)
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

        private bool IsInputAllowed(KeyEventArgs keyEvent = null)
        {
            var caretPos = GetCaretPositionWithinText();
            var selOffset = Selection.Start.GetOffsetToPosition(_inputStartPointer);

            if (keyEvent != null)
            {
                var key = keyEvent.Key;
                if (keyEvent.KeyboardDevice.IsKeyDown(Key.LeftCtrl)
                    && (key == Key.C || key == Key.A || key == Key.Z || key == Key.Y))
                {
                    return true;
                }

                switch (key)
                {
                    case Key.Back:
                        return caretPos > Prefix.Length
                            && selOffset < 0;
                    case Key.Left:
                        return true;
                    case Key.Right:
                        return true;
                }
            }

            if (selOffset > 0) return false;
            return caretPos >= Prefix.Length;
        }

        private void PrefixedTextBox_Paste(object sender, DataObjectPastingEventArgs e)
        {
            var txt = e.DataObject.GetData(typeof(string)) as string;
            if (string.IsNullOrWhiteSpace(txt)) return;

            var unprefixedTextLength = GetUnprefixedTextLength();
            if (MaxLength > 0 && unprefixedTextLength + txt.Length > MaxLength)
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
            if (!IsInputAllowed(e)) e.Handled = true;
        }

        private void PrefixedTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsInputAllowed()) e.Handled = true;
        }

        private void PrefixedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.All(x => x.AddedLength == x.RemovedLength)) return;
            if (_inputStartPointer == null) return;

            if (GetUnprefixedTextLength() == 0) ScrollToHome();
            EnforceMaxLength();

            var range = new TextRange(_inputStartPointer, Document.ContentEnd);
            range.ApplyPropertyValue(TextElement.ForegroundProperty, Foreground);
        }

        private void EnforceMaxLength()
        {
            if (MaxLength <= 0) return;

            var unprefixedTextLength = GetUnprefixedTextLength();
            if (unprefixedTextLength <= MaxLength) return;

            var gap = 0;
            while (CaretPosition?.DeleteTextInRun(-1) == 0)
            {
                CaretPosition = CaretPosition.GetPositionAtOffset(--gap);
            }
        }
    }
}
