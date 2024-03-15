using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CheckPoint : MonoBehaviour{
    public static event EventHandler<OnCheckPointReachedEventArgs> OnCheckPointReached;
    [SerializeField] private GameObject checkPointHighlightPrefab;

    private bool hasReached = false;
    private object elementAssigned = null;
    private Light lightSource = null;
    private GameObject checkPointHighlight;

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
                Destroy(checkPointHighlight);
            }
        }
    }

    private void Interactable_OnInteractorPositionChanged(object sender, IInteractable.OnInteractEventArgs e){
        float intensity = Vector3.Distance(transform.position, e.position) * 10f;
        AdjustLightIndicatorIntensity(intensity);
    }

    public void CreateCheckPointHighlight(){
        checkPointHighlight = Instantiate(checkPointHighlightPrefab, transform.position, Quaternion.identity);
        checkPointHighlight.transform.position += new Vector3(0f, 2.5f, 0f);
        Canvas checkpointHighlightCanvas = checkPointHighlight.GetComponent<Canvas>();
        checkpointHighlightCanvas.worldCamera = FindObjectOfType<Camera>();
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
