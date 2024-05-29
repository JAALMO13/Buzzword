using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour
{
    private void Update() {
        foreach(Transform child in transform) Destroy(child.gameObject);
    }
}
