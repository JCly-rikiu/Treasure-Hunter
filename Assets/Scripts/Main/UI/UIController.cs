using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    
    public RectTransform timebar;
    public Text timetext;
    public RectTransform energybar;
    public Text energytext;
    public Text myscore;
    public Text otherscore;
    public GameObject lightkey;
    public GameObject Endmenu;
    public Text FinalMyScore;
    public Text FinalOtherScore;
    public GameObject WinTitle;
    public GameObject WinCrown;
    public GameObject LoseTitle;
    public GameObject LoseCrown;
    
    float timewidth = 0f;
    float timeheight = 0f;
    float currentwidth = 0;

    float energywidth = 0f;
    float energyheight = 0f;
    float currentheight = 0f;

    void Awake(){

        timewidth = timebar.sizeDelta.x;
        timeheight = timebar.sizeDelta.y;
        currentwidth = timewidth;

        energywidth = energybar.sizeDelta.x;
        energyheight = energybar.sizeDelta.y;
        currentheight = energyheight;
    }
    public void Start(){
        
    }
    public void endturn(){
        HexGameController.endTurn = true;

    }
    public void exitGame()
    {
        Application.Quit();
    }
    public void StartCounting(float curtime){
        
        
        currentwidth = Mathf.Lerp(0,timewidth, 1 - curtime );
        if(currentwidth < 0)
        {
            currentwidth = 0;
        }
        timebar.sizeDelta = new Vector2(currentwidth,timeheight);
        
        curtime = 15 - curtime * 15;
        int inttime = Mathf.CeilToInt(curtime);
        timetext.text = inttime.ToString();
    }
    public void EnergyCounting(int curenergy){
        
        energytext.text = curenergy.ToString();
        float floatenergy = (float)curenergy / 30;
        currentheight = Mathf.Lerp(0, energyheight,floatenergy );
        
        energybar.sizeDelta = new Vector2(energywidth, currentheight);
        
    }
    public void MyScore(int score){
        myscore.text = score.ToString();
    }
    public void OtherScore(int score){
        otherscore.text = score.ToString();;
    }
    public void GetKey(){
        lightkey.SetActive(true);
    }
    public void isWin(bool win){
        FinalMyScore.text = myscore.text;
        FinalOtherScore.text = otherscore.text;
        Endmenu.SetActive(true);
        if(win){
            WinTitle.SetActive(true);
            WinCrown.SetActive(true);
            
        }else{
            LoseTitle.SetActive(true);
            LoseCrown.SetActive(true);
            
        }
    }
    
}
