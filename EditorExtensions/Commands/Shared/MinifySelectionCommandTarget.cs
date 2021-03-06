﻿using System;
using System.IO;
using EnvDTE80;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace MadsKristensen.EditorExtensions
{
    internal class MinifySelection : CommandTargetBase
    {
        private DTE2 _dte;

        public MinifySelection(IVsTextView adapter, IWpfTextView textView)
            : base(adapter, textView, GuidList.guidMinifyCmdSet, PkgCmdIDList.MinifySelection)
        {
            _dte = EditorExtensionsPackage.DTE;
        }

        protected override bool Execute(uint commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (TextView != null)
            {
                string content = TextView.Selection.SelectedSpans[0].GetText();
                string extension = Path.GetExtension(_dte.ActiveDocument.FullName).ToLowerInvariant();
                string result = MinifyFileMenu.MinifyString(extension, content);

                if (result != content)
                {
                    using (EditorExtensionsPackage.UndoContext(("Minify")))
                        TextView.TextBuffer.Replace(TextView.Selection.SelectedSpans[0].Span, result);
                }
            }

            return true;
        }

        protected override bool IsEnabled()
        {
            // Don't minify Markdown
            if (TextView.GetSelection("Markdown").HasValue)
                return false;

            if (TextView != null && TextView.Selection.SelectedSpans.Count > 0)
                return TextView.Selection.SelectedSpans[0].Length > 0;

            return false;
        }
    }
}