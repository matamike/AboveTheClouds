using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private bool isRotating = false;

    private void Update(){
        //Rotating
        if (isRotating) gameObject.transform.Rotate(Vector3.up, 300f);
        else gameObject.transform.eulerAngles = Vector3.Lerp(gameObject.transform.transform.eulerAngles, Vector3.zero, 5f * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out DropSource dropSource))
        {
            isRotating = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out DropSource dropSource))
        {
            isRotating = false;
        }
    }
}
