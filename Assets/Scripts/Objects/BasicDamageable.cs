using Data.ScriptableObjectBases;
using Interfaces;
using UnityEngine;

namespace Objects
{
    public class BasicDamageable : MonoBehaviour, IDamageable
    {
        
        [Tooltip("Baseline Health Stats")]
        public DamageableStats damageableStats;
        
        public string DamageableName => damageableStats.name;
        public GameObject GameObject => gameObject;
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        
        public bool Invincible { get; set; }
        public bool IsDead { get; set; }

        private void Start()
        {
            CurrentHealth = damageableStats.maxHealth;
            // TODO: Add handler for determining either EnemyStats or PlayerStats from the parent object
        }

        public void OnHeal(float healAmount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + healAmount, 0f, damageableStats.maxHealth);
        }

        public void OnDamage(float damage)
        {
            Debug.Log("I AM TAKING DAMAGE");
            if (Invincible)
                return;
            
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, damageableStats.maxHealth);
            
            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        public void Kill()
        {
            OnDamage(damageableStats.maxHealth);
        }

        public void Die()
        {
            Destroy(gameObject);
        }

    }
}