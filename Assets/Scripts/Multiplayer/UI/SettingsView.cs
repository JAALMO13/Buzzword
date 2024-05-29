using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using DG.Tweening;

public class SettingsView : View
{
    [SerializeField]
    AudioMixer mixer;

    // background music
    [SerializeField]
    private Slider bgSlider;

    // sound fx
    [SerializeField]
    private Slider soundSlider;

    [SerializeField]
    GameObject colours;

    [SerializeField]
    Transform selector;

    [SerializeField]
    Transform title;
    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    Outline outline;
    // exit menu
    [SerializeField]
    private Button exitButton;
    int selected = 4;

    public int moveDist = 130;


    public override void SetupAnimation(){
        canvasGroup.DOFade(1, 0);
        title.GetComponent<TextMeshProUGUI>().DOFade(1, 0);
        exitButton.GetComponent<Image>().DOFade(1, 0);
        title.DOMoveY(title.position.y + moveDist, 0);
        exitButton.transform.DOMoveX(exitButton.transform.position.x - moveDist, 0);
    }

    public override void Initialise()
    {
        base.Initialise();
        PlayerData data = SaveSystem.LoadFromJson<PlayerData>();
        if(data != null) {
            SetMusicVolume(data.musicVal);
            SetSFXVolume(data.sfxVal);
        }
        SetupAnimation();
        exitButton.onClick.AddListener(() =>
        {
            StartCoroutine(ExitAnimation());
        });

        foreach (Transform child in colours.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() => GetSelected(child));
        }

        bgSlider.onValueChanged.AddListener(SetMusicVolume);
        soundSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public override void Update()
    {
        base.Update();
        selector.position = colours.transform.GetChild(selected).position;
        ColourMode.Instance.colour = colours.transform.GetChild(selected).GetComponent<Image>().color;
        outline.effectColor = ColourMode.Instance.colour;
        if (bgSlider.value == 0) bgSlider.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = ColourMode.Instance.darkLightGrey;
        else bgSlider.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = ColourMode.Instance.colour;
        if (soundSlider.value == 0) soundSlider.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = ColourMode.Instance.darkLightGrey;
        else soundSlider.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = ColourMode.Instance.colour;
    }

    void GetSelected(Transform child)
    {
        selected = child.GetSiblingIndex();
    }

    void SetMusicVolume(float value)
    {
        var val = Mathf.Log10(value) * 20;
        mixer.SetFloat("MusicVolume", val);
        AudioManager.Instance.musicVal = val;
    }
    void SetSFXVolume(float value)
    {
        var val = Mathf.Log10(value) * 20;
        mixer.SetFloat("SFXVolume", val);
        AudioManager.Instance.sfxVal = val;
    }

    public override IEnumerator Animate()
    {
        title.DOMoveY(title.position.y - moveDist, move / 3);
        exitButton.transform.DOMoveX(exitButton.transform.position.x + moveDist, move / 3);
        yield return new WaitWhile(() => DOTween.IsTweening(title));
        canvasGroup.DOFade(1, move/2);
    }

    IEnumerator ExitAnimation() 
    {
        title.GetComponent<TextMeshProUGUI>().DOFade(0, move / 2);
        exitButton.GetComponent<Image>().DOFade(0, move / 2);
        title.GetComponent<TextMeshProUGUI>().DOFade(1, 0);
        exitButton.GetComponent<Image>().DOFade(1, 0);
        canvasGroup.DOFade(0, move / 2);
        yield return new WaitUntil(() => !DOTween.IsTweening(canvasGroup));
        ViewManager.Instance.ShowPrevious();
    }


}
