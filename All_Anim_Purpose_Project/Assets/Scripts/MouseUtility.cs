using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class MouseUtility{
    private static Vector3 _mousePosition = Vector3.zero;
    private static float raycastDistance = 400f;

    public static void SetMousePosition(Vector2 mousePosition) => _mousePosition = new Vector3(mousePosition.x, mousePosition.y, raycastDistance);

    public static Vector3 GetMousePosition() => _mousePosition;

    public static GameObject GetMouseToWorldRayHit(){
        _mousePosition = Camera.main.ScreenToWorldPoint(_mousePosition);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.blue, 2f);

        bool result = Physics.Raycast(ray, out RaycastHit hitInfo, raycastDistance);
        if (result) return hitInfo.collider.transform.root.gameObject;
        else return null;
    }
}
