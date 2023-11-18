using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour, IInteractable{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private bool _isLoosened = false;
    [SerializeField] private Vector3 startPosition, endPosition;
    [SerializeField] private float fallDownTimer = 0.0f, fallDownWaitTime= 3f;
    private string[] lookUpNames = { "Player", "DroppedObject" };
    private float speed = 0.5f;

    private void Update(){
        DropBehavior();
    }

    private void DropBehavior(){
        if (_isLoosened){
            fallDownTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, fallDownTimer * speed);  
            if (fallDownTimer > fallDownWaitTime){
                _rigidbody.isKinematic = false;
                fallDownTimer = 0.0f;
                _isLoosened = false;               
            }
        }
    }


    //IInteractable Interface
    public void Interact(GameObject invokeSource){
        if (!_isLoosened && LayerUtility.LayerIsName(invokeSource.layer, lookUpNames)){
            startPosition = transform.position;
            endPosition = transform.position + (Vector3.down * 0.3f);
            _isLoosened = true;
        }
    }

    public void CancelInteracion(GameObject invokeSource){
        Debug.Log(invokeSource.name + "stopped interacting with " + gameObject.name);
    }
}