﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChemFormatter
{
    public static class RDigitQuery
    {
        
        public static List<PCommand> MakeCommand(string text)
        {
            var commands = new List<PCommand>();

            CommandFactory.AddRDigitCommands(commands, text);

            return commands;
        }
    }
}
