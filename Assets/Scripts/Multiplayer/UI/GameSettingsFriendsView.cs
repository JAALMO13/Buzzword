using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using FirstGearGames.LobbyAndWorld.Lobbies;
using FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases;

public class GameSettingsFriendsView : View
{
    [SerializeField]
    Button playButton;
    [SerializeField]
    GameObject minLetters;
    [SerializeField]
    GameObject maxLetters;
    [SerializeField]
    GameObject timeLimit;
    [SerializeField]
    Slider timeSlider;
    [SerializeField]
    TextMeshProUGUI sliderVal;
    [SerializeField]
    Button backButton;
    [SerializeField]
    Button settingsButton;
    // * friends view
    [SerializeField]
    Button joinButton;
    // * friends view
    [SerializeField]
    TextMeshProUGUI password;

    [SerializeField]
    CanvasGroup canvasGroup;
    [SerializeField]
    Transform title;

    int selectedMin = 1;
    int oldMin = -1;
    int selectedMax = 6;
    int oldMax = -1;
    int selectedTime = 0;
    int oldTime = -2;
    bool buttonPressed = true;
    float oldVal = 0;

    [SerializeField]
    GameObject roomManager;

    public float moveDist = 200;

    public override void SetupAnimation()
    {
        canvasGroup.DOFade(0, 0);
        title.GetComponent<TextMeshProUGUI>().DOFade(1, 0);
        settingsButton.GetComponent<Image>().DOFade(1, 0);
        backButton.GetComponent<Image>().DOFade(1, 0);
        title.DOMoveY(title.position.y + moveDist, 0);
        settingsButton.transform.DOMoveX(settingsButton.transform.position.x + moveDist, 0);
        backButton.transform.DOMoveX(backButton.transform.position.x - moveDist, 0);
    }

    public override void Initialise()
    {
        base.Initialise();
        SetupAnimation();
        // roomManager = ObjectFinder.Instance.FindInAll("RoomManager");
        timeSlider.maxValue /= 5;
        playButton.interactable = false;
        playButton.onClick.AddListener(() =>
        {
            CreateRoom();
            AudioManager.Instance.PlayPressed();
        });
        backButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.Show<TitleView>();
            AudioManager.Instance.PlayPressed();
        });
        settingsButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.Show<SettingsView>();
            AudioManager.Instance.PlayPressed();
        });
        foreach (Transform child in minLetters.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                Selected(child, 0);
                AudioManager.Instance.PlayPressed();
            });
        }
        foreach (Transform child in maxLetters.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                Selected(child, 1);
                AudioManager.Instance.PlayPressed();
            });
        }
        foreach (Transform child in timeLimit.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                Selected(child, 2);
                AudioManager.Instance.PlayPressed();
            });
        }
        timeSlider.value = 60 * 0.2f;
        timeSlider.onValueChanged.AddListener((output) =>
        {
            buttonPressed = false;
            AudioManager.Instance.PlayMoved();
        });
        // * friends view
        joinButton.onClick.AddListener(() =>
        {
            StartCoroutine(ExitAnimation<PasswordView>());
            AudioManager.Instance.PlayPressed();
        });
        // * friends view
        password.text = Utility.GenerateRandomString(8, false);
    }

    public override void Update()
    {
        base.Update();
        // * friends view
        password.color = ColourMode.Instance.colour;
        joinButton.GetComponent<Outline>().effectColor = ColourMode.Instance.colour;
        if (ViewManager.Instance.currentView != this) return;
        if (selectedMin >= 0 && selectedMax >= 0 && (timeSlider.value > 0)) playButton.interactable = true; // or slider val is not zero
        else playButton.interactable = false;

        // highlighting when slider is equal to time on button
        if (!buttonPressed)
        {
            switch (timeSlider.value * 5)
            {
                case 60:
                    {
                        if (0 == selectedTime) break;

                        oldTime = selectedTime;
                        selectedTime = 0;
                        print(selectedTime);
                        break;
                    }
                case 120:
                    {
                        if (1 == selectedTime) break;

                        oldTime = selectedTime;
                        selectedTime = 1;
                        print(selectedTime);
                        break;
                    }
                case 180:
                    {
                        if (2 == selectedTime) break;

                        oldTime = selectedTime;
                        selectedTime = 2;
                        print(selectedTime);
                        break;
                    }
                default:
                    {
                        if (-1 == selectedTime) break;

                        oldTime = selectedTime;
                        selectedTime = -1;
                        print(selectedTime);
                        break;
                    }
            }

        }



        if (selectedMin >= 0)
        {
            minLetters.transform.GetChild(selectedMin).GetComponent<Image>().color = ColourMode.Instance.colour;
            minLetters.transform.GetChild(selectedMin).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            if (oldMin > -1)
            {
                minLetters.transform.GetChild(oldMin).GetComponent<Image>().color = ColourMode.Instance.darkLightGrey;
                minLetters.transform.GetChild(oldMin).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
            }
            for (int i = 0; i < selectedMin; i++)
            {
                maxLetters.transform.GetChild(i).GetComponent<Button>().interactable = false; // ! set interactable colour in inspector 
            }
            for (int j = selectedMin; j < maxLetters.transform.childCount; j++)
            {
                maxLetters.transform.GetChild(j).GetComponent<Button>().interactable = true; // ! set interactable colour in inspector 
            }
        }
        if (selectedMax >= 0)
        {
            maxLetters.transform.GetChild(selectedMax).GetComponent<Image>().color = ColourMode.Instance.colour;
            maxLetters.transform.GetChild(selectedMax).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            if (oldMax > -1)
            {
                maxLetters.transform.GetChild(oldMax).GetComponent<Image>().color = ColourMode.Instance.darkLightGrey;
                maxLetters.transform.GetChild(oldMax).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
            }
            if (!maxLetters.transform.GetChild(selectedMax).GetComponent<Button>().interactable)
            {
                maxLetters.transform.GetChild(selectedMax).GetComponent<Image>().color = ColourMode.Instance.darkLightGrey;
                maxLetters.transform.GetChild(selectedMax).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
                selectedMax = -1;
            }
        }
        if (selectedTime >= 0)
        {

            timeLimit.transform.GetChild(selectedTime).GetComponent<Image>().color = ColourMode.Instance.colour;
            timeLimit.transform.GetChild(selectedTime).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;

        }
        if (oldTime > -1)
        {
            timeLimit.transform.GetChild(oldTime).GetComponent<Image>().color = ColourMode.Instance.darkLightGrey;
            timeLimit.transform.GetChild(oldTime).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
        }

        sliderVal.transform.DOMoveX(timeSlider.transform.GetChild(2).GetChild(0).GetChild(0).position.x, 0);
        sliderVal.text = ToClock((int)timeSlider.value * 5);


        if (timeSlider.value == 0) timeSlider.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = ColourMode.Instance.darkLightGrey;
        else timeSlider.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = ColourMode.Instance.colour;
    }

    public void ValueChangeCheck(Slider slider)
    {
        if (slider.value > slider.minValue && slider.value < slider.maxValue)
        {
            float oldValue = Mathf.Round(slider.value);
            float newValue = Mathf.RoundToInt(slider.value / 5) * 5;
            float difference = newValue - oldValue;
            slider.value += difference;
        }
    }

    void Selected(Transform child, int type)
    {
        buttonPressed = true;
        timeSlider.onValueChanged.RemoveAllListeners();
        if (type == 0)
        {
            if (selectedMin == child.GetSiblingIndex()) return;
            oldMin = selectedMin;
            selectedMin = child.GetSiblingIndex();
        }

        else if (type == 1)
        {
            if (selectedMax == child.GetSiblingIndex()) return;
            oldMax = selectedMax;
            selectedMax = child.GetSiblingIndex();
        }

        else if (type == 2)
        {
            if (selectedTime == child.GetSiblingIndex()) return;
            oldTime = selectedTime;
            selectedTime = child.GetSiblingIndex();
            timeSlider.value = FromClock(timeLimit.transform.GetChild(selectedTime).GetChild(0).GetComponent<TextMeshProUGUI>().text) / 5;
            oldVal = timeSlider.value;

        }
        // if(timeSlider.value != oldVal && !once) timeSlider.onValueChanged.AddListener((output) => {
        //     buttonPressed = false;
        //     once = true;
        // });
    }

    string ToClock(int time)
    {
        int seconds = time % 60;
        int minutes = Mathf.FloorToInt(time / 60);
        if (minutes == 0) return seconds.ToString() + "s";
        return minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    int FromClock(string time)
    {
        if (time[time.Length - 1] == 's') return int.Parse(time[0].ToString() + time[1].ToString());
        int minutes = int.Parse(time[1].ToString());
        int seconds = int.Parse(time[3].ToString() + time[4].ToString());
        return minutes * 60 + seconds;
    }
    void CreateRoom()
    {
        // get settings
        int min = int.Parse(minLetters.transform.GetChild(selectedMin).GetChild(0).GetComponent<TextMeshProUGUI>().text);
        int max = int.Parse(maxLetters.transform.GetChild(selectedMax).GetChild(0).GetComponent<TextMeshProUGUI>().text);
        int time = (int)(timeSlider.value * 5);
        // create room
        // check if room available with same settings
        
        string roomID = Utility.GenerateRandomString(12);
        print("created room");
        Player.Instance.roomID = roomID;
        LobbyNetwork.CreateRoom(roomID, password.text, min, max, time);
        StartCoroutine(ExitAnimation<MatchView>());
    
        
    }

    public override IEnumerator Animate()
    {
        title.DOMoveY(title.position.y - moveDist, move / 3);
        settingsButton.transform.DOMoveX(settingsButton.transform.position.x - moveDist, move / 3);
        backButton.transform.DOMoveX(backButton.transform.position.x + moveDist, move / 3);
        yield return new WaitWhile(() => DOTween.IsTweening(title));
        canvasGroup.DOFade(1, move / 2);
    }

    IEnumerator ExitAnimation<TView>() where TView : View
    {
        title.GetComponent<TextMeshProUGUI>().DOFade(0, move / 2);
        settingsButton.GetComponent<Image>().DOFade(0, move / 2);
        backButton.GetComponent<Image>().DOFade(0, move / 2);
        canvasGroup.DOFade(0, move / 2);
        yield return new WaitUntil(() => !DOTween.IsTweening(canvasGroup));
        ViewManager.Instance.Show<TView>();
    }
}

