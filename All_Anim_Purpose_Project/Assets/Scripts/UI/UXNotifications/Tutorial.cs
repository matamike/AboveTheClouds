using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    };

    public string GetText(UXManager.UXType_Tutorial tutorialType) => tutorials[tutorialType].Item2;
    public string GetTitle(UXManager.UXType_Tutorial tutorialType) => tutorials[tutorialType].Item1;

    public object GetUXPopupType() => this.GetType();
}
