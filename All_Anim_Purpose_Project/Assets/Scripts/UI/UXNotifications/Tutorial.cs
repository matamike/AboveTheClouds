using System;
using System.Collections.Generic;
public class Tutorial : IUXPopup{
    private readonly Dictionary<UXManager.UXType_Tutorial, Tuple<string, string>> tutorials = new Dictionary<UXManager.UXType_Tutorial, Tuple<string, string>>() {
        {UXManager.UXType_Tutorial.Welcome_Tutorial,
            Tuple.Create("<color=yellow>Tutorial</color>",
                "<color=red> Welcome to the Game -> Above The clouds! </color> \n " +
                "This is an obstacle course game. \n Visit each of the portal around the game area to learn more!")},
        {UXManager.UXType_Tutorial.Game_Tutorial,
            Tuple.Create("<color=yellow>Tutorial</color>","In this level you need to cross from the <color=yellow>starting point</color> to the <color=yellow>last checkpoint</color>! \n" +
                "Each Random Level will be build <b>differently</b> and according to the diffuclty preset! \n"
                + "Good Luck! \n")},
        {UXManager.UXType_Tutorial.Creator_Tutorial,
            Tuple.Create("<color=yellow>Tutorial</color>","This is <color=yellow>Creator</color> Mode. \n" +
                "Toggle the creator button on the upper right corner to begin.\n" + 
                "Select one of the templates to modify them. \n" +
                "Each tile can be <i>changed, saved and tested.</i> \n"
                + "Have fun creating! \n")},
        {UXManager.UXType_Tutorial.Creator_Tutorial_TemplatesList,
            Tuple.Create("<color=yellow>Tutorial</color>","The <color=yellow>Template List</color> contains all the available containers to load and edit. \n" +
                "Press on one of the following option to test/modify their mapping. \n")},
        {UXManager.UXType_Tutorial.Creator_Tutorial_TileChange,
            Tuple.Create("<color=yellow>Tutorial</color>","When pressing the <color=red>designated tile </color> : \n" +
                "You can change the tile type for that <b>specific</b> position.\n" +
                "You can save the whole template after finishing with multiple changes or 1 by 1. \n" +
                "You can test the mapping whenever you are ready <i>changed, saved and tested.</i> \n"
                + "Have fun creating! \n")},
        {UXManager.UXType_Tutorial.Creator_Tutorial_SaveTemplate,
            Tuple.Create("<color=yellow>Tutorial</color>","Save the <color=red>template </color> : \n" +
                "You can then briefly test it or use the <color=white> white </color> portal back in <b>Hub</b> to test it.\n")},
        {UXManager.UXType_Tutorial.Creator_Tutorial_TestTemplate,
            Tuple.Create("<color=yellow>Tutorial</color>","Test the <color=red>template </color> : \n" +
                "You can test the template ( saved or unsaved) before commit any change by pressing the Test button.\n")},
    };

    public string GetText(UXManager.UXType_Tutorial tutorialType) => tutorials[tutorialType].Item2;
    public string GetTitle(UXManager.UXType_Tutorial tutorialType) => tutorials[tutorialType].Item1;

    public object GetUXPopupType() => this.GetType();
}
