using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine.UI;

public class Letter : NetworkBehaviour
{
    public char character {get; private set;}
    public int id{get; private set;}
    [HideInInspector] public bool pressed = false;
    [HideInInspector] public int posID = -1;
    Button btn;
    private void Start() {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => {
            pressed = true;
            AudioManager.Instance.PlayPressed();
            AudioManager.Instance.PlayMoved();
        });
    }
    public void SetChar(char c){
        character = c;
        transform.GetChild(transform.childCount-1).GetComponent<TextMeshProUGUI>().text = c.ToString().ToUpper();
    }
    public void SetId(int i){
        id = i;
    }
    public void SetPosition(Vector3 pos){
        transform.position = pos;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show(){
        gameObject.SetActive(true);
    }

    public void Setup(){
        Game ins = Game.Instance;
        transform.SetParent(ins.lettersParent.transform);
        gameObject.layer = LayerMask.NameToLayer("UI");
    }
}
