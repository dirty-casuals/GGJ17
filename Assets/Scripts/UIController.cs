using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private TextMesh scoreText;

	void Start ()
    {
		foreach (Transform child in transform)
        {
            if (child.tag == Constants.ScoreUITag)
            {
                scoreText = child.GetComponent<TextMesh>();

                if(!scoreText)
                {
                    Debug.Break();
                }
                
                scoreText.text = "sCORE 0";
            }
        }
	}
	
	void Update ()
    {
		
	}

    public void UpdateScore(float fNewScore)
    {
        scoreText.text = "sCORE " + Mathf.FloorToInt(fNewScore);
    }
}
