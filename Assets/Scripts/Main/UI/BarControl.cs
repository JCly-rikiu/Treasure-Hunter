using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarControl : MonoBehaviour
{
    // Start is called before the first frame update
    float CurrentWidth = 0f;
    float CurrentHeight = 0f;
    float Totalwidth;
    float TotalHeight;
    public bool isWidth ;
    RectTransform rt;
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        Totalwidth = rt.sizeDelta.x;
        TotalHeight = rt.sizeDelta.y;
        CurrentWidth = Totalwidth;
        CurrentHeight = TotalHeight;
    }

    public void StartCounting(float curtime){
        if(isWidth)
        {
            CurrentWidth = Mathf.Lerp(0,Totalwidth, 1 - curtime );
            if(CurrentWidth < 0)
            {
                CurrentWidth = 0;
            }
            rt.sizeDelta = new Vector2(CurrentWidth, TotalHeight);
        }else
        {
            CurrentHeight = Mathf.Lerp(0, TotalHeight, 1 - curtime );
            if(CurrentHeight < 0)
            {
                CurrentHeight = 0;
            }
            rt.sizeDelta = new Vector2(Totalwidth, CurrentHeight);
        }
        
    }
}
