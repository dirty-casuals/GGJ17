using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeChildRotation : MonoBehaviour {

    Quaternion qRotation;

    void LateUpdate()
    {
        this.transform.rotation = qRotation;
    }

    // Use this for initialization
    void Start () {
        qRotation = this.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
