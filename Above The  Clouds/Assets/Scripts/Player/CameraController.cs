using System;
using UnityEngine;

public class CameraController : Singleton<CameraController>{
    public static event EventHandler OnDirectionChanged;

    [SerializeField] private Transform _rotateAroundTransform;
    [SerializeField][Range(1f, 10f)] private float rotationSpeed = 2f;
    [SerializeField] private Transform _followFocusTransform;
    [SerializeField][Range(0f, 1f)] private float followSpeed = 0.25f;
    [SerializeField][Range(1f, 10f)] private float followDistance = 6f;
    [SerializeField][Range(0f, -79f)] private float _XAxisAngleMinThreshold = -35f;
    [SerializeField][Range(0f, 79f)] private float _XAxisAngleMaxThreshold = 35f;
    private Vector3 _targetVelocity;
    private Vector3 _calculatedOffset;
    private float _lastKnownMousePositionXAxis, _lastKnownMousePositionYAxis;
    //Directions
    private Vector3 _cameraForward, _cameraBack;
    private Vector3 _cameraLeft, _cameraRight;
    private Vector3 _cameraForwardLeft, _cameraForwardRight;
    private Vector3 _cameraBackLeft, _cameraBackRight;
    private Vector3 _cameraUp, _cameraDown;
    [SerializeField] private float blockedViewFollowDistance = 2f;
    private bool _rotationLocked = false;
    //Prefs
    private float horizontalRotation = 1f, verticalRotation = 1f;

    private void OnEnable(){
        IUICursorToggle.OnCursorShow += IUICursorToggle_CursorShow;
        IUICursorToggle.OnCursorHide += IUICursorToggle_CursorHide;
        PreferencesUtility.OnInvertedMouseHorizontalChanged += PreferencesUtility_OnInvertedMouseHorizontalChanged;
        PreferencesUtility.OnInvertedMouseVerticalChanged += PreferencesUtility_OnInvertedMouseVerticalChanged;
    }

    private void OnDisable(){
        IUICursorToggle.OnCursorShow -= IUICursorToggle_CursorShow;
        IUICursorToggle.OnCursorHide -= IUICursorToggle_CursorHide;
        PreferencesUtility.OnInvertedMouseHorizontalChanged -= PreferencesUtility_OnInvertedMouseHorizontalChanged;
        PreferencesUtility.OnInvertedMouseVerticalChanged -= PreferencesUtility_OnInvertedMouseVerticalChanged;
    }

    private void IUICursorToggle_CursorHide(object sender, EventArgs e) => SetLockCameraStatus(false);
    private void IUICursorToggle_CursorShow(object sender, EventArgs e) => SetLockCameraStatus(true);

    private void PreferencesUtility_OnInvertedMouseVerticalChanged(object sender, PreferencesUtility.OnInvertedMouseVerticalEventArgs e){
        ChangeVerticalAxisCameraOrientationDirection(e.verticalInverted);
    }

    private void PreferencesUtility_OnInvertedMouseHorizontalChanged(object sender, PreferencesUtility.OnInvertedMouseHorizontalEventArgs e){
        ChangeHorizontalAxisCameraOrientationDirection(e.horizontalInverted);
    }

    private void Start(){
        _targetVelocity = Vector3.zero;
        CalculateOffset();
        ChangeVerticalAxisCameraOrientationDirection(PreferencesUtility.GetInvertedMouseVerticalAxisState());
        ChangeHorizontalAxisCameraOrientationDirection(PreferencesUtility.GetInvertedMouseHorizontalAxisState());
    }

    private void Update(){
        if(!_rotationLocked) RotateAroundTarget();
    }

    private void LateUpdate(){
        FollowTarget();
        UpdateDirections();
        CalculateOffset();
    }

    private void ChangeHorizontalAxisCameraOrientationDirection(bool flag){
        if (flag) horizontalRotation = 1f;
        else horizontalRotation = -1f;
    }

    private void ChangeVerticalAxisCameraOrientationDirection(bool flag)
    {
        if (flag) verticalRotation = 1f;
        else verticalRotation = -1f;
    }

    public void AssignFollowTransform(Transform followTransform){
        if (followTransform == null) return;
        _followFocusTransform = followTransform;
    }

    public void AssignRotateAroundTransform(Transform rotateAroundTransform){
        if (rotateAroundTransform == null) return;
        _rotateAroundTransform = rotateAroundTransform;
    }

    private void UpdateDirections(){
        if(_cameraForward != transform.forward) {
            _cameraForward = transform.forward;
            _cameraBack = -transform.forward;
            _cameraRight = transform.right;
            _cameraLeft = -transform.right;
            _cameraForwardRight = (transform.forward + transform.right).normalized;
            _cameraForwardLeft = (transform.forward + (-transform.right)).normalized;
            _cameraBackLeft = (-transform.forward + (-transform.right)).normalized;
            _cameraBackRight = (-transform.forward + (transform.right)).normalized;
            _cameraUp = transform.up;
            _cameraDown = -transform.up;
        }
        OnDirectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetLockCameraStatus(bool flag) => _rotationLocked = flag;

    public Vector3 GetCameraUp() => _cameraUp;
    public Vector3 GetCameraDown() => _cameraDown;
    public Vector3 GetCameraForward() => _cameraForward;
    public Vector3 GetCameraBack() => _cameraBack;
    public Vector3 GetCameraLeft() => _cameraLeft;
    public Vector3 GetCameraRight() => _cameraRight;
    public Vector3 GetCameraForwardLeft() => _cameraForwardLeft;
    public Vector3 GetCameraForwardRight() => _cameraForwardRight;
    public Vector3 GetCameraBackLeft() => _cameraBackLeft;
    public Vector3 GetCameraBackRight() => _cameraBackRight;

    private void FollowTarget(){
        if (_followFocusTransform == null) return;
        Vector3 targetPosition = _followFocusTransform.position + _calculatedOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _targetVelocity, followSpeed);
    }
    private void CalculateOffset(){
        if (_followFocusTransform == null) return;
        float calculatedFollowDistance = GetCalculatedClippingDistanceToTarget();

        _calculatedOffset = (transform.position - _followFocusTransform.position).normalized * calculatedFollowDistance;
    }

    private float GetCalculatedClippingDistanceToTarget(){
        float finalFollowDistance;
        
        //Get All Object in the Raycast Line (through distance)
        RaycastHit[] hitsFront = Physics.BoxCastAll(transform.position, Vector3.one * 0.15f, GetCameraForward(), Quaternion.identity, followDistance + 0.1f);
        RaycastHit[] hitsBack = Physics.BoxCastAll(transform.position, Vector3.one * 0.15f, GetCameraBack(), Quaternion.identity, followDistance + 0.1f);
        blockedViewFollowDistance = 2.0f;
        if (hitsFront.Length > 1){
            //Check for player clipping by other objects
            foreach(var hit in hitsFront){
                float distToFocus = Vector3.Distance(hit.point, _followFocusTransform.position);
                if (distToFocus < followDistance && distToFocus < blockedViewFollowDistance) blockedViewFollowDistance = distToFocus;
            }
            finalFollowDistance = blockedViewFollowDistance;
            if (finalFollowDistance < 0.5f) finalFollowDistance += 0.5f;
        }
        else if(hitsFront.Length == 1) {
            if (hitsBack.Length == 0) finalFollowDistance = followDistance; //No Object is clipping the back of Camera.
            else{
                //Object(s) clipping camera to desired default distance (extra computing)
                float hitAvgDistance = 0.0f;
                foreach(RaycastHit hit in hitsBack) hitAvgDistance += hit.distance;
                finalFollowDistance = hitAvgDistance/hitsBack.Length;
            }
        }
        else finalFollowDistance = followDistance;

        return finalFollowDistance;
    }
    private void RotateAroundTarget(){
        if (_rotateAroundTransform == null) return;
        //Retrieve normalized mouse X,Y Axis Values
        _lastKnownMousePositionXAxis = MouseUtility.GetMouseXNormalized() * horizontalRotation;
        _lastKnownMousePositionYAxis = MouseUtility.GetMouseYNormalized() * verticalRotation;

        //Calculate offset (Apply Negative spectrum)
        float xRotationEulerAngles;
        if(transform.eulerAngles.x > 0f && transform.eulerAngles.x < 80f) xRotationEulerAngles = transform.eulerAngles.x;
        else xRotationEulerAngles = transform.eulerAngles.x - 360f;

        //Rotation X,Y Axeses
        transform.RotateAround(_rotateAroundTransform.position, transform.up, _lastKnownMousePositionXAxis * rotationSpeed); //Mouse X (Y Axis - Rotation)

        //Mouse Y (X Axis - Rotation)
        if ((xRotationEulerAngles > 0f && xRotationEulerAngles < _XAxisAngleMaxThreshold) || (xRotationEulerAngles < 0f && xRotationEulerAngles > _XAxisAngleMinThreshold)){
            transform.RotateAround(_rotateAroundTransform.position, transform.right, _lastKnownMousePositionYAxis * rotationSpeed); 
        }
        else //Limits Reached
        {
            if(xRotationEulerAngles > _XAxisAngleMaxThreshold && _lastKnownMousePositionYAxis < 0f)
                transform.RotateAround(_rotateAroundTransform.position, transform.right, _lastKnownMousePositionYAxis * rotationSpeed);

            if (xRotationEulerAngles < _XAxisAngleMinThreshold && _lastKnownMousePositionYAxis > 0f)
                transform.RotateAround(_rotateAroundTransform.position, transform.right, _lastKnownMousePositionYAxis * rotationSpeed);
        }

        //Face Target
        transform.LookAt(_rotateAroundTransform, Vector3.up);
    }
}