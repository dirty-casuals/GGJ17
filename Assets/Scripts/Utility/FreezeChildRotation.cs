using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeChildRotation : MonoBehaviour {

    Vector3 vLocalPosition;
    Quaternion qRotation;

    void LateUpdate()
    {
        this.transform.rotation = qRotation;
        this.transform.localPosition = vLocalPosition;
    }

    // Use this for initialization
    void Start () {
        qRotation = this.transform.rotation;
        vLocalPosition = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
