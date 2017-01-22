using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    void Start()
    {
        SetRagdoll( false );
    }

    public void SetRagdoll( bool enabled )
    {
        if( enabled )
            enabled = true;

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        var colliders = GetComponentsInChildren<Collider>();
        var joints = GetComponentsInChildren<Joint>();        

        SetEnableAll( colliders, enabled );
        SetEnableAll( rigidBodies, enabled );
        SetEnableAll( joints, enabled );

        rigidBodies = GetComponentsInParent<Rigidbody>();
        colliders = GetComponentsInParent<Collider>();
        joints = GetComponentsInParent<Joint>();
        var animators = GetComponentsInParent<Animator>();

        SetEnableAll( colliders, !enabled );
        SetEnableAll( rigidBodies, !enabled );
        SetEnableAll( joints, !enabled );
        SetEnableAll( animators, !enabled );
    }

    private void SetEnableAll( Collider[] colliders, bool enable )
    {
        foreach( Collider component in colliders )
        {
            component.enabled = enable;
        }
    }

    private void SetEnableAll( Rigidbody[] rigidbodies, bool enable )
    {
        foreach( Rigidbody component in rigidbodies )
        {
            component.isKinematic = !enable;
        }
    }

    private void SetEnableAll( Joint[] joints, bool enable )
    {
        foreach( Joint component in joints )
        {
            component.enablePreprocessing = enable;
        }
    }

    private void SetEnableAll( Animator[] animators, bool enable )
    {
        foreach( Animator component in animators )
        {
            component.enabled = enable;
        }
    }
}
