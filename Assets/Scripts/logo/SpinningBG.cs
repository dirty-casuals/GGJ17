using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBG : MonoBehaviour {

    public float spinningSpeed = 55.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, spinningSpeed * Time.deltaTime));
	}
}
