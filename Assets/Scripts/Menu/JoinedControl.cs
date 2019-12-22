using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class JoinedControl : MonoBehaviour {
	public Button yourButton;
	public GameObject Open;
	public GameObject Close;
	
	void Start () {
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		if(PhotonNetwork.CurrentRoom != null){
			Debug.Log("Now in a room");
			Open.SetActive(true);
			Close.SetActive(false);
		}
		Debug.Log ("You have clicked the button!");
	}
}
