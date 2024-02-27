using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DropSource : MonoBehaviour{
    private bool isShrinking = false;
    private bool finishedShrinking = false;
    private float shrinkSpeed = 15f;
    private Vector3 shrinkTarget = new Vector3(0.33f, 0.33f, 0.33f); 

    private void Update(){
        if (isShrinking) ShinkAndDestroy();
    }

    private void OnCollisionEnter(Collision collision) => isShrinking = true;

    private void ShinkAndDestroy(){
        transform.localScale = Vector3.LerpUnclamped(transform.localScale, shrinkTarget, shrinkSpeed * Time.deltaTime * TimeMultiplierUtility.GetTimeMultiplier());
        if(transform.localScale == shrinkTarget){
            finishedShrinking = true;
        }

        if (finishedShrinking){
            isShrinking = false;
            Destroy(gameObject);
        }
    }
}
