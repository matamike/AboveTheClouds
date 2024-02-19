using TMPro;
using UnityEngine;
using UnityEngine.iOS;
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
        Tip_SelectMode_Random,
        Tip_SelectMode_UserDefined,
        Tip_SelectMode_Creator,
    }

    //UI Variables
    [SerializeField] private GameObject _uxContainer;
    [SerializeField] private GameObject _uxCardTemplatePrefab;

    //Modules
    private Notification notification;
    private Tip tip;

    private void Start(){
        InitializeModules();
    }

    private void InitializeModules(){
        notification = new();
        tip = new();
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.F1)){
            FireUX(UXType_Notification.Notification_GAMEWON);
        }
    }

    private GameObject CreateUXCard(){
        GameObject card = Instantiate(_uxCardTemplatePrefab, _uxContainer.transform);
        card.transform.position = _uxContainer.transform.position + Vector3.down * 400f;
        return card;
    }

    public void FireUX(UXType_Notification uxNotificationType){
        string message = notification.GetNotification(uxNotificationType);
        string title = notification.GetTitle(uxNotificationType);
        PrepareUXAlert(message, title);
    }

    public void FireUX(UXType_Tip uxTipType){
        string message = tip.GetTip(uxTipType);
        string title = tip.GetTitle(uxTipType);
        PrepareUXAlert(message, title);
    }

    private void PrepareUXAlert(string uxText, string uxTitle){
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
        //button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>{
            InputManager.Instance.SetControlLockStatus(false);
            CameraController.Instance.SetLockCameraStatus(false);
            TweenParameters tweenParamsReturn = new(card, _uxContainer.transform.position + Vector3.down * 1000f, Vector3.zero, card.transform.localScale, 6f, 2f);
            TweenHandler.Instance.CreateTween(tweenParamsReturn);
            Destroy(card, 2f);
        });

        //Assign Original Position
        InputManager.Instance.SetControlLockStatus(true);
        CameraController.Instance.SetLockCameraStatus(true);
        TweenParameters tweeParamsFire = new(card, _uxContainer.transform.position, new Vector3(0,0,359.9f), card.transform.localScale, 12f, 2f);
        TweenHandler.Instance.CreateTween(tweeParamsFire);
    }
}
