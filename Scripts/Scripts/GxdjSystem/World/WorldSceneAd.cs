using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSceneAd : WorldScene
{
    //在当前场景的人
    public List<short> sceneHumanID = new();
    public WorldSceneAd(WorldScene currentScene)
    {
        scnId = currentScene.scnId;
        scnArea=currentScene.scnArea;
        scnFunction= currentScene.scnFunction;
        scnIndustry=currentScene.scnIndustry;
        scnCompany=currentScene.scnCompany;
        scnName=currentScene.scnName;
        scnAcademy=currentScene.scnAcademy;
        scnCoLevel=currentScene.scnCoLevel;
        scnVisible=currentScene.scnVisible;
        scnCost=currentScene.scnCost;
        scnIsWork=currentScene.scnIsWork;
        scnIsPartTime=currentScene.scnIsPartTime;
        scnNote=currentScene.scnNote;
        scnNote = currentScene.scnNote;

        scnX=currentScene.scnX;
        scnY=currentScene.scnY;
        scnImg=currentScene.scnImg;
        mapBtnImg=currentScene.mapBtnImg;
        mpcBtnImgChosen=currentScene.mpcBtnImgChosen;

        if(currentScene.oriBody01!=0)
            sceneHumanID.Add(currentScene.oriBody01);
        if (currentScene.oriBody02 != 0)
            sceneHumanID.Add(currentScene.oriBody02);
        if (currentScene.oriBody03 != 0)
            sceneHumanID.Add(currentScene.oriBody03);
        if (currentScene.oriBody04 != 0)
            sceneHumanID.Add(currentScene.oriBody04);
        if (currentScene.oriBody05 != 0)
            sceneHumanID.Add(currentScene.oriBody05);
        if (currentScene.oriBody06 != 0)
            sceneHumanID.Add(currentScene.oriBody06);
        if (currentScene.oriBody07 != 0)
            sceneHumanID.Add(currentScene.oriBody07);
        if (currentScene.oriBody08 != 0)
            sceneHumanID.Add(currentScene.oriBody08);

    }
}
