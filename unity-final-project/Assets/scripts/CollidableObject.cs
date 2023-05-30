using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class CollidableObject : MonoBehaviour
{
    public static void AttachToScript(GameObject gameObject, string tag)
    {
        if (gameObject != null)
        {
            gameObject.tag = tag;
            U.GetOrAddComponent<CollidableObject>(gameObject);
        }
    }
    void OnCollisionEnter(Collision collision) {
        var thisMainPlayer = GetComponent<MainPlayer>();
        var thisZombiePlayer = GetComponent<Zombie>();
        var thisBulletObject = GetComponent<Bullet>();
        var thisWeapon = GetComponent<Weapon>();
        var thisAmmunition = GetComponent<Ammunition>();
        var thisConsumable = GetComponent<Consumable>();
        var thisRepairObject = GetComponent<RepairObject>();

        CollidableObject other = collision.gameObject.GetComponent<CollidableObject>();
        if (other != null)
        {
            var otherMainPlayer = other.GetComponent<MainPlayer>();
            var otherZombiePlayer = other.GetComponent<Zombie>();
            var otherBullet = other.GetComponent<Bullet>();
            var otherWeapon = other.GetComponent<Weapon>();
            var otherAmmunition = other.GetComponent<Ammunition>();
            var otherConsumable = other.GetComponent<Consumable>();
            var otherRepairObject = other.GetComponent<RepairObject>();

            if (otherConsumable != null && thisMainPlayer != null)
            {
                otherConsumable.PickUp(thisMainPlayer);
            }
            else if (otherWeapon != null && thisMainPlayer != null)
            {
                otherWeapon.PickUp(thisMainPlayer);
            }
            else if (otherRepairObject != null && thisMainPlayer != null)
            {
                otherRepairObject.PickUp(thisMainPlayer);
            }
            else if (otherAmmunition != null && thisMainPlayer != null)
            {
                otherAmmunition.PickUp(thisMainPlayer);
            }
            else if (otherZombiePlayer != null && thisMainPlayer != null)
            {
                otherZombiePlayer.Attack(thisMainPlayer);
            }
            else if (otherBullet != null && thisZombiePlayer != null)
            {
                otherBullet.CollideWithPlayer(thisZombiePlayer);
            }
            else if (otherBullet != null && thisMainPlayer != null)
            {
                otherBullet.CollideWithPlayer(thisMainPlayer);
            }else if (otherBullet != null && thisBulletObject != null)
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            }else if (thisBulletObject != null && thisBulletObject.ownerGO == collision.gameObject)
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain") && thisBulletObject != null)
        {
            thisBulletObject.CollideWithNothing();
        }
    }
}
