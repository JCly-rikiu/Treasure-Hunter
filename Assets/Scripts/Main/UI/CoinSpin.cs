using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpin : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    public float rotatespeed = 60;
    void Update()
    {
        transform.Rotate(0,rotatespeed *Time.deltaTime,0);
    }
}
