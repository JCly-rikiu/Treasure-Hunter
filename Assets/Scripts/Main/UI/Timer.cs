using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update
    float CurrentWidth = 0f;
    float StartWidth = 300f;
    public GameObject Bar;
    RectTransform rect;
    void Start()
    {
    	rect = Bar.GetComponent<RectTransform>();
        CurrentWidth = StartWidth;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentWidth -= 15 * Time.deltaTime;
        if(CurrentWidth < 0)
        {
        	CurrentWidth = 0;
        }
        rect.sizeDelta = new Vector2(CurrentWidth,80);
    }
}
