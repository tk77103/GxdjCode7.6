using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PositionCapacity
{
    public List<short> SceneHuman = new();
    public short sceneCapacity;
    //��������
    public string scnName;
    //����ͼƬ·��
    public string scnImg;
    public PositionCapacity() { }
    public PositionCapacity(List<short> huamanId, short huamanIdLimit, string scnName, string scnImg)
    {
        SceneHuman.AddRange(huamanId);
        SceneHuman.Capacity = huamanIdLimit;
        sceneCapacity = huamanIdLimit;
        scnName = scnName;
        scnImg = scnImg;
    }
}


public class CompanyMgr : BaseManger<CompanyMgr>
{
    #region ��������
    private Dictionary<short, Company> WorldAllCompany = new();
    private List<Company> workCompany = new();
    private List<Company> meetPlace = new();
    private List<Company> InvitationPlace = new();
    private System.Random r = new();
    #endregion
    #region ����
    private CompanyMgr() { }
    #region ��ʼ��
    public void InitWorldCompany(Dictionary<string, WorldScene> worldSecne)
    {
        short id = 0;
        short[] oriBodyValues;
        foreach (var key in worldSecne.Keys)
        {
            //��������Ӧ�˵�id�ҵ�
            oriBodyValues = new[] { worldSecne[key].oriBody01, worldSecne[key].oriBody02,worldSecne[key].oriBody03, worldSecne[key].oriBody04,
                                         worldSecne[key].oriBody05, worldSecne[key].oriBody06, worldSecne[key].oriBody07,worldSecne[key].oriBody08};
            List<short> humanId = new();
            foreach (var z in oriBodyValues) if (z != 0) humanId.Add(z);
            List<PositionCapacity> positionCapacity = new();
            positionCapacity.Add(new(humanId, worldSecne[key].bodyTotal, worldSecne[key].scnName, worldSecne[key].scnImg));
            id = short.Parse(key.Split(',')[0]);
            if (!WorldAllCompany.ContainsKey(id))
            {
                WorldAllCompany.Add(id, new Company()
                {
                    ID = id,
                    Name = worldSecne[key].scnCompany,
                    area = worldSecne[key].scnArea,
                    companyLevel= worldSecne[key].scnCoLevel,
                    Industry = worldSecne[key].scnIndustry,
                    segmentationFunction = worldSecne[key].scnFunction,
                    academicBias = worldSecne[key].scnAcademy,
                    isVisible = worldSecne[key].scnVisible,
                    scnCost = worldSecne[key].scnCost,
                    canWork = worldSecne[key].scnIsWork,
                    canPartTime = worldSecne[key].scnIsPartTime,
                    comPos = new Vector2(worldSecne[key].scnX, worldSecne[key].scnY),
                    mapBtnImg = worldSecne[key].mapBtnImg,
                    mpcBtnImgChosen = worldSecne[key].mpcBtnImgChosen,
                    humanAmount = worldSecne[key].bodyTotal
                });
            }
            WorldAllCompany[id].companyScene.Add(key, positionCapacity);
        }
    }
    #endregion
    #region ���ֳ�������
    public void SplitFunction()
    {
        workCompany = WorldAllCompany.Values.Where(e => e.segmentationFunction == "ְ��").ToList();
        InvitationPlace= WorldAllCompany.Values.Where(e => e.Industry == "Ʒ������"&&
        (e.segmentationFunction.Contains("�Ƶ�")||e.segmentationFunction.Contains("����") || e.segmentationFunction.Contains("����"))).ToList();
        meetPlace = WorldAllCompany.Values.Where(e => e.Industry == "Ʒ������").Except(InvitationPlace).ToList();
    }
    #endregion
    #region ��������ʵ��
    #region �������
    #region �������
    #endregion
    #region �����������
    #endregion
    #endregion
    #region �ۻ����
    public int GetMeetCost(short npcSocialRank,int humanAmount)
    {
        return meetPlace.Filter(e=>e.companyLevel==npcSocialRank).GetRandomItem().scnCost*humanAmount;
    }
    #endregion
    #region ��Լ���
    public int GetInvitatioCost(short npcSocialRank)
    {
        return InvitationPlace.Filter(e => e.companyLevel == npcSocialRank).GetRandomItem().scnCost * 2;
    }
    #endregion
    #endregion
    #endregion
}
