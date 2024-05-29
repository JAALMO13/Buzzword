using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet;
public class GameModeView : View
{
    public List<string> games = new();
    public List<GameObject> buttons = new();
    public override void Initialise()
    {
        float max = 500;
        int total = games.Count;
        for (int i = 0; i < total; i++)
        {
            string gameName = string.Join(" ", Utility.SplitByUppercase(games[i]));
            // if total is greater than max on line change y cordinates
            GameObject newButton = new GameObject(gameName + " Button");
            RectTransform rect = newButton.AddComponent<RectTransform>();
            Image image = newButton.AddComponent<Image>();
            Button button = newButton.AddComponent<Button>();
            TextMeshProUGUI text = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            text.transform.SetParent(newButton.transform);
            text.text = gameName.Substring(0, gameName.Length - 4);
            text.fontSize = 64;
            text.color = Color.black;
            text.alignment = TextAlignmentOptions.Center;
            text.enableWordWrapping = false;
            newButton.transform.SetParent(transform);
            float y = (float)i / (total - 1) * max * 2 - max;
            if (float.IsNaN(y)) y = 0;
            rect.localPosition = new(0, y, 0);
            button.targetGraphic = image;
            rect.sizeDelta = new(text.preferredWidth + 25, text.preferredHeight + 25);
            string find = gameName; // .Substring(0, gameName.name.Length - 6);
            string gameObj = null;
            for (int j = 0; j < games.Count; j++)
            {
                if (find != games[j]) continue;
                gameObj = games[j];
                break;
            }
            buttons.Add(newButton);
        }

        foreach (var button in buttons)
        {
            button.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(button.GetComponent<Button>()));
        }

        base.Initialise();
    }

    void ButtonClicked(Button btn)
    {

        // Room check = new();
        // check.gameMode = find;
        if (btn.name.Contains("Default"))
        {
            // ! skip game settings view
            // go straight into lobby
            if (InstanceFinder.ServerManager.StartConnection())
            {
                print("starting server");
            }
            else
            {
                print("server started");
            }
            InstanceFinder.ClientManager.StartConnection();

        }
        else
        {
            // show game settings view
        }
        // if(RoomManager.Instance.rooms.Contains(check) && !RoomManager.Instance.rooms[RoomManager.Instance.rooms.IndexOf(check)].locked){
        //     RoomManager.Instance.AddPlayer(RoomManager.Instance.rooms[RoomManager.Instance.rooms.IndexOf(check)], Player.Instance);
        // }
        // else{
        //     Room room = RoomManager.Instance.AddRoom(find);
        //     RoomManager.Instance.AddPlayer(room, Player.Instance);
        // }
        // // gameObj.SetActive(true);
        // Player.Instance.gameSelected = find;
        // ViewManager.Instance.Show<GameSettingsView>();

    }
}
