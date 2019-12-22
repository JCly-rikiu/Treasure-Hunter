using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSetup : MonoBehaviour
{
    // This script will be added to any multiplayer scene
	public Transform InitialPositionPlayer1;
	public Transform InitialPositionPlayer2;

    void Start()
    {
        CreatePlayer(); //Create a networked player object for each player that loads into the multiplayer scenes.
    }

    private void CreatePlayer()
    {
        Debug.Log("Creating Player");
        if (PhotonNetwork.IsMasterClient ){
        	PhotonNetwork.Instantiate(Path.Combine("PlayerPrefab", "PlayerAvatar1"),
        		InitialPositionPlayer1.position, InitialPositionPlayer1.rotation,0);
        }else{
        	PhotonNetwork.Instantiate(Path.Combine("PlayerPrefab", "PlayerAvatar2"),
        		InitialPositionPlayer2.position, InitialPositionPlayer2.rotation,0);
        }
    }
    /*
    void Update(){
        if(PhotonNetwork.CurrentRoom.PlayerCount != 2){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
    */
}
