using UnityEngine;

public class SimpleTile : MonoBehaviour, IInteractable{
    private string[] lookUpNames = { "Player", "DroppedObject" };
    private TileAudio tileAudio;
    private bool hasObjOnIt = false;

    
    private void Awake()
    {
        tileAudio = transform.root.GetComponent<TileAudio>();
    }

    public void CancelInteracion(GameObject invokeSource){
        if (LayerUtility.LayerIsName(invokeSource.layer, lookUpNames)){
            hasObjOnIt = false;
        }
    }

    public void Interact(GameObject invokeSource){
        if (LayerUtility.LayerIsName(invokeSource.layer, lookUpNames)){
            if (!hasObjOnIt){
                tileAudio.PlayTileSFX(TileAudio.TILE_SFX_TYPE.Interaction);
                hasObjOnIt = true;
            }
        }
    }
}
