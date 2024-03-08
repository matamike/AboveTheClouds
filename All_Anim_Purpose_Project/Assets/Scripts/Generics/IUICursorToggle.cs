using System;

public interface IUICursorToggle{
    public static EventHandler<EventArgs> OnToggle;
    public static EventHandler<EventArgs> OnForceClose;
    public static EventHandler<EventArgs> OnCursorHide;
    public static EventHandler<EventArgs> OnCursorShow;
}