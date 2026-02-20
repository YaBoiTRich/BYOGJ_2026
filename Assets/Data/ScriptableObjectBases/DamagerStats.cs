using UnityEngine;

namespace Data.ScriptableObjectBases
{
    [CreateAssetMenu(fileName = "DamagerStats", menuName = "Scriptable Objects/DamagerStats")]
    public class DamagerStats : ScriptableObject
    {
        public float damageGiven;
    }
}
