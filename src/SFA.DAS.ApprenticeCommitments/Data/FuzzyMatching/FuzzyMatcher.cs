using System;
using System.Collections.Generic;
using System.Linq;
using F23.StringSimilarity;
using NinjaNye.SearchExtensions.Soundex;

namespace SFA.DAS.ApprenticeCommitments.Data.FuzzyMatching
{
    public class FuzzyMatcher
    {
        public int _similarityThreshold { get; set; } = 0;

        public FuzzyMatcher(int similarityThreshold)
            => _similarityThreshold = similarityThreshold;

        public bool IsSimilar(string string1, string string2)
        {
            if (string1 == string2)
            {
                return true;
            }

            string1 = string1.Trim();
            string2 = string2.Trim();

            if (string1.Contains(" ") || string2.Contains(" ") || string1.Contains("-") || string2.Contains("-"))
            {
                foreach (var separator in new List<char>() { ' ', '-' })
                {
                    // lets first remove the char and do a match, so "harry" should match "ha r r y"   
                    if (GetSimilarity(string1.Replace(separator.ToString(), ""), string2.Replace(separator.ToString(), "")) >= _similarityThreshold)
                    {
                        return true;
                    }
                }

                if (CheckSeperators(string1, string2))
                {
                    return true;
                }
            }

            if (GetSimilarity(string1, string2) >= _similarityThreshold)
            {
                return true;
            }

            if (string1.ToSoundex() == string2.ToSoundex())
            {
                return true;
            }

            return false;
        }

        private bool CheckSeperators(string string1, string string2)
        {
            var match = false;

            new List<Check>() { new Check(' ', ' '), new Check('-', '-'), new Check(' ', '-'), new Check('-', ' ') }.ForEach(x =>
            {
                if (CheckSeperatorCharacters(string1, string2, x.char1, x.char2))
                {
                    match = true;
                    return;
                }
            });

            return match;
        }

        private bool CheckSeperatorCharacters(string string1, string string2, char separator1, char separator2)
        {
            var match = false;

            // "harry jane" with match "harry" or "jane", or "harry-jane-sally" will match "micheal jane chris"
            string1.Split(separator1).ToList().ForEach(s1 =>
            {
                string2.Split(separator2).ToList().ForEach(s2 =>
                {
                    if (GetSimilarity(s1, s2) >= _similarityThreshold)
                    {
                        match = true;
                        return;
                    }
                });
            });

            return match;
        }

        public double GetSimilarity(string string1, string string2)
        {
            var l = new NormalizedLevenshtein();
            return Math.Ceiling(l.Similarity(string1.ToUpper(), string2.ToUpper()) * 100);
        }

        private class Check
        {
            public char char1 { get; set; }
            public char char2 { get; set; }

            public Check(char ichar1, char ichar2)
            {
                char1 = ichar1;
                char2 = ichar2;
            }
        }
    }
}
