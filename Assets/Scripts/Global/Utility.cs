using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
public class Utility
{
    public static bool BoxInsideBox(Rect rect1, Rect rect2)
    {
        if (rect1.xMin <= rect2.xMax && rect1.xMax >= rect2.xMin &&
        rect1.yMin <= rect2.yMax && rect1.yMax >= rect2.yMin)
        {
            return true;
        }
        return false;
    }
    
    public static bool PointInsideBox(Vector2 pos, Rect rect){
        if((pos.x >= rect.xMin && pos.x <= rect.xMax) && (pos.y >= rect.yMin && pos.y <= rect.yMax)){
            return true;
        }
        return false;
    }

    public static void DrawRect(Rect rect)
    {
        Debug.DrawLine(new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMin, rect.yMin), new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMax, rect.yMin), Color.magenta);
        Debug.DrawLine(new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMin, rect.yMax), new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMax, rect.yMax), Color.magenta);
        Debug.DrawLine(new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMax, rect.yMin), new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMax, rect.yMax), Color.magenta);
        Debug.DrawLine(new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMin, rect.yMin), new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMin, rect.yMax), Color.magenta);
    }

    public static void DrawRect(Rect rect, Vector3 pos)
    {
        Debug.DrawLine(pos + new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMin, rect.yMin), pos + new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMax, rect.yMin), Color.magenta, 10f);
        Debug.DrawLine(pos + new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMin, rect.yMax), pos + new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMax, rect.yMax), Color.magenta, 10f);
        Debug.DrawLine(pos + new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMax, rect.yMin), pos + new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMax, rect.yMax), Color.magenta, 10f);
        Debug.DrawLine(pos + new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMin, rect.yMin), pos + new Vector3(rect.center.x, rect.center.y) - new Vector3(rect.xMin, rect.yMax), Color.magenta, 10f);
    }

    public static string Shuffle(string str)
    {
        char[] array = str.ToCharArray();
        System.Random rng = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
        return new string(array);
    }
    
    public static IEnumerator ShowLoading(float waitTime){
        ViewManager.Instance.Show<LoadingView>();
        yield return new WaitForSeconds(waitTime);
        ViewManager.Instance.ShowPrevious();
    }

    public static GameObject CreateWord(Transform parent, string word, TMP_FontAsset font){
        GameObject newWord = new GameObject("Word");
        TextMeshProUGUI text = newWord.AddComponent<TextMeshProUGUI>();       
        text.font = font;
        // upper case
        // newWord.GetComponent<TextMeshProUGUI>().text = word.ToUpper();
        text.text = Capitalise(word);
        text.color = Color.black;
        text.fontSize = 54;
        text.rectTransform.sizeDelta = new(text.preferredWidth, text.preferredHeight);
        newWord.transform.SetParent(parent, false);
        return newWord;
    }

    public static string Capitalise(string word){
        char[] charWord = word.ToCharArray();
        charWord[0] = word[0].ToString().ToUpper()[0];
        return string.Join("", charWord);
    }

    public static string GenerateRandomString(int length, bool captitals=true){
        
        const string glyphs =  "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; //add the characters you want
        string str = "";
        int start = 0;
        if(!captitals) start = 26;
        for(int i = 0; i < length; i++){
            str += glyphs[Random.Range(start, glyphs.Length)];
        }
        return str;
    }

    public static string[] SplitByUppercase(string s){
        return Regex.Split(s, @"(?<!^)(?=[A-Z])");
    }
}

