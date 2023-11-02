using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour{
    [SerializeField] private Rigidbody rb;


    private void OnCollisionEnter(Collision collision){
        if(collision.collider.gameObject.TryGetComponent(out DropSource dropSource))
        {
            rb.isKinematic = false;
            Destroy(rb.transform.root.gameObject, 5f);
        }
    }
}
