using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public HexGameController hexcontroller;
    public GameObject timebar;
    public Text timetext;
    public GameObject energybar;
    public Text energytext;
    public Text myscore;
    public Text otherscore;
    public GameObject lightkey;

    public GameObject buff;
    public Text bufftext;
    public GameObject debuff;
    public Text debufftext;
    public GameObject Endmenu;
    public Text FinalMyScore;
    public Text FinalOtherScore;
    public GameObject WinTitle;
    public GameObject WinCrown;
    public GameObject LoseTitle;
    public GameObject LoseCrown;
    public GameObject SlotLock;

    public GameObject [] slots;
    public Item[] items;

    RectTransform energybartransform;
    Image energybarimage;
    RectTransform timebartransform;
    Image timebarimage;
    float timewidth = 0f;
    float timeheight = 0f;
    float currentwidth = 0;

    float energywidth = 0f;
    float energyheight = 0f;
    float currentheight = 0f;

    void Awake(){
        //gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
        UIInfo.boxisFull = false;
        UIInfo.changeisFull = false;
        UIInfo.poisonisFull = false;
        energybartransform = energybar.GetComponent<RectTransform>();
        energybarimage = energybar.GetComponent<Image>();
        timebartransform = timebar.GetComponent<RectTransform>();
        timebarimage = timebar.GetComponent<Image>();
        timewidth = timebartransform.sizeDelta.x;
        timeheight = timebartransform.sizeDelta.y;
        currentwidth = timewidth;

        energywidth = energybartransform.sizeDelta.x;
        energyheight = energybartransform.sizeDelta.y;
        currentheight = energyheight;
    }
    void Update(){

        if(Input.GetKeyDown(KeyCode.F1))
        {
            if(UIInfo.boxisFull == false)
            {
                UIInfo.boxisFull = true;

                Item newitem = Instantiate(items[0], slots[0].transform, false);
                newitem.hexcontroller = hexcontroller;
            }
        }
        if(Input.GetKeyDown(KeyCode.F2))
        {
            if(UIInfo.changeisFull == false)
            {
                UIInfo.changeisFull = true;
                Item newitem = Instantiate(items[1], slots[1].transform, false);
                newitem.hexcontroller = hexcontroller;
            }
        }
        if(Input.GetKeyDown(KeyCode.F3))
        {
            if(UIInfo.poisonisFull == false)
            {
                UIInfo.changeisFull = true;
                Item newitem = Instantiate(items[2], slots[2].transform, false);
                newitem.hexcontroller = hexcontroller;
            }
        }
        if(Input.GetKeyDown(KeyCode.F9)){
            hexcontroller.setTreasure();
        }

    }
    public void Start(){
        Endmenu.SetActive(false);
    }

    public void exitGame()
    {
        Application.Quit();
    }
    public void Buff(int round){
        if(round >= 1){
            bufftext.text = round.ToString();
            buff.SetActive(true);
        }else{
            bufftext.text = "";
            buff.SetActive(false);
        }

    }
    public void Debuff(int round){
        if(round >= 1){
            debufftext.text = round.ToString();
            debuff.SetActive(true);
        }else
        {
            debufftext.text = "";
            debuff.SetActive(false);
        }
    }
    public void StartCounting(float curtime){
        if(curtime == 1){
            SlotLock.SetActive(true);
        }
        if(curtime >= 0){
            SlotLock.SetActive(false);
            currentwidth = Mathf.Lerp(0,timewidth, 1 - curtime );
            if(currentwidth < 0)
            {
                currentwidth = 0;
            }
            timebartransform.sizeDelta = new Vector2(currentwidth,timeheight);

            curtime = 15 - curtime * 15;
            int inttime = Mathf.CeilToInt(curtime);
            timetext.text = inttime.ToString();

            if (curtime < 5){
                timebarimage.color = Color.red;
            }else{
                timebarimage.color = Color.white;
            }
        }else{
            SlotLock.SetActive(true);
            timetext.text = "";
            timebartransform.sizeDelta = new Vector2(timewidth,timeheight);
            timebarimage.color = Color.white;
        }

    }
    public void EnergyCounting(int curenergy){
        if ( curenergy >= 0 ){
            if(curenergy > 30){
                energybarimage.color = new Color(255,88,99,255);
            }else{
                energybarimage.color = Color.white;
            }
            energytext.text = curenergy.ToString();
            float floatenergy = (float)curenergy / 30;
            currentheight = Mathf.Lerp(0, energyheight,floatenergy );
            energybartransform.sizeDelta = new Vector2(energywidth, currentheight);
        }else {
            energytext.text = "";
            energybartransform.sizeDelta = new Vector2(energywidth,energyheight );
            energybarimage.color = Color.white;
        }


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
    public void LoseKey(){
        lightkey.SetActive(false);
    }
    public void isWin(bool win){

        Debug.Log("Gamesetiswin" + win);
        FinalMyScore.text = myscore.text;
        FinalOtherScore.text = otherscore.text;
        if(win){
            Invoke("showwin",5);
        }else{
            Invoke("showlose",5);
        }
    }
    void showwin(){
        Endmenu.SetActive(true);

        WinTitle.SetActive(true);
        WinCrown.SetActive(true);
    }
    void showlose(){
        Endmenu.SetActive(true);
        LoseTitle.SetActive(true);
        LoseCrown.SetActive(true);
    }
    public void GetItem(HexItemType type){
        if(type == HexItemType.FakeTreasureItem){
            if(UIInfo.boxisFull == false)
            {
                UIInfo.boxisFull = true;
                Item newitem = Instantiate(items[0], slots[0].transform, false);
                newitem.hexcontroller = hexcontroller;
            }
        }else if (type == HexItemType.Change){
            if(UIInfo.changeisFull == false)
            {
                UIInfo.changeisFull = true;
                Item newitem = Instantiate(items[1], slots[1].transform, false);
                newitem.hexcontroller = hexcontroller;
            }
        }else if (type == HexItemType.Poison){
            if(UIInfo.poisonisFull == false)
            {
                UIInfo.poisonisFull = true;
                Item newitem = Instantiate(items[2], slots[2].transform, false);
                newitem.hexcontroller = hexcontroller;
            }
        }
    }
}
