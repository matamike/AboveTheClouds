using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputManager;

public static class PreferencesUtility{
    //Preferences
    private static Tuple<string,bool> _uxActivation = Tuple.Create("UX Active", true);
    public static event EventHandler<OnUXOneTimeForAllUnitsOfTypeEventArgs> OnUXOneTimeForAllUnitsOfTypeLock;
    public static event EventHandler<OnUXOneTimeForAllUnitsOfTypeEventArgs> OnUXOneTimeForAllUnitsOfTypeReset;

    private static Dictionary<UXTrigger.UXEntityType, bool> oneTimeLocksForAllInstancesUnits = new Dictionary<UXTrigger.UXEntityType, bool>() { };

    public class OnUXOneTimeForAllUnitsOfTypeEventArgs : EventArgs{
        public int special_id;
    }

    public static void RequestLockOneTimeForAllOfTheType(object sender, UXTrigger.UXEntityType entityType) {
        oneTimeLocksForAllInstancesUnits[entityType] = true;

        OnUXOneTimeForAllUnitsOfTypeLock?.Invoke(sender, new OnUXOneTimeForAllUnitsOfTypeEventArgs(){
            special_id = (int)entityType
        });
    }

    public static void RequestResetOneTimeForAllOfTheType(object sender, UXTrigger.UXEntityType entityType){
        oneTimeLocksForAllInstancesUnits[entityType] = true;
        OnUXOneTimeForAllUnitsOfTypeReset?.Invoke(sender, new OnUXOneTimeForAllUnitsOfTypeEventArgs(){
        });
    }

    public static bool IsEntityOneTimeLocked(UXTrigger.UXEntityType entityType){
        if (oneTimeLocksForAllInstancesUnits.ContainsKey(entityType)){
            return oneTimeLocksForAllInstancesUnits[entityType];
        }
        return false;
    }

    public static void ToggleUX(string key, bool flag) => _uxActivation = Tuple.Create(key,flag);
    public static bool HasUXActive() => _uxActivation.Item2;
    public static string GetUXActivationKey() => _uxActivation.Item1;
}
