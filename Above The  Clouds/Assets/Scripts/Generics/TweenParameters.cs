using System;
using UnityEngine;

public class TweenParameters{
    private GameObject _tweenable;
    private Vector3 _targetPosition, _targetRotationEuler, _targetScale;
    private float _tweenSpeed, _tweenTimeout;
    private Action _callback;

    public TweenParameters(GameObject tweenable, Vector3 targetPosition, Vector3 targetRotationEuler, Vector3 targetScale, float tweenSpeed, float tweenTimeout, Action action = default){
        _tweenable = tweenable;
        _targetPosition = targetPosition;
        _targetRotationEuler = targetRotationEuler;
        _targetScale = targetScale;
        _tweenSpeed = tweenSpeed;
        _tweenTimeout = tweenTimeout;
        _callback = action;
    }

    public GameObject GetTweenable() => _tweenable;
    public float GetTimeOut() => _tweenTimeout;

    public void ExecuteCallback() => _callback();
    public void TweenAll(){
        TweenPosition();
        TweenRotation();
        TweenScale();
    }

    public void TweenPosition() => _tweenable.transform.position = Vector3.LerpUnclamped(_tweenable.transform.position, _targetPosition, _tweenSpeed * Time.deltaTime);
    public void TweenRotation() => _tweenable.transform.eulerAngles = Vector3.Lerp(_tweenable.transform.eulerAngles, _targetRotationEuler, (_tweenSpeed - _tweenTimeout) * Time.deltaTime);
    public void TweenScale() => _tweenable.transform.localScale = Vector3.LerpUnclamped(_tweenable.transform.localScale, _targetScale, _tweenSpeed * Time.deltaTime);
}