using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex = 1; //Number for the build index to the multiplay scene.
    public GameObject JoinGame;
    public GameObject CreatingMenu;
    public GameObject CreatingGame;
    public GameObject notEntered;
    public GameObject Entered;

    public Text seed;

    private PhotonView PV;

    public GameObject lightgo;
    public GameObject darkgo;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom() //Callback function for when we successfully create or join a room.
    {
        if (CreatingGame.activeSelf == false)
        {
            JoinGame.SetActive(true);
            CreatingMenu.SetActive(false);
        }
    }
    public override void OnCreatedRoom (){
        Debug.Log("Create Room successfully");
        CreatingMenu.SetActive(false);
        CreatingGame.SetActive(true);
    }
    public void IsReady()
    {
        MenuInfo.Ready = 1;
        PV.RPC("ReadyCount", RpcTarget.AllBuffered, MenuInfo.Ready);
    }
    public void NotReady()
    {
        MenuInfo.Ready = 0;
        PV.RPC("ReadyCount", RpcTarget.AllBuffered, MenuInfo.Ready);
    }
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public void StartGame() //Function for loading into the multiplayer scene.
    {
        MenuInfo.Seed = seed.text;
        PV.RPC("SendSeed", RpcTarget.AllBuffered, seed.text);
        PhotonNetwork.CurrentRoom.IsVisible = false; 
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }
    public void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
        	if(CreatingGame.activeSelf)
        	{
            	if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            	{

            		Entered.SetActive(true);
            		notEntered.SetActive(false);
                	if (MenuInfo.Ready == 1)
                	{	
                		Entered.SetActive(false);
                    	lightgo.SetActive(true);
                    	darkgo.SetActive(false);
                	}
                	else if (MenuInfo.Ready == 0)
                	{
                		Entered.SetActive(true);
                    	lightgo.SetActive(false);
                    	darkgo.SetActive(true);
                	}
            	}
            	else
            	{	
            		notEntered.SetActive(true);
            		Entered.SetActive(false);
                	lightgo.SetActive(false);
                	darkgo.SetActive(true);
                	MenuInfo.Ready = 0;
            	}
            }else if(JoinGame.activeSelf){

            	//Debug.Log("count of PlayerCount  " + PhotonNetwork.CurrentRoom.PlayerCount);
                //Debug.Log("is visible   " + PhotonNetwork.CurrentRoom.IsVisible);
                //Debug.Log("is open   " + PhotonNetwork.CurrentRoom.IsOpen);
            	if (PhotonNetwork.CurrentRoom.PlayerCount != 2){
                    PhotonNetwork.LeaveRoom();
                    PhotonNetwork.Disconnect();
                    PhotonNetwork.ConnectUsingSettings();
            		Debug.Log("LeaveROom");
            		JoinGame.SetActive(false);
            		CreatingMenu.SetActive(true);
            	}
                
            }
        }
    }

    [PunRPC]
    void ReadyCount(int intoready)
    {
        MenuInfo.Ready = intoready;
    }
    [PunRPC]
    void SendSeed(string seedtext)
    {
        MenuInfo.Seed = seedtext;
    }
}
