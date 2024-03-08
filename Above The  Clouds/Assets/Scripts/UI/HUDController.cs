using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : Singleton<HUDController>{
    [SerializeField] private GameObject hudContainer;
    [SerializeField] private Image remainingAttemptsImage;
    [SerializeField] private TextMeshProUGUI remainingAttemptsText;

    private void Start(){
        InitializeRemainingLifes();
    }

    private void OnEnable(){
        MyGameManager.OnPlayerRespawned += MyGameManager_OnPlayerRespanwed;
    }

    private void OnDisable(){
        MyGameManager.OnPlayerRespawned -= MyGameManager_OnPlayerRespanwed;
    }

    private void MyGameManager_OnPlayerRespanwed(object sender, EventArgs args){
        UpdateRemainingLifes(MyGameManager.Instance.GetRemainingLifes(), MyGameManager.Instance.GetStartingLifes());
    }

    private void InitializeRemainingLifes(){
        int startingLifes = MyGameManager.Instance.GetStartingLifes();
        int remainingLifes = MyGameManager.Instance.GetRemainingLifes();
        float remainingLifesPercentage = remainingLifes / startingLifes;
        remainingAttemptsImage.fillAmount = remainingLifesPercentage;
        remainingAttemptsText.text = remainingLifes.ToString();
    }

    public void UpdateRemainingLifes(float remainingLifes, float startingLifes){
        float remainingLifesPercentage = remainingLifes / startingLifes;
        remainingAttemptsImage.fillAmount = remainingLifesPercentage;
        remainingAttemptsText.text = remainingLifes.ToString();
    }
}
