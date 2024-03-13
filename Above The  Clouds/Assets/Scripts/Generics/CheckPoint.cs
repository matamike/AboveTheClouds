using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CheckPoint : MonoBehaviour{
    public static event EventHandler<OnCheckPointReachedEventArgs> OnCheckPointReached;
    private bool hasReached = false;
    [SerializeField] private object elementAssigned = null;
    private Light lightSource = null;

    public class OnCheckPointReachedEventArgs : EventArgs{
        public int index;
        public Transform self;
    }

    private string[] layerNames = { "Player" };
    private int checkpointIndex = -1;

    private void Awake(){
        lightSource = GetComponentInChildren<Light>();
    }


    private void OnEnable(){
        IInteractable.OnInteractorPositionChanged += Interactable_OnInteractorPositionChanged;
    }

    private void OnDisable(){
        IInteractable.OnInteractorPositionChanged -= Interactable_OnInteractorPositionChanged;
    }

    private void OnDestroy(){
        if(elementAssigned != null) elementAssigned = null;
    }

    private void OnCollisionEnter(Collision collision){
        if (LayerUtility.LayerIsName(collision.gameObject.layer, layerNames)){
            if (!hasReached){
                MyGameManager.Instance.ChangeRespawnPoint(transform);
                OnCheckPointReached?.Invoke(this, new OnCheckPointReachedEventArgs{
                    index = checkpointIndex,
                    self = transform,
                });
                hasReached = true;
                DisableLightSource();
            }
        }
    }

    private void Interactable_OnInteractorPositionChanged(object sender, IInteractable.OnInteractEventArgs e){
        float intensity = Vector3.Distance(transform.position, e.position) * 10f;
        AdjustLightIndicatorIntensity(intensity);
    }

    private void DisableLightSource(){
        if(lightSource != null){
            lightSource.enabled = false;
        }
    }

    private void AdjustLightIndicatorIntensity(float intensity) => lightSource.intensity = intensity;
    public void SetCheckPointIndex(int index) => checkpointIndex = index;

    public void SetElementBoundToCheckoint(object element) => elementAssigned = element;

    public void RequestRemoval(object invoker){
        if (invoker == elementAssigned){
            if(gameObject != null) Destroy(gameObject);
        }
    }
}
