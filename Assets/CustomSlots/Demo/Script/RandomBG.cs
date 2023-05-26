using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBG : MonoBehaviour {

	public Sprite[] bgSprite = new Sprite[5];
	// Use this for initialization
	void Start () {
		int rand = Random.Range (0, 5);
		Debug.Log (rand);
		transform.GetComponent<UnityEngine.UI.Image> ().sprite = bgSprite[rand];
	}
}
