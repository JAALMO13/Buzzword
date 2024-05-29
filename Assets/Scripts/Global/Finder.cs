using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using DigitalRuby.Threading;

public class Finder : MonoBehaviour
{
    public static Finder Instance {get; private set;}
    //  string path = Application.persistentDataPath + "/WordList/";
    public  List<string> usedWords = new List<string>();
    public  List<string> allPossibleWords = new();
    private void Awake() {
        Instance = this;
    }
    // finding if word entered exists
    public  bool validWord(string word, int length, int minLength = 2)
    {
        if (word.Length > length || word.Length < minLength)
        {
            return false;
        }
        // find first letter
        char first = word[0];
        // index for list beginning with first
        int index = first - 'a';
        // search for word in list
        List<string> letterList = Parse.allWords[index];
        for (int i = 0; i < letterList.Count; i++)
        {
            if (letterList[i].Trim().ToLower() == word.Trim().ToLower())
            {
                return true;
            }
        }
        return false;

    }

    public  List<string> findAllWordsOfLength(int len)
    {
        List<string> wordsOfLen = new List<string>();
        List<List<string>> all = Parse.allWords;
        foreach (var l in all)
        {
            foreach (var word in l)
            {
                if (word.Length == len + 1)wordsOfLen.Add(word);
            }
        }
        return wordsOfLen;
    }

    public  bool InUsedWords(string word)
    {
        for (int i = 0; i < usedWords.Count; i++)
        {
            if (word == usedWords[i])
            {
                return true;
            }
        }
        return false;
    }

    public  void AddWord(string word)
    {
        usedWords.Add(word);
    }

    public  void GenAllPossible(string str)
    {
        for(int i = Game.Instance.minLength; i <= str.Length; i++) GeneratePermutations(str, i, "", allPossibleWords);
        List<string> temp = allPossibleWords.Distinct().ToList();
        allPossibleWords.Clear();
        foreach(var word in temp){
            if(validWord(word, str.Length, Game.Instance.minLength)) allPossibleWords.Add(word);
        }
    }

    public  void GeneratePermutations(string str, int range, string current, List<string> allPossibleWords)
    {
        if (current.Length == range)
        {
            allPossibleWords.Add(current);
            return;
        }
        for (int i = 0; i < str.Length; i++)
        {
            string newStr = str.Substring(0, i) + str.Substring(i + 1);
            GeneratePermutations(newStr, range, current + str[i], allPossibleWords);
        }
    }

    public  void ResetLists(){
        allPossibleWords.Clear();
        usedWords.Clear();
    }
}
