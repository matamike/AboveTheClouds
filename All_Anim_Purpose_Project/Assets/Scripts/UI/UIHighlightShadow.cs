using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHighlightShadow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    private Shadow _shadow;
    private void Awake() => _shadow = GetComponent<Shadow>();
    public void OnPointerEnter(PointerEventData eventData) => _shadow.enabled = true;
    public void OnPointerExit(PointerEventData eventData) => _shadow.enabled = false;
}