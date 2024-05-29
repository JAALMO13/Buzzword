using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using FirstGearGames.LobbyAndWorld.Lobbies;
using System;

// ! test
public class PasswordView : View
{
    [SerializeField]
    Button playButton;

    [SerializeField]
    TMP_InputField input;
    [SerializeField]
    Outline outline;

    [SerializeField]
    Button backButton;

    [SerializeField]
    Button exitButton;

    [SerializeField]
    GameObject prompt;

    [SerializeField]
    View friendsView;

    [SerializeField]
    GameObject roomManager;

    [SerializeField]
    TextMeshProUGUI incorrectPrompt;

    [SerializeField]
    CanvasGroup canvasGroup;

    // distance to move up
    [SerializeField]
    int dist = 300;

    float yVal;

    public override void SetupAnimation()
    {
        canvasGroup.alpha = 1;
        playButton.transform.parent.GetComponent<CanvasGroup>().alpha = 0;
        incorrectPrompt.alpha = 0;
        yVal = prompt.transform.localPosition.y;
    }

    public override void Initialise()
    {
        base.Initialise();
        move = 0.5f;
        SetupAnimation();
        Animate();
        playButton.onClick.AddListener(() =>
        {
            // get input.text
            // find if there is match in passworded rooms with same password
            // if not display some message
            bool isRoom = false;
            for (int i = 0; i < roomManager.transform.childCount; i++)
            {
                Room r = roomManager.transform.GetChild(i).GetComponent<Room>();
                if (input.text == r.password)
                {
                    // join this room
                    Player.Instance.roomID = r.roomName;
                    LobbyNetwork.JoinRoom(r.roomName, input.text);
                    StartCoroutine(ExitAnimation<MatchView>());
                }               
            }
            if(!isRoom){
                incorrectPrompt.alpha = 0;
                prompt.transform.DOLocalMoveY(yVal, 0);
                StartCoroutine(Incorrect());
            }
            AudioManager.Instance.PlayPressed();
        });
        exitButton.onClick.AddListener(() =>
        {

            ViewManager.Instance.Show<TitleView>();
            AudioManager.Instance.PlayPressed();
        });
        backButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.Show<GameSettingsFriendsView>();
            AudioManager.Instance.PlayPressed();
        });

        input.onSelect.AddListener(inp =>
        {
            ChangeOutline(new Color32(43, 43, 43, 255));
            AudioManager.Instance.PlayPressed();
        });
        input.onDeselect.AddListener(inp => ChangeOutline(new Color32(225, 225, 225, 255)));
    }

    IEnumerator Incorrect(){
        prompt.transform.DOLocalMoveY(yVal+dist, move);
        yield return new WaitUntil(() => !DOTween.IsTweening(prompt.transform));
        incorrectPrompt.DOFade(1, move);
        yield return new WaitForSeconds(1.5f);
        incorrectPrompt.DOFade(0, move);
        yield return new WaitUntil(() => !DOTween.IsTweening(incorrectPrompt));
        prompt.transform.DOLocalMoveY(yVal, move);
    }

    IEnumerator ExitAnimation<TView>() where TView : View
    {
        // animations 
        canvasGroup.DOFade(0, move);
        yield return new WaitUntil(() => !DOTween.IsTweening(canvasGroup));
        ViewManager.Instance.Show<TView>();
    }

    public override IEnumerator Animate()
    {
        playButton.transform.parent.GetComponent<CanvasGroup>().DOFade(1, move);
        yield return new WaitWhile(() => DOTween.IsTweening(playButton.transform.parent));
    }

    private void ChangeOutline(Color colour)
    {
        outline.effectColor = colour;
    }
}
