using System;
using Data.ScriptableObjectBases;
using Interfaces;
using UnityEngine;

namespace Objects
{
    public class BasicDamager: MonoBehaviour, IDamager
    {
        [Tooltip("Baseline Damager Stats")]
        public DamagerStats damagerStats;
        
        public GameObject GameObject => gameObject;
        public string DamagerName => damagerStats.name;

        private void OnTriggerEnter(Collider other)
        {
            DamageOtherObject(other);
        }

        protected void DamageOtherObject(Collider other)
        {
            var damageable = other.gameObject.GetComponent<IDamageable>();
            damageable?.OnDamage(damagerStats.damageGiven);
        }

        private void OnCollisionEnter(Collision collision)
        {
            
            // if (collision.gameObject.CompareTag("Player")) return;
            
            // Check if the object we hit is damageable and then hurt it
        }

        public void OnDamaging()
        {
            // TODO: Implement if needed -- like show particles, etc
        }

    }
}