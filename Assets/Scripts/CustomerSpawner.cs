using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    private const float SCAN_CUSTOMERS_REFRASH_RATE = 2;

    public GameObject customerPrefab;    
    public DifficultyParams difficultyParams;
    int currentWave = 0;

    void Start()
    {
        StartCoroutine( SpawnCustomers() );
    }

    private IEnumerator SpawnCustomers()
    {
        while( true )
        {
            if( Constants.bGameOver )
            {
                yield return null;
            }

            if( difficultyParams.gameMode == GameMode.Normal &&
                currentWave > difficultyParams.numberOfWaves )
            {
                yield break;
            }

            currentWave++;

            float gameStage = currentWave / difficultyParams.numberOfWaves;
            gameStage = ModeGameStage( gameStage, difficultyParams.gameMode );

            float timeBetweenWaves = difficultyParams.timeBetweenWaves.Evaluate(gameStage);
            float timeVariation = difficultyParams.timeBetweenWavesVariation.Evaluate(gameStage);

            timeBetweenWaves += Variation( timeVariation );

            float waveSize = difficultyParams.waveSize.Evaluate(gameStage);
            float waveSizeVariation = difficultyParams.waveSizeVariation.Evaluate(gameStage);
            timeBetweenWaves += Variation( waveSizeVariation );

            float spawnSpeed = difficultyParams.waveSpawnSpeed.Evaluate(gameStage);
            float spawnSpeedVariation = difficultyParams.waveSpawnSpeedVariation.Evaluate(gameStage);

            for( int i = 0; i < waveSize; i++ )
            {
                if( Constants.bGameOver )
                {
                    yield return null;
                }
                float variation = Variation( spawnSpeedVariation );
                yield return new WaitForSeconds( 1 / (spawnSpeed + variation) );
                SpawnCustomer();                                  
            }


            yield return new WaitForSeconds( timeBetweenWaves );

            int numCustomers = 100;
            while( numCustomers > difficultyParams.minCustomerCount )
            {
                numCustomers = FindObjectsOfType<CustomerAI>().Length;
                yield return new WaitForSeconds( SCAN_CUSTOMERS_REFRASH_RATE );
            }
        }
    }

    private float ModeGameStage( float gameStage, GameMode gameMode )
    {
        switch( gameMode )
        {
            case GameMode.Endless:
                return Mathf.Clamp01( gameStage );
            case GameMode.EndlessRepeat:
                return Mathf.Repeat( gameStage, 1 );
            default:
                return gameStage;
        }
    }

    private void SpawnCustomer()
    {
        Instantiate( customerPrefab, transform.position, transform.rotation );
    }

    private float Variation( float variation )
    {
        return (Random.value * 2 - 1) * variation;
    }
}
