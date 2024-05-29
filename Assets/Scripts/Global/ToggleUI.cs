using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ToggleUI : MonoBehaviour
{
    public bool isOn = false;
    int dist = 60;
    public float move;
    Color defaultColour;
    Color32 temp;
    private void Start() {
        defaultColour = GetComponent<Image>().color;
    }
    private void Update() {
        
        if(Input.GetMouseButtonDown(0) && RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition)){
            isOn = !isOn;
            Animate();
        } 
        // colour background
        if(isOn) GetComponent<Image>().color = ColourMode.Instance.colour;
        else GetComponent<Image>().color = defaultColour;
    }

    void Animate(){
        if(isOn){
            transform.GetChild(0).DOMoveX(transform.GetChild(0).position.x + dist, move).SetEase(Ease.InCubic);
        } 
        else {
            transform.GetChild(0).DOMoveX(transform.GetChild(0).position.x - dist, move).SetEase(Ease.InCubic);
        }
    }
}
