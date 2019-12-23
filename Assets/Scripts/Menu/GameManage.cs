using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManage : MonoBehaviour
{
    // Start is called before the first frame update
    public void SetRes1920(){
    	Screen.SetResolution(1920,1080,false);
    }
    public void SetRes1600(){
    	Screen.SetResolution(1600,900,false);
    }
    public void SetRes1280(){
    	Screen.SetResolution(1289,720,false);
    }
    public void FullScreen(){
    	 Screen.fullScreen = !Screen.fullScreen;
    }

}

