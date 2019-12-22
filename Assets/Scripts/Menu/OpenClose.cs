using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenClose : MonoBehaviour
{
	//public GameObject Open;
	public bool endanimation;
    public GameObject Open;
    public GameObject Close;

    // Start is called before the first frame update
    public void Update(){
    	if(endanimation ==true){
    		endanimation = false;
        	Open.SetActive(true);
        	Close.SetActive(false);
        }
    }
   

}
