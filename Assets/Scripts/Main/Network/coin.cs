using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject win;
    public GameObject lose;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other){
    	Debug.Log("collide");
    	Debug.Log(other.name);
    	if(other.name == "PlayerAvatar1(Clone)"){
    		if (PhotonNetwork.IsMasterClient ){
    			win.SetActive(true);
    			
    		}else{
    			lose.SetActive(true);
    			
    		}
    		
    	}else if (other.name == "PlayerAvatar2(Clone)"){
    		if (PhotonNetwork.IsMasterClient ){
    			lose.SetActive(true);
    			
    		}else{
    			win.SetActive(true);
    			
    		}
    		
    	}
    	
    }

}
