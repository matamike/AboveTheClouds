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
            "ObstacleCourse",
            "LevelCreator",
        };
        
        public enum Place{
            Hub,
            ObstacleCourse,
            LevelCreator,
        }

        public static Place _currentPlace = Place.Hub;
        private static Dictionary<Place, int> _placeToBuildIndex = new Dictionary<Place, int>(){
            {Place.Hub, 0},
            {Place.ObstacleCourse, 1},
            {Place.LevelCreator, 2},      
        };

        public static void MoveToPlace(Place place){
            if (_currentPlace == place) return;
            else _currentPlace = place;

            int buildIndex = _placeToBuildIndex[place];
            Debug.Log(_currentPlace.ToString());
            Debug.Log(buildIndex);
            SceneManager.LoadScene(GetSceneNameByBuildIndex(buildIndex));
            _currentPlace = place;
        }

        public static bool IsPlace(int buildIndex, Place comparePlace) => ((int)comparePlace == buildIndex) ? true : false;

        public static Place GetCurrentPlace() => _currentPlace;

        public static string GetSceneNameByBuildIndex(int index) => _sceneNames[index];
    }
}
