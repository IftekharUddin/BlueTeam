﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// <para>This matcher reads in a list of words (in frequency order) and matches substrings of the password against that dictionary.</para>
    ///
    /// <para>The dictionary to be used can be specified directly by passing an enumerable of strings through the constructor (e.g. for
    /// matching agains user inputs). Most dictionaries will be in word list files.</para>
    ///
    /// <para>Using external files is a departure from the JS version of Zxcvbn which bakes in the word lists, so the default dictionaries
    /// have been included in the Zxcvbn assembly as embedded resources (to remove the external dependency). Thus when a word list is specified
    /// by name, it is first checked to see if it matches and embedded resource and if not is assumed to be an external file. </para>
    ///
    /// <para>Thus custom dictionaries can be included by providing the name of an external text file, but the built-in dictionaries (english.lst,
    /// female_names.lst, male_names.lst, passwords.lst, us_tv_and_film.lst, surnames.lst) can be used without concern about locating a dictionary file in an accessible
    /// place.</para>
    ///
    /// <para>Dictionary word lists must be in decreasing frequency order and contain one word per line with no additional information.</para>
    /// </summary>
    public class DictionaryMatcher : IMatcher
    {
        private const string DictionaryPattern = "dictionary";

        private readonly string _dictionaryName;
        private readonly Dictionary<string, int> _rankedDictionary;

        /// <summary>
        /// Creates a new dictionary matcher. <paramref name="wordListPath"/> must be the path (relative or absolute) to a file containing one word per line,
        /// entirely in lowercase, ordered by frequency (decreasing); or <paramref name="wordListPath"/> must be the name of a built-in dictionary.
        /// </summary>
        /// <param name="name">The name provided to the dictionary used</param>
        /// <param name="wordListPath">The filename of the dictionary (full or relative path) or name of built-in dictionary</param>
        public DictionaryMatcher(string name, string wordListPath)
        {
            _dictionaryName = name;
            _rankedDictionary = BuildRankedDictionary(wordListPath);
        }

        /// <summary>
        /// Creates a new dictionary matcher from the passed in word list. If there is any frequency order then they should be in
        /// decreasing frequency order.
        /// </summary>
        public DictionaryMatcher(string name, IEnumerable<string> wordList)
        {
            _dictionaryName = name;

            // Must ensure that the dictionary is using lowercase words only
            _rankedDictionary = BuildRankedDictionary(wordList.Select(w => w.ToLower()));
        }

        private IEnumerable<DictionaryMatch> query(string password)
        {
            int len = password.Length;
            foreach (int i in EnumerableUtility.Range(0, len))
            {
                foreach (int j in EnumerableUtility.Range(i, len))
                {
                    string psub = password.ToLower().Substring(i, j - i + 1);
                    if (_rankedDictionary.ContainsKey(psub))
                    {
                        yield return new DictionaryMatch
                        {
                            // DictionaryPattern = DictionaryPattern,
                            i = i,
                            j = j,
                            Token = password.Substring(i, j - i + 1), // could have diff case so pull from password
                            MatchedWord = psub,
                            Rank = _rankedDictionary[psub],
                            DictionaryName = _dictionaryName,
                            Cardinality = _rankedDictionary.Count,
                            Reversed = false
                        };
                    }
                }
            }
        }

        private IEnumerable<DictionaryMatch> reverseMatch(string password)
        {
            string reversedPassword = Utility.StringReverse(password.ToLower());
            List<DictionaryMatch> matches = query(reversedPassword).ToList();
            foreach (DictionaryMatch m in matches)
            {
                m.Token = Utility.StringReverse(m.Token);

                int i = m.i;
                int j = m.j;
                m.i = password.Length - 1 - j;
                m.j = password.Length - 1 - i;
                m.Reversed = true;
            }
            return matches;
        }

        /// <inheritdoc />
        /// <summary>
        /// Match substrings of password agains the loaded dictionary
        /// </summary>
        /// <param name="password">The password to match</param>
        /// <returns>An enumerable of dictionary matches</returns>
        /// <seealso cref="T:Zxcvbn.Matcher.DictionaryMatch" />
        public IEnumerable<Match> MatchPassword(string password)
        {
            var passwordLower = password.ToLower();

            List<DictionaryMatch> matches = this.query(password).ToList();
            matches = matches.Concat(this.reverseMatch(password)).ToList();

            foreach (var match in matches) CalculateEntropyForMatch(match);

            return matches;
        }

        private static Dictionary<string, int> BuildRankedDictionary(string wordListFile)
        {
            string baseFile = wordListFile.Split('.')[0];

            var lines = Resources.Load<TextAsset>(baseFile).text.Split('\n');

            return BuildRankedDictionary(lines);
        }

        private static Dictionary<string, int> BuildRankedDictionary(IEnumerable<string> wordList)
        {
            var dict = new Dictionary<string, int>();

            var i = 1;
            foreach (var word in wordList)
            {
                // The word list is assumed to be in increasing frequency order
                dict[word] = i++;
            }

            return dict;
        }

        private static void CalculateEntropyForMatch(DictionaryMatch match)
        {
            match.BaseEntropy = Math.Log(match.Rank, 2);
            match.UppercaseEntropy = PasswordScoring.CalculateUppercaseEntropy(match.Token);

            match.Entropy = match.BaseEntropy + match.UppercaseEntropy;
        }
    }
}