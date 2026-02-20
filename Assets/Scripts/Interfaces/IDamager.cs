using UnityEngine;

namespace Interfaces
{
    public interface IDamager
    {
        public void OnDamaging();
        string DamagerName { get; }
        GameObject GameObject { get; }
    }
}
