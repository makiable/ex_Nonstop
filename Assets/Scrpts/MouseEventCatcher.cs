using UnityEngine;
using System.Collections;

public class MouseEventCatcher : MonoBehaviour {

	public GameObject mReceiver;

	//실행할 메소드 명.
	public string mMethodName;
	public string inputString;

	void OnMouseDown(){ //층돌체가 존제하는 오브젝트라면 OnMouseDown을 선언하면 마우스 입력을 받을 수 있음.
		mReceiver.SendMessage (mMethodName, inputString);

		Debug.Log (inputString + "  hit key");
	}
}
