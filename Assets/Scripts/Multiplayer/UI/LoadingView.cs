using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingView : View
{
    public bool show = false;
    public override void Update()
    {
        base.Update();
        if(!show){
            for(int i = 0 ; i < transform.childCount; i++){
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else{
            for(int i = 0 ; i < transform.childCount; i++){
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
            
    }
}
