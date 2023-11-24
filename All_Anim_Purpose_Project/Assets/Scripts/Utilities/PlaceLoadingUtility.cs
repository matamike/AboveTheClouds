using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlaceLoadingUtility{
    //Contains all the active to the build scenes
    private static readonly string[] _sceneNames = new string[] {
        "Hub",
        "ObstacleCourse_Random",
    };

    public static Place _currentPlace = Place.Hub; 

    public enum Place{
        None,
        Hub,
        ObstacleCourseRandom_Any,
        ObstacleCourseRandom_Easy,
        ObstacleCourseRandom_Medium,
        ObstacleCourseRandom_Hard,
    }


    public static void MoveToPlace(Place place){
        if (_currentPlace == place || place == Place.None) return;
        else _currentPlace = place;

        switch (place)
        {
            case Place.Hub:
                SceneManager.LoadScene(GetSceneNameByBuildIndex(0));
                _currentPlace = Place.Hub;
                break;
            case Place.ObstacleCourseRandom_Any:
                SceneManager.LoadScene(GetSceneNameByBuildIndex(1));
                _currentPlace = Place.ObstacleCourseRandom_Any;
                break;
            case Place.ObstacleCourseRandom_Easy:
                SceneManager.LoadScene(GetSceneNameByBuildIndex(1));
                _currentPlace = Place.ObstacleCourseRandom_Easy;
                break;
            case Place.ObstacleCourseRandom_Medium:
                SceneManager.LoadScene(GetSceneNameByBuildIndex(1));
                _currentPlace = Place.ObstacleCourseRandom_Medium;
                break;
            case Place.ObstacleCourseRandom_Hard:
                SceneManager.LoadScene(GetSceneNameByBuildIndex(1));
                _currentPlace = Place.ObstacleCourseRandom_Hard;
                break;
        }
    }

    public static Place GetCurrentPlace() => _currentPlace;

    public static string GetSceneNameByBuildIndex(int index) => _sceneNames[index];
}
