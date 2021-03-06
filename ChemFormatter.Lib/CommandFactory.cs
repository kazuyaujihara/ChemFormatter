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

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChemFormatter
{
    public static class CommandFactory
    {
        static Regex ReRDigit = new Regex(@"[A-Za-z](?<subdigit>\d+(\-\d+)?)", RegexOptions.Compiled);
        static Regex ReSubscript { get; } = new Regex(@"[\)\}\]A-Za-z](?<subdigit>\d+(\-\d+)?)", RegexOptions.Compiled);
        static Regex ReIon { get; } = new Regex(@"[\)\}\]A-Za-z0-9](?<superscript>[2-7]?[\+\-])", RegexOptions.Compiled);
        static Regex ReTripleBond { get; } = new Regex(@"\#", RegexOptions.Compiled);
        const string DefaultItalicPrefixes = "syn|anti|meso|racemi|cis|trans|rel|l|d|dl|i|endo|exo|sec|tert|n|s|t|o|m|p|vic|gem|cisoid|transoid|r|t|c|ent|gache|erythro|threo";
        static Regex ReItalicPrefix = new Regex(@"\b(?<prefix>" + DefaultItalicPrefixes + @")\-", RegexOptions.Compiled);
        const string StereoPrefixes = "E|Z|EZ|ZE|R|S|RS|SR";
        static Regex ReStereoPrefix = new Regex(@"\b\d*(?<prefix>" + StereoPrefixes + @")\b", RegexOptions.Compiled);
        const string SmallPrefix = "D|L|DL";
        static Regex ReSmallPrefix = new Regex(@"\b(?<prefix>" + SmallPrefix + @")\-", RegexOptions.Compiled);
        const string Elements = "H|He|Li|Be|B|C|N|O|F|Ne|Na|Mg|Al|Si|P|S|Cl|Ar|K|Ca|Sc|Ti|V|Cr|Mn|Fe|Co|Ni|Cu|Zn|Ga|Ge|As|Se|Br|Kr|Rb|Sr|Y|Zr|Nb|Mo|Tc|Ru|Rh|Pd|Ag|Cd|In|Sn|Sb|Te|I|Xe|Cs|Ba|La|Ce|Pr|Nd|Pm|Sm|Eu|Gd|Tb|Dy|Ho|Er|Tm|Yb|Lu|Hf|Ta|W|Re|Os|Ir|Pt|Au|Hg|Tl|Pb|Bi|Po|At|Rn|Fr|Ra|Ac|Th|Pa|U|Np|Pu|Am|Cm|Bk|Cf|Es|Fm|Md|No|Lr";
        static Regex ReElementsPrefix = new Regex(@"\b\d*(?<element>" + Elements + @")\b", RegexOptions.Compiled);

        public static void AddChemFormulaPrefixCommands(List<PCommand> commands, string text)
        {
            foreach (Match match in ReItalicPrefix.Matches(text))
            {
                var g = match.Groups["prefix"];
                commands.Add(new ItalicCommand(g.Index, g.Length));
            }
            foreach (Match match in ReStereoPrefix.Matches(text))
            {
                var g = match.Groups["prefix"];
                commands.Add(new ItalicCommand(g.Index, g.Length));
            }
            foreach (Match match in ReSmallPrefix.Matches(text))
            {
                var g = match.Groups["prefix"];
                commands.Add(new SmallCapitalCommand(g.Index, g.Length));
            }
        }

        public static void AddChemNamePrefixCommands(List<PCommand> commands, string text)
        {
            AddChemFormulaPrefixCommands(commands, text);
            foreach (Match match in ReElementsPrefix.Matches(text))
            {
                var g = match.Groups["element"];
                commands.Add(new ItalicCommand(g.Index, g.Length));
            }
        }

        public static void AddRDigitCommands(List<PCommand> commands, string text)
        {
            foreach (Match match in ReRDigit.Matches(text))
            {
                var g = match.Groups["subdigit"];
                commands.Add(new ChangeScriptCommand(g.Index, g.Length));
            }
        }

        public static void AddSubscriptCommands(List<PCommand> commands, string text)
        {
            foreach (Match match in ReSubscript.Matches(text))
            {
                var g = match.Groups["subdigit"];
                commands.Add(new SubscriptCommand(g.Index, g.Length));
            }
        }

        public static void AddIonCommands(List<PCommand> commands, string text)
        {
            foreach (Match match in ReIon.Matches(text))
            {
                var g = match.Groups["superscript"];
                commands.Add(new SuperscriptCommand(g.Index, g.Length));
            }
        }

        public static void AddTripleBondCommands(List<PCommand> commands, string text)
        {
            foreach (Match match in ReTripleBond.Matches(text))
            {
                commands.Add(new ReplaceStringCommand(match.Index, match.Length, "≡"));
            }
        }
    }
}
