using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerUtility{
    public static LayerMask GetLayerMask(GameObject go) {
        string layerName = LayerMask.LayerToName(go.layer);
        LayerMask mask = LayerMask.GetMask(layerName);
        return mask;
    }

    public static bool LayerIsName(int layer, string[] lookUpLayerNames){
        for(int i = 0; i<lookUpLayerNames.Length; i++){
            if(LayerMask.LayerToName(layer) == lookUpLayerNames[i]) return true;
        }  
        return false;
    }
}