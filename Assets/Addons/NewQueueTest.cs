using FishNet.Utility.Performance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewQueueTest : MonoBehaviour
{
    public bool Test;
    public int Count = 15;
    private BasicQueue<int> _bq = new BasicQueue<int>();

    private void Update()
    {
        if (Test)
        {
            Test = false;

            //_bq.Clear();

            for (int i = 0; i < Count; i++)
                _bq.Enqueue(i);

            Debug.Log("Pre dequeue " + _bq.Count);
            //_bq.Dequeue();
            _bq.Dequeue();
            Debug.Log("Post dequeue " + _bq.Count);
            
            if (Count > 10)
            {
                int c = _bq.Count;
                Debug.LogWarning("Showing results");
                for (int i = 0; i < c; i++)
                {
                    int result = _bq.Dequeue();
                    Debug.Log(result);
                }
            }
            Debug.Log("End " + _bq.Count);

        }
    }
}
