using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ObjTween;

public class UXManager : Singleton<UXManager>{
    //UI Variables
    [SerializeField] private GameObject _uxContainer;
    [SerializeField] private GameObject _uxCardTemplatePrefab;

    public void FireUX(string message = "", string title = "", Action preActionCallback = null, Action postActionCallback = null, bool cursorToggle = true)
    {
        PrepareUXAlert(message, title, preActionCallback, postActionCallback, cursorToggle);
    }


    private GameObject CreateUXCard(){
        GameObject card = Instantiate(_uxCardTemplatePrefab, _uxContainer.transform);
        card.transform.position = _uxContainer.transform.position + Vector3.down * 400f;
        return card;
    }

    private void PrepareUXAlert(string uxText, string uxTitle, Action preActionCallback, Action postActionCallback, bool cursorHideAfterAction = true){
        GameObject card = CreateUXCard();
        UXCard uxCard = card.GetComponent<UXCard>();

        //Description Text
        TextMeshProUGUI textDescription = uxCard.GetUXCardDescriptionTextHolder();
        textDescription.text = uxText; //set text

        //Title Text
        TextMeshProUGUI textTitle = uxCard.GetUXCardTitleTextHolder();
        textTitle.text = uxTitle;

        //Handle Button Listeners
        Button button = uxCard.GetUXCardButton();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>{
            InputManager.Instance.SetControlLockStatus(false);
            CameraController.Instance.SetLockCameraStatus(false);
            TweenParameters tweenParamsReturn = new(card, _uxContainer.transform.position + Vector3.down * 1000f, Vector3.zero, card.transform.localScale, 6f, 2f);
            TweenHandler.Instance.CreateTween(tweenParamsReturn);
            Destroy(card, 2f);
            if(postActionCallback != null) postActionCallback();
            if(cursorHideAfterAction) CursorVisibilityUtility.SetCursorVisibility(false);
        });

        //Assign Original Position
        InputManager.Instance.SetControlLockStatus(true);
        CameraController.Instance.SetLockCameraStatus(true);
        TweenParameters tweeParamsFire = new(card, _uxContainer.transform.position, new Vector3(0,0,359.9f), card.transform.localScale, 12f, 2f);
        TweenHandler.Instance.CreateTween(tweeParamsFire);
        if (preActionCallback != null) preActionCallback();
        CursorVisibilityUtility.SetCursorVisibility(true);
    }
}