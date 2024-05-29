using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AnagramGameView : View
{
    // submit
    [SerializeField]
    private Button submitButton;

    // shuffle
    [SerializeField]
    private Button shuffleButton;

    // clear
    [SerializeField]
    private Button clearButton;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI tenSeconds;
    float time;
    bool done = false;

    [SerializeField]
    Transform minLettersStatic;

    [SerializeField]
    Transform loading;

    [SerializeField]
    CanvasGroup canvasGroup;
    float timer = 0;
    int count = 1;
    Image[] ellipses;

    public override void SetupAnimation()
    {
        canvasGroup.alpha = 1;
        loading.GetComponent<Image>().color = Color.white;
        // loading.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 1;
        ellipses = loading.GetChild(1).GetComponentsInChildren<Image>();
        loading.transform.GetChild(0).GetComponent<Image>().DOFade(1, 0);
        for (int i = 0; i < loading.transform.GetChild(1).childCount; i++) loading.transform.GetChild(1).GetChild(i).GetComponent<Image>().DOFade(1, 0);
    }
    public override void Initialise()
    {
        base.Initialise();
        // SetupAnimation();
    }

    public override void Update()
    {
        base.Update();
        // timer += Time.deltaTime;

        if (!once)
        {
            SetupAnimation();
            Anagrams game = GameManager.Instance.game.GetComponent<Anagrams>();
            game.CentreLetters(minLettersStatic, game.minLength, 140, 40);
            // get btn methods
            clearButton.onClick.AddListener(() =>
            {
                game.btnClear();
                AudioManager.Instance.PlayPressed();
            });
            submitButton.onClick.AddListener(() =>
            {
                game.btnCheckWord();
                AudioManager.Instance.PlayPressed();
            });
            shuffleButton.onClick.AddListener(() =>
            {
                ShuffleButton(game);
                AudioManager.Instance.PlayPressed();
            });
            once = true;
        }
        if (Time.time - timer >= 0.3f && ellipses[0].color.a == 1)
        {
            
            timer = Time.time;
            count = count >= 3 ? 0 : count + 1;
            
            for (int i = 0; i < count; i++){
                ellipses[i].color = ColourMode.Instance.colour;
            } 
            for (int i = count; i < ellipses.Length; i++){
                ellipses[i].color = ColourMode.Instance.lightGrey;
            } 
        }
        time = Mathf.FloorToInt(Timer.current.getTime());
        string stringTime = time.ToString();
        if (Game.Instance.useTimer) timerText.text = string.Format("{0:D2}:{1:D2}", (int)time / 60, (int)time % 60);
        if (time <= 10)
        {
            timerText.color = ColourMode.Instance.exit;
            StartCoroutine(TenSeconds());
        }
        if (Timer.current.getTime() <= 0) StartCoroutine(ExitAnimation());
        scoreText.text = Game.Instance.score.ToString("D3");

    }

    public override void ResetView()
    {
        base.ResetView();
        clearButton.onClick.RemoveAllListeners();
        submitButton.onClick.RemoveAllListeners();
        shuffleButton.onClick.RemoveAllListeners();
    }

    void ShuffleButton(Anagrams game)
    {
        string word;
        word = game.btnShuffle();
    }

    IEnumerator TenSeconds()
    {
        if (!done)
        {
            done = true;
            tenSeconds.DOFade(1, move);
            yield return new WaitForSeconds(1);
            tenSeconds.DOFade(0, move);
        }
    }

    IEnumerator ExitAnimation()
    {
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => ViewManager.Instance.Show<GameOverView>());
        // yield return new WaitUntil(() => !DOTween.IsTweening(canvasGroup));
        yield return null;
    
    }
}
