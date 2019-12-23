using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseC : MonoBehaviour
{
	//public GameObject Open;
	public bool endcloud;
    public GameObject close;
    public GameObject close2;
    public GameObject close3;
    public GameObject close4;
    public GameObject music;

    // Start is called before the first frame update
    public void Update(){
    	if(endcloud ==true){
    		endcloud = false;
        	close.SetActive(false);
        	close2.SetActive(false);
        	close3.SetActive(false);
        	close4.SetActive(false);
        	music.SetActive(true);
        }
    }
   

}