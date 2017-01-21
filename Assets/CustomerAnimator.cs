using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAnimator : MonoBehaviour
{
    private static string ANIM_SPEED = "Speed";
    private static string ANIM_PICKUP = "Pickup";
    private static string ANIM_SWIPE = "Swipe";
    private static string ANIM_IDLE_BLEND = "Idle Blend";

    Animator animator;
    CustomerAI customer;
    IPawn pawn;
    float idleBlend;


    void Start()
    {
        pawn = GetComponentInParent<IPawn>();        
        animator = GetComponent<Animator>();

        pawn.onPickup += OnItemPickup;
        pawn.onItemSwipe += OnItemSwipe;

        StartCoroutine( ChangeIdleBlend() );
    }    

    private void OnItemPickup()
    {
        animator.SetTrigger( ANIM_PICKUP );
    }

    private void OnItemSwipe()
    {
        animator.SetTrigger( ANIM_SWIPE );
    }

    void Update()
    {
        animator.SetFloat( ANIM_SPEED, pawn.speed );
    }

    private IEnumerator ChangeIdleBlend()
    {
        while( true )
        {
            idleBlend = Random.value;
            animator.SetFloat( ANIM_IDLE_BLEND, idleBlend );
            yield return new WaitForSeconds( Random.Range( 1, 10 ) );            
        }
    }
}
