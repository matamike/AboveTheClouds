using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerUtility{
    public static LayerMask GetLayerMask(GameObject go) {
        string layerName = LayerMask.LayerToName(go.layer);
        LayerMask mask = LayerMask.GetMask(layerName);
        return mask;
    }

    public static bool LayerIsName(int layer, string[] lookUpNames){
        for(int i = 0; i<lookUpNames.Length; i++){
            if(LayerMask.LayerToName(layer) == lookUpNames[i]) return true;
        }  
        return false;
    }
}