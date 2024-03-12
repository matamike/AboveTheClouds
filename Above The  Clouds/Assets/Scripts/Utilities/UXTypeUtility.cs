using System;
using System.Collections.Generic;

public static class UXTypeUtility{
        public enum UXType{
            _TileType_DropTile,
            _TileType_SpinningTile,
            _TileType_BouncyTile,
            _SelectMode_Random_Easy,
            _SelectMode_Random_Medium,
            _SelectMode_Random_Hard,
            _SelectMode_UserDefined,
            _SelectMode_Creator,
            _Notification_GAMELOST,
            _Notification_GAMEWON,
            _Notification_REACHED_WORLDBOUNDS,
            _Welcome_Tutorial,
            _Game_Tutorial,
            _Creator_Tutorial,
            _Creator_Tutorial_TemplatesList,
            _Creator_Tutorial_TileChange,
            _Creator_Tutorial_SaveTemplate,
            _Creator_Tutorial_TestTemplate,
        }

    private static readonly Dictionary<UXType, Tuple<string, string>> _UxDictionary = new Dictionary<UXType, Tuple<string, string>>() {
        {UXType._TileType_DropTile,
            Tuple.Create("<color=yellow>Tip</color>",
                "Drop Tiles activate upon touch with the player and fall after a short while. \n" +
                " Beware!")},
        {UXType._TileType_SpinningTile,
            Tuple.Create("<color=yellow>Tip</color>",
                "Spinning Tiles rotate upon activation with player \n " +
                "and player has to hold onto the tile. \n " +
                "Beware!")},
        {UXType._TileType_BouncyTile,
            Tuple.Create("<color=yellow>Tip</color>",
                "Bouncy Tiles charge and launch the player on the air.\n " +
                "You can use it to your advantage (or not).")},
        {UXType._SelectMode_Random_Easy,
            Tuple.Create("<color=green>Tip</color>", 
                "Random Generation of Tiles Mode.\n (X,X) size of grid will be generated with specific tile types. \n" +
                "Walk at the center of the portal to start! \n <color=green> Easy Mode</color>")},
        {UXType._SelectMode_Random_Medium,
            Tuple.Create("<color=yellow>Tip</color>", 
                "Random Generation of Tiles Mode.\n (X,X) size of grid will be generated with specific tile types.\n" +
                "Walk at the center of the portal to start! \n <color=yellow> Medium Mode</color>")},
        {UXType._SelectMode_Random_Hard,
            Tuple.Create("<color=red>Tip</color>", 
                "Random Generation of Tiles Mode.\n (X,X) size of grid will be generated with specific tile types.\n" +
                "Walk at the center of the portal to start! \n  <color=red> Hard Mode </color>")},
        {UXType._SelectMode_UserDefined,
            Tuple.Create("<color=blue>Tip</color>", 
                "In this mode you select one of the following templates.\n " +
                "Walk at the center of the portal to start! \n " +
                "You can edit those templates in the Creator.")},
        {UXType._SelectMode_Creator,
            Tuple.Create("<color=white>Tip</color>",
                "Creator Mode allows you to modify Templates to your liking.\n" +
                "Walk at the center of the portal to start! \n " +
                "Head over to the <color=white> WHITE </color> portal to access it! \n")},
        {UXType._Welcome_Tutorial,
            Tuple.Create("<color=yellow>Tutorial</color>",
                "<color=red>Welcome to the game: Above The clouds!</color> \n " +
                "This is an obstacle course game. \n " +
                "Visit each of the portal around the game area to learn more!")},
        {UXType._Game_Tutorial,
            Tuple.Create("<color=yellow>Tutorial</color>", 
                "In this level you need to cross from the <color=yellow>starting point</color> to the <color=yellow>last checkpoint</color>! \n" +
                "Each Random Level will be build <b>differently</b> and according to the difficulty preset! \n"
                + "Good Luck! \n")},
        {UXType._Creator_Tutorial,
            Tuple.Create("<color=yellow>Tutorial</color>", 
                "This is <color=yellow>Creator</color> Mode. \n" +
                "Toggle the creator button on the upper right corner to begin.\n" +
                "Select one of the templates to modify them. \n" +
                "Each tile can be <i>changed, saved and tested.</i> \n"
                + "Have fun creating! \n")},
        {UXType._Creator_Tutorial_TemplatesList,
            Tuple.Create("<color=yellow>Tutorial</color>", 
                "The <color=yellow>Template List</color> contains all the available containers to load and edit. \n" +
                "Press on one of the following option to test/modify their mapping. \n")},
        {UXType._Creator_Tutorial_TileChange,
            Tuple.Create("<color=yellow>Tutorial</color>", 
                "When pressing the <color=red>designated tile </color> : \n" +
                "You can change the tile type for that <b>specific</b> position.\n" +
                "You can save the whole template after finishing with multiple changes or 1 by 1. \n" +
                "You can test the mapping whenever you are ready <i>changed, saved and tested.</i> \n"
                + "Have fun creating! \n")},
        {UXType._Creator_Tutorial_SaveTemplate,
            Tuple.Create("<color=yellow>Tutorial</color>", 
                "Save the <color=red>template </color> : \n" +
                "You can then briefly test it or use the <color=white> white </color> portal back in <b>Hub</b> to test it.\n")},
        {UXType._Creator_Tutorial_TestTemplate,
            Tuple.Create("<color=yellow>Tutorial</color>", 
                "Test the <color=red>template </color> : \n" +
                "You can test the template ( saved or unsaved) before commit any change by pressing the Test button.\n")},
        {UXType._Notification_GAMELOST,
            Tuple.Create("<color=red>Notification</color>",
                "<color=red> Game Lost! </color> \n " +
                "Return to Hub and try again.")},
        {UXType._Notification_GAMEWON,
            Tuple.Create("<color=green>Notification</color>", 
                "<color=green> Won! </color> \n " +
                "Return to Hub and try again or select a different mode.")},
        {UXType._Notification_REACHED_WORLDBOUNDS,
            Tuple.Create("<color=yellow>Warning</color>", 
                "<color=yellow> Reached World Bounds </color> \n " +
                "Please remain inside the game area.")}
    };
    public static string GetUXHeader(UXType uxType) => _UxDictionary[uxType].Item1;
    public static string GetUXText(UXType uxType) => _UxDictionary[uxType].Item2;
};