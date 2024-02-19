using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UXCard : MonoBehaviour{
    [SerializeField] private TextMeshProUGUI uxCardDescriptionText;
    [SerializeField] private TextMeshProUGUI uXCardTitleText;
    [SerializeField] private Button uxCardButton;
    public TextMeshProUGUI GetUXCardDescriptionTextHolder() => uxCardDescriptionText;
    public TextMeshProUGUI GetUXCardTitleTextHolder() => uXCardTitleText;
    public Button GetUXCardButton() => uxCardButton;
}