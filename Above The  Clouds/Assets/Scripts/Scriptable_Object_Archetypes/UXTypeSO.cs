using UnityEngine;

[CreateAssetMenu()]
public class UXTypeSO : ScriptableObject{
    [SerializeField]private UXTypeUtility.UXType UXType;
    public string GetUXDescription() => UXTypeUtility.GetUXText(UXType);
    public string GetUXTitle() => UXTypeUtility.GetUXHeader(UXType);
    public UXTypeUtility.UXType GetUXType() => UXType;
}
