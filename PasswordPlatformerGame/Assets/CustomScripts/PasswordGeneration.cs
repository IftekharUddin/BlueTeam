using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Zxcvbn;
using System.Text;
using System;
using System.Diagnostics;
using System.Linq;

/// <summary> 
/// A singleton class which takes care of generating passwords. Originally this was done at run time, but now the passwords 
/// are loaded from a file, chosen randomly, and passed along to their destination. 
/// </summary>
public class PasswordGeneration : MonoBehaviour
{

    private DifficultyUtility.Difficulty difficulty;
    public TextAsset easyBadPasswordsText, easyGoodPasswordsText, mediumBadPasswordsText, mediumGoodPasswordsText, hardBadPasswordsText, hardGoodPasswordsText;

    private static PasswordGeneration _instance;
    private List<string> easyBadPasswords = new List<string>();
    private List<string> easyGoodPasswords = new List<string>();
    private List<string> mediumBadPasswords = new List<string>();
    private List<string> mediumGoodPasswords = new List<string>();
    private List<string> hardBadPasswords = new List<string>();
    private List<string> hardGoodPasswords = new List<string>();
    private Zxcvbn.Zxcvbn passwordChecker = new Zxcvbn.Zxcvbn();

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

        this.readTextAssetIntoList(this.easyBadPasswordsText, this.easyBadPasswords);
        this.readTextAssetIntoList(this.easyGoodPasswordsText, this.easyGoodPasswords);
        this.readTextAssetIntoList(this.mediumBadPasswordsText, this.mediumBadPasswords);
        this.readTextAssetIntoList(this.mediumGoodPasswordsText, this.mediumGoodPasswords);
        this.readTextAssetIntoList(this.hardBadPasswordsText, this.hardBadPasswords);
        this.readTextAssetIntoList(this.hardGoodPasswordsText, this.hardGoodPasswords);
    }

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
        // PlayerPrefs is the preferred way to pass (simple) data between scenes
        // https://docs.unity3d.com/ScriptReference/PlayerPrefs.html
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

    private string getRandFromList(List<string> lst)
    {
        return lst[Mathf.FloorToInt(UnityEngine.Random.value * lst.Count)];
    }

    private string generateBadPassword(DifficultyUtility.Difficulty diff)
    {
        if (diff == DifficultyUtility.Difficulty.EASY)
        {
            return this.getRandFromList(this.easyBadPasswords);
        }
        else if (diff == DifficultyUtility.Difficulty.MEDIUM)
        {
            return this.getRandFromList(this.mediumBadPasswords);
        }
        else
        {
            return this.getRandFromList(this.hardBadPasswords);
        }
    }

    public string GetBadPassword()
    {
        return this.generateBadPassword(this.difficulty);
    }

    private string generateGoodPassword(DifficultyUtility.Difficulty diff)
    {
        if (diff == DifficultyUtility.Difficulty.EASY)
        {
            return this.getRandFromList(this.easyGoodPasswords);
        }
        else if (diff == DifficultyUtility.Difficulty.MEDIUM)
        {
            return this.getRandFromList(this.mediumGoodPasswords);
        }
        else
        {
            return this.getRandFromList(this.hardGoodPasswords);
        }
    }

    public string GetGoodPassword()
    {
        return this.generateGoodPassword(this.difficulty);
    }
}
