using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Zxcvbn;
using System.Text;
using System;
using System.Diagnostics;
using System.Linq;


public class PasswordGeneration : MonoBehaviour
{
    private string[] punctuation = new string[] { ";", ".", "!", "?", "-", "_", "+", "=", "/", "\\", "~", "|" };
    private string[] months = new string[] { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december", "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sept", "oct", "nov", "dec" };
    private string[] days = new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen", "twenty", "twenty-one", "twenty-two", "twenty-three", "twenty-four", "twenty-five", "twenty-six", "twenty-seven", "twenty-eight", "twenty-nine", "thirty", "thirty-one" };

    private DifficultyUtility.Difficulty difficulty;
    public TextAsset badPasswordsText;
    public TextAsset zxcvbnPasswordsText;
    public TextAsset maleNamesText;
    public TextAsset femaleNamesText;
    public TextAsset surnamesText;
    public TextAsset englishWordsText;

    private static PasswordGeneration _instance;
    private List<string> badPasswordsFromInternet = new List<string>();
    private List<string> zxcvbnPasswords = new List<string>();
    private List<string> maleNames = new List<string>();
    private List<string> femaleNames = new List<string>();
    private List<string> surnames = new List<string>();
    private List<string> englishWords = new List<string>();
    private Zxcvbn.Zxcvbn passwordChecker;

    private List<string> badPasswords = new List<string>();
    private List<string> goodPasswords = new List<string>();
    private int currBadPassWord = 0;
    private int currGoodPassWord = 0;

    /// <value> The singleton instance of this class which can be accessed by whoever wants to generate passwords. </value>
    public static PasswordGeneration Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        // DontDestroyOnLoad(this.gameObject);

        this.passwordChecker = new Zxcvbn.Zxcvbn();

        string passwords = this.badPasswordsText.text;

        this.readTextAssetIntoList(this.badPasswordsText, this.badPasswordsFromInternet);

        this.readTextAssetIntoList(this.zxcvbnPasswordsText, this.zxcvbnPasswords);

        this.readTextAssetIntoList(this.maleNamesText, this.maleNames);

        this.readTextAssetIntoList(this.femaleNamesText, this.femaleNames);

        this.readTextAssetIntoList(this.surnamesText, this.surnames);

        this.readTextAssetIntoList(this.englishWordsText, this.englishWords);

        foreach (var i in Enumerable.Range(0, 50))
        {
            Stopwatch sw = Stopwatch.StartNew();
            this.badPasswords.Add(this.generateBadPassword());
            sw.Stop();
            // UnityEngine.Debug.Log($"Bad: {sw.ElapsedMilliseconds}");
            Stopwatch sw2 = Stopwatch.StartNew();
            this.goodPasswords.Add(this.generateGoodPassword());
            sw2.Stop();
            // UnityEngine.Debug.Log($"Good: {sw2.ElapsedMilliseconds}");
        }

    }

    // void FixedUpdate()
    // {
    //     if (this.badPasswords.Count < 100)
    //     {
    //         this.badPasswords.Add(this.generateBadPassword());
    //     }
    //     if (this.goodPasswords.Count < 100)
    //     {
    //         this.goodPasswords.Add(this.generateGoodPassword());
    //     }
    // }

    private void readTextAssetIntoList(TextAsset asset, List<string> list)
    {
        using (StringReader reader = new StringReader(asset.text))
        {
            string line = string.Empty;
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    list.Add(line);
                }
            } while (line != null);
        }
    }

    void OnEnable()
    {
        int difficulty = PlayerPrefs.GetInt("difficulty");
        switch (difficulty)
        {
            case 0:
                this.difficulty = DifficultyUtility.Difficulty.EASY;
                break;
            case 1:
                this.difficulty = DifficultyUtility.Difficulty.MEDIUM;
                break;
            case 2:
                this.difficulty = DifficultyUtility.Difficulty.HARD;
                break;
            default:
                Application.Quit();
                return;
        }
    }

    public string EvaluatePassword(string password)
    {
        var result = this.passwordChecker.EvaluatePassword(password);
        return $"{result.CrackTimeDisplay}!";
    }

    private string getRandFromArray(List<string> arr)
    {
        return arr[Mathf.FloorToInt(UnityEngine.Random.value * arr.Count)];
    }

    private string getRandPunctuation()
    {
        return this.punctuation[Mathf.FloorToInt(UnityEngine.Random.value * this.punctuation.Length)];
    }

    private string reverse(string s)
    {
        char[] chars = s.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    private string badPassword(DifficultyUtility.Difficulty diff)
    {
        if (diff == DifficultyUtility.Difficulty.EASY)
        {
            int randIndex = Mathf.FloorToInt(UnityEngine.Random.Range(0f, (float)this.badPasswordsFromInternet.Count));
            string item = this.badPasswordsFromInternet[randIndex];
            // this.badPasswordsFromInternet.RemoveAt(randIndex);
            return item;
        }
        else if (diff == DifficultyUtility.Difficulty.MEDIUM)
        {
            int choice = Mathf.FloorToInt(UnityEngine.Random.value * 5);
            string res;
            switch (choice)
            {
                case 0:
                    int randIndex = Mathf.FloorToInt(UnityEngine.Random.value * this.zxcvbnPasswords.Count);
                    string item = this.zxcvbnPasswords[randIndex];
                    this.zxcvbnPasswords.RemoveAt(randIndex);
                    res = item;
                    break;
                case 1:
                    res = this.getRandFromArray(this.maleNames);
                    break;
                case 2:
                    res = this.getRandFromArray(this.femaleNames);
                    break;
                case 3:
                    res = this.getRandFromArray(this.surnames);
                    break;
                case 4:
                    int numberWords = Mathf.FloorToInt(UnityEngine.Random.value * 3 + 1);
                    StringBuilder sb = new StringBuilder();
                    for (var i = 0; i < numberWords; i++)
                    {
                        sb.Append(this.getRandFromArray(this.englishWords));
                    }
                    res = sb.ToString();
                    break;
                default:
                    res = badPassword(DifficultyUtility.Difficulty.EASY);
                    break;
            }
            if (UnityEngine.Random.value > 0.75f)
            {
                return l33tword(res);
            }
            return res;
        }
        else
        {
            int choice = Mathf.FloorToInt(UnityEngine.Random.value * 7);
            string res;
            switch (choice)
            {
                case 0:
                    int randIndex = Mathf.FloorToInt(UnityEngine.Random.value * this.zxcvbnPasswords.Count);
                    string item = this.zxcvbnPasswords[randIndex];
                    this.zxcvbnPasswords.RemoveAt(randIndex);
                    res = item;
                    break;
                case 1:
                    res = this.getRandFromArray(this.maleNames) + this.getRandFromArray(this.surnames);
                    break;
                case 2:
                    res = this.getRandFromArray(this.femaleNames) + this.getRandFromArray(this.surnames);
                    break;
                case 3:
                    res = this.getRandFromArray(this.surnames) + this.getRandFromArray(this.maleNames);
                    break;
                case 4:
                    res = this.getRandFromArray(this.surnames) + this.getRandFromArray(this.femaleNames);
                    break;
                case 5:
                    int numberWords = Mathf.FloorToInt(UnityEngine.Random.value * 3 + 1);
                    StringBuilder sb = new StringBuilder();
                    for (var i = 0; i < numberWords; i++)
                    {
                        sb.Append(this.getRandFromArray(englishWords));
                        if (UnityEngine.Random.value > .8f)
                        {
                            sb.Append(this.getRandPunctuation());
                        }
                    }
                    string val = sb.ToString();
                    if (UnityEngine.Random.value > .9f)
                    {
                        res = this.reverse(val);
                        break;
                    }
                    res = val;
                    break;
                case 6:
                    int monthInt = Mathf.FloorToInt(UnityEngine.Random.value * 12 + 1);
                    string monthString = monthInt < 10 ? "0" + monthInt : "" + monthInt;
                    string month = (UnityEngine.Random.value > 0.5f) ? this.getRandFromArray(this.months.ToList()) : monthString;

                    int dayInt = Mathf.FloorToInt(UnityEngine.Random.value * 31 + 1);
                    string dayString = dayInt < 10 ? "0" + dayInt : "" + dayInt;
                    string day = (UnityEngine.Random.value > 0.5f) ? this.getRandFromArray(this.days.ToList()) : dayString;

                    bool hasSeparator = UnityEngine.Random.value > 0.5f;
                    bool hasYear = UnityEngine.Random.value > 0.5f;
                    if (hasSeparator)
                    {
                        string separator = (UnityEngine.Random.value > 0.5f) ? "-" : "/";
                        if (hasYear)
                        {
                            string year = "" + Mathf.FloorToInt(UnityEngine.Random.value * 90 + 10);
                            return month + separator + day + separator + year;
                        }
                        return month + separator + day;
                    }
                    else
                    {
                        if (hasYear)
                        {
                            string year = "" + Mathf.FloorToInt(UnityEngine.Random.value * 90 + 10);
                            return month + day + year;
                        }
                        return month + day;
                    }
                default:
                    res = badPassword(DifficultyUtility.Difficulty.EASY);
                    break;
            }
            if (UnityEngine.Random.value > 0.75f)
            {
                return l33tword(res);
            }
            return res;
        }
    }

    private string generateBadPassword()
    {
        string word = this.badPassword(this.difficulty);
        while (this.passwordChecker.EvaluatePassword(word).Score > 2 || word.Length > 24)
        {
            word = this.badPassword(this.difficulty);
        }
        return word;
    }

    public string GetBadPassword()
    {
        return this.badPasswords[this.currBadPassWord++];
    }

    private string l33t(char ch)
    {
        switch (ch)
        {
            case 'a':
            case 'A':
                return sub(ch, new string[] { "4", "@" });
            case 'b':
            case 'B':
                return sub(ch, new string[] { "8" });
            case 'c':
            case 'C':
                return sub(ch, new string[] { "(", "{", "[", "<" });
            case 'e':
            case 'E':
                return sub(ch, new string[] { "3" });
            case 'g':
            case 'G':
                return sub(ch, new string[] { "6", "9" });
            case 'i':
            case 'I':
                return sub(ch, new string[] { "1", "!", "|" });
            case 'l':
            case 'L':
                return sub(ch, new string[] { "1", "|", "7" });
            case 'o':
            case 'O':
                return sub(ch, new string[] { "0" });
            case 's':
            case 'S':
                return sub(ch, new string[] { "$", "5" });
            case 't':
            case 'T':
                return sub(ch, new string[] { "+", "7" });
            case 'x':
            case 'X':
                return sub(ch, new string[] { "%" });
            case 'z':
            case 'Z':
                return sub(ch, new string[] { "2" });
            default:
                return "" + ch;
        }
    }

    private string sub(char ch, string[] choices)
    {
        return sub(ch, choices, .25f);
    }

    private string sub(char ch, string[] choices, float probLeet)
    {
        return (UnityEngine.Random.value > probLeet) ? ("" + ch) : choices[Mathf.FloorToInt(UnityEngine.Random.value * choices.Length)];
    }

    private string l33tword(string word)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < word.Length; i++)
        {
            sb.Append(this.l33t(word[i]));
        }
        return sb.ToString();
    }

    private string generateGoodCandidatePassword()
    {
        int numWords = Mathf.FloorToInt(UnityEngine.Random.value * 2 + 2);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < numWords; i++)
        {
            sb.Append(l33tword(this.getRandFromArray(this.englishWords)));
        }
        return sb.ToString();
    }

    private string generateGoodPassword()
    {
        string pw = generateGoodCandidatePassword();

        var result = this.passwordChecker.EvaluatePassword(pw);

        int cutoff = 24;

        while (result.Score <= 3 || pw.Length > cutoff)
        {
            pw = generateGoodCandidatePassword();
            result = this.passwordChecker.EvaluatePassword(pw);
        }
        return pw;
    }

    public string GetGoodPassword()
    {
        // UnityEngine.Debug.Log(this.currGoodPassWord);
        return this.goodPasswords[this.currGoodPassWord++];
    }
}
