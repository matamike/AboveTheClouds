using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelUtility{
    //Difficulty Preset Holders -> Template Defined and Created Difficulty (User defined)
    private static DifficultyPresetSO _difficultyPresetSO;
    //private static UserDefinedMappedDifficultySO _userDefinedMappedDifficultySO;

    public static void SetDifficultyModeWithRandomPlacement(DifficultyPresetSO difficultyPresetSO){
        _difficultyPresetSO = difficultyPresetSO;
        //_userDefinedMappedDifficultySO = null;
    }

    //public static void SetUserDefinedMappedDifficulty(UserDefinedMappedDifficultySO userDefinedMappedDifficultySO){
      //  _userDefinedMappedDifficultySO = userDefinedMappedDifficultySO;
      //  _difficultyPresetSO = null;
    //}

    public static DifficultyPresetSO GetActiveDifficultyPreset() => _difficultyPresetSO;

    //public static UserDefinedMappedDifficultySO GetActiveUserDefinedMappedDifficulty() => _userDefinedMappedDifficultySO;
}
