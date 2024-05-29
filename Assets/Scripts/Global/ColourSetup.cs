using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourSetup : MonoBehaviour
{

    public int rows = 3;
    public int cols = 6;
    public float x_offset = 150;
    public float y_offset = 100;
    public float x_start = 0;
    public float y_start = 0;
    // Start is called before the first frame update
    void Start()
    {       
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float x = x_start + j * x_offset;
                float y = y_start + -i * y_offset;
                transform.GetChild(i * cols + j).localPosition = new(x, y);
            }
        }
    }
}
