using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class MouseUtility{
    private static float raycastDistance = 400f;

    public static Vector3 GetMousePosition() => new Vector3(Input.mousePosition.x, Input.mousePosition.y, raycastDistance);

    public static GameObject GetMouseToWorldRayHit(){
        //Ray Prep
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //DEBUG
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.blue, 3f);

        //Result
        bool result = Physics.Raycast(ray, out RaycastHit hitInfo, raycastDistance);
        if (result) return hitInfo.collider.transform.root.gameObject;
        else return null;
    }

    public static float GetMouseXNormalized(){
        return Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f);
    }

    public static float GetMouseYNormalized()
    {
        return Mathf.Clamp(Input.GetAxis("Mouse Y"), -1f, 1f);
    }
}
