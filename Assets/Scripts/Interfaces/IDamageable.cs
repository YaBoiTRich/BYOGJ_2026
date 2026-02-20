using Data.ScriptableObjectBases;
using UnityEngine;

namespace Interfaces
{
    public interface IDamageable
    {
        
        string DamageableName { get; }
        GameObject GameObject { get; }
        
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        public void OnDamage(float damage);
        public void OnHeal(float heal);
        public bool Invincible { get; set; }
        public bool CanPickup() => CurrentHealth < MaxHealth;
        public bool IsDead { get; set; }
        public void Die();

    }
}
