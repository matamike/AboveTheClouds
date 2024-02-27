using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UXManager : Singleton<UXManager>{
    public enum UXType_Notification{
        Notification_GAMEWON,
        Notification_GAMELOST,
    }

    public enum UXType_Tip{
        Tip_TileType_DropTile,
        Tip_TileType_SpinningTile,
        Tip_TileType_BouncyTile,
        Tip_SelectMode_Random_Easy,
        Tip_SelectMode_Random_Medium,
        Tip_SelectMode_Random_Hard,
        Tip_SelectMode_UserDefined,
        Tip_SelectMode_Creator,
    }

    public enum UXType_Tutorial
    {
        Welcome_Tutorial,
        Game_Tutorial,
    }

    //UI Variables
    [SerializeField] private GameObject _uxContainer;
    [SerializeField] private GameObject _uxCardTemplatePrefab;

    //Modules
    private Notification notification;
    private Tip tip;
    private Tutorial tutorial;

    private void Start(){
        InitializeModules();
    }

    private void InitializeModules(){
        notification = new();
        tip = new();
        tutorial = new();
    }

    private GameObject CreateUXCard(){
        GameObject card = Instantiate(_uxCardTemplatePrefab, _uxContainer.transform);
        card.transform.position = _uxContainer.transform.position + Vector3.down * 400f;
        return card;
    }

    public void FireUX(UXType_Notification uxNotificationType, Action preActionCallback = null, Action postActionCallback = null)
    {
        string message = notification.GetText(uxNotificationType);
        string title = notification.GetTitle(uxNotificationType);
        PrepareUXAlert(message, title, preActionCallback, postActionCallback);
    }

    public void FireUX(UXType_Tip uxTipType, Action preActionCallback = null, Action postActionCallback = null)
    {
        string message = tip.GetText(uxTipType);
        string title = tip.GetTitle(uxTipType);
        PrepareUXAlert(message, title, preActionCallback, postActionCallback);
    }

    public void FireUX(UXType_Tutorial uxTipType, Action preActionCallback = null, Action postActionCallback = null)
    {
        string message = tutorial.GetText(uxTipType);
        string title = tutorial.GetTitle(uxTipType);
        PrepareUXAlert(message, title, preActionCallback, postActionCallback);
    }

    private void PrepareUXAlert(string uxText, string uxTitle, Action preActionCallback, Action postActionCallback)
    {
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
            Debug.Log("Close Prompt Detected!");
            InputManager.Instance.SetControlLockStatus(false);
            CameraController.Instance.SetLockCameraStatus(false);
            TweenParameters tweenParamsReturn = new(card, _uxContainer.transform.position + Vector3.down * 1000f, Vector3.zero, card.transform.localScale, 6f, 2f);
            TweenHandler.Instance.CreateTween(tweenParamsReturn);
            Destroy(card, 2f);
            if(postActionCallback != null) postActionCallback();
        });

        //Assign Original Position
        InputManager.Instance.SetControlLockStatus(true);
        CameraController.Instance.SetLockCameraStatus(true);
        TweenParameters tweeParamsFire = new(card, _uxContainer.transform.position, new Vector3(0,0,359.9f), card.transform.localScale, 12f, 2f);
        TweenHandler.Instance.CreateTween(tweeParamsFire);
        if (postActionCallback != null) preActionCallback();
    }
}