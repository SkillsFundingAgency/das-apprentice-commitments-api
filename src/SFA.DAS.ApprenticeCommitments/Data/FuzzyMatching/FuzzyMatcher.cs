using System;
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

            var s1 = string1.Split(new char[] { ' ', '-' });
            var s2 = string2.Split(new char[] { ' ', '-' });

            if (s1.Any(s => s2.Any(r => GetSimilarity(s, r) >= _similarityThreshold)))
            {
                return true;
            }

            if (string1.ToSoundex() == string2.ToSoundex())
            {
                return true;
            }

            return false;
        }

        public double GetSimilarity(string string1, string string2)
        {
            var l = new NormalizedLevenshtein();
            return Math.Ceiling(l.Similarity(string1.ToUpper(), string2.ToUpper()) * 100);
        }
    }
}
