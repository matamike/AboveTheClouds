using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSource : MonoBehaviour{
    private bool isShrinking = false;
    private float shrinkSpeed = 15f;

    private void Update(){
        if (isShrinking) ShinkAndDestroy();
    }

    private void OnCollisionEnter(Collision collision) => isShrinking = true;

    private void ShinkAndDestroy(){
        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, Vector3.one * 0.33f, shrinkSpeed * Time.deltaTime);
        Destroy(gameObject, shrinkSpeed);
    }
}
