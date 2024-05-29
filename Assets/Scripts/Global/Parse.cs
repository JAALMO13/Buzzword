using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class Parse : MonoBehaviour
{
    string path = "Assets/Other/TXT files/allWords.txt";
    // 26 for each letter 
    public static List<List<string>> allWords;
    string[] rawWordList;

    private void Awake()
    {
        readTextFile();
        getWordList();
    }

    private void readTextFile()
    {
        StreamReader reader = new StreamReader(path);
        string content = reader.ReadToEnd();
        rawWordList = content.Split("\n"[0]);
        reader.Close();
    }

    // separate words into lists of words beginning with the same character
    private void getWordList()
    {
        allWords = new List<List<string>>();
        List<string> words = new List<string>();
        char oldChar = 'a';
        for (int i = 0; i < rawWordList.Length; i++)
        {
            if (rawWordList[i][0] != oldChar)
            {

                oldChar = rawWordList[i][0];
                // print(oldChar);

                allWords.Add(new List<string>(words));
                // clear word
                words.Clear();
                // add word to new list
                words.Add(rawWordList[i]);
            }
            // add word to list
            else
            {
                words.Add(rawWordList[i]);
            }
        }
        // add z words to end of list
        allWords.Add(new List<string>(words));
        words.Clear();
    }

}
