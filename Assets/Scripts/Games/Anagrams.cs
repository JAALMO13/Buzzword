using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;
using Nobi.UiRoundedCorners;

public class Anagrams : Game
{

    [HideInInspector] public GameObject createdWordFront;
    [HideInInspector] public GameObject createdWordMid;
    [HideInInspector] public GameObject createdWordBack;
    [HideInInspector] public GameObject CreatedWordBin;
    [HideInInspector] public Button submit;
    [HideInInspector] public Button shuffle;
    [HideInInspector] public Camera cam;
    [HideInInspector] public TextMeshProUGUI scoreText;
    [HideInInspector] public TextMeshProUGUI wordCount;
    public float waitTime = 0.2f;
    // min letters (greyed out) positions
    [HideInInspector] public GameObject minLettersStatic;
    // letter positions
    [HideInInspector] public Transform LettersBoxStatic;
    public int size = 140;
    public int spacing = 40;
    public int newSpacing = 20;
    public int newSize = 120;
    public float move = 0.5f;
    int total;
    List<int> empty = new();

    public override void Initialise()
    {
        base.Initialise();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        createdWordFront = ObjectFinder.Instance.FindInAll("CreatedWordFront");
        createdWordMid = ObjectFinder.Instance.FindInAll("CreatedWordMid");
        createdWordBack = ObjectFinder.Instance.FindInAll("CreatedWordBack");
        CreatedWordBin = ObjectFinder.Instance.FindInAll("CreatedWordBin");
        submit = ObjectFinder.Instance.FindInAll("SubmitAnagram").GetComponent<Button>();
        shuffle = ObjectFinder.Instance.FindInAll("ShuffleAnagram").GetComponent<Button>();
        minLettersStatic = ObjectFinder.Instance.FindInAll("MinLettersStatic");
        LettersBoxStatic = ObjectFinder.Instance.FindInAll("StaticLetterBox").transform;
        scoreText = ObjectFinder.Instance.FindInAll("ScoreAnagram").GetComponent<TextMeshProUGUI>();
        wordCount = ObjectFinder.Instance.FindInAll("WordCountAnagram").GetComponent<TextMeshProUGUI>();

    }

    public override void Update()
    {
        base.Update();
        if (state == -1) return;
        if (state == 0 && GameManager.Instance.sharedWord != "" && ViewManager.Instance.currentView.GetType().ToString() == "AnagramGameView")
        {
            Transform lTransform = lettersParent.transform;
            for (int i = 0; i < lTransform.childCount; i++) letterPrefabs.Add(lTransform.GetChild(i).gameObject);
            StartCoroutine(InitialisePrefabs());
            state = 1;
        }

        // if createdword is more than minLength 
        submit.interactable = createdWordFront.transform.childCount >= minLength ? true : false;

        for (int i = 0; i < letterPrefabs.Count; i++)
        {
            Transform l = letterPrefabs[i].transform;
            if (l.GetComponent<Letter>().pressed)
            {
                l.GetComponent<Letter>().pressed = false;
                // # move from middle
                if (l.parent == createdWordFront.transform)
                {
                    // empty 
                    MoveLetter(l, lettersParent.transform, LettersBoxStatic);
                    int posid = l.GetComponent<Letter>().posID;
                    empty.Add(posid);
                    posid = -1;
                    CorrectCreatedWord();
                }
                // # move to middle
                else MoveLetter(l, createdWordFront.transform, null, createdWordFront.transform.childCount);
            }

        }


        if (gameOver && state == 1)
        {
            btnClear();
            foreach (Transform child in createdWordBack.transform) Destroy(child.gameObject);
            foreach (Transform child in createdWordMid.transform) Destroy(child.gameObject);
            foreach (Transform child in CreatedWordBin.transform) Destroy(child.gameObject);
            state = 2;
        }
    }

    void MoveLetter(Transform letter, Transform moveTo, Transform points, int index = -1, bool animate = false)
    {
        if (moveTo.name == createdWordFront.name && moveTo.childCount >= length) return;
        // sets parent of letter 
        // if no set index move id in array else move index
        int i = index < 0 ? letter.GetComponent<Letter>().id : index;

        // i is posid
        empty.Sort();
        if (empty.Count > 0 && index >= 0)
        {
            i = empty[0];
            empty.RemoveAt(0);
        }
        // null means created word else letter parent
        if (points == null)
        {
            if (moveTo.childCount >= minLength)
            {
                minLettersStatic.SetActive(false);
                letter.SetParent(moveTo.transform);
                letter.GetComponent<Letter>().posID = i;
                moveTo = SortMoveTo(moveTo);
                letter.GetComponent<Image>().color = ColourMode.Instance.lightGrey;
                CentreLetters(moveTo.transform, moveTo.childCount, size, spacing);
            }
            else
            {
                minLettersStatic.SetActive(true);
                letter.GetComponent<Letter>().posID = i;
                lettersParent.transform.GetChild(letter.GetSiblingIndex()).position = minLettersStatic.transform.GetChild(i).position;
                letter.GetComponent<Image>().color = ColourMode.Instance.lightGrey;
                letter.SetParent(moveTo.transform);
            }
            return;
        }
        letter.SetParent(moveTo.transform);
        letter.position = points.GetChild(i).position;
        letter.GetComponent<Image>().color = Color.white;
    }

    Transform SortMoveTo(Transform moveTo)
    {
        List<Transform> children = new();

        foreach (Transform child in moveTo) children.Add(child);

        children.Sort((a, b) => a.GetComponent<Letter>().posID.CompareTo(b.GetComponent<Letter>().posID));
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }
        return moveTo;
    }

    public void btnCheckWord()
    {
        submit.interactable = false;
        List<string> guess = new List<string>();
        int wordLen = createdWordFront.transform.childCount;
        // get word submitted 
        for (int i = 0; i < wordLen; i++) guess.Add(createdWordFront.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text.ToLower());

        bool correct = false;
        // check word
        string joinedGuess = string.Join("", guess);
        // word found
        if (Finder.Instance.validWord(joinedGuess, length, minLength) && !Finder.Instance.InUsedWords(joinedGuess))
        {
            // word found in list
            score += Calculator.WordTotal(joinedGuess, minLength);
            scoreText.text = score.ToString();
            total++;
            wordCount.text = total.ToString();
            Finder.Instance.AddWord(joinedGuess);
            // GameManager.Instance.AddWord(joinedGuess);
            Player.Instance.usedWords.Add(joinedGuess);
            print("Player Count: " + Player.Instance.usedWords.Count);
            // ! not white text
            correct = true;
            AudioManager.Instance.PlayCorrect();
            StartCoroutine(ChangeColours(ColourMode.Instance.colour, correct, true, true));
        }

        // not a word
        else if (!Finder.Instance.validWord(joinedGuess, length, minLength) && !Finder.Instance.InUsedWords(joinedGuess))
        {
            StartCoroutine(ChangeColours(ColourMode.Instance.exit, correct, true));
            var sequence = DOTween.Sequence();
            sequence.Append(createdWordFront.transform.DOLocalMoveX(createdWordFront.transform.localPosition.x + 20, move));
            sequence.Append(createdWordFront.transform.DOLocalMoveX(createdWordFront.transform.localPosition.x - 20, move));
            sequence.Append(createdWordFront.transform.DOLocalMoveX(createdWordFront.transform.localPosition.x, move));
            sequence.SetLoops(3);
            AudioManager.Instance.PlayWrong();
        }

        // already found
        else
        {
            // shake created word transform 
            var sequence = DOTween.Sequence();
            sequence.Append(createdWordFront.transform.DOLocalMoveX(createdWordFront.transform.localPosition.x + 20, move));
            sequence.Append(createdWordFront.transform.DOLocalMoveX(createdWordFront.transform.localPosition.x - 20, move));
            sequence.Append(createdWordFront.transform.DOLocalMoveX(createdWordFront.transform.localPosition.x, move));
            sequence.SetLoops(3);
            AudioManager.Instance.PlayWrong();
        }


        // ! if correct
        if (correct)
        {
            // ! move letters up to new parent
            // ! try instantiation should work as other player does not need to see this
            // ! use centre
            List<Transform> children = new();
            foreach (Transform child in CreatedWordBin.transform) children.Add(child);
            for (int i = children.Count - 1; i >= 0; i--) Destroy(CreatedWordBin.transform.GetChild(i).gameObject);


            foreach (Transform child in createdWordBack.transform) children.Add(child);
            for (int i = children.Count - 1; i >= 0; i--)
            {
                children[i].GetComponent<Image>().DOFade(0, move * 5);
                children[i].DOScale(Vector3.zero, move * 5);
                children[i].DOMoveY(CreatedWordBin.transform.position.y, move * 5);
                children[i].SetParent(CreatedWordBin.transform);
            }
            children.Clear();

            foreach (Transform child in createdWordMid.transform) children.Add(child);
            for (int i = 0; i < children.Count; i++)
            {
                Transform child = children[i];
                Color c = child.GetComponent<Image>().color;
                c.a = 0.3f;
                child.GetComponent<Image>().DOColor(c, move * 10);
                child.DOMoveY(createdWordBack.transform.position.y, move * 10);
                child.transform.DOScale(0.65f, move * 10);
                child.SetParent(createdWordBack.transform);
            }
            children.Clear();

            foreach (Transform child in createdWordFront.transform)
            {
                GameObject newChild = new("Letter_" + child.GetSiblingIndex().ToString());
                Image img = newChild.AddComponent<Image>();
                newChild.GetComponent<RectTransform>().sizeDelta = new(140, 140);
                ImageWithRoundedCorners corners = newChild.AddComponent<ImageWithRoundedCorners>();
                corners.radius = 50;
                corners.Refresh();
                TextMeshProUGUI text = new GameObject("Text").AddComponent<TextMeshProUGUI>();
                text.fontSize = 80;
                text.alignment = TextAlignmentOptions.Center;
                text.alignment = TextAlignmentOptions.Midline;
                text.font = child.GetChild(0).GetComponent<TextMeshProUGUI>().font;
                text.transform.SetParent(newChild.transform);
                text.text = child.GetComponent<Letter>().character.ToString().ToUpper();
                newChild.transform.position = createdWordFront.transform.GetChild(child.GetSiblingIndex()).position;
                newChild.transform.localScale = Vector3.one;
                newChild.transform.DOMoveY(createdWordMid.transform.position.y, move * 10);
                newChild.transform.DOScale(0.85f, move * 10);
                newChild.transform.SetParent(createdWordMid.transform);
                Color c = child.GetComponent<Image>().color;
                c.a = 0.6f;
                img.DOColor(c, move * 10);
            }
            StartCoroutine(WaitForTweens());
        }
        StartCoroutine(MoveLettersBack(guess, correct));
    }

    IEnumerator WaitForTweens()
    {
        yield return new WaitWhile(() => DOTween.IsTweening(createdWordFront.transform));
        yield return new WaitWhile(() => DOTween.IsTweening(createdWordMid.transform));
        yield return new WaitWhile(() => DOTween.IsTweening(createdWordBack.transform));
        CentreLetters(createdWordMid.transform, createdWordMid.transform.childCount, size * 0.85f, spacing);
        CentreLetters(createdWordBack.transform, createdWordBack.transform.childCount, size * 0.65f, spacing);
    }

    IEnumerator MoveLettersBack(List<string> guess, bool correct)
    {
        if (!correct) yield return new WaitForSeconds(waitTime);
        // move letters back to original position 
        for (int i = guess.Count - 1; i >= 0; i--) MoveLetter(createdWordFront.transform.GetChild(i), lettersParent.transform, LettersBoxStatic);
        minLettersStatic.SetActive(true);
        submit.interactable = true;
    }

    IEnumerator ChangeColours(Color colour, bool correct, bool whiteText, bool permanent = false)
    {
        Color previous = createdWordFront.transform.GetChild(0).GetComponent<Image>().color;
        foreach (Transform t in createdWordFront.transform) t.GetComponent<Image>().color = colour;
        if (whiteText)
        {
            foreach (Transform t in createdWordFront.transform) t.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        if (!correct) yield return new WaitForSeconds(waitTime);
        if (!permanent) foreach (Transform t in createdWordFront.transform) t.GetComponent<Image>().color = previous;
        foreach (Transform t in createdWordFront.transform) t.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
    }

    public void btnClear()
    {
        for (int i = createdWordFront.transform.childCount - 1; i >= 0; i--) MoveLetter(createdWordFront.transform.GetChild(i), lettersParent.transform, LettersBoxStatic);
        minLettersStatic.SetActive(true);
    }

    public string btnShuffle()
    {
        // TODO animate this
        // * move all to middle wait then go 

        // disable shuffle until finished
        shuffle.interactable = false;

        btnClear();

        word = Utility.Shuffle(word);

        StartCoroutine(MoveFromCentre(0.5f));

        return word;
    }

    IEnumerator MoveFromCentre(float wait)
    {
        for (int i = 0; i < letterPrefabs.Count; i++)
        {
            letterPrefabs[i].transform.SetParent(LettersBoxStatic.parent);
            letterPrefabs[i].transform.DOMove(LettersBoxStatic.transform.position, move * 3);
            letterPrefabs[i].GetComponent<Letter>().SetId(-1);
        }

        foreach (Transform child in LettersBoxStatic) child.gameObject.SetActive(false);
        yield return new WaitForSeconds(wait);
        for (int i = 0; i < word.Length; i++)
        {
            for (int j = 0; j < letterPrefabs.Count; j++)
            {
                string text = letterPrefabs[j].transform.GetChild(letterPrefabs[j].transform.childCount - 1).GetComponent<TextMeshProUGUI>().text;
                if (text.ToLower() == word[i].ToString().ToLower() && letterPrefabs[j].GetComponent<Letter>().id < 0)
                {
                    letterPrefabs[j].GetComponent<Letter>().SetId(i);
                    letterPrefabs[j].gameObject.SetActive(true);
                    MoveLetter(letterPrefabs[j].transform, lettersParent.transform, LettersBoxStatic);
                    break;
                }
            }
        }
        foreach (Transform child in LettersBoxStatic) child.gameObject.SetActive(true);
        for (int i = 0; i < word.Length; i++) LettersBoxStatic.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = word[i].ToString().ToUpper();
        shuffle.interactable = true;
    }

    IEnumerator InitialisePrefabs()
    {
        yield return new WaitForSeconds(0.5f);
        if (online) word = GameManager.Instance.sharedWord;
        word = Utility.Shuffle(word);
        // create letterPrefabs
        yield return new WaitUntil(() => letterPrefabs.Count > 0);
        for (int i = 0; i < letterPrefabs.Count; i++)
        {
            Letter l = letterPrefabs[i].GetComponent<Letter>();
            l.Setup();
            l.SetChar(word[i]);
            l.SetPosition(LettersBoxStatic.GetChild(i).position);
            l.SetId(i);
            l.Hide();
        }
        for (int i = 0; i < word.Length; i++) LettersBoxStatic.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = word[i].ToString().ToUpper();
        yield return new WaitForSeconds(0.25f);
        for (int j = 0; j < letterPrefabs.Count; j++) letterPrefabs[j].GetComponent<Letter>().Show();
        Player.Instance.canStart = true;
    }

    void CorrectCreatedWord()
    {
        if (createdWordFront.transform.childCount < minLength) minLettersStatic.SetActive(true);
        if (createdWordFront.transform.childCount >= minLength)
        {
            CentreLetters(createdWordFront.transform, createdWordFront.transform.childCount, size, spacing);
            minLettersStatic.SetActive(false);
        }
    }

    public void CentreLetters(Transform parentObj, int _length, float _size, int _spacing)
    {
        float m = (float)newSize / (float)size;
        for (int i = 0; i < parentObj.childCount; i++)
        {
            if (_length == 8) parentObj.transform.GetChild(i).localScale = new(m, m, m);
            else parentObj.transform.GetChild(i).localScale = Vector3.one;
            if (i < _length) parentObj.transform.GetChild(i).gameObject.SetActive(true);
            else parentObj.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_length >= 7) _spacing = newSpacing;
        if (_length == 8) _size = newSize;
        int mod = _length % 2;
        float start = (1 - mod) * (_size / 2 + _spacing / 2);
        List<Vector3> points = new();
        if (mod == 1) points.Add(new(0, 0, 0));
        for (int i = 0; i < Mathf.FloorToInt((_length - mod) / 2); i++)
        {
            float x = start + _size * (i + mod) + _spacing * (i + mod);
            points.Add(new(x, 0, 0));
            points.Add(new(-x, 0, 0));
        }
        points.Sort((a, b) => a.x.CompareTo(b.x));
        for (int i = 0; i < points.Count; i++)
        {
            parentObj.transform.GetChild(i).localPosition = points[i];
        }
    }

}
