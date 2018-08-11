﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using Microsoft.Office.Tools.Ribbon;
using System.Text.RegularExpressions;

namespace ChemFormatter.WordAddIn
{
    public static class Applyer
    {
        public static void ButtonRDigitChanger_Click(object sender, RibbonControlEventArgs e)
        {
            var text = Globals.ThisAddIn.Application.Selection.Text;
            text = Utility.Normalize(text);
            var commands = RDigitQuery.MakeCommand(text);
            if (commands == null)
                return;

            Apply(commands);
        }

        public static void ButtonChemFormular_Click(object sender, RibbonControlEventArgs e)
        {
            var text = Globals.ThisAddIn.Application.Selection.Text;
            text = Utility.Normalize(text);
            var commands = ChemFormulaQuery.MakeCommand(text);
            Apply(commands);
        }

        public static void Apply(List<PCommand> commands)
        {
            var save = KeepSelection();
            try
            {
                var app = Globals.ThisAddIn.Application;
                var start = app.Selection.Start;
                using (var saver = new AutoCorrectSaver())
                {
                    foreach (var command in commands)
                    {
                        switch (command)
                        {
                            case ReplaceStringCommand rsc:
                                SelectAndAction(start, rsc, () => 
                                {
                                    var diff = rsc.Replacement.Length - rsc.Length;
                                    save.End += diff;
                                    app.Selection.TypeText(rsc.Replacement);
                                });
                                break;
                            case SubscriptCommand sbsc:
                                SelectAndAction(start, sbsc, () => ApplyScript(ScriptMode.Subscript));
                                break;
                            case ChangeScriptCommand ssc:
                                SelectAndAction(start, ssc, () =>
                                {
                                    ScriptMode next;
                                    if (app.Selection.Font.Subscript == (int)Office.MsoTriState.msoTrue)
                                        next = ScriptMode.Superscript;
                                    else if (app.Selection.Font.Superscript == (int)Office.MsoTriState.msoTrue)
                                        next = ScriptMode.Normal;
                                    else
                                        next = ScriptMode.Subscript;

                                    ApplyScript(next);
                                });
                                break;
                        }
                    }
                }
            }
            finally
            {
                RestoreSelection(save);
            }
        }

        private static void ApplyScript(ScriptMode next)
        {
            var app = Globals.ThisAddIn.Application;
            switch (next)
            {
                case ScriptMode.Superscript:
                    app.Selection.Font.Superscript = (int)Office.MsoTriState.msoTrue;
                    break;
                case ScriptMode.Normal:
                    app.Selection.Font.Subscript = (int)Office.MsoTriState.msoFalse;
                    app.Selection.Font.Superscript = (int)Office.MsoTriState.msoFalse;
                    break;
                case ScriptMode.Subscript:
                default:
                    app.Selection.Font.Subscript = (int)Office.MsoTriState.msoTrue;
                    break;
            }
        }

        private static void SelectAndAction(int start, ApplyFormatCommand command, System.Action action)
        {
            Globals.ThisAddIn.Application.Selection.SetRange(start + command.Start, start + command.Start + command.Length);
            action();
        }

        public static SelectionKeeper KeepSelection()
        {
            var sel = Globals.ThisAddIn.Application.Selection;
            var start = sel.Start;
            var end = sel.End;
            return new SelectionKeeper(start, end);
        }

        public static void RestoreSelection(SelectionKeeper selection)
        {
            Globals.ThisAddIn.Application.Selection.SetRange(selection.Start, selection.End);
        }
    }
}