using UnityEngine;
using System.Collections;

public class LayerSort : MonoBehaviour {

	public enum Layers{
		BG,
		FG,
		EFFECT,
		UI,
		Menu,

	}

	//레이어 이름
	public Layers mLayerName;
	//레이어 오더.
	public int mOrderNumber = 0;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().sortingLayerName = mLayerName.ToString();
		GetComponent<Renderer>().sortingOrder = mOrderNumber;
	}
}
