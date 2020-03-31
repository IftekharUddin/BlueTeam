using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Zxcvbn;
using System.Text;


public class PasswordGeneration : MonoBehaviour
{
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
        if (ch == 'a' || ch == 'A')
        {
            return sub(ch, new string[] { "4", "@" });
        }
        else if (ch == 'b' || ch == 'B')
        {
            return sub(ch, new string[] { "8", "|3", "|8", "lo", "|o", });
        }
        else if (ch == 'c' || ch == 'C')
        {
            return sub(ch, new string[] { "<", "{", "[", "(" });
        }
        else if (ch == 'd' || ch == 'D')
        {
            return sub(ch, new string[] { "|)", "|}", "|]", "|>" });
        }
        else if (ch == 'e' || ch == 'E')
        {
            return sub(ch, new string[] { "3" });
        }
        else if (ch == 'f' || ch == 'F')
        {
            return "" + ch;
        }
        else if (ch == 'g' || ch == 'G')
        {
            return sub(ch, new string[] { "6" });
        }
        else if (ch == 'h' || ch == 'H')
        {
            return sub(ch, new string[] { "|-|", "[-]", "}-{", "}{", "|=|", "[=]" });
        }
        else if (ch == 'i' || ch == 'I')
        {
            return sub(ch, new string[] { "1", "|", "!" });
        }
        else if (ch == 'j' || ch == 'J')
        {
            return sub(ch, new string[] { "_|", "_)", "_]", "_}" });
        }
        else if (ch == 'k' || ch == 'K')
        {
            return sub(ch, new string[] { "|<", "1<", "l<" });
        }
        else if (ch == 'l' || ch == 'L')
        {
            return sub(ch, new string[] { "|_", "|", "1" });
        }
        else if (ch == 'm' || ch == 'M')
        {
            return sub(ch, new string[] { "|\\/|", "/\\/\\" });
        }
        else if (ch == 'n' || ch == 'N')
        {
            return sub(ch, new string[] { "|\\|" });
        }
        else if (ch == 'o' || ch == 'O')
        {
            return sub(ch, new string[] { "0", "()", "[]", "{}", "<>", "oh" });
        }
        else if (ch == 'p' || ch == 'P')
        {
            return sub(ch, new string[] { "|O", "|>", "|*" });
        }
        else if (ch == 'q' || ch == 'Q')
        {
            return sub(ch, new string[] { "O_" });
        }
        else if (ch == 'r' || ch == 'R')
        {
            return "" + ch;
        }
        else if (ch == 's' || ch == 'S')
        {
            return sub(ch, new string[] { "5", "$" });
        }
        else if (ch == 't' || ch == 'T')
        {
            return sub(ch, new string[] { "+", "'|'", "`|`", "~|~", "-|-" });
        }
        else if (ch == 'u' || ch == 'U')
        {
            return sub(ch, new string[] { "|_|" });
        }
        else if (ch == 'v' || ch == 'V')
        {
            return sub(ch, new string[] { "\\/" });
        }
        else if (ch == 'w' || ch == 'W')
        {
            return sub(ch, new string[] { "\\/\\/" });
        }
        else if (ch == 'x' || ch == 'X')
        {
            return sub(ch, new string[] { "><" });
        }
        else if (ch == 'y' || ch == 'Y')
        {
            return "" + ch;
        }
        else if (ch == 'z' || ch == 'Z')
        {
            return "" + ch;
        }
        return "" + ch;
    }

    private string sub(char ch, string[] choices)
    {
        return (Random.value > .1f) ? ("" + ch) : choices[Mathf.FloorToInt(Random.Range(0, choices.Length))];
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
        return this.commonWords[Mathf.FloorToInt(Random.Range(0, this.commonWords.Count))] as string;
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

        // var result = this.passwordChecker.EvaluatePassword(first);
        // Debug.Log($"{first}: {result.CrackTime} {result.CrackTimeDisplay} {result.Score} {result.CalcTime}");
        var result = this.passwordChecker.EvaluatePassword(pw);

        while (result.Score < 3)
        {
            pw = generateGoodPassword();
            result = this.passwordChecker.EvaluatePassword(pw);
        }
        return pw;
    }
}
