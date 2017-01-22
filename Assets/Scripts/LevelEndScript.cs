using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndScript : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
    private bool bSetEndScriptUp = false;

	// Update is called once per frame
	void Update ()
    {
		if(Constants.bGameOver && !bSetEndScriptUp)
        {
            bSetEndScriptUp = true;
            foreach( Transform child in transform )
            {
                if(child.tag == Constants.EndScoreUITag)
                {
                    child.GetComponent<Text>().text = "sCORE: " + GameObject.FindGameObjectWithTag(Constants.PlayerTag).GetComponent<PlayerController>().fPlayerScore;
                }

                child.gameObject.SetActive(true);
            }
        }
	}
}
