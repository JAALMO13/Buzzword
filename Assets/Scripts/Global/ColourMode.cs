using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourMode : MonoBehaviour
{
    public static ColourMode Instance { get; private set; }
    public Color colour;
    public Color32 exit = new(243, 82, 82, 255);
    public Color32 darkLightGrey = new(217, 217, 217, 255);
    public Color32 lightGrey = new(246, 246, 246, 255);
    public Color32 avatar;
    public Color32 black = new(43, 43, 43, 255);
    public Color avatarText;
    private void Awake()
    {
        // ! load data and set that colour
        Instance = this;
        PlayerData data = SaveSystem.LoadFromJson<PlayerData>();
        if (data == null)
        {
            colour = new Color32(102, 182, 255, 255);
            avatar = new Color();
        }
        else
        {
            colour = new Color(data.colourModeR, data.colourModeG, data.colourModeB);
            avatar = new Color(data.colourR, data.colourG, data.colourB);
        }
        
        if(avatar.Equals(new Color32(255, 246, 166, 255)) || avatar.Equals(new Color32(239, 239, 239, 255))){
            // first one is pale yellow 
            // may not keep black for it
            avatarText = black;
        }
        else avatarText = Color.white;
    }
}
