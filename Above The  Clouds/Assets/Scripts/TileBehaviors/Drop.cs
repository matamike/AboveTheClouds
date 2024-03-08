using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour, IInteractable{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private bool _isLoosened = false;
    [SerializeField] private Vector3 startPosition = Vector3.zero, endPosition = Vector3.zero;
    [SerializeField] private float fallDownTimer = 0.0f, fallDownWaitTime= 3f;
    private string[] lookUpNames = { "Player", "DroppedObject" };
    private float speed = 0.5f;
    private GameObject entityActivator;
    private TileAudio tileAudio;
    private bool pendingDestruction = false;

    private void Awake(){
        tileAudio = transform.root.GetComponent<TileAudio>();
    }

    private void Update(){
        DropBehavior();
    }

    private void DropBehavior(){
        if (_isLoosened){
            fallDownTimer += Time.deltaTime * TimeMultiplierUtility.GetTimeMultiplier();
            transform.position = Vector3.Lerp(startPosition, endPosition, fallDownTimer * speed * TimeMultiplierUtility.GetTimeMultiplier());
            //Destroy Drop Tile SubProcess
            if (fallDownTimer > fallDownWaitTime){
                _rigidbody.isKinematic = false;
                fallDownTimer = 0.0f;
                GetComponent<Collider>().enabled = false;
                _isLoosened = false;
                Destroy(gameObject, 5f);
                pendingDestruction = true;
                DispatcherUtility.RequestBroadcast(gameObject, entityActivator);
            }
        }
        else{
            if(transform.position != startPosition && startPosition != endPosition){
                transform.position = Vector3.Lerp(transform.position, startPosition, fallDownTimer * speed * TimeMultiplierUtility.GetTimeMultiplier());
            }
        }
    }


    //IInteractable Interface
    public void Interact(GameObject invokeSource){
        if (LayerUtility.LayerIsName(invokeSource.layer, lookUpNames)){
            if (!_isLoosened){
                if (!pendingDestruction){
                    tileAudio.PlayTileSFX(TileAudio.TILE_SFX_TYPE.Activation);
                    entityActivator = invokeSource;
                    if (startPosition == Vector3.zero){
                        startPosition = transform.position;
                        endPosition = transform.position + (Vector3.down * 0.3f);
                    }
                    _isLoosened = true;
                }
            }
        }
    }

    public void CancelInteracion(GameObject invokeSource){
        if (LayerUtility.LayerIsName(invokeSource.layer, lookUpNames)){
            tileAudio.PlayTileSFX(TileAudio.TILE_SFX_TYPE.Activation);
            _isLoosened = false;
            fallDownTimer = 0.0f;
        }
    }
}