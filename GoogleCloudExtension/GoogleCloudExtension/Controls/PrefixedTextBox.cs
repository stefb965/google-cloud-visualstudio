// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GoogleCloudExtension.Controls
{
    internal class PrefixedTextBox : RichTextBox
    {
        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(
           "Prefix", typeof(string), typeof(PrefixedTextBox),
           new PropertyMetadata((obj, e) => (obj as PrefixedTextBox)?.PrefixTextUpdate((string)e.NewValue)));

        public static readonly DependencyProperty UnprefixedTextProperty = DependencyProperty.Register(
            "UnprefixedText", typeof(string), typeof(PrefixedTextBox),
            new PropertyMetadata((obj, e) => (obj as PrefixedTextBox)?.UnprefixedTextUpdate((string)e.NewValue)));

        public static readonly DependencyProperty PrefixForegroundProperty = DependencyProperty.Register(
           "PrefixForeground", typeof(Brush), typeof(PrefixedTextBox),
           new PropertyMetadata(new SolidColorBrush(Colors.White)));

        private TextPointer _inputStartPointer;

        public int MaxLength { get; set; }

        public string Prefix
        {
            get { return (string)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }

        public string UnprefixedText
        {
            get { return (string)GetValue(UnprefixedTextProperty); }
            set { SetValue(UnprefixedTextProperty, value); }
        }

        public Brush PrefixForeground
        {
            get { return (Brush)GetValue(PrefixForegroundProperty); }
            set { SetValue(PrefixForegroundProperty, value); }
        }

        static PrefixedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PrefixedTextBox),
                new FrameworkPropertyMetadata(typeof(PrefixedTextBox)));
        }

        public PrefixedTextBox()
        {
            DataObject.AddPastingHandler(this, PrefixedTextBox_Paste);
            PreviewTextInput += PrefixedTextBox_PreviewTextInput;
            PreviewKeyDown += PrefixedTextBox_PreviewKeyDown;
            TextChanged += PrefixedTextBox_TextChanged;

            AcceptsReturn = false;

            //Disable drag&drop of parts of the text. 
            //That prevents prefix changes by dragging.
            AllowDrop = false;

            //Set caret position to the end of document. 
            //So user will be able to start text imput right after prefix.
            CaretPosition = Document.ContentEnd;
        }

        private void PrefixTextUpdate(string prefix)
        {
            Document.PageWidth = 3000;

            var range = new TextRange(Document.ContentStart, Document.ContentEnd) { Text = prefix };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, PrefixForeground);

            //Clear undo stack
            IsUndoEnabled = false;
            IsUndoEnabled = true;

            _inputStartPointer = GetPointerByOffset(prefix.Length - 1, Document);
            SetUnprefixedText(UnprefixedText);
        }

        private void UnprefixedTextUpdate(string text)
        {
            if (text == GetText(true)) return;

            SetUnprefixedText(text);
        }

        private string GetText(bool isUnprefixed = false)
        {
            var range = new TextRange(Document.ContentStart, Document.ContentEnd);
            var text = range.Text.Replace(Environment.NewLine, string.Empty);

            if (!isUnprefixed) return text;
            var prefLength = Prefix?.Length ?? 0;
            return text.Substring(prefLength, text.Length - prefLength);
        }

        private void SetUnprefixedText(string text)
        {
            if (_inputStartPointer == null) return;

            var range = new TextRange(_inputStartPointer, Document.ContentEnd) { Text = text ?? string.Empty };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, Foreground);
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

            if (!IsInputAllowed())
            {
                e.CancelCommand();
                return;
            }

            var unprefixedTextLength = GetText(true).Length;
            if (MaxLength > 0 && (unprefixedTextLength + txt.Length > MaxLength))
            {
                txt = txt.Substring(0, MaxLength - unprefixedTextLength);
                var dataObj = new DataObject();
                dataObj.SetData(DataFormats.Text, txt);
                e.DataObject = dataObj;
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

            var unprefixedText = GetText(true);
            UnprefixedText = unprefixedText;

            if (unprefixedText.Length == 0) ScrollToHome();
            EnforceMaxLength(unprefixedText.Length);
            SetUnprefixedTextForeground();
        }

        private void SetUnprefixedTextForeground()
        {
            var range = new TextRange(_inputStartPointer, Document.ContentEnd);
            range.ApplyPropertyValue(TextElement.ForegroundProperty, Foreground);
        }

        private void EnforceMaxLength(int unprefixedTextLength)
        {
            if (MaxLength <= 0) return;
            if (unprefixedTextLength <= MaxLength) return;

            var gap = 0;
            while (CaretPosition?.DeleteTextInRun(-1) == 0)
            {
                CaretPosition = CaretPosition.GetPositionAtOffset(--gap);
            }
        }
    }
}
