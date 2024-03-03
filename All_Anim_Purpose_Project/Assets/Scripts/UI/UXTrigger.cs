using UnityEngine;

public class UXTrigger : MonoBehaviour{
    //Type of UX
    [SerializeField] private UXManager.UXType_Tip selectedTip;
    [SerializeField] private UXManager.UXType_Notification selectedNotification;
    [SerializeField] private UXManager.UXType_Tutorial selectedTutorial;
    [SerializeField] private bool isTip = false;
    [SerializeField] private bool isNotification = false;
    [SerializeField] private bool isTutorial = false;
    
    //Is one time for all
    [SerializeField] private bool isOneTimeForAllInstancesOfSameType = false;
    [SerializeField] private bool isGUITrigger = false;

    public enum UXEntityType{
        None,
        SimpleTile,
        DropTile,
        RotatingTile,
        BouncyTile,
        Checkpoint,
        Portal,
        Creator_StartingPoint,
        Hub_StartingPoint,
        Game_StartingPoint,
        Creator_TemplateList,
        Creator_LoadTemplate,
        Creator_SaveTemplate,
        Creator_TestTemplate,
    }
    [SerializeField] private UXEntityType uxEntityType;

    private bool isUxGlobalEnabled = false;
    private bool locked = false;
    private string[] layerNames = { "Player" };

    private void Start(){
        isUxGlobalEnabled = PreferencesUtility.HasUXActive();
        if (isOneTimeForAllInstancesOfSameType){
            locked = PreferencesUtility.IsEntityOneTimeLocked(uxEntityType);
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
        if(isOneTimeForAllInstancesOfSameType && ((int)uxEntityType == e.special_id)) locked = true;
    }

    private void OnTriggerEnter(Collider other){
        isUxGlobalEnabled = PreferencesUtility.HasUXActive();
        if (!isUxGlobalEnabled) return;

        if (!LayerUtility.LayerIsName(other.gameObject.layer, layerNames)) return;

        if (!locked && UXManager.Instance != null){
            ActivateUXPrompt();
            if (isOneTimeForAllInstancesOfSameType){
                PreferencesUtility.RequestLockOneTimeForAllOfTheType(this, uxEntityType);
            }
        }    
    }

    public void OnPointerEnterUX(){
        if (isGUITrigger){
            isUxGlobalEnabled = PreferencesUtility.HasUXActive();
            if (!isUxGlobalEnabled) return;

            if (!locked && UXManager.Instance != null){
                ActivateUXPrompt();
                if (isOneTimeForAllInstancesOfSameType){
                    PreferencesUtility.RequestLockOneTimeForAllOfTheType(this, uxEntityType);
                }
            }
        }
    }

    private void ActivateUXPrompt(){
        if (isTip) UXManager.Instance.FireUX(selectedTip, () => { TimeMultiplierUtility.PauseTime(); }, () => { TimeMultiplierUtility.ResumeTime(); });
        else if (isNotification) UXManager.Instance.FireUX(selectedNotification, () => { TimeMultiplierUtility.PauseTime(); }, () => { TimeMultiplierUtility.ResumeTime(); });
        else if (isTutorial) UXManager.Instance.FireUX(selectedTutorial, () => { TimeMultiplierUtility.PauseTime(); }, () => { TimeMultiplierUtility.ResumeTime(); });
    }
}