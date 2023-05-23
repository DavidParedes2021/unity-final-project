using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public int damage;
    private Rigidbody _rigidbody;
    public GameObject ownerGO;
    
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
        }
        player.TakeDamage(this);
        Destroy(this);
    }
    public void CollideWithNothing()
    {
        Destroy(this);
    }
}