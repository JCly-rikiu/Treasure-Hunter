using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex = 1; //Number for the build index to the multiplay scene.
    public GameObject Open;
    public GameObject Close;
    public GameObject Check;
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
        if (Check.activeSelf == false)
        {
            Open.SetActive(true);
            Close.SetActive(false);
        }
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
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }
    public void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
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
        }
    }

    [PunRPC]
    void ReadyCount(int intoready)
    {
        MenuInfo.Ready = intoready;
        Debug.Log("ready told");
    }
    [PunRPC]
    void SendSeed(string seedtext)
    {
        MenuInfo.Seed = seedtext;
    }
}
