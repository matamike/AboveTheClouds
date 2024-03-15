using UnityEngine;
using ObjTween;

public class Letter : MonoBehaviour{
    [SerializeField] private char letter = ' ';
    private string[] layerNames = { "Player" };

    private void OnTriggerEnter(Collider other){
        if (LayerUtility.LayerIsName(other.gameObject.layer, layerNames)){
            FadeOutLetter();
        }
    }

    private void FadeOutLetter(){
        float tweenTime = Random.Range(1f, 2f);
        float timeout = tweenTime * 2;
        Vector3 targetPosition = transform.position + new Vector3(0f, -10f, 0f);
        TweenParameters tweenParameters = new TweenParameters(gameObject, targetPosition, Vector3.zero, gameObject.transform.localScale, tweenTime, timeout);
        TweenHandler.Instance.CreateTween(tweenParameters);
        LogoHandler.Instance.RequestCharacterRemoval(gameObject, timeout);
    }

    public char GetLetter() => letter;
}
