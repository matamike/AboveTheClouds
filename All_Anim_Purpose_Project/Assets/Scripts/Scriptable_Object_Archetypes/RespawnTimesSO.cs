using UnityEngine;

[CreateAssetMenu()]
public class RespawnTimesSO : ScriptableObject{
    [SerializeField] private int numberOfRespawns;
    public int GetNumberOfRespawns() => numberOfRespawns;
}
