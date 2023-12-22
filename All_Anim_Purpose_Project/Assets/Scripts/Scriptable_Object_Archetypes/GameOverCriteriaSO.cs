using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameOverCriteriaSO : ScriptableObject{
    [SerializeField] private int numberOfRespawns;
    public int GetGameOverCriteria() => numberOfRespawns;
}
