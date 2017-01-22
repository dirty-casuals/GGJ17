using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSoundEffects : MonoBehaviour
{
    public AudioClip pickupSound;
    public AudioClip leaveSound;
    public AudioClip hitSound;
    public AudioClip tillSwipe;    

    AudioSource audioSource;

    public AudioSource stepsSource;

    IPawn pawn;

    void Start()
    {
        pawn = GetComponentInParent<IPawn>();
        audioSource = GetComponent<AudioSource>();
        pawn.onPickupItem += OnPickupItem;
        pawn.onLeaveItem += OnLeaveItem;
        pawn.onHit += OnHit;
    }    

    private void OnHit()
    {
        PlaySFX( hitSound );
    }

    private void OnLeaveItem()
    {
        PlaySFX( leaveSound );
    }

    private void OnPickupItem()
    {
        PlaySFX( pickupSound );
    }

    public void OnSwipeSoundEffectPlay()
    {
        PlaySFX( tillSwipe );
    }

    private void OnPlayStepSoundEffect()
    {
        stepsSource.Play();
    }

    private void PlaySFX( AudioClip clip )
    {
        if( !audioSource.isPlaying )
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
