using UnityEngine;

public interface ITriggerable
{
    void OnActivate();
    void OnDeactivate();
    void Toggle();
    bool IsToggled { get; }
    GameObject GameObject { get; }
}
