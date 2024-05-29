using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FishNet;
using FirstGearGames.LobbyAndWorld.Clients;
using FishNet.Object;
using FishNet.Transporting;

public class SplashView : View
{
    [SerializeField]
    Image logo;
    public float wait = 3;
    [SerializeField]
    View signIn;
    [SerializeField]
    View title;
    PlayerData data;
    [SerializeField]
    Transform playersHolder;
    public bool allow = true;


    private void Start()
    {
        // SaveSystem.DeleteFile();
        // return;
        // data = new(null, ColourMode.Instance.colour, AudioManager.Instance);
        // if(SaveSystem.LoadFromJson<PlayerData>() == null){
        //     SaveSystem.SaveToJson(data);
        //     loaded = true;
        // } 
        // if(!loaded) data = SaveSystem.LoadFromJson<PlayerData>();
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
        InstanceFinder.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        DOTween.SetTweensCapacity(500, 50);
    }

    void ClientManager_OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started) StartCoroutine(Animate());
    }

    public override IEnumerator Animate()
    {
        yield return new WaitUntil(() => playersHolder.childCount > 0);
        yield return new WaitForSecondsRealtime(wait);
        // if first time logging in go to sign in page
        // Sign in View
        Player player = GetPlayer();
        if (string.IsNullOrEmpty(player.data.username) && !allow) // data != null && string.IsNullOrEmpty(data.username) &&
        {

            logo.transform.DOMoveY(Screen.height / 8 - 20, move);
            logo.transform.DOScale(0.6f, move + 0.5f);
            yield return new WaitUntil(() => !DOTween.IsTweening(logo.transform));
            yield return new WaitForSecondsRealtime(0.25f);
            Hide();
            transform.root.gameObject.SetActive(false);
            ViewManager.Instance.Show<SignInView>();
        }
        // Title view
        else
        {
            if (string.IsNullOrEmpty(player.data.username)) Debug.LogWarning("Player does not have name");
            // straight to home page
            logo.transform.DOLocalMoveY(415, move);
            yield return new WaitUntil(() => !DOTween.IsTweening(logo.transform));
            yield return new WaitForSecondsRealtime(0.25f);
            // move logo
            Hide();
            transform.root.gameObject.SetActive(false);
            ViewManager.Instance.Show<TitleView>();
        }
    }

    Player GetPlayer()
    {
        List<GameObject> players = ObjectFinder.Instance.GetPlayers();
        // compare players[i] with client
        foreach (var p in players)
        {
            if (p.GetComponent<NetworkObject>().IsOwner) return p.GetComponent<Player>();
        }
        return null;
    }
}
