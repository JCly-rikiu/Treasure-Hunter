using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UI : MonoBehaviour
{
	public GameObject ExitMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
        	ExitMenu.SetActive(true);
         	//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
         }
    }



    public void BacktoMenu (){
        PhotonNetwork.LeaveRoom(); 
    	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
