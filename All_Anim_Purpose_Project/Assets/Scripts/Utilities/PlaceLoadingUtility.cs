using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility.PlaceUtility
{

    public static class PlaceLoadingUtility
    {
        //Contains all the active to the build scenes
        private static readonly string[] _sceneNames = new string[] {
            "Hub",
            "ObstacleCourse_Random",
        };
        
        public enum Place{
            None,
            Hub,
            ObstacleCourseRandom,
            ObstacleCourseUserDefined,
            LevelCreator,
        }

        public static Place _currentPlace = Place.Hub;
        private static Dictionary<Place, int> _placeToBuildIndex = new Dictionary<Place, int>(){
            {Place.None, -1},
            {Place.Hub, 0},
            {Place.ObstacleCourseRandom, 1},
            //{Place.ObstacleCourseUserDefined, 2} - TODO Implement (Same structure as 1) ~maybe will keep 1 index (will see)
            //{Place.LevelCreator, 3}, - TODO Implement (Creator Scene will be index 2)
        };

        public static void MoveToPlace(Place place){
            if (_currentPlace == place || place == Place.None) return;
            else _currentPlace = place;

            int buildIndex = _placeToBuildIndex[place];
            SceneManager.LoadScene(GetSceneNameByBuildIndex(buildIndex));
            _currentPlace = place;
        }

        public static Place GetCurrentPlace() => _currentPlace;

        public static string GetSceneNameByBuildIndex(int index) => _sceneNames[index];
    }
}
