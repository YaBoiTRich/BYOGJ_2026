using UnityEngine;

namespace Data.ScriptableObjectBases
{
    [CreateAssetMenu(fileName = "DamageableStats", menuName = "Scriptable Objects/DamageableStats")]
    public class DamageableStats : ScriptableObject
    {
        public float maxHealth;
    }
}
