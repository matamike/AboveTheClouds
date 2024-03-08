using System;
using System.Collections.Generic;
using UnityEngine;

public class UIVisibilityHandler : Singleton<UIVisibilityHandler>{
    [SerializeField] private List<KeyCode> keycodes = new List<KeyCode>();
    [SerializeField] private List<MonoBehaviour> types = new List<MonoBehaviour>();
    private Dictionary<KeyCode, Tuple<MonoBehaviour, Action>> interactiveEntities = new Dictionary<KeyCode, Tuple<MonoBehaviour, Action>> ();

    private void Start(){
        InitializeDictionary();
        CursorVisibilityUtility.SetCursorVisibility(false);
    }

    private void Update(){
        HandleInputUI();   
    }

    private void HandleInputUI(){
        if (Input.anyKeyDown){
            foreach (KeyCode keyCode in keycodes){
                if (interactiveEntities[keyCode]!= null){
                    interactiveEntities[keyCode].Item2();
                }
            }
        }
    }

    private void InitializeDictionary(){
        for (int i =0; i<keycodes.Count; i++){
            KeyCode keycode = keycodes[i];
            if ((types.Count -1) >= i ){ 
                MonoBehaviour type = types[i];
                //Prepare Action callback
                Action action = () => {
                    if (Input.GetKeyDown(keycode)){
                        CursorVisibilityUtility.BroadCastToEntity(interactiveEntities[keycode].Item1);
                    }
                };
                //Assign Entry to Dictionary
                interactiveEntities.Add(keycode, Tuple.Create(type, action));
            }
            else{
                interactiveEntities.Add(keycode, null);
            }           
        }
    }
}