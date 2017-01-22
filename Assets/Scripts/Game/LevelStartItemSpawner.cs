using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStartItemSpawner : MonoBehaviour {

    public List<GameObject> listOfFoodItemPrefabs = new List<GameObject>();
    private Transform[] randomSpawnpoints;

    private List<int> listOfPreviousSpawnPointIndices = new List<int>();

    private int iStaggeredFrameIndex = 0;
    private bool bStopSpawning = false;
    private int iTotalMade = 0;

	//spawns items in random locations
	void Start ()
    {
        //Don't use the first transform
		randomSpawnpoints = gameObject.GetComponentsInChildren<Transform>();

        int iRandomPointOne = Random.Range(1, (randomSpawnpoints.Length/2)-1);
        int iRandomPointTwo = Random.Range(randomSpawnpoints.Length/2, randomSpawnpoints.Length-1);

        int iRandomItemOne = Random.Range(0, listOfFoodItemPrefabs.Count-1);
        int iRandomItemTwo = Random.Range(0, listOfFoodItemPrefabs.Count-1);

       // GameObject go = (GameObject)Instantiate(listOfFoodItemPrefabs[iRandomItemOne], randomSpawnpoints[iRandomPointOne]);
        //go.transform.localPosition = new Vector3(0,0,0);

        //GameObject goTwo = (GameObject)Instantiate(listOfFoodItemPrefabs[iRandomItemTwo], randomSpawnpoints[iRandomPointTwo]);
        //goTwo.transform.localPosition = new Vector3(0,0,0);

        int iMod = randomSpawnpoints.Length / (listOfFoodItemPrefabs.Count * 2);
        int iMin, iMax;

        iMin = 0;
        iMax = iMod;

        int iIndex = 0;

        for(int item = 0; item < listOfFoodItemPrefabs.Count; item++)
        {
            for(int i = 0; i < 2; i++)
            {
                int iSpawnPoint = Random.Range(iMin, iMax);
                Debug.Log("ITEM " + item + "(" + i + ") min: " + iMin + ", max: " + iMax + ", actual: " + iSpawnPoint);

                if(iSpawnPoint > -1 && iSpawnPoint < randomSpawnpoints.Length)
                {
                    GameObject go = (GameObject)Instantiate(listOfFoodItemPrefabs[iIndex], randomSpawnpoints[iSpawnPoint]);
                    go.transform.localPosition = new Vector3(0,0,0);
                }

                iMin += iMod;
                iMax += iMod;
                iIndex++;

                if(iIndex >= listOfFoodItemPrefabs.Count)
                    iIndex = 0;
            }
        }










	}
	
	
	//void Update ()
 //   {
 //       if(bStopSpawning)
 //       {
 //           return;
 //       }


	//	if(iStaggeredFrameIndex < listOfFoodItemPrefabs.Count)
 //       {
 //           int iNumOfThisItemToCreate = 1;//Random.Range(1, 2);
 //           int iRandSpawnDivision = randomSpawnpoints.Length / listOfFoodItemPrefabs.Count;

 //           int iSpawnPointMin = iStaggeredFrameIndex == 0 ? 1 : iRandSpawnDivision * iStaggeredFrameIndex;
 //           int iSpawnPointMax = iRandSpawnDivision * (iStaggeredFrameIndex+1);
 //           int iRandSpawnPoint;

 //           iSpawnPointMin = Mathf.Clamp(iSpawnPointMin, 0, randomSpawnpoints.Length - iRandSpawnDivision);
 //           iSpawnPointMax = Mathf.Clamp(iSpawnPointMax, iRandSpawnDivision, randomSpawnpoints.Length);

 //           if( iSpawnPointMin > iSpawnPointMax )
 //               iSpawnPointMin = iSpawnPointMax / 2;

 //           iRandSpawnPoint = Random.Range(iSpawnPointMin, iSpawnPointMax);

 //           //already used this spawn point, just skip instead of holding up
 //           if(listOfPreviousSpawnPointIndices.Contains(iRandSpawnPoint))
 //           {
 //               iStaggeredFrameIndex++;
 //               return;
 //           }

 //           for(int i = 0; i < iNumOfThisItemToCreate; i++)
 //           {
 //               if(iRandSpawnPoint + i < randomSpawnpoints.Length)
 //               {
 //                   GameObject go = (GameObject)Instantiate(listOfFoodItemPrefabs[iStaggeredFrameIndex], randomSpawnpoints[iRandSpawnPoint + i]);
 //                   go.transform.localPosition = new Vector3(0,0,0);
 //                   listOfPreviousSpawnPointIndices.Add(iRandSpawnPoint + i);
 //                   iTotalMade++;

 //                   if(iTotalMade == 2)
 //                   {
 //                       bStopSpawning = true;
 //                       return;
 //                   }
 //               }
 //               else break;
 //           }

 //           iStaggeredFrameIndex++;
 //       }
	//}
}
