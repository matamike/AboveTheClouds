using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IInteractable{
    public static EventHandler<OnInteractEventArgs> OnInteractorPositionChanged;

    public class OnInteractEventArgs : EventArgs{
        public Vector3 position;
    }

    public void Interact(GameObject invokeSource);
    public void CancelInteracion(GameObject invokeSource);
}
