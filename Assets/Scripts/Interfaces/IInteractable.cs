using UnityEngine;

namespace Interfaces
{
    /// <summary>
    /// Interface used for when an object is targeted by RayCast
    /// This is to essentially define what happens when being looked at and/or an action is taken
    /// </summary>
    public interface IInteractable
    {
        void OnTargeted(int playerID);
        void OnUnTargeted(int playerID);

        // Player control inputs
        void OnPrimaryInteraction(int playerID);
        void OnSecondaryInteraction(int playerID);
        void OnTertiaryInteraction(int playerID);
        // void OnGaze(int playerID, Vector2? lookDelta, float? scaleDelta, bool gazeComplete);

        GameObject GameObject { get; }
    }
}

// TODO: If we want to not arbitrarily force OnUnTargeted
// NOTES: Can use the below to set (essentially) as virtual --
// void OnUnTargeted(int playerID) { }

// When inheriting, do so via:

// public abstract void OnUnTargeted...
//  - further: public void OnUnTargeted...

// public virtual void OnUnTargeted...
//  - further: override public void OnUnTargeted...

// void IInteractable.OnUnTargeted...
//  - further: ???

// public void OnUnTargeted...
//  - further: new public void OnUnTargeted...

