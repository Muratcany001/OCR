using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR.Helpers.OutputHelpers
{
    public class CharacterOptimizer
    {
        public static string ToNumeric(string text)
        {
            return text
                .Replace("O", "0")
                .Replace("I", "1")
                .Replace("S", "5")
                .Replace("B", "8")
                .Replace("Z", "2");
        }
        public static string ToAlpha(string text)
        {
            return text
                .Replace("0", "O")
                .Replace("1", "I")
                .Replace("5", "S");
        }
    }
}
