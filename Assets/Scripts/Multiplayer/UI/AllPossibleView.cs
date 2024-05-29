using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
public class AllPossibleView : View
{
    [SerializeField]
    TMP_FontAsset font;
    [SerializeField]
    private Transform scrollBarContent1;

    [SerializeField]
    private Transform scrollBarContent2;

    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    List<string> all = new();

    public override void Initialise()
    {
        quitButton.onClick.AddListener(() =>
        {
            // ! no host migration change to use a server instead of client 
            // remove client 
            // InstanceFinder.ClientManager.StopConnection();
            // remove server if no players
            ViewManager.Instance.ShowPrevious();
        });


        base.Initialise();
    }
    public override void Update()
    {
        base.Update();
        if (Player.Instance.allWords.Count > 0) all = Player.Instance.allWords.ToList();
        if (!once && all.Count > 0 && ViewManager.Instance.currentView.GetType().ToString() == "AllPossibleView")
        {
            once = true;
            scoreText.text = "Total Possible Score: " + Player.Instance.allWordsScore.ToString();
            for (int i = 0; i < all.Count; i++)
            {
                if (i % 2 == 0) Utility.CreateWord(scrollBarContent1, all[i], font);
                else Utility.CreateWord(scrollBarContent2, all[i], font);
            }
        }

    }
    public override void ResetView()
    {
        base.ResetView();
        for (int i = 0; i < scrollBarContent1.childCount; i++) Destroy(scrollBarContent1.GetChild(i).gameObject);
        for (int j = 0; j < scrollBarContent2.childCount; j++) Destroy(scrollBarContent2.GetChild(j).gameObject);
    }

}
