using UnityEngine;

public class LogoTrigger : MonoBehaviour{
    private bool hasTriggeredLogo = false;
    private string[] layerNames = { "Player" };

    private void OnCollisionEnter(Collision collision){
        if (!LayerUtility.LayerIsName(collision.gameObject.layer, layerNames) || hasTriggeredLogo) return;
        hasTriggeredLogo = true;
        LogoHandler.Instance.RequestTrigger();
    }
}
