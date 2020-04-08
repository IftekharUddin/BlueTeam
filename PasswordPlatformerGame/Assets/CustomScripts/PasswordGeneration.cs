using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Zxcvbn;
using System.Text;


public class PasswordGeneration : MonoBehaviour
{
    private enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD
    }
    private Difficulty difficulty;
    public TextAsset badPasswordsText;
    public TextAsset wordsText;

    private static PasswordGeneration _instance;
    private List<string> badPasswords;
    private List<string> commonWords;
    private Zxcvbn.Zxcvbn passwordChecker;

    /// <value> The singleton instance of this class which can be accessed by whoever wants to generate passwords. </value>
    public static PasswordGeneration Instance
    {
        get
        {
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.passwordChecker = new Zxcvbn.Zxcvbn();

        string passwords = this.badPasswordsText.text;

        this.badPasswords = new List<string>();
        using (StringReader reader = new StringReader(passwords))
        {
            string line = string.Empty;
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    this.badPasswords.Add(line);
                }
            } while (line != null);
        }

        // string first = this.badPasswords[0] as string;
        // var result = this.passwordChecker.EvaluatePassword(first);
        // Debug.Log($"{first}: {result.CrackTime} {result.CrackTimeDisplay} {result.Score} {result.CalcTime}");

        string words = this.wordsText.text;

        this.commonWords = new List<string>();
        using (StringReader reader = new StringReader(words))
        {
            string line = string.Empty;
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    this.commonWords.Add(line);
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
                this.difficulty = Difficulty.EASY;
                break;
            case 1:
                this.difficulty = Difficulty.MEDIUM;
                break;
            case 2:
                this.difficulty = Difficulty.HARD;
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

    public string GetBadPassword()
    {
        int randIndex = Mathf.FloorToInt(Random.Range(0f, (float)this.badPasswords.Count));
        string item = this.badPasswords[randIndex];
        this.badPasswords.RemoveAt(randIndex);
        return item;
    }

    private string l33t(char ch)
    {
        // ['a'] = "4@",
        //         ['b'] = "8",
        //         ['c'] = "({[<",
        //         ['e'] = "3",
        //         ['g'] = "69",
        //         ['i'] = "1!|",
        //         ['l'] = "1|7",
        //         ['o'] = "0",
        //         ['s'] = "$5",
        //         ['t'] = "+7",
        //         ['x'] = "%",
        //         ['z'] = "2"
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
        return sub(ch, choices, .1f);
    }

    private string sub(char ch, string[] choices, float probLeet)
    {
        return (Random.value > probLeet) ? ("" + ch) : choices[Mathf.FloorToInt(Random.value * choices.Length)];
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

    private string getRandomWord()
    {
        return this.commonWords[Mathf.FloorToInt(Random.value * this.commonWords.Count)] as string;
    }

    private string generateGoodPassword()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 2; i++)
        {
            sb.Append(l33tword(getRandomWord()));
        }
        return sb.ToString();
    }

    public string GetGoodPassword()
    {
        string pw = generateGoodPassword();

        var result = this.passwordChecker.EvaluatePassword(pw);

        while (result.Score < 3)
        {
            pw = generateGoodPassword();
            result = this.passwordChecker.EvaluatePassword(pw);
        }
        return pw;
    }
}
