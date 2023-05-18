using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public int damage;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(direction * (speed * Time.deltaTime));
    }

}