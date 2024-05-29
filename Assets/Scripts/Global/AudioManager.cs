using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [HideInInspector]public float musicVal;
    [HideInInspector]public float sfxVal;
    // ! load from player data and set volume
    // https://www.youtube.com/watch?v=pbuJUaO-wpY 12:52
    private void Awake() {
        if(Instance == null){
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }
    
    public void PlayCorrect(){
        transform.GetChild(0).GetComponent<AudioSource>().Play();
    }
    public void PlayPressed(){
        transform.GetChild(1).GetComponent<AudioSource>().Play();
    }
    public void PlayMoved(){
        transform.GetChild(2).GetComponent<AudioSource>().Play();
    }
    public void PlayWrong(){
        transform.GetChild(3).GetComponent<AudioSource>().Play();
    }
}
