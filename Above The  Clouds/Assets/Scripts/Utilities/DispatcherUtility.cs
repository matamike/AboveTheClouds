using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class DispatcherUtility{
    public static event EventHandler<SpecificMemberEventArgs> OnRemoveRequest;

    public class SpecificMemberEventArgs : EventArgs{
        public GameObject _sender;
        public GameObject _receiver;
    }

    public static void RequestBroadcast(GameObject sender, GameObject receiver){
        OnRemoveRequest?.Invoke(sender, new SpecificMemberEventArgs {_sender = sender , _receiver = receiver });
    }
}
