using UnityEngine;

namespace Objects
{
    public class Projectile: BasicDamager 
    {
        private void OnTriggerEnter(Collider other)
        {
            DamageOtherObject(other);
            Destroy(gameObject);
        }
    }
}