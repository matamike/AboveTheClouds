using System;
using System.Collections.Generic;
using UnityEngine;

public static class PreferencesUtility{
    //Preferences
    private static Tuple<string,bool> _uxActivation = Tuple.Create("UX Active", true);
    public static event EventHandler<OnUXOneTimeForAllUnitsOfTypeEventArgs> OnUXOneTimeForAllUnitsOfTypeLock;
    public static event EventHandler<OnUXOneTimeForAllUnitsOfTypeEventArgs> OnUXOneTimeForAllUnitsOfTypeReset;
    public static event EventHandler<OnInvertedMouseHorizontalEventArgs> OnInvertedMouseHorizontalChanged;
    public static event EventHandler<OnInvertedMouseVerticalEventArgs> OnInvertedMouseVerticalChanged;
    public static event EventHandler<OnMouseSensitivityEventArgs> OnMouseSensitivityChanged;

    private static Dictionary<UXTypeUtility.UXType, bool> oneTimeLocksForAllInstancesUnits = new Dictionary<UXTypeUtility.UXType, bool>() { };
    private static bool invertMouseHorizontal = true;
    private static bool invertMouseVertical = true;
    private static float mouseSensitivity = 5f;

    public class OnUXOneTimeForAllUnitsOfTypeEventArgs : EventArgs{
        public int special_id;
    }

    public class OnInvertedMouseHorizontalEventArgs : EventArgs{
        public bool horizontalInverted;
    }

    public class OnInvertedMouseVerticalEventArgs : EventArgs{
        public bool verticalInverted;
    }

    public class OnMouseSensitivityEventArgs : EventArgs{
        public float sensitivity;
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

    public static void SetInvertedMouseHorizontalAxis(bool flag){
        invertMouseHorizontal = flag;
        OnInvertedMouseHorizontalChanged?.Invoke(null, new OnInvertedMouseHorizontalEventArgs{
            horizontalInverted = invertMouseHorizontal
        });
    }
    public static void SetInvertedMouseVerticalAxis(bool flag){
        invertMouseVertical = flag;
        OnInvertedMouseVerticalChanged?.Invoke(null, new OnInvertedMouseVerticalEventArgs{
            verticalInverted = invertMouseVertical
        });
    }
    public static bool GetInvertedMouseHorizontalAxisState() => invertMouseHorizontal;
    public static bool GetInvertedMouseVerticalAxisState() => invertMouseVertical;

    public static void SetMouseSensitivity(float sensitivity){
        mouseSensitivity = sensitivity;
        OnMouseSensitivityChanged?.Invoke(null, new OnMouseSensitivityEventArgs{
            sensitivity = mouseSensitivity
        });
    }

    public static float GetMouseSensitivity() => mouseSensitivity;
}
