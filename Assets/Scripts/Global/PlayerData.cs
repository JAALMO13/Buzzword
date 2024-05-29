using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstGearGames.LobbyAndWorld.Clients;

[System.Serializable]
// https://www.youtube.com/watch?v=XOjd_qU2Ido&t=325s Save system 
public class PlayerData
{
    // data to be stored
    public string username;
    public float colourR;
    public float colourG;
    public float colourB;
    public float colourModeR;
    public float colourModeG;
    public float colourModeB;
    public float musicVal;
    public float sfxVal;
    public PlayerData(){}
    public PlayerData(PlayerSettings player, Color _colourMode, AudioManager audio)
    {
        if (player != null)
        {
            username = player.GetUsername();
            colourR = player.GetColour()[0];
            colourG = player.GetColour()[1];
            colourB = player.GetColour()[2];
        }
        else{
            username = "";
            colourR = 0;
            colourG = 0;
            colourB = 0;
        }

        colourModeR = _colourMode[0];
        colourModeG = _colourMode[1];
        colourModeB = _colourMode[2];
        musicVal = audio.musicVal;
        sfxVal = audio.sfxVal;
    }
}
