using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerSettingsHolder : NetworkBehaviour
{
    public static PlayerSettingsHolder Instance;
    [field: SyncVar]
    public string username;
    [field: SyncVar]
    public Color colour;
    private void Awake() {
        Instance = this;
    }
}
