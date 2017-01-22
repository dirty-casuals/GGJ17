using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCooldownBar : MonoBehaviour
{
    PlayerAttack playerAttack;
    float defaultScale = 0;

    // Use this for initialization
    void Start()
    {
        defaultScale = transform.localScale.x;
        playerAttack = transform.root.GetComponentInChildren<PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = transform.localScale;
        float normalizedCooldown = Mathf.Clamp01( playerAttack.attackCooldown / Constants.AttackCooldown );
        scale.x = defaultScale * normalizedCooldown;
        transform.localScale = scale;
    }
}
