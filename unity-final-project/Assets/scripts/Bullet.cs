﻿using System;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public int damage;
    private Rigidbody _rigidbody;
    public GameObject ownerGO;
    public GameObject targetGO;
    public float destroyAfter { get; set; } = 5;

    private void Awake()
    {
        _rigidbody = U.GetOrAddComponent<Rigidbody>(gameObject);
        CollidableObject.AttachToScript(this.gameObject,nameof(MainPlayer));
    }

    public void StartMoving(GameObject owner)
    {
        this.ownerGO = owner;
        _rigidbody.velocity = direction * speed;
    }

    public void CollideWithPlayer(Player player)
    {
        if (player.gameObject == ownerGO)
        {
            return;
        }else if (targetGO != null && targetGO != player.gameObject)
        {
            return;
        }
        player.TakeDamage(this);
        Destroy(this.gameObject);
    }
    public void CollideWithNothing()
    {
        Invoke(nameof(DestroyGo),destroyAfter);
    }

    private void DestroyGo()
    {
        Destroy(gameObject);
    }
}