using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ! use this script for algorithms for game modes
public class WordGenerator
{
    // generate random word
    public static string randomWord(int length){
        List<string> wordList = Finder.Instance.findAllWordsOfLength(length);
        // select random word from list
        int index = Random.Range(0, wordList.Count);
        return wordList[index].Trim();
    }
    
}
