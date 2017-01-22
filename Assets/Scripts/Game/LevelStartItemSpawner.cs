using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStartItemSpawner : MonoBehaviour {

    public List<GameObject> listOfFoodItemPrefabs = new List<GameObject>();
    private Transform[] randomSpawnpoints;

    private List<int> listOfPreviousSpawnPointIndices = new List<int>();

    private int iStaggeredFrameIndex = 0;

	//spawns items in random locations
	void Start ()
    {
        //Don't use the first transform
		randomSpawnpoints = gameObject.GetComponentsInChildren<Transform>();
	}
	
	
	void Update ()
    {
		if(iStaggeredFrameIndex < listOfFoodItemPrefabs.Count)
        {
            int iNumOfThisItemToCreate = Random.Range(1, 2);
            int iRandSpawnDivision = randomSpawnpoints.Length / listOfFoodItemPrefabs.Count;

            int iSpawnPointMin = iStaggeredFrameIndex == 0 ? 1 : iRandSpawnDivision * iStaggeredFrameIndex;
            int iSpawnPointMax = iRandSpawnDivision * (iStaggeredFrameIndex+1);
            int iRandSpawnPoint;

            iSpawnPointMin = Mathf.Clamp(iSpawnPointMin, 0, randomSpawnpoints.Length - iRandSpawnDivision);
            iSpawnPointMax = Mathf.Clamp(iSpawnPointMax, iRandSpawnDivision, randomSpawnpoints.Length);

            if( iSpawnPointMin > iSpawnPointMax )
                iSpawnPointMin = iSpawnPointMax / 2;

            iRandSpawnPoint = Random.Range(iSpawnPointMin, iSpawnPointMax);

            //already used this spawn point, just skip instead of holding up
            if(listOfPreviousSpawnPointIndices.Contains(iRandSpawnPoint))
            {
                iStaggeredFrameIndex++;
                return;
            }

            for(int i = 0; i < iNumOfThisItemToCreate; i++)
            {
                if(iRandSpawnPoint + i < randomSpawnpoints.Length)
                {
                    GameObject go = (GameObject)Instantiate(listOfFoodItemPrefabs[iStaggeredFrameIndex], randomSpawnpoints[iRandSpawnPoint + i]);
                    go.transform.localPosition = new Vector3(0,0,0);
                    listOfPreviousSpawnPointIndices.Add(iRandSpawnPoint + i);
                }
                else break;
            }

            iStaggeredFrameIndex++;
        }
	}
}
