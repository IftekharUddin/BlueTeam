using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zxcvbn.Matcher;
using System.Text.RegularExpressions;

namespace Zxcvbn
{
    /// <summary>
    /// <para>Zxcvbn is used to estimate the strength of passwords. </para>
    ///
    /// <para>This implementation is a port of the Zxcvbn JavaScript library by Dan Wheeler:
    /// https://github.com/lowe/zxcvbn</para>
    ///
    /// <para>To quickly evaluate a password, use the <see cref="MatchPassword"/> static function.</para>
    ///
    /// <para>To evaluate a number of passwords, create an instance of this object and repeatedly call the <see cref="EvaluatePassword"/> function.
    /// Reusing the the Zxcvbn instance will ensure that pattern matchers will only be created once rather than being recreated for each password
    /// e=being evaluated.</para>
    /// </summary>
    public class Zxcvbn
    {
        private const string BruteforcePattern = "bruteforce";

        private readonly Translation translation;
        private static IMatcherFactory matcherFactory;

        /// <summary>
        /// Create a new instance of Zxcvbn that uses the default matchers.
        /// </summary>
        public Zxcvbn(Translation translation = Translation.English)
            : this(new DefaultMatcherFactory())
        {
            this.translation = translation;
        }

        /// <summary>
        /// Create an instance of Zxcvbn that will use the given matcher factory to create matchers to use
        /// to find password weakness.
        /// </summary>
        /// <param name="matcherFactory">The factory used to create the pattern matchers used</param>
        /// <param name="translation">The language in which the strings are returned</param>
        public Zxcvbn(IMatcherFactory matcherFactory, Translation translation = Translation.English)
        {
            matcherFactory = matcherFactory;
            this.translation = translation;
        }

        /// <summary>
        /// <para>A static function to match a password against the default matchers without having to create
        /// an instance of Zxcvbn yourself, with supplied user data. </para>
        ///
        /// <para>Supplied user data will be treated as another kind of dictionary matching.</para>
        /// </summary>
        /// <param name="password">the password to test</param>
        /// <param name="userInputs">optionally, the user inputs list</param>
        /// <returns>The results of the password evaluation</returns>
        public static Result MatchPassword(string password, IEnumerable<string> userInputs = null)
        {
            var zx = new Zxcvbn(new DefaultMatcherFactory());
            return zx.EvaluatePassword(password, userInputs);
        }

        /// <summary>
        /// <para>Perform the password matching on the given password and user inputs, returing the result structure with information
        /// on the lowest entropy match found.</para>
        ///
        /// <para>User data will be treated as another kind of dictionary matching, but can be different for each password being evaluated.</para>para>
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="userInputs">Optionally, an enumarable of user data</param>
        /// <returns>Result for lowest entropy match</returns>
        public Result EvaluatePassword(string password, IEnumerable<string> userInputs = null, bool taylor = true)
        {
            matcherFactory = matcherFactory ?? new DefaultMatcherFactory();
            userInputs = userInputs ?? new string[0];
            if (!taylor)
            {
                IEnumerable<Match> matches = new List<Match>();

                var timer = System.Diagnostics.Stopwatch.StartNew();

                foreach (var matcher in matcherFactory.CreateMatchers(userInputs))
                {
                    var currMatches = matcher.MatchPassword(password);
                    foreach (Match item in currMatches)
                    {
                        matches = matches.Concat(new[] { item });
                    }
                    // matches = matches.Union(matcher.MatchPassword(password));
                }

                List<Match> matchList = matches.ToList();
                matchList.Sort(delegate (Match m1, Match m2)
                {
                    if (m1.i != m2.i)
                    {
                        return m1.i - m2.i;
                    }
                    return m1.j - m2.j;
                });

                var result = FindMinimumEntropyMatch(password, matches);

                timer.Stop();
                result.CalcTime = timer.ElapsedMilliseconds;

                return result;
            }
            else
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();
                IEnumerable<Match> matches = Omnimatch(password);

                var result = MostGuessableMatchSequence(password, matches);

                timer.Stop();
                result.CalcTime = timer.ElapsedMilliseconds;

                return result;
            }
        }

        /// <summary>
        /// Returns a new result structure initialised with data for the lowest entropy result of all of the matches passed in, adding brute-force
        /// matches where there are no lesser entropy found pattern matches.
        /// </summary>
        /// <param name="matches">Password being evaluated</param>
        /// <param name="password">List of matches found against the password</param>
        /// <returns>A result object for the lowest entropy match sequence</returns>
        private Result FindMinimumEntropyMatch(string password, IEnumerable<Match> matches)
        {
            var bruteforce_cardinality = PasswordScoring.PasswordCardinality(password);

            // Minimum entropy up to position k in the password
            var minimumEntropyToIndex = new double[password.Length];
            var bestMatchForIndex = new Match[password.Length];

            for (var k = 0; k < password.Length; k++)
            {
                // Start with bruteforce scenario added to previous sequence to beat
                minimumEntropyToIndex[k] = (k == 0 ? 0 : minimumEntropyToIndex[k - 1]) + Math.Log(bruteforce_cardinality, 2);

                // All matches that end at the current character, test to see if the entropy is less
                foreach (var match in matches.Where(m => m.j == k))
                {
                    var candidate_entropy = (match.i <= 0 ? 0 : minimumEntropyToIndex[match.i - 1]) + match.Entropy;
                    if (candidate_entropy < minimumEntropyToIndex[k])
                    {
                        minimumEntropyToIndex[k] = candidate_entropy;
                        bestMatchForIndex[k] = match;
                    }
                }
            }

            // Walk backwards through lowest entropy matches, to build the best password sequence
            var matchSequence = new List<Match>();
            for (var k = password.Length - 1; k >= 0; k--)
            {
                if (bestMatchForIndex[k] != null)
                {
                    matchSequence.Add(bestMatchForIndex[k]);
                    k = bestMatchForIndex[k].i; // Jump back to start of match
                }
            }

            // The match sequence might have gaps, fill in with bruteforce matching
            // After this the matches in matchSequence must cover the whole string (i.e. match[k].j == match[k + 1].i - 1)
            if (matchSequence.Count == 0)
            {
                // To make things easy, we'll separate out the case where there are no matches so everything is bruteforced
                matchSequence.Add(new Match()
                {
                    i = 0,
                    j = password.Length,
                    Token = password,
                    Cardinality = bruteforce_cardinality,
                    Pattern = BruteforcePattern,
                    Entropy = Math.Log(Math.Pow(bruteforce_cardinality, password.Length), 2)
                });
            }
            else
            {
                // There are matches, so find the gaps and fill them in
                var matchSequenceCopy = new List<Match>();
                for (var k = 0; k < matchSequence.Count; k++)
                {
                    var m1 = matchSequence[k];
                    var m2 = (k < matchSequence.Count - 1 ? matchSequence[k + 1] : new Match() { i = password.Length }); // Next match, or a match past the end of the password

                    matchSequenceCopy.Add(m1);
                    if (m1.j < m2.i - 1)
                    {
                        // Fill in gap
                        var ns = m1.j + 1;
                        var ne = m2.i - 1;
                        matchSequenceCopy.Add(new Match()
                        {
                            i = ns,
                            j = ne,
                            Token = password.Substring(ns, ne - ns + 1),
                            Cardinality = bruteforce_cardinality,
                            Pattern = BruteforcePattern,
                            Entropy = Math.Log(Math.Pow(bruteforce_cardinality, ne - ns + 1), 2)
                        });
                    }
                }

                matchSequence = matchSequenceCopy;
            }

            var minEntropy = (password.Length == 0 ? 0 : minimumEntropyToIndex[password.Length - 1]);
            var crackTime = PasswordScoring.EntropyToCrackTime(minEntropy);

            var result = new Result()
            {
                Password = password,
                Entropy = Math.Round(minEntropy, 3),
                MatchSequence = matchSequence,
                CrackTime = Math.Round(crackTime, 3),
                CrackTimeDisplay = Utility.DisplayTime(crackTime, translation),
                Score = PasswordScoring.CrackTimeToScore(crackTime)
            };
            return result;
        }


        public static IEnumerable<Match> Omnimatch(string password)
        {
            IEnumerable<Match> matches = new List<Match>();

            foreach (var matcher in matcherFactory.CreateMatchers(new List<string>()))
            {
                var currMatches = matcher.MatchPassword(password);
                foreach (Match item in currMatches)
                {
                    matches = matches.Concat(new[] { item });
                }
                // matches = matches.Union(matcher.MatchPassword(password));
            }

            List<Match> matchList = matches.ToList();
            matchList.Sort(delegate (Match m1, Match m2)
            {
                if (m1.i != m2.i)
                {
                    return m1.i - m2.i;
                }
                return m1.j - m2.j;
            });

            return matchList;
        }

        private static int factorial(int n)
        {
            if (n < 2) return 1;
            int f = 1;
            foreach (int i in EnumerableUtility.Range(2, n + 1))
            {
                f *= i;
            }
            return f;
        }

        public static Result MostGuessableMatchSequence(string password, IEnumerable<Match> matches, bool _exclude_additive = false)
        {
            int n = password.Length;

            List<List<Match>> matchesByJ = new List<List<Match>>();

            foreach (int i in EnumerableUtility.Range(0, n))
            {
                matchesByJ.Add(new List<Match>());
            }

            foreach (Match m in matches)
            {
                matchesByJ[m.j].Add(m);
            }

            foreach (List<Match> lst in matchesByJ)
            {
                lst.Sort(delegate (Match m1, Match m2)
                {
                    return m1.i - m2.i;
                });
            }

            Optimal optimal = new Optimal(n);

            /*
                # helper: considers whether a length-l sequence ending at match m is better (fewer guesses)
                # than previously encountered sequences, updating state if so.
            */
            void update(Match m, int l)
            {
                // helper: considers whether a length-l sequence ending at match m is better (fewer guesses)
                // than previously encountered sequences, updating state if so.
                int k = m.j;
                double pi = EstimateGuesses(m, password);
                if (l > 1)
                {
                    /*
                        # we're considering a length-l sequence ending with match m:
                        # obtain the product term in the minimization function by multiplying m's guesses
                        # by the product of the length-(l-1) sequence ending just before m, at m.i - 1.
                    */
                    pi *= optimal.pi[m.i - 1][l - 1];
                }
                // calculate the minimization func
                double g = factorial(l) * pi;
                if (!_exclude_additive)
                {
                    g += Math.Pow(MIN_GUESSES_BEFORE_GROWING_SEQUENCE, l - 1);
                }
                // update state if new best
                // first see if any competing sequences covering this prefix, with l or fewer matches,
                // fare better than this sequence. if so, skip it and return
                foreach (KeyValuePair<int, double> val in optimal.g[k])
                {
                    int competingL = val.Key;
                    double competingG = val.Value;
                    if (competingL > l) continue;
                    if (competingG <= g) return;
                }
                optimal.g[k][l] = g;
                optimal.m[k][l] = m;
                optimal.pi[k][l] = pi;
            }

            // helper: make bruteforce match objects spanning i to j, inclusive.
            Match makeBruteForceMatch(int i, int j)
            {
                Match m = new Match();
                m.Token = password.Substring(i, j - i + 1);
                m.Pattern = "bruteforce";
                m.i = i;
                m.j = j;
                return m;
            }

            // helper: evaluate bruteforce matches ending at k.
            void bruteForceUpdate(int k)
            {
                // see eif a single bruteforce match spanning the k-prefix is optimal
                Match m = makeBruteForceMatch(0, k);
                update(m, 1);
                foreach (int i in EnumerableUtility.Range(1, k + 1))
                {
                    // generate k bruteforce matches, spanning from (i=1, j=k) up to (i=k, j=k)
                    // see if adding these new matches to any of the sequences in optimal[i-1]
                    // leads to new bests
                    m = makeBruteForceMatch(i, k);
                    foreach (KeyValuePair<int, Match> val in optimal.m[i - 1])
                    {
                        int l = val.Key;
                        // corner: an optimal sequence will never have two adjacent bruteforce matches
                        // it is strictly better to have a single bruteforce match spanning the same region:
                        // same contribution to the guess product with a lower length
                        // --> safe to skip those cases
                        if (val.Value.Pattern != null && val.Value.Pattern.Equals("bruteforce")) continue;
                        // try adding m to this length-l sequence
                        update(m, l + 1);
                    }
                }
            }

            // helper: step backwards through optimal.m starting at the end,
            // constructing the final optimal match sequence
            List<Match> unwind(int nN)
            {
                List<Match> optimalMatchSequenceList = new List<Match>();
                int k = nN - 1;
                // find the final best seqence length and score
                int l = 1;
                double g = Double.MaxValue;

                foreach (KeyValuePair<int, double> val in optimal.g[k])
                {
                    int candidateL = val.Key;
                    double candidateG = val.Value;
                    if (candidateG < g)
                    {
                        l = candidateL;
                        g = candidateG;
                    }
                }

                while (k >= 0)
                {
                    Match m = optimal.m[k][l];
                    optimalMatchSequenceList.Insert(0, m);
                    k = m.i - 1;
                    l--;
                }
                return optimalMatchSequenceList;
            }

            foreach (int k in EnumerableUtility.Range(0, n))
            {
                foreach (Match m in matchesByJ[k])
                {
                    if (m.i > 0)
                    {
                        foreach (int l in optimal.m[m.i - 1].Keys)
                        {
                            update(m, l + 1);
                        }
                    }
                    else
                    {
                        update(m, 1);
                    }
                }
                bruteForceUpdate(k);
            }
            List<Match> optimalMatchSequence = unwind(n);
            int optimalL = optimalMatchSequence.Count;

            double guesses = (password.Length == 0) ? 1 : optimal.g[n - 1][optimalL];

            double seconds = guesses / Math.Pow(10, 4);

            double crackTime = PasswordScoring.GuessesToCrackTime(guesses);
            // UnityEngine.Debug.Log($"Seconds: {seconds}");

            // Debug.Log($"Score: {PasswordScoring.GuessesToScore(guesses)}");

            int score = PasswordScoring.GuessesToScore(guesses);

            return new Result
            {
                Password = password,
                Guesses = guesses,
                GuessesLog10 = Math.Log10(guesses),
                MatchSequence = optimalMatchSequence,
                CrackTime = Math.Round(crackTime, 3),
                CrackTimeDisplay = Utility.DisplayTime(crackTime),
                Score = score
            };
        }

        private class Optimal
        {
            /*
            # optimal.m[k][l] holds final match in the best length-l match sequence covering the
            # password prefix up to k, inclusive.
            # if there is no length-l sequence that scores better (fewer guesses) than
            # a shorter match sequence spanning the same prefix, optimal.m[k][l] is undefined.
            */
            public List<Dictionary<int, Match>> m = new List<Dictionary<int, Match>>();
            /*
            # same structure as optimal.m -- holds the product term Prod(m.guesses for m in sequence).
            # optimal.pi allows for fast (non-looping) updates to the minimization function.
            */
            public List<Dictionary<int, double>> pi = new List<Dictionary<int, double>>();
            // # same structure as optimal.m -- holds the overall metric.
            public List<Dictionary<int, double>> g = new List<Dictionary<int, double>>();
            public Optimal(int n)
            {
                foreach (int i in EnumerableUtility.Range(0, n))
                {
                    m.Add(new Dictionary<int, Match>());
                    pi.Add(new Dictionary<int, double>());
                    g.Add(new Dictionary<int, double>());
                }
            }
        }

        const int BRUTEFORCE_CARDINALITY = 10;
        const int MIN_GUESSES_BEFORE_GROWING_SEQUENCE = 10000;
        const int MIN_SUBMATCH_GUESSES_SINGLE_CHAR = 10;
        const int MIN_SUBMATCH_GUESSES_MULTI_CHAR = 50;

        private static int nCk(int n, int k)
        {
            if (k > n) return 0;
            if (k == 0) return 1;
            int r = 1;
            foreach (int d in EnumerableUtility.Range(1, k + 1))
            {
                r *= n;
                r /= d;
                n -= 1;
            }
            return r;
        }

        private static double EstimateGuesses(Match match, string password)
        {
            if (match.Guesses.HasValue) return match.Guesses.Value;

            double min_guesses = 1;
            if (match.Token.Length < password.Length)
            {
                min_guesses = (match.Token.Length == 1) ? MIN_SUBMATCH_GUESSES_SINGLE_CHAR : MIN_SUBMATCH_GUESSES_MULTI_CHAR;
            }
            double guesses;
            Type type = match.GetType();
            if (type == typeof(SequenceMatch))
            {
                guesses = SequenceGuesses(match);
            }
            else if (type == typeof(DateMatch))
            {
                guesses = DateGuesses(match);
            }
            else if (type == typeof(SpatialMatch))
            {
                guesses = SpatialGuesses(match);
            }
            else if (type == typeof(DictionaryMatch) || type == typeof(L33tDictionaryMatch))
            {
                guesses = DictionaryGuesses(match);
            }
            else
            {
                guesses = Math.Max(BruteforceGuesses(match), RegexGuesses(match));
            }
            match.Guesses = Math.Max(guesses, min_guesses);
            match.GuessesLog10 = Math.Log10(match.Guesses.Value);
            return match.Guesses.Value;
        }

        private static double BruteforceGuesses(Match match)
        {
            double guesses = Math.Pow(BRUTEFORCE_CARDINALITY, match.Token.Length);
            if (guesses == Double.PositiveInfinity)
            {
                guesses = Double.MaxValue;
            }
            // small detail: make bruteforce matches at minimum one guess bigger than smallest allowed
            // submatch guesses, such that non-bruteforce submatches over the same [i..] take precedence
            double min_guesses = (match.Token.Length == 1) ? MIN_SUBMATCH_GUESSES_SINGLE_CHAR + 1 : MIN_SUBMATCH_GUESSES_MULTI_CHAR + 1;
            return Math.Max(guesses, min_guesses);
        }

        private static double RepeatGuesses(Match match)
        {
            RepeatMatch rM = (RepeatMatch)match;
            return rM.BaseGuesses * rM.RepeatCount;
        }

        private static double SequenceGuesses(Match match)
        {
            char firstChar = match.Token[0];

            double baseGuesses = 0;
            if ((new HashSet<char> { 'a', 'A', 'z', 'Z', '0', '1', '9' }).Contains(firstChar))
            {
                baseGuesses = 4;
            }
            else
            {
                if (Char.IsDigit(firstChar))
                {
                    baseGuesses = 10;
                }
                else
                {
                    baseGuesses = 26;
                }
            }
            SequenceMatch sm = (SequenceMatch)match;
            if (!sm.Ascending)
            {
                baseGuesses *= 2;
            }
            return baseGuesses * match.Token.Length;
        }

        private const int MIN_YEAR_SPACE = 20;
        private const int REFERENCE_YEAR = 2020;

        private static double RegexGuesses(Match match)
        {
            if (match.Pattern.Equals("digits"))
            {
                return Math.Pow(10, match.Token.Length);
            }
            else if (match.Pattern.Equals("year"))
            {
                double yearSpace = Math.Abs(Int16.Parse(match.Token) - REFERENCE_YEAR);
                return Math.Max(yearSpace, MIN_YEAR_SPACE);
            }
            List<double> choices = new List<double> { 26, 26, 52, 62, 10, 33 };
            int randIdx = Mathf.FloorToInt(choices.Count);
            // return Math.Pow(choices[randIdx], match.Token.Length);
            return 0;
        }

        private static double DateGuesses(Match match)
        {
            DateMatch dm = (DateMatch)match;
            double yearSpace = Math.Max(Math.Abs(dm.Year - REFERENCE_YEAR), MIN_YEAR_SPACE);
            double guesses = yearSpace * 365;
            if (dm.Separator.Length > 0) guesses *= 4;
            return guesses;
        }

        private static double SpatialGuesses(Match match)
        {
            SpatialMatch sm = (SpatialMatch)match;
            double s;
            double d;
            if (sm.Graph.Equals("qwerty") || sm.Graph.Equals("dvorak"))
            {
                s = 4.596;
                d = 94;
            }
            else
            {
                s = 5.0;
                d = 15.5;
            }
            double guesses = 0;
            int L = sm.Token.Length;
            int t = sm.Turns;
            foreach (int i in EnumerableUtility.Range(2, L + 1))
            {
                int possible_turns = (int)Math.Min(t, i - 1);
                foreach (int j in EnumerableUtility.Range(1, possible_turns + 1))
                {
                    guesses += nCk(i - 1, j - 1) * s * Math.Pow(d, j);
                }
            }
            if (sm.ShiftedCount > 0)
            {
                int S = sm.ShiftedCount;
                int U = sm.Token.Length - S;
                if (S == 0 || U == 0) guesses *= 2;
                else
                {
                    double shiftedVariations = 0;
                    int rangeEnd = (int)Math.Min(S, U);
                    foreach (int i in EnumerableUtility.Range(1, rangeEnd + 1))
                    {
                        shiftedVariations += nCk(S + U, i);
                    }
                    guesses *= shiftedVariations;
                }
            }

            return guesses;
        }

        private static double DictionaryGuesses(Match match)
        {
            DictionaryMatch dm = (DictionaryMatch)match;

            double uppercase_variations = UppercaseVariations(dm);
            double l33t_variatioins = L33tVariations(dm);
            int reversedVariations = (dm.Reversed) ? 2 : 1;
            return dm.Rank * uppercase_variations * l33t_variatioins * reversedVariations;
        }

        private static double UppercaseVariations(DictionaryMatch dictionaryMatch)
        {
            string word = dictionaryMatch.Token;
            Regex all_lower = new Regex(@"^[^a-z]+$");
            if (all_lower.IsMatch(word) || word.ToLower().Equals(word)) return 1;

            Regex startUpper = new Regex(@"^[A-Z][^A-Z]+$");
            Regex endUpper = new Regex(@"^[^A-Z]+[A-Z]$");
            Regex allUpper = new Regex(@"^[^A-Z]+$");

            foreach (Regex r in new Regex[] { startUpper, endUpper, allUpper })
            {
                if (r.IsMatch(word)) return 2;
            }

            int U = word.ToCharArray().Where(c => char.IsUpper(c)).Count();
            int L = word.ToCharArray().Where(c => char.IsLower(c)).Count();

            double variations = 0;
            int end = (int)Math.Min(U, L);
            foreach (int i in EnumerableUtility.Range(1, end + 1))
            {
                variations += nCk(U + L, i);
            }

            return variations;
        }

        private static double L33tVariations(DictionaryMatch dictionaryMatch)
        {
            if (dictionaryMatch.GetType() != typeof(L33tDictionaryMatch)) return 1;

            double variations = 1;

            L33tDictionaryMatch ldm = (L33tDictionaryMatch)dictionaryMatch;
            foreach (KeyValuePair<char, char> val in ldm.Subs)
            {
                char subbed = val.Key;
                char unsubbed = val.Value;
                char[] chrs = ldm.Token.ToLower().ToCharArray();
                int S = chrs.Where(c => c == subbed).Count();
                int U = chrs.Where(c => c == unsubbed).Count();
                if (S == 0 || U == 0)
                {
                    variations *= 2;
                }
                else
                {
                    int p = (int)Math.Min(U, S);
                    double possibilities = 0;
                    foreach (int i in EnumerableUtility.Range(1, p + 1))
                    {
                        possibilities += nCk(U + S, i);
                    }
                    variations *= possibilities;
                }
            }
            return variations;
        }
    }
}