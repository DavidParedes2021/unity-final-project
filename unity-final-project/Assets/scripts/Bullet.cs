using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public int damage;

    private void Awake()
    {
        CollidableObject.AttachToScript(this.gameObject,nameof(MainPlayer));
    }

    private void Update()
    {
        Move();
    }

    public void CollideWithPlayer(Player player)
    {
        player.TakeDamage(this);
        Destroy(this);
    }
    public void CollideWithNothing()
    {
        Destroy(this);
    }
    private void Move()
    {
        transform.Translate(direction * (speed * Time.deltaTime));
    }

}