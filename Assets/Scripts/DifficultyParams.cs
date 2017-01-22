using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    Normal,
    Endless,
    EndlessRepeat,    
}

[CreateAssetMenu(fileName ="DifficultyParams", menuName = "Difficulty Parameters")]
public class DifficultyParams : ScriptableObject
{
    public float numberOfWaves = 10;
    public int minCustomerCount = 2;
    public GameMode gameMode = GameMode.Normal;

    public AnimationCurve timeBetweenWaves;
    public AnimationCurve timeBetweenWavesVariation;    

    public AnimationCurve waveSize;
    public AnimationCurve waveSizeVariation;

    public AnimationCurve waveSpawnSpeed;
    public AnimationCurve waveSpawnSpeedVariation;    
}
