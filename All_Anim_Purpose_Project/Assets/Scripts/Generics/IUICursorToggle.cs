using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUICursorToggle{
    public static EventHandler<EventArgs> OnHide;
    public static EventHandler<EventArgs> OnShow;
}
