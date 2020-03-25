using UnityEngine;
using System.IO;
using System.Collections;
using Zxcvbn;
using System.Text;


public class PasswordGeneration : MonoBehaviour
{
    private static PasswordGeneration _instance;
    private ArrayList badPasswords;
    private ArrayList commonWords;
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

        TextAsset text = Resources.Load<TextAsset>("BAD_PASSWORDS");
        string passwords = text.text;

        this.badPasswords = new ArrayList();
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

        TextAsset wordsAsset = Resources.Load<TextAsset>("1000_COMMON_WORDS");
        string words = wordsAsset.text;

        this.commonWords = new ArrayList();
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
        string item = this.badPasswords[randIndex] as string;
        this.badPasswords.RemoveAt(randIndex);
        return item;
    }

    private string l33t(char ch)
    {
        if (ch == 'a' || ch == 'A')
        {
            return sub(ch, new string[] { "4", "/-\\", "/_\\", "@", "/\\" });
        }
        else if (ch == 'b' || ch == 'B')
        {
            return sub(ch, new string[] { "8", "|3", "13", "|}", "|:", "|8", "18", "6", "|B", "|8", "lo", "|o", "j3" });
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
            return sub(ch, new string[] { "|=", "ph", "|#", "|\"" });
        }
        else if (ch == 'g' || ch == 'G')
        {
            return sub(ch, new string[] { "[", "-", "[+", "6", "C-" });
        }
        else if (ch == 'h' || ch == 'H')
        {
            return sub(ch, new string[] { "4", "|-|", "[-]", "{-}", "}-{", "}{", "|=|", "[=]", "{=}", "/-/", "(-)", ")-(", ":-:", "I+I" });
        }
        else if (ch == 'i' || ch == 'I')
        {
            return sub(ch, new string[] { "1", "|", "!", "9" });
        }
        else if (ch == 'j' || ch == 'J')
        {
            return sub(ch, new string[] { "_|", "_/", "_7", "_)", "_]", "_}" });
        }
        else if (ch == 'k' || ch == 'K')
        {
            return sub(ch, new string[] { "|<", "1<", "l<", "|{", "l{" });
        }
        else if (ch == 'l' || ch == 'L')
        {
            return sub(ch, new string[] { "|_", "|", "1", "][" });
        }
        else if (ch == 'm' || ch == 'M')
        {
            return sub(ch, new string[] { "44", "|\\/|", "^^", "/\\/\\", "/X\\", "[]\\/][", "[]V[]", "][\\\\//][", "(V),//.", ".\\\\", "N\\\\," });
        }
        else if (ch == 'n' || ch == 'N')
        {
            return sub(ch, new string[] { "|\\|", "/\\/", "/V", "][\\\\][" });
        }
        else if (ch == 'o' || ch == 'O')
        {
            return sub(ch, new string[] { "0", "()", "[]", "{}", "<>", "oh" });
        }
        else if (ch == 'p' || ch == 'P')
        {
            return sub(ch, new string[] { "|o", "|O", "|>", "|*", "|°", "|D", "/o", "[]D", "|7}" });
        }
        else if (ch == 'q' || ch == 'Q')
        {
            return sub(ch, new string[] { "O_", "9", "(,)", "kw" });
        }
        else if (ch == 'r' || ch == 'R')
        {
            return sub(ch, new string[] { "|2", "12", ".-", "|^" });
        }
        else if (ch == 's' || ch == 'S')
        {
            return sub(ch, new string[] { "5", "$" });
        }
        else if (ch == 't' || ch == 'T')
        {
            return sub(ch, new string[] { "7", "+", "7`", "'|'", "`|`", "~|~", "-|-", "']['" });
        }
        else if (ch == 'u' || ch == 'U')
        {
            return sub(ch, new string[] { "|_|", "\\_\\", "/_/", "\\_/", "(_)", "[_]", "{_}" });
        }
        else if (ch == 'v' || ch == 'V')
        {
            return sub(ch, new string[] { "\\/" });
        }
        else if (ch == 'w' || ch == 'W')
        {
            return sub(ch, new string[] { "\\/\\/", "(/\\)", "\\^/", "|/\\|", "\\X/", "\\\\'", "'//", "\\_|_/", "\\\\//\\\\//", "2u", "\\V/" });
        }
        else if (ch == 'x' || ch == 'X')
        {
            return sub(ch, new string[] { "%", "*", "><", "}{", ")(" });
        }
        else if (ch == 'y' || ch == 'Y')
        {
            return sub(ch, new string[] { "`/", "]\\|/" });
        }
        else if (ch == 'z' || ch == 'Z')
        {
            return sub(ch, new string[] { "2", "5", "7_", ">_", "(/)" });
        }
        return "" + ch;
    }

    private string sub(char ch, string[] choices)
    {
        return (Random.value > .2f) ? ("" + ch) : choices[Mathf.FloorToInt(Random.Range(0, choices.Length))];
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

    public string GetGoodPassword()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 2; i++)
        {
            sb.Append(l33tword(getRandomWord()));
        }
        return sb.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
