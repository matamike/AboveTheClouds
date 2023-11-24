using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BezierPoint : MonoBehaviour{
  
    private void Awake(){
        GetComponent<Collider>().isTrigger = true;      
    }

    private void OnTriggerEnter(Collider other){
        BezierTest.Instance.SetLastTransformVisited(transform);
    }
}
