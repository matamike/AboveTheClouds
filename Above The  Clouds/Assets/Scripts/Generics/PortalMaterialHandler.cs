using UnityEngine;

public class PortalMaterialHandler : MonoBehaviour{
    private Material material;
    private bool changingSpeed = false;
    private float targetSpeed = 0.0f;

    private void Awake(){
        material = GetComponent<MeshRenderer>().sharedMaterial;
        targetSpeed = material.GetFloat("_speed");
    }

    private void Start(){
        ChangeIntensity(100f);
        TryChangeSpeed(0.1f);
    }

    private void Update(){
        UpdateSpeed();
    }

    public void TryChangeSpeed(float speed){
        if(speed != material.GetFloat("_speed")){
            targetSpeed = speed;
            changingSpeed = true;
        }   
    }

    public void ChangeIntensity(float intensity) => material.SetFloat("_intensity", intensity);

    private void UpdateSpeed(){
        if (changingSpeed){
            float tweenSpeedValue = Mathf.Lerp(material.GetFloat("_speed"), targetSpeed, 10f * Time.deltaTime);
            material.SetFloat("_speed", tweenSpeedValue);
            if (Mathf.Abs(tweenSpeedValue - targetSpeed) < 0.01f) changingSpeed = false;
        }
    }
}
