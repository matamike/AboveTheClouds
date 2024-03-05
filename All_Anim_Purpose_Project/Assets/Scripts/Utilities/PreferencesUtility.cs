using System;
using System.Collections.Generic;

public static class PreferencesUtility{
    //Preferences
    private static Tuple<string,bool> _uxActivation = Tuple.Create("UX Active", true);
    public static event EventHandler<OnUXOneTimeForAllUnitsOfTypeEventArgs> OnUXOneTimeForAllUnitsOfTypeLock;
    public static event EventHandler<OnUXOneTimeForAllUnitsOfTypeEventArgs> OnUXOneTimeForAllUnitsOfTypeReset;

    private static Dictionary<UXTypeUtility.UXType, bool> oneTimeLocksForAllInstancesUnits = new Dictionary<UXTypeUtility.UXType, bool>() { };

    public class OnUXOneTimeForAllUnitsOfTypeEventArgs : EventArgs{
        public int special_id;
    }

    public static void RequestLockOneTimeForAllOfTheType(object sender, UXTypeUtility.UXType entityType) {
        oneTimeLocksForAllInstancesUnits[entityType] = true;

        OnUXOneTimeForAllUnitsOfTypeLock?.Invoke(sender, new OnUXOneTimeForAllUnitsOfTypeEventArgs(){
            special_id = (int)entityType
        });
    }

    public static void RequestResetOneTimeForAllOfTheType(object sender, UXTypeUtility.UXType entityType){
        oneTimeLocksForAllInstancesUnits[entityType] = true;
        OnUXOneTimeForAllUnitsOfTypeReset?.Invoke(sender, new OnUXOneTimeForAllUnitsOfTypeEventArgs(){
        });
    }

    public static bool IsEntityOneTimeLocked(UXTypeUtility.UXType entityType){
        if (oneTimeLocksForAllInstancesUnits.ContainsKey(entityType)){
            return oneTimeLocksForAllInstancesUnits[entityType];
        }
        return false;
    }

    public static void ToggleUX(string key, bool flag) => _uxActivation = Tuple.Create(key,flag);
    public static bool HasUXActive() => _uxActivation.Item2;
    public static string GetUXActivationKey() => _uxActivation.Item1;
}
