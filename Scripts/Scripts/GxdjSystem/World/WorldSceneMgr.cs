using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WorldSceneMgr : BaseManger<WorldSceneMgr>
{
    #region ����
    System.Random r = new();
    public Dictionary<short, AllNpc> allNpc;
    public Dictionary<string, WorldScene> worldScene;

    //���õ����糡���ֵ�
    public Dictionary<string, WorldSceneAd> worldSceneAd = new();
    //����npc�ֵ�
    public Dictionary<short, NpcBase> worldAllNpc = new();
    //���ڼ���ÿ���ײ�����
    List<List<NpcBase>> eachSocial = new();
    #endregion
    #region ����
    #region ���캯��
    private WorldSceneMgr()
    {
        BinaryDataMgr.Instance.LoadTable<AllNpcContainer, AllNpc>();
        BinaryDataMgr.Instance.LoadTable<WorldSceneContainer, WorldScene>();
        allNpc = BinaryDataMgr.Instance.GetTable<AllNpcContainer>().dataDic;
        worldScene = BinaryDataMgr.Instance.GetTable<WorldSceneContainer>().dataDic;
        LoadTable();
        LoadNpcData();
        GenrateWorldNpc();
    }
    #endregion
    #region ����Ӧ����id��䵽������
    public void LoadTable()
    {
        foreach (var currentScene in worldScene.Values)
            worldSceneAd.Add(currentScene.scnId, new WorldSceneAd(currentScene));
    }
    #endregion
    #region ���ݹ�˾��ϢΪnpc����������
    public void LoadNpcData()
    {//ȡ������ûһ������ �������µ�npc��worldAllNpc��
        foreach (var currentScene in worldSceneAd.Values)
        {
            if (currentScene.sceneHumanID.Any())
            {
                foreach (short id in currentScene.sceneHumanID)
                {
                    worldAllNpc.Add(id, new NpcBase()
                    {
                        #region ������Ϣ
                        Id = allNpc[id].id,
                        name = allNpc[id].name,
                        gender = allNpc[id].gender,
                        npcAge = allNpc[id].npcAge,
                        race = (Nation)allNpc[id].race,
                        zodiac = (Constellation)allNpc[id].month,
                        #endregion
                        #region ��ʶ����
                        degree = (Degree)allNpc[id].degree,
                        intellegence = allNpc[id].intellegence,
                        growType = allNpc[id].growType,
                        #endregion
                        #region �������
                        #region н�����
                        jobLevel = allNpc[id].jobTag,
                        industry = currentScene.scnIndustry,
                        jobTitle = allNpc[id].jobType,
                        #endregion
                        #region ��������Ҫ�����
                        companyAcademy = currentScene.scnAcademy,
                        #endregion
                        company = currentScene.scnCompany,
                        #endregion
                        moral = allNpc[id].moral,
                        //moralTag = NpcEmotionMgr.Instance.GetMoralTag(allNpc[id].moral),
                        charm = allNpc[id].charm,
                        defaultScene = allNpc[id].scene,



                        isWorkplace = currentScene.scnFunction == "ְ��" ? true : false,


                        wealthRank = allNpc[id].wealthRank,


                        investTendency = allNpc[id].investPro,
                        rumorType = allNpc[id].spreadPro,

                        image = allNpc[id].charLocation,
                        imageName = allNpc[id].charImg,
                        companyArea = currentScene.scnArea
                    });
                }
            }
        }
    }
    #endregion
    #region ����npc����
    private void GenrateWorldNpc()
    {
        foreach (var npc in worldAllNpc.Values)
        {
            #region ������Ϣ
            npc.pregnance = false;
            #endregion
            #region ��ʶ����
            #endregion
            #region �������
            npc.basicSalary = WorkMgr.Instance.MatchSalaryFirstAmendment(npc);
            #endregion
            #region λ����Ϣ

            #endregion
            #region �������
            npc.preferred = (ItemSort)r.Next(0, 4);
            #endregion
            #region �Ƹ��Ʋ����
            npc.money = Wealth.Instance.BaseJobLevelToMoney(npc.jobLevel);
            //if (npc.jobLevel >= 1 && npc.basicSalary == 0)
            //   Debug.Log("�˹�˾δ�ܽ���н�ʼ���" + "_" + npc.company+"_" +"��ҵ"+npc.industry+ npc.jobLevel + npc.jobTitle + "npcIDΪ" + "_" + npc.Id);
            Estate myHouse = EstateMgr.Instance.GnerateOriginEst(npc.jobLevel);
            if (myHouse != null)
                npc.estate.Add(myHouse);
            EstateMgr.Instance.ConfrimNpcLiveAddress(npc);
            Vehicles myCar = VehiclesMgr.Instance.GetNpcVehicles(npc.jobLevel);
            if (myCar != null)
                npc.vehicles.Add(myCar);
            List<StockTrade> myStock = StockMgr.Instance.StockOriginGenerate(npc.jobLevel, npc.company);
            if (myStock != null)
                npc.stock.AddRange(myStock);
            #endregion
            #region �˼ʹ�ϵ
            npc.knowPlayer = false;
            npc.privateInfoShow = false;
            npc.secretInfoShow = false;
            npc.sexWithPlayer = false;
            npc.loveStatus = NpcEmotionMgr.Instance.GetNpcLoveStatus(npc.npcAge);
            #endregion
            #region ���Ӱ��ȼ�
            npc.socialRank = Wealth.Instance.CurrentSocailRank(npc);
            #endregion
        }
        #region ������Ӫ��ҵ����
        SelfEmployedMgr.Instance.GenerateSelfEmployed(worldAllNpc);
        #endregion
        #region npc��ϵƥ��
        #region ��ż
        NpcEmotionMgr.Instance.GenerateSpouse(worldAllNpc);
        #endregion
        #region ���� ��������
        NpcEmotionMgr.Instance.MatchWorldCouple(worldAllNpc);
        #endregion
        #region ͬ��
        //NpcEmotionMgr.Instance.GenreatCollugeRealthion(worldAllNpc);
        #endregion
        #region ȷ����Ľ����
        #endregion
        //NpcEmotionMgr.Instance.GetAdmirationObject(worldAllNpc);
        #endregion
        #region ��ʼ�����繫˾
        CompanyMgr.Instance.InitWorldCompany(worldScene);
        #region ���ֹ���
        CompanyMgr.Instance.SplitFunction();
        #endregion
        #endregion
    }
    #endregion

    #region ����npc��������
    public string ListenData(bool isfirst)
    {
        for (int i = 0; i < 6; i++)
        {
            if (worldAllNpc.FilterValues(e => e.socialRank == i).Any())
                eachSocial.Add(worldAllNpc.FilterValues(e => e.socialRank == i));
        }

        string txt = "";
        txt += WorldClock.Instance.clockExpress + "\n";
        var ageBounds = new[] { 15, 25, 35, 45, 55, 65, 75, 85 };
        var counts = new int[9]; // 8 ������� + >85 ��
        if (isfirst)
        {
            txt += $"����������{worldAllNpc.Count}\n";
            txt += $"���������ֲ�Ϊ�Ĵ���{worldAllNpc.FilterValues(e => e.race == Nation.�Ĵ�).Count}\n" +
                $"����:{worldAllNpc.FilterValues(e => e.race == Nation.����).Count}\n" +
                $"ӣɽ:{worldAllNpc.FilterValues(e => e.race == Nation.ӣɽ).Count}\n" +
                $"̩��:{worldAllNpc.FilterValues(e => e.race == Nation.̩��).Count}\n" +
                $"����:{worldAllNpc.FilterValues(e => e.race == Nation.����).Count}\n";
        }
        #region ����ͳ��
        if (worldAllNpc != null)
        {
            foreach (var npc in worldAllNpc.Values)
            {
                if (npc != null && npc.npcAge >= 0)
                {
                    int i = 0;
                    while (i < ageBounds.Length && npc.npcAge > ageBounds[i])
                        i++;
                    counts[i]++;
                }
            }
        }
        txt += string.Join("\n", ageBounds.Select((b, i) => $"����{b}��������{counts[i]}"));
        if (counts[8] > 0)
            txt += $"\n����{ageBounds.Last()}������������{counts[8]}";
        #endregion
        #region ְ�� �Ƹ� ���ȼ�����
        txt += string.Join("\n", Enumerable.Range(0, 6).Select(i =>
$"�������ȼ�Ϊ{i}������{worldAllNpc.FilterValues(e => e.wealthRank == i).Count}"));

        txt += string.Join("\n", Enumerable.Range(0, 6).Select(i =>
             $"�������ȼ�Ϊ{i}������{worldAllNpc.FilterValues(e => e.socialRank == i).Count}"));

        txt += string.Join("\n", Enumerable.Range(0, 6).Select(i =>
                $"����ְ��ȼ�Ϊ{i}������{worldAllNpc.FilterValues(e => e.jobLevel == i).Count}"));
        #endregion
        #region ��ʶ�ȼ�����
        txt += $"���������ȼ�Ϊ0-20���������Ϊ{worldAllNpc.FilterValues(e => e.intellegence > 0 && e.intellegence <= 20).Count}\n" +
    $"���������ȼ�Ϊ20-40���������Ϊ{worldAllNpc.FilterValues(e => e.intellegence > 20 && e.intellegence <= 40).Count}\n" +
    $"���������ȼ�Ϊ40-60���������Ϊ{worldAllNpc.FilterValues(e => e.intellegence > 40 && e.intellegence <= 60).Count}\n" +
    $"���������ȼ�Ϊ60-80���������Ϊ{worldAllNpc.FilterValues(e => e.intellegence > 60 && e.intellegence <= 80).Count}\n" +
    $"���������ȼ�Ϊ80-100���������Ϊ{worldAllNpc.FilterValues(e => e.intellegence > 80 && e.intellegence <= 100).Count}\n" +
    $"���������ȼ�Ϊ100-120���������Ϊ{worldAllNpc.FilterValues(e => e.intellegence > 100 && e.intellegence <= 120).Count}\n" +
    $"���������ȼ�Ϊ120-140���������Ϊ{worldAllNpc.FilterValues(e => e.intellegence > 120 && e.intellegence <= 140).Count}\n" +
    $"���������ȼ�Ϊ140-160���������Ϊ{worldAllNpc.FilterValues(e => e.intellegence > 140 && e.intellegence <= 160).Count}\n";
        #endregion
        #region ѧλ����
        txt += OutputDregeeRatio(worldAllNpc);
        #endregion

        txt += ListenEmotion(worldAllNpc);
        txt += ListenEst(worldAllNpc);
        txt += ListenSelfEmploed(worldAllNpc);
        txt += ListenWealth(worldAllNpc);
        eachSocial.Clear();
        return txt;
    }
    #region ѧλ����
    private string OutputDregeeRatio(Dictionary<short, NpcBase> worldAllNpc)
    {
        string txt = $"\n�����ܼƣ�����:{worldAllNpc.FilterValues(e => e.degree == Degree.����).Count}," +
            $"����{worldAllNpc.FilterValues(e => e.degree == Degree.����).Count}��" +
            $"ѧʿ{worldAllNpc.FilterValues(e => e.degree == Degree.ѧʿ).Count}��" +
            $"˶ʿ{worldAllNpc.FilterValues(e => e.degree == Degree.˶ʿ).Count}��" +
            $"��ʿ{worldAllNpc.FilterValues(e => e.degree == Degree.��ʿ).Count}";


        for (int i = 0; i < eachSocial.Count; i++)
        {
            txt += $"\n����:{eachSocial[i].Filter(e => e.degree == Degree.����).Count}," +
                $"����:{eachSocial[i].Filter(e => e.degree == Degree.����).Count}��" +
                $"ѧʿ:{eachSocial[i].Filter(e => e.degree == Degree.ѧʿ).Count}" +
                $"˶ʿ:{eachSocial[i].Filter(e => e.degree == Degree.˶ʿ).Count}��" +
                $"��ʿ:{eachSocial[i].Filter(e => e.degree == Degree.��ʿ).Count}�����׼�{i}��";
        }
        return txt;
    }
    #endregion
    #region ��й�ϵ����
    private string ListenEmotion(Dictionary<short, NpcBase> worldAllNpc)
    {
        string txt = "";
        txt += $"\n����npcƽ������Ϊ{worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.friendship >= 30 && e.Value.friendship < 60) / worldAllNpc.Values.Count:F4)},\n" +
            $"����npcƽ��֪��Ϊ{(float)worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.friendship > 60) / worldAllNpc.Values.Count:F4},\n" +
            $"����npcƽ������Ϊ{(float)worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.personalRelations == Kinship.����) / worldAllNpc.Values.Count:F4},\n" +
            $"����npcƽ����żΪ{(float)worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.personalRelations == Kinship.��ż) / worldAllNpc.Values.Count:F4},\n" +
            $"����npcƽ������Ϊ{(float)worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.romanticRelations == 1) / worldAllNpc.Values.Count:F4}\n";
        txt += $"npc��ϵƽ��ֵΪ{(float)worldAllNpc.Values.SelectMany(e => e.relationships).Sum(e => e.Value.friendship) / worldAllNpc.Values.SelectMany(e => e.relationships).Count():F4}\n";



        txt += $"�������˶���Ϊ{worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.personalRelations == Kinship.����) / 2}��{worldAllNpc.Values.Count}��,\n" +
            $"������ż����Ϊ{worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.personalRelations == Kinship.��ż) / 2}��{worldAllNpc.Values.Count}��,\n" +
            $"�������˶���Ϊ{worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.romanticRelations == 1) / 2}��{worldAllNpc.Values.Count}��,\n";
        return txt;
    }
    #endregion
    #region ���ݼ���
    private string ListenEst(Dictionary<short, NpcBase> wordAllNpc)
    {
        string txt = "";
        txt += $"\n��ǰ���ⷿ��npc��{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estIsRent == true)},\n";
        txt += $"\n��ס����npc��{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estIsHold == true && e.estIsLiving == true)},\n";
        txt += $"\n��ס����npc��{wordAllNpc.FilterValues(e => e.estate.Any()).Count},\n";
        for (int i = 0; i < eachSocial.Count; i++)
        {
            txt += $"�׼�{i}�з�����Ϊ{eachSocial[i].SelectMany(e => e.estate).Count(e => e.estIsHold == true)}\n";
        }
        txt += $"����1��ס��������Ϊ{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 1 && e.estType == 0)}\n" +
            $"����2��ס��������Ϊ{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 2 && e.estType == 0)}\n" +
            $"����3��ס��������Ϊ{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 3 && e.estType == 0)}\n" +
            $"����4��ס��������Ϊ{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 4 && e.estType == 0)}\n" +
            $"����5��ס��������Ϊ{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 5 && e.estType == 0)}\n" +

            $"����1��д��¥������Ϊ{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 1 && e.estType == 1)}\n" +
            $"����2��д��¥������Ϊ{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 2 && e.estType == 1)}\n" +
            $"����3��д��¥������Ϊ{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 3 && e.estType == 1)}\n" +
            $"����4��д��¥������Ϊ{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 4 && e.estType == 1)}\n" +
            $"����5��д��¥������Ϊ{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 5 && e.estType == 1)}\n";
        txt += $"������Ӫ�����з�����{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estIsCpn == true && e.estIsHold == true)}";
        txt += $"������Ӫ�����÷�����{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estIsCpn == true && e.estIsHold == false)}";
        return txt;
    }
    #endregion
    #region ְ������
    #endregion
    #region ��Ӫ��ҵ����
    public string ListenSelfEmploed(Dictionary<short, NpcBase> worldAllNpc)
    {
        string txt = "";
        int z = 0;
        txt += $"\n���統ǰ����Ӫ��Ϊ{worldAllNpc.Values.Select(e => e.selfEmployed).Count(e => e != null)}\n";
        //������Ӫ����
        for (int i = 0; i < eachSocial.Count; i++)
        {
            z = 0;
            txt += $"�׼�{i}����Ӫ��Ϊ{eachSocial[i].Select(e => e.selfEmployed).Count(e => e != null)}\n";
            for (int j = 0; j < eachSocial[i].Count; j++)
            {
                if (eachSocial[i][j].selfEmployed != null)
                {
                    z += eachSocial[i][j].selfEmployed.equityStructure.Count;
                }
            }
            txt += $"�׼�{i}����Ӫ��ҵ�Ĺ�Ȩ��������Ϊ{z}\n";
        }
        //��Ӫ�ر�����
        //��Ӫ��������
        return txt;
    }
    #endregion
    #region �������
    public string ListenWealth(Dictionary<short, NpcBase> worldAllNpc)
    {
        string txt = "";
        float z = 0;
        float f = 0;
        int estSumprice = 0;
        int vehicleSumprice = 0;
        long sumMoney = 0;
        for (int i = 0; i < eachSocial.Count; i++)
        {
            z = 0;
            f = 0;
            estSumprice = 0;
            vehicleSumprice = 0;
            sumMoney = 0;
            for (int j = 0; j < eachSocial[i].Count; j++)
            {
                if (eachSocial[i][j].debts.Any())
                {
                    for (int k = 0; k < eachSocial[i][j].debts.Count; k++)
                    {
                        z += WorldBank.Instance.GetDebtAmount(eachSocial[i][j].debts[k]);
                    }
                }
                if (eachSocial[i][j].stock.Any())
                {
                    for (int n = 0; n < eachSocial[i][j].stock.Count; n++)
                    {
                        f += StockMgr.Instance.GetStockValue(eachSocial[i][j].stock[n]);
                    }
                }
                if (eachSocial[i][j].estate.Any())
                {
                    for (int l = 0; l < eachSocial[i][j].estate.Count; l++)
                    {
                        estSumprice += eachSocial[i][j].estate[l].estSellPrice;
                    }
                }
                if (eachSocial[i][j].vehicles.Any())
                {
                    for (int c = 0; c < eachSocial[i][j].vehicles.Count; c++)
                    {
                        vehicleSumprice += eachSocial[i][j].vehicles[c].price;
                    }
                }
                sumMoney += eachSocial[i][j].money;
            }
            txt += $"���ײ�{i}ÿ����ƽ��ծ��Ϊ{z / eachSocial[i].Count}\n";
            txt += $"���ײ�{i}ÿ����ƽ����Ʊ��ֵΪΪ{f / eachSocial[i].Count}\n";
            txt += $"��ǰ{i}���������Ϊ{eachSocial[i].Count}\n";
            txt += $"���ײ�{i}ÿ����ƽ���ֽ�Ϊ{sumMoney / eachSocial[i].Count}\n";
            txt += $"���ײ�{i}ÿ����ƽ���ؾ���Ϊ{eachSocial[i].Sum(e => e.vehicles.Count) / eachSocial[i].Count}\n";
            txt += $"���ײ�{i}���ʲ�ƽ��ΪΪ���ݼ�ֵΪ{estSumprice / eachSocial[i].Count}\n,������ֵ{vehicleSumprice / eachSocial[i].Count}\n,��Ʊ��ֵ{f / eachSocial[i].Count},\n���ʲ�ƽ��Ϊ{(sumMoney + estSumprice + vehicleSumprice + f) / eachSocial[i].Count}\n";
        }
        return txt;
    }
    #endregion
    #endregion
    #endregion
}
