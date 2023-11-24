using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BezierTest : Singleton<BezierTest>{
    [SerializeField] List<Transform> points = new List<Transform>();
    [SerializeField] Transform _moveable;
    private bool _interpolate = false;
    private float _interpolationAmount;
    [SerializeField] private int _lastIndex = 0;
    private Vector3 finalMoveablePosition = new Vector3(0,1,0);
    private int[] lastKnownVisitingIndices = new int[3];

    private Dictionary<Transform, int> _transformToIndexDictionary = new Dictionary<Transform, int>();
    private void Start()
    {
        for(int i = 0; i < points.Count; i++) _transformToIndexDictionary[points[i]] = i; //Populate Dictionary
    }

    public void SetLastTransformVisited(Transform t){
        if (!lastKnownVisitingIndices.Contains(_transformToIndexDictionary[t])) return;

        Debug.Log("CurrentIndex: " + _lastIndex);
        Debug.Log("Triggered Index: " + _transformToIndexDictionary[t]);
        _interpolationAmount = 0;
        finalMoveablePosition = Vector3.Lerp(finalMoveablePosition, t.position, _interpolationAmount);
        _lastIndex = _transformToIndexDictionary[t];
    }


    private void Update(){
        if (Input.GetKeyDown(KeyCode.Backspace))_interpolate = !_interpolate;
        
        if(_interpolate) _interpolationAmount = (_interpolationAmount + Time.deltaTime) % 1f;
        else _interpolationAmount = 0;

        QuadraticBezier();
        _moveable.transform.position = finalMoveablePosition;
    }

    private void QuadraticBezier()
    {
        if (!_interpolate) return;

        Transform[] interpolatingPointsGiven = GetInterpolatingPoints(_lastIndex);
        Vector3 ab_position = Vector3.Lerp(interpolatingPointsGiven[0].position, interpolatingPointsGiven[1].position, _interpolationAmount); //A->B
        Vector3 bc_position = Vector3.Lerp(interpolatingPointsGiven[1].position, interpolatingPointsGiven[2].position, _interpolationAmount); //B->C
        //Vector3 cd_position = Vector3.LerpUnclamped(interpolatingPointsGiven[2].position, interpolatingPointsGiven[3].position, _interpolationAmount); //C->D
        //Vector3 da_position = Vector3.LerpUnclamped(interpolatingPointsGiven[3].position, interpolatingPointsGiven[0].position, _interpolationAmount); //D->A
        Vector3 ab_bc_position = Vector3.Lerp(ab_position, bc_position, _interpolationAmount); //AB->BC 
        //Vector3 bc_cd_position = Vector3.LerpUnclamped(bc_position, cd_position, _interpolationAmount); //BC->CD
        //Vector3 cd_da_position = Vector3.LerpUnclamped(cd_position, da_position, _interpolationAmount); //CD->DA
        //Vector3 abcd_position = Vector3.LerpUnclamped(ab_bc_position, bc_cd_position, _interpolationAmount);
        //Vector3 cdab_position = Vector3.LerpUnclamped(bc_cd_position, cd_da_position, _interpolationAmount);
        //Vector3 finalPos = Vector3.LerpUnclamped(abcd_position, cdab_position, _interpolationAmount);
        finalMoveablePosition = Vector3.Lerp(finalMoveablePosition, ab_bc_position, _interpolationAmount);
    }

    public Transform[] GetInterpolatingPoints(int currentIndex)
    {
        List<int> indices = new List<int>();
        int resetIndex = 0;

        //Assign new Indices as interpolating points
        for (int i = 0; i < 3; i++)
        {
            if ((currentIndex + i) <= (points.Count - 1)) indices.Add(currentIndex + i);
            else
            {
                indices.Add(resetIndex);
                resetIndex++;
                if (resetIndex > (points.Count - 1)) resetIndex = 0;
            }
        }

        lastKnownVisitingIndices = indices.ToArray();
        Transform[] interpolatingPoints = new Transform[3];
        for (int i = 0; i < indices.Count; i++) interpolatingPoints[i] = points[indices[i]];

        for (int j=0; j < points.Count; j++){
            bool lightEnabled = false;
            if (indices.Contains(j)) lightEnabled = true;
            points[j].gameObject.GetComponent<Light>().enabled = lightEnabled;
        }

        return interpolatingPoints;
    }
}
