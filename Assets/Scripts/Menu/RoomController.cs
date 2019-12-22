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

    private PhotonView PV;

    public Button go;

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
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }
    public void Update(){
        if(PhotonNetwork.CurrentRoom != null){
            if(PhotonNetwork.CurrentRoom.PlayerCount == 2){
                if(ready == 1){
                    go.interactable = true;
                }else if (ready == 0){
                    go.interactable = false;
                }
            }else{
                go.interactable = false;
                ready = 0;
            }
        }
    }

    [PunRPC]
    void ReadyCount(int intoready){
        ready = intoready;
        Debug.Log("ready told");
    }

}


