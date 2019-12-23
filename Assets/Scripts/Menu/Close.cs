using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Close : MonoBehaviour
{
	//public GameObject Open;
	public bool endanimation;
    public GameObject close;

    // Start is called before the first frame update
    public void Update(){
    	if(endanimation ==true){
    		endanimation = false;
        	close.SetActive(false);
        }
    }
   

}