using UnityEngine;
using System.Collections;

public class Main_GameManager : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Use this for initialization
	void OnButtonDown(string trigger){
		if (trigger == "GameStart") {
			//mCameraControl.SetStatus(CameraControl.Status.Start);
			Invoke("StartButton",0.5f);
		}

		if (trigger == "EnterDungeun0001") {
			//mCameraControl.SetStatus(CameraControl.Status.Start);
			Application.LoadLevel("DungeunScene01");
		}

	}
	
	void StartButton(){
		Application.LoadLevel("Menu_default_Scene");
	}

}
