using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable{
    public void Interact(GameObject invokeSource);
    public void CancelInteracion(GameObject invokeSource);
}
