using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{
	public AudioSource Control;
    public AudioClip ClickSound;

    public void ClicktoPlay(){
    	Control.PlayOneShot(ClickSound);
    }
    /*
    void Update(){
    	if(Input.GetKeyDown(KeyCode.O)){
    		Control.PlayOneShot(ClickSound);
    	}
    }*/
    
}
