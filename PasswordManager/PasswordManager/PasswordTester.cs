/*
 * Program:         PasswordManager.exe
 * Date:            May 20, 2019
 * Revision:        May 24, 2019 - Renamed this class from 'Password' to 'PasswordTester'
 * Course:          INFO-3138
 * Description:     Password strength analyzer loosely based on http://www.passwordmeter.com/
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    public class PasswordTester
    {
        // Private member variables and constants

        private enum CharType { None = -1, Letter = 0, Digit = 1, Symbol = 2, Upper = 3, Lower = 4 };
        private enum Trend { None = -1, Increasing = 0, Decreasing = 1 };
        private const int MIN_LENGTH = 8;
        
        // Non-default, public constructor method

        public PasswordTester(string p)
        {
            _SetStrength(p);
            Value = p;
        }

        // Public properties

        public string Value { get; private set; }

        public ushort StrengthPercent { get; private set; }

        public string StrengthLabel { get; private set; }

        
        // Private helper methods

        public void _SetStrength(string pwd)
        {
            /*
             * Purpose:     Calculates the 'strength' of a given password where 0% is the weakest
             *              and 100% is the strongest.
             * Accepts:     The password for analysis as a string
             * Returns:     An int which is the strength as a percentage
             */

            if (pwd.Any(x => Char.IsWhiteSpace(x)))
                throw new ArgumentException("ERROR: Passwords may not contain whitespace characters.");

            int score = 0, midNumOrSym = 0, sequenceLength = 0, repeatChars = 0;
            int[] basicCount = { 0, 0, 0, 0, 0 };
            int[] consecutiveCount = { 0, 0, 0, 0, 0 };
            int[] sequentialCount = { 0, 0, 0, 0 };
            char lastChar = ' ';
            CharType type = CharType.None, lastType = CharType.None, sequenceType = CharType.None, lastSequenceType = CharType.None;
            Trend sequenceTrend = Trend.None, lastSequenceTrend = Trend.None;
            int uniqueCount = 0, repeatCount = 0;
            double repeatDeduct = 0;

            // Get counts required for scoring
            for (int i = 0; i < pwd.Count(); i++)
            {
                char c = pwd[i];
                if (Char.IsLetter(c))
                {
                    basicCount[(int)CharType.Letter]++;
                    if (Char.IsUpper(c))
                        type = CharType.Upper;
                    else
                        type = CharType.Lower;
                    sequenceType = CharType.Letter;
                }
                else
                {
                    if (Char.IsNumber(c))
                        type = CharType.Digit;
                    else
                        type = CharType.Symbol;
                    sequenceType = type;
                }

                basicCount[(int)type]++;
                consecutiveCount[(int)type] += (type == lastType) ? 1 : 0;
                repeatChars += (Char.ToLower(c) == Char.ToLower(lastChar)) ? 1 : 0;

                if (c - lastChar == 1)
                    sequenceTrend = Trend.Increasing;
                else if (lastChar - c == 1)
                    sequenceTrend = Trend.Decreasing;
                else
                    sequenceTrend = Trend.None;

                if (sequenceTrend != Trend.None && sequenceType == lastSequenceType && (sequenceTrend == lastSequenceTrend || lastSequenceTrend == Trend.None))
                {
                    sequenceLength++;
                    //if (sequenceLength >= 3 && sequenceLength > longestSequence[(int)sequenceType])
                    if (sequenceLength == 3)
                    {
                        sequentialCount[(int)sequenceType]++;
                        //longestSequence[(int)sequenceType] = sequenceLength;
                    }
                }
                else
                {
                    sequenceTrend = Trend.None;
                    sequenceLength = 1;
                }

                // If character is repeated, adjust repetition deduction
                bool repeated = false;
                for (int j = 0; j < pwd.Length; j++)
                {
                    if (i != j && pwd[i] == pwd[j])
                    {
                        repeated = true;
                        //if (j < i)
                        repeatDeduct += Math.Abs((double)pwd.Length / (j - i));
                    }
                }
                if (repeated)
                {
                    repeatCount++;
                    uniqueCount = pwd.Length - repeatCount;
                    repeatDeduct = (uniqueCount == 0) ? Math.Ceiling(repeatDeduct) : Math.Ceiling(repeatDeduct / (double)uniqueCount);
                }

                // Reinitialize for next character in password
                lastChar = c;
                lastType = type;
                lastSequenceType = sequenceType;
                lastSequenceTrend = sequenceTrend;
            }

            repeatDeduct = (uniqueCount == 0) ? repeatCount : (int)Math.Ceiling((double)repeatCount / uniqueCount);

            midNumOrSym = basicCount[(int)CharType.Digit] + basicCount[(int)CharType.Symbol] - (Char.IsLetter(pwd.First()) ? 0 : 1) - (Char.IsLetter(pwd.Last()) ? 0 : 1);

            // Calculate score

            // + Number of characters
            score = pwd.Length * 4;
            //Console.WriteLine("Number of Characters: " + pwd.Length * 4);

            // + Uppercase letters
            score += (pwd.Length - basicCount[(int)CharType.Upper]) * 2;
            //Console.WriteLine("Uppercase Letters: " + (pwd.Length - basicCount[(int)CharType.Upper]) * 2);

            // + Lowercase letters
            score += (pwd.Length - basicCount[(int)CharType.Lower]) * 2;
            //Console.WriteLine("Lowercase Letters: " + (pwd.Length - basicCount[(int)CharType.Lower]) * 2);

            // + Digits
            score += basicCount[(int)CharType.Digit] * 4;
            //Console.WriteLine("Lowercase Letters: " + basicCount[(int)CharType.Digit] * 4);

            // + Symbols
            score += basicCount[(int)CharType.Symbol] * 6;
            //Console.WriteLine("Lowercase Letters: " + basicCount[(int)CharType.Symbol] * 6);

            // + Middle digits or symbols
            int mid = basicCount[(int)CharType.Digit] + basicCount[(int)CharType.Symbol];
            mid -= (Char.IsLetter(pwd.First()) ? 0 : 1) + (Char.IsLetter(pwd.Last()) ? 0 : 1);
            score += mid * 2;
            //Console.WriteLine("Middle Digits or Symbols: " + mid * 2);

            // + Other requirements
            int req = 0;
            req += (pwd.Length < MIN_LENGTH) ? 0 : 1;
            req += basicCount[(int)CharType.Lower] > 0 ? 1 : 0;
            req += basicCount[(int)CharType.Upper] > 0 ? 1 : 0;
            req += basicCount[(int)CharType.Digit] > 0 ? 1 : 0;
            req += basicCount[(int)CharType.Symbol] > 0 ? 1 : 0;
            req = req < 4 ? 0 : req * 2;
            score += req;
            //Console.WriteLine("Requirements: " + req);

            // - Letters only
            score -= (pwd.Length == basicCount[(int)CharType.Letter]) ? pwd.Length : 0;
            //Console.WriteLine("Letters Only: " + ((pwd.Length == basicCount[(int)CharType.Letter]) ? -pwd.Length : 0));

            // - Digits only
            score -= (pwd.Length == basicCount[(int)CharType.Digit]) ? pwd.Length : 0;
            //Console.WriteLine("Numbers Only: " + ((pwd.Length == basicCount[(int)CharType.Digit]) ? -pwd.Length : 0));

            // - Repeat characters
            score = (int)(score - repeatDeduct);
            //Console.WriteLine("Repeat Characters: " + -(int)repeatDeduct);

            // - Consecutive uppercase letters
            score -= consecutiveCount[(int)CharType.Upper] * 2;
            //Console.WriteLine("Consecutive Uppercase Letters: " + -consecutiveCount[(int)CharType.Upper] * 2);

            // - Consecutive lowercase letters
            score -= consecutiveCount[(int)CharType.Lower] * 2;
            //Console.WriteLine("Consecutive Lowercase Letters: " + -consecutiveCount[(int)CharType.Lower] * 2);

            // - Consecutive digits
            score -= consecutiveCount[(int)CharType.Digit] * 2;
            //Console.WriteLine("Consecutive Digits: " + -consecutiveCount[(int)CharType.Digit] * 2);

            // - Sequential letters
            score -= sequentialCount[(int)CharType.Letter] * 3;
            //Console.WriteLine("Sequential Letters: " + -sequentialCount[(int)CharType.Letter] * 3);

            // - Sequential digits
            score -= sequentialCount[(int)CharType.Digit] * 3;
            //Console.WriteLine("Sequential Digits: " + -sequentialCount[(int)CharType.Digit] * 3);

            // - Sequential letters
            score -= sequentialCount[(int)CharType.Symbol] * 3;
            //Console.WriteLine("Sequential Symbols: " + -sequentialCount[(int)CharType.Symbol] * 3);

            // Prevent exceeding the expected value range
            score = Math.Min(Math.Max(0, score), 100);

            // Initialize the Password's public properties
            StrengthPercent = (ushort)score;
            if (score < 20)
                StrengthLabel = "very weak";
            else if (score < 40)
                StrengthLabel = "weak";
            else if (score < 60)
                StrengthLabel = "good";
            else if (score < 80)
                StrengthLabel = "strong";
            else 
                StrengthLabel = "very strong";

        } // end _SetStrength()

    } // end class
}
