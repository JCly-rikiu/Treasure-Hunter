using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private PhotonView PV;
	private CharacterController myCC;
	public float movementSpeed;
	public float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        myCC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PV.IsMine){
        	BasicMovement();
        	BasicRotation();
        }
    }
    void BasicMovement(){
    	if(Input.GetKey(KeyCode.T)){
    		myCC.Move(transform.forward * Time.deltaTime * movementSpeed);
    	}
    	if(Input.GetKey(KeyCode.F)){
    		myCC.Move(-transform.right * Time.deltaTime * movementSpeed);
    	}
    	if(Input.GetKey(KeyCode.G)){
    		myCC.Move(-transform.forward * Time.deltaTime * movementSpeed);
    	}
    	if(Input.GetKey(KeyCode.H)){
    		myCC.Move(transform.right * Time.deltaTime * movementSpeed);
    	}
    }

    void BasicRotation(){
    	float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
    	transform.Rotate(new Vector3(0,mouseX,0));
    }
}
