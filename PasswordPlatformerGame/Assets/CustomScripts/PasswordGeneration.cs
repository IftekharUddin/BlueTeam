﻿using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Zxcvbn;
using System.Text;
using System;
using System.Diagnostics;
using System.Linq;


public class PasswordGeneration : MonoBehaviour
{

    private DifficultyUtility.Difficulty difficulty;
    public TextAsset goodPasswordsText, easyBadPasswordsText, mediumBadPasswordsText, hardBadPasswordsText;

    private static PasswordGeneration _instance;
    private List<string> goodPasswords = new List<string>();
    private List<string> easyBadPasswords = new List<string>();
    private List<string> mediumBadPasswords = new List<string>();
    private List<string> hardBadPasswords = new List<string>();
    private List<string> badPasswordsFromInternet = new List<string>();
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

        this.readTextAssetIntoList(this.goodPasswordsText, this.goodPasswords);
        this.readTextAssetIntoList(this.easyBadPasswordsText, this.easyBadPasswords);
        this.readTextAssetIntoList(this.mediumBadPasswordsText, this.mediumBadPasswords);
        this.readTextAssetIntoList(this.hardBadPasswordsText, this.hardBadPasswords);
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

    private string generateGoodPassword()
    {
        return this.getRandFromList(this.goodPasswords);
    }

    public string GetGoodPassword()
    {
        return this.generateGoodPassword();
    }
}
