using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class GameManageMain : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Setmenu;
    public GameObject EndMenu;
    public void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            Setmenu.SetActive(true);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
         }
        if(PhotonNetwork.CurrentRoom.PlayerCount != 2){
            if(!EndMenu.activeSelf)
            {
                PhotonNetwork.LeaveRoom(); 
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }else{
                
            }
        }
        
    }
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
    public void Quit(){
        PhotonNetwork.LeaveRoom(); 
        PhotonNetwork.Disconnect();
        EndMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
     public void endturn(){
        HexGameController.endTurn = true;

    }
}

