using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeMultiplierUtility{
    private static float _timeMultiplier = 1f;

    public static void ChangeMultiplier(float timeMultiplier)=> _timeMultiplier = timeMultiplier;
    public static void PauseTime() => _timeMultiplier = 0f;
    public static void ResumeTime() => _timeMultiplier = 1f;
    public static float GetTimeMultiplier() => _timeMultiplier;
}
