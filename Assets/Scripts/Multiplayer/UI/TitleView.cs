using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet;
using FishNet.Transporting;
using DG.Tweening;
using TMPro;

public class TitleView : View
{
    [SerializeField]
    private Button StartButton;
    [SerializeField]
    private Button FriendsButton;
    [SerializeField]
    private Button OfflineButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button StatsButton;
    [SerializeField]
    private Button AccountButton;

    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    View settings;
    [SerializeField]
    GameObject icon;
    [SerializeField]
    int moveStart = 120;
    
    public override void SetupAnimation(){
        StartButton.interactable = true;
        FriendsButton.interactable = true;
        StatsButton.interactable = true;
        AccountButton.interactable = true;
        settingsButton.interactable = true;
        icon.GetComponent<Image>().DOFade(1, 0);
        canvasGroup.alpha = 0;
        settingsButton.transform.DOMoveX(settingsButton.transform.position.x + moveStart, 0);
        StatsButton.transform.DOMoveX(StatsButton.transform.position.x + moveStart, 0);
        AccountButton.transform.DOMoveX(AccountButton.transform.position.x + moveStart, 0);
        settingsButton.GetComponent<Image>().DOFade(1, 0);
        StatsButton.GetComponent<Image>().DOFade(1, 0);
        AccountButton.GetComponent<Image>().DOFade(1, 0);
    }
    public override void Initialise()
    {
        base.Initialise();
        SetupAnimation();
        StartButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayPressed();
            StartCoroutine(ExitAnimation<GameSettingsView>());
        });

        FriendsButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayPressed();
            StartCoroutine(ExitAnimation<GameSettingsFriendsView>());
            
        });
        settingsButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayPressed();
            StartCoroutine(ExitAnimation<SettingsView>());
        });
        AccountButton.onClick.AddListener(() => SaveSystem.DeleteFile());
        // OfflineButton.onClick.AddListener(() => {}); // offline mode
    }

    public override void Update()
    {
        base.Update();
        FriendsButton.transform.GetComponent<Outline>().effectColor = ColourMode.Instance.colour;
        FriendsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = ColourMode.Instance.colour;
    }

    public override IEnumerator Animate()
    {
        canvasGroup.DOFade(1, move);
        settingsButton.transform.DOMoveX(settingsButton.transform.position.x - moveStart, move);
        yield return new WaitWhile(() => DOTween.IsTweening(settingsButton.transform));
        StatsButton.transform.DOMoveX(StatsButton.transform.position.x - moveStart, move);
        yield return new WaitWhile(() => DOTween.IsTweening(StatsButton.transform));
        AccountButton.transform.DOMoveX(AccountButton.transform.position.x - moveStart, move);
        yield return new WaitWhile(() => DOTween.IsTweening(AccountButton.transform));
    }
    
    
    IEnumerator ExitAnimation<TView>() where TView : View
    {
        StartButton.interactable = false;
        FriendsButton.interactable = false;
        StatsButton.interactable = false;
        AccountButton.interactable = false;
        settingsButton.interactable = false;

        icon.GetComponent<Image>().DOFade(0, move / 2);
        settingsButton.GetComponent<Image>().DOFade(0, move/2);
        StatsButton.GetComponent<Image>().DOFade(0, move/2);
        AccountButton.GetComponent<Image>().DOFade(0, move/2);
        canvasGroup.DOFade(0, move / 2);
        yield return new WaitUntil(() => !DOTween.IsTweening(canvasGroup));
        ViewManager.Instance.Show<TView>();
    } 
}
