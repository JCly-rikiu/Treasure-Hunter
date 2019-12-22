using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communicate : MonoBehaviour
{	
	private PhotonView PV;
	public Transform MyPosition;
	public int takex;
	public float takey;
	public int takez;
	//public GameObject tmp2;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        /*
        if(PV.IsMine){
        	PV.RPC("tmp",RpcTarget.AllBuffered,takex,takey,takez);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Z)){
        	MyPosition.position = new Vector3 (takex,takey,takez);
        }
        if(Input.GetKey(KeyCode.C)){
        	if(PV.IsMine){
        		PV.RPC("tmp",RpcTarget.AllBuffered,takex,takey,takez);
       		}
       	}


    }

    [PunRPC]
    void tmp(int x,float y,int z){
    	takex = x;
    	takey = y;
    	takez = z; 
    }
}
