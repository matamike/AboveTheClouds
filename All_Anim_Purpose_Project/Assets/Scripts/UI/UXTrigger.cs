using UnityEngine;

public class UXTrigger : MonoBehaviour{
    //Is one time for all
    [SerializeField] private bool isOneTimeForAllInstancesOfSameType = false;
    [SerializeField] private bool isGUITrigger = false;
    [SerializeField] private UXTypeSO uxTypeSO;

    private bool isUxGlobalEnabled = false;
    private bool locked = false;
    private string[] layerNames = { "Player" };

    private void Start(){
        isUxGlobalEnabled = PreferencesUtility.HasUXActive();
        if (isOneTimeForAllInstancesOfSameType){
            locked = PreferencesUtility.IsEntityOneTimeLocked(uxTypeSO.GetUXType());
        }
    }

    private void OnEnable(){
        PreferencesUtility.OnUXOneTimeForAllUnitsOfTypeLock += PreferencesUtility_OnUXOneTimeForAllUnitsOfTypePerformed;
        PreferencesUtility.OnUXOneTimeForAllUnitsOfTypeReset += PreferencesUtility_OnUXOneTimeForAllUnitsOfTypeReset;
    }

    private void OnDisable(){
        PreferencesUtility.OnUXOneTimeForAllUnitsOfTypeLock -= PreferencesUtility_OnUXOneTimeForAllUnitsOfTypePerformed;
        PreferencesUtility.OnUXOneTimeForAllUnitsOfTypeReset -= PreferencesUtility_OnUXOneTimeForAllUnitsOfTypeReset;
    }

    private void PreferencesUtility_OnUXOneTimeForAllUnitsOfTypeReset(object sender, PreferencesUtility.OnUXOneTimeForAllUnitsOfTypeEventArgs e){
        if (isOneTimeForAllInstancesOfSameType) locked = false;
    }

    private void PreferencesUtility_OnUXOneTimeForAllUnitsOfTypePerformed(object sender, PreferencesUtility.OnUXOneTimeForAllUnitsOfTypeEventArgs e){
        if(isOneTimeForAllInstancesOfSameType && ((int)uxTypeSO.GetUXType() == e.special_id)) locked = true;
    }

    private void OnTriggerEnter(Collider other){
        if (!LayerUtility.LayerIsName(other.gameObject.layer, layerNames)) return;
        TryTriggerUX();
    }

    public void OnPointerEnterUX(){
        if (isGUITrigger){
            TryTriggerUX();
        }
    }

    private void TryTriggerUX(){
        isUxGlobalEnabled = PreferencesUtility.HasUXActive();
        if (!isUxGlobalEnabled) return;

        if (!locked && UXManager.Instance != null){
            ActivateUXPrompt();
            if (isOneTimeForAllInstancesOfSameType){
                PreferencesUtility.RequestLockOneTimeForAllOfTheType(this, uxTypeSO.GetUXType());
            }
        }
    }

    private void ActivateUXPrompt(){
        UXManager.Instance.FireUX(
                    uxTypeSO.GetUXDescription(), 
                    uxTypeSO.GetUXTitle(), 
                    () => { TimeMultiplierUtility.PauseTime(); }, 
                    () => { TimeMultiplierUtility.ResumeTime(); });
    }
}