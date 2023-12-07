using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> :MonoBehaviour where T : MonoBehaviour{
    private static T _instance;
    public static T Instance{ get; private set; }
    private void Awake() => InitializeSingleton();
    private void InitializeSingleton(){
        if (_instance == null){
            _instance = FindObjectOfType<T>();
            if(_instance == null) CreateSceneObjectSingleton();
        }
        else if (_instance != FindObjectOfType<T>()) Destroy(FindObjectOfType<T>());
        //DontDestroyOnLoad(gameObject);

        Instance = _instance;
    }

    private void CreateSceneObjectSingleton(){
        GameObject gameObject = new GameObject(typeof(T).Name + "_Singleton");
        gameObject.AddComponent<T>();
        _instance = FindObjectOfType<T>();
    }
}