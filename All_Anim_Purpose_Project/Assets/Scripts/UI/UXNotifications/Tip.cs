using System;
using System.Collections.Generic;

public class Tip : IUXPopup{
    private readonly Dictionary<UXManager.UXType_Tip, Tuple<string,string>> tips = new Dictionary<UXManager.UXType_Tip, Tuple<string,string>>() {
        {UXManager.UXType_Tip.Tip_TileType_DropTile, 
            Tuple.Create("Tip","Drop Tiles activate upon touch with the player and fall after a short while. \n Beware!")},
        {UXManager.UXType_Tip.Tip_TileType_SpinningTile, 
            Tuple.Create("Tip","Spinning Tiles rotate upon activation with player \n and player has to hold onto the tile. \n Beware!")},
        {UXManager.UXType_Tip.Tip_TileType_BouncyTile, 
            Tuple.Create("Tip","Bouncy Tiles charge and launch the player on the air.\n You can use it to your advantage (or not).")},
        {UXManager.UXType_Tip.Tip_SelectMode_Random_Easy, 
            Tuple.Create("Tip", "Random Generation of Tiles Mode.\n (X,X) size of grid will be generated with specific tile types. \n <color=green>Easy Mode</color>")},
        {UXManager.UXType_Tip.Tip_SelectMode_Random_Medium,
            Tuple.Create("Tip", "Random Generation of Tiles Mode.\n (X,X) size of grid will be generated with specific tile types. <color=yellow>Medium Mode</color>")},
        {UXManager.UXType_Tip.Tip_SelectMode_Random_Hard,
            Tuple.Create("Tip", "Random Generation of Tiles Mode.\n (X,X) size of grid will be generated with specific tile types. <color=red>Hard Mode </color>")},

        {UXManager.UXType_Tip.Tip_SelectMode_UserDefined, 
            Tuple.Create("Tip", "In this mode you select one of the following templates.\n You can edit those templates in the Creator.")},
        {UXManager.UXType_Tip.Tip_SelectMode_Creator,
            Tuple.Create("Tip","Creator Mode allows you to modify Templates to your liking.\n Head over to the <color=white> WHITE </color> portal to access it! \n")},
    };


    public string GetText(UXManager.UXType_Tip tipType) => tips[tipType].Item2;
    public string GetTitle(UXManager.UXType_Tip tipType) => tips[tipType].Item1;

    public object GetUXPopupType() => this.GetType();
}
