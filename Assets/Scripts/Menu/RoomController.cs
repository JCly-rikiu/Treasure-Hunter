using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex = 1; //Number for the build index to the multiplay scene.
    public int ready = 0;
    public GameObject Open;
    public GameObject Close;
    public GameObject Check;
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
        if(Check.activeSelf == false){
            Open.SetActive(true);
            Close.SetActive(false);
        }
        Debug.Log(" Room Name" + PhotonNetwork.CurrentRoom.Name);
    }
    public void IsReady(){
        ready = 1;
        PV.RPC("ReadyCount",RpcTarget.AllBuffered,ready);
    }
    public void NotReady(){
        ready = 0;
         PV.RPC("ReadyCount",RpcTarget.AllBuffered,ready);
    }
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public void StartGame() //Function for loading into the multiplayer scene.
    {   
        StaticClass.CrossSceneInformation = seed.text;
        PV.RPC("SendSeed",RpcTarget.AllBuffered,seed.text);
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }
    public void Update(){
        if(PhotonNetwork.CurrentRoom != null){
            if(PhotonNetwork.CurrentRoom.PlayerCount == 2){
                if(ready == 1){
                	lightgo.SetActive(true);
                	darkgo.SetActive(false);
                }else if (ready == 0){
                	lightgo.SetActive(false);
                	darkgo.SetActive(true);   
                }
            }else{
                lightgo.SetActive(false);
                darkgo.SetActive(true);
                ready = 0;
            }
        }
    }

    [PunRPC]
    void ReadyCount(int intoready){
        ready = intoready;
        Debug.Log("ready told");
    }
    [PunRPC]
    void SendSeed(string seedtext){
    	StaticClass.CrossSceneInformation = seedtext;	
    }

}


