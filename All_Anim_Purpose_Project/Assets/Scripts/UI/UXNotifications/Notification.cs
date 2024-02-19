using System;
using System.Collections.Generic;

public class Notification{
    private readonly Dictionary<UXManager.UXType_Notification, Tuple<string,string>> notifications = new Dictionary<UXManager.UXType_Notification, Tuple<string,string>>() {
        {UXManager.UXType_Notification.Notification_GAMELOST, 
            Tuple.Create("<color=red>Notification</color>","<color=red> Game Lost! </color> \n Return to Hub and try again.")},
        {UXManager.UXType_Notification.Notification_GAMEWON, 
            Tuple.Create("<color=green>Notification</color>","<color=green> Won! </color> \n Return to Hub and try again or select a different mode.")},
    };

    public string GetNotification(UXManager.UXType_Notification notificationType) => notifications[notificationType].Item2;
    public string GetTitle(UXManager.UXType_Notification notificationType) => notifications[notificationType].Item1;
}
