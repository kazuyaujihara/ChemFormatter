﻿// MIT License
// 
// Copyright (c) 2018 Kazuya Ujihara
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.Office.Core;
using Parago.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using static ChemFormatter.DocumentProperties;
using static Microsoft.Office.Interop.Word.WdSelectionType;
using Word = Microsoft.Office.Interop.Word;

namespace ChemFormatter.WordAddIn
{
    public partial class ThisAddIn
    {
        protected override IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            // This is taken from the following.
            // https://stackoverflow.com/questions/8523812/localize-vsto-addin-according-to-the-language-of-the-office-product
            var app = this.GetHostItem<Word.Application>(typeof(Word.Application), "Application");
            var lcid = app.LanguageSettings.LanguageID[MsoAppLanguageID.msoLanguageIDUI];
            CommonResourceManager.Culture = new CultureInfo(lcid);

            return base.CreateRibbonExtensibilityObject();
        }

        private NMRFormat currentNMRFormat;

        public NMRFormat CurrentNMRFormat
        {
            get
            {
                if (currentNMRFormat == null)
                {
                    OnDocumentOpen(Globals.ThisAddIn.Application.ActiveDocument);
                }
                return currentNMRFormat;
            }

            set
            {
                currentNMRFormat = value;
            }
        }

        private static string GetNMRFormatProperty(Word.Document doc)
        {
            string prop = null;
            try
            {
                prop = doc.CustomDocumentProperties.Item(NMRFormatKey).Value;
            }
            catch (Exception)
            {
            }

            return prop;
        }

        private void SetNMRFormat(string format)
        {
            try
            {
                var ribbon = Globals.Ribbons.GetRibbon<Ribbon>();

                Microsoft.Office.Tools.Ribbon.RibbonDropDownItem find(string label)
                    => ribbon.dropDownNMRFormat.Items.Where(n => n.Label == format).FirstOrDefault();

                var item = find(format);
                if (item == null)
                {
                    format = NMRFormat.Default.Label;
                    item = find(format);
                }
                if (item == null)
                    Trace.TraceError($"Default {nameof(NMRFormat)} should be contained in {ribbon.dropDownNMRFormat}.");

                Globals.Ribbons.GetRibbon<Ribbon>().dropDownNMRFormat.SelectedItem = item;
            }
            catch (Exception)
            {
            }

            CurrentNMRFormat = NMRFormat.FromLabel(format);
        }

        private void OnDocumentOpen(Word.Document doc)
        {
            SetNMRFormat(GetNMRFormatProperty(doc));
        }

        private void OnDocumentBeforeSave(Word.Document doc, ref bool saveAsUI, ref bool cancel)
        {
            var prop = GetNMRFormatProperty(doc);

            if (prop == null && CurrentNMRFormat == NMRFormat.Default)
            {
                // do not save
            }
            else
            {
                try
                {
                    doc.CustomDocumentProperties.Item(NMRFormatKey).Delete();
                }
                catch (Exception)
                {
                }
                try
                {
                    doc.CustomDocumentProperties.Add(NMRFormatKey, false, Microsoft.Office.Core.MsoDocProperties.msoPropertyTypeString, CurrentNMRFormat.Label, missing);
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Failed to set {nameof(doc.CustomDocumentProperties)}: {e.Message}");
                }
            }
        }

        internal void Fire(Func<string, IEnumerable<PCommand>> makeCommand, bool normalize = true)
        {
            var result = ProgressDialog.Execute(null, "Formatting", () => _Fire(makeCommand, normalize), ProgressDialogSettings.WithSubLabelAndCancel);
        }

        private void _Fire(Func<string, IEnumerable<PCommand>> makeCommand, bool normalize)
        {
            var sel = Globals.ThisAddIn.Application.Selection;
            switch (sel.Type)
            {
                case wdSelectionInlineShape:
                case wdSelectionShape:
                    foreach (Word.Shape shape in sel.ChildShapeRange)
                    {
                        try
                        {
                            shape.TextFrame.TextRange.Select();
                            Fire(makeCommand);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;
                case wdSelectionColumn:
                    var cells = sel.Cells;
                    if (cells.Count == 1)
                        goto default;
                    Word.Cell firstCell = null;
                    Word.Cell lastCell = null;
                    foreach (Word.Cell cell in cells)
                    {
                        if (firstCell == null)
                            firstCell = cell;
                        lastCell = cell;

                        sel.SetRange(cell.Range.Start, cell.Range.End);
                        sel.Select();
                        Fire(makeCommand);
                    }
                    if (firstCell != null)
                        sel.SetRange(firstCell.Range.Start, lastCell.Range.End);
                    break;
                default:
                    var text = sel.Text;
                    if (text == null)
                        break;
                    // remove cell separator, which is '\a' in MS-Word
                    if (text[text.Length - 1] == '\a')
                        text = text.Substring(0, text.Length - 1);
                    if (normalize)
                        text = Utility.Normalize(text);
                    var commands = makeCommand(text);
                    WordApplyer.Apply(commands);
                    break;
            }
            ProgressDialog.Current.ReportWithCancellationCheck("");
        }

        internal void ButtonRDigitChanger_Click(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            Fire(RDigitQuery.MakeCommand);
        }

        internal void ButtonChemFormular_Click(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            Fire(ChemFormulaQuery.MakeCommand);
        }

        internal void ButtonIonFormular_Click(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            Fire(IonFormulaQuery.MakeCommand);
        }

        internal void ButtonChemName_Click(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            Fire(ChemNameQuery.MakeCommand);
        }

        internal void ButtonStyleCitation_Click(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            Fire(JournalReferenceQuery.MakeCommand);
        }

        internal void ButtonStyleAsChar_Click(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            Fire(StyleByCharQuery.MakeCommand, normalize: false);
        }

        public void ButtonNMRSpec_Click(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            var app = Globals.ThisAddIn.Application;
            var text = app.Selection.Text;
            text = Utility.Normalize(text);
            var query = new NMRSpectrumQuery() { Format = CurrentNMRFormat };
            var commands = query.MakeCommand(text);
            WordApplyer.Apply(commands);
        }

        public void ButtonAlphaD_Click(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            Fire(AlphaDQuery.MakeCommand);
        }

        internal void DropDownNMRFormat_SelectionChanged(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            var ribbon = Globals.Ribbons.GetRibbon<Ribbon>();
            var format = ribbon.dropDownNMRFormat.SelectedItem.Label;
            SetNMRFormat(format);
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Globals.ThisAddIn.Application.DocumentOpen += OnDocumentOpen;
            Globals.ThisAddIn.Application.DocumentBeforeSave += OnDocumentBeforeSave;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
