using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour
{
    static Dictionary<char, int> letterScore;

    private void Awake() {
        InitDict();
    }

    private void InitDict(){
        letterScore = new Dictionary<char, int>();
        int[] scores = {1,3,3,2,1,4,2,4,1,8,5,1,3,1,1,3,10,1,1,1,1,4,4,8,4,10};
        for (int i = 'a'; i < 'a'+26; i++){
            letterScore.Add((char) i, scores[i-'a']);
        }
    }

    public static int WordTotal(string word, int min){
        word = word.Trim();
        int count = 0;
        for (int i = 0; i < word.Length; i++){
            count += letterScore[word[i]];
        }
        return count + word.Length-min;
    }

    public static int WordTotalList(List<string> words, int min){
        int total = 0;
        foreach(var word in words) total += WordTotal(word, min);
        return total;
    }
    
}
