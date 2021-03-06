﻿using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.TextManager.Interop;

namespace MadsKristensen.EditorExtensions
{
    internal class EnterIndentation : CommandTargetBase
    {
        private Regex _indent = new Regex(@"^([\s]+)", RegexOptions.Compiled);

        public EnterIndentation(IVsTextView adapter, IWpfTextView textView)
            : base(adapter, textView, typeof(Microsoft.VisualStudio.VSConstants.VSStd2KCmdID).GUID, 3)
        { }

        protected override bool Execute(uint commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            int position = TextView.Caret.Position.BufferPosition.Position;
            SnapshotPoint point = new SnapshotPoint(TextView.TextBuffer.CurrentSnapshot, position);
            IWpfTextViewLine line = TextView.GetTextViewLineContainingBufferPosition(point);

            string text = TextView.TextBuffer.CurrentSnapshot.GetText(line.Start, line.Length);

            Match match = _indent.Match(text);

            if (match.Success)
            {
                using (EditorExtensionsPackage.UndoContext("Smart Indent"))
                {
                    TextView.TextBuffer.Insert(position, Environment.NewLine + match.Value);
                }

                return true;
            }

            return false;
        }

        protected override bool IsEnabled()
        {
            return true;
        }
    }
}