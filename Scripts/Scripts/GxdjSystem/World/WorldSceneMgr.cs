using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WorldSceneMgr : BaseManger<WorldSceneMgr>
{
    #region 容器
    System.Random r = new();
    public Dictionary<short, AllNpc> allNpc;
    public Dictionary<string, WorldScene> worldScene;

    //可用的世界场景字典
    public Dictionary<string, WorldSceneAd> worldSceneAd = new();
    //世界npc字典
    public Dictionary<short, NpcBase> worldAllNpc = new();
    //用于监听每个阶层人物
    List<List<NpcBase>> eachSocial = new();
    #endregion
    #region 函数
    #region 构造函数
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
    #region 将对应人物id填充到场景中
    public void LoadTable()
    {
        foreach (var currentScene in worldScene.Values)
            worldSceneAd.Add(currentScene.scnId, new WorldSceneAd(currentScene));
    }
    #endregion
    #region 根据公司信息为npc填充相关数据
    public void LoadNpcData()
    {//取出世界没一个场景 并生成新的npc到worldAllNpc中
        foreach (var currentScene in worldSceneAd.Values)
        {
            if (currentScene.sceneHumanID.Any())
            {
                foreach (short id in currentScene.sceneHumanID)
                {
                    worldAllNpc.Add(id, new NpcBase()
                    {
                        #region 基础信息
                        Id = allNpc[id].id,
                        name = allNpc[id].name,
                        gender = allNpc[id].gender,
                        npcAge = allNpc[id].npcAge,
                        race = (Nation)allNpc[id].race,
                        zodiac = (Constellation)allNpc[id].month,
                        #endregion
                        #region 智识能力
                        degree = (Degree)allNpc[id].degree,
                        intellegence = allNpc[id].intellegence,
                        growType = allNpc[id].growType,
                        #endregion
                        #region 工作相关
                        #region 薪资相关
                        jobLevel = allNpc[id].jobTag,
                        industry = currentScene.scnIndustry,
                        jobTitle = allNpc[id].jobType,
                        #endregion
                        #region 工作能力要求相关
                        companyAcademy = currentScene.scnAcademy,
                        #endregion
                        company = currentScene.scnCompany,
                        #endregion
                        moral = allNpc[id].moral,
                        //moralTag = NpcEmotionMgr.Instance.GetMoralTag(allNpc[id].moral),
                        charm = allNpc[id].charm,
                        defaultScene = allNpc[id].scene,



                        isWorkplace = currentScene.scnFunction == "职场" ? true : false,


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
    #region 世界npc生成
    private void GenrateWorldNpc()
    {
        foreach (var npc in worldAllNpc.Values)
        {
            #region 基础信息
            npc.pregnance = false;
            #endregion
            #region 智识能力
            #endregion
            #region 工作相关
            npc.basicSalary = WorkMgr.Instance.MatchSalaryFirstAmendment(npc);
            #endregion
            #region 位置信息

            #endregion
            #region 个性相关
            npc.preferred = (ItemSort)r.Next(0, 4);
            #endregion
            #region 财富财产情况
            npc.money = Wealth.Instance.BaseJobLevelToMoney(npc.jobLevel);
            //if (npc.jobLevel >= 1 && npc.basicSalary == 0)
            //   Debug.Log("此公司未能进行薪资计算" + "_" + npc.company+"_" +"行业"+npc.industry+ npc.jobLevel + npc.jobTitle + "npcID为" + "_" + npc.Id);
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
            #region 人际关系
            npc.knowPlayer = false;
            npc.privateInfoShow = false;
            npc.secretInfoShow = false;
            npc.sexWithPlayer = false;
            npc.loveStatus = NpcEmotionMgr.Instance.GetNpcLoveStatus(npc.npcAge);
            #endregion
            #region 社会影响等级
            npc.socialRank = Wealth.Instance.CurrentSocailRank(npc);
            #endregion
        }
        #region 世界自营企业生成
        SelfEmployedMgr.Instance.GenerateSelfEmployed(worldAllNpc);
        #endregion
        #region npc关系匹配
        #region 配偶
        NpcEmotionMgr.Instance.GenerateSpouse(worldAllNpc);
        #endregion
        #region 情侣 包含情人
        NpcEmotionMgr.Instance.MatchWorldCouple(worldAllNpc);
        #endregion
        #region 同事
        //NpcEmotionMgr.Instance.GenreatCollugeRealthion(worldAllNpc);
        #endregion
        #region 确定倾慕对象
        #endregion
        //NpcEmotionMgr.Instance.GetAdmirationObject(worldAllNpc);
        #endregion
        #region 初始化世界公司
        CompanyMgr.Instance.InitWorldCompany(worldScene);
        #region 划分功能
        CompanyMgr.Instance.SplitFunction();
        #endregion
        #endregion
    }
    #endregion

    #region 世界npc参数监听
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
        var counts = new int[9]; // 8 个年龄段 + >85 岁
        if (isfirst)
        {
            txt += $"世界总人数{worldAllNpc.Count}\n";
            txt += $"各族人数分部为夏川：{worldAllNpc.FilterValues(e => e.race == Nation.夏川).Count}\n" +
                $"歌铎:{worldAllNpc.FilterValues(e => e.race == Nation.歌铎).Count}\n" +
                $"樱山:{worldAllNpc.FilterValues(e => e.race == Nation.樱山).Count}\n" +
                $"泰马:{worldAllNpc.FilterValues(e => e.race == Nation.泰马).Count}\n" +
                $"亚兰:{worldAllNpc.FilterValues(e => e.race == Nation.亚蓝).Count}\n";
        }
        #region 年龄统计
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
        txt += string.Join("\n", ageBounds.Select((b, i) => $"世界{b}岁人数：{counts[i]}"));
        if (counts[8] > 0)
            txt += $"\n世界{ageBounds.Last()}岁以上人数：{counts[8]}";
        #endregion
        #region 职务 财富 社会等级监听
        txt += string.Join("\n", Enumerable.Range(0, 6).Select(i =>
$"世界社会等级为{i}级人数{worldAllNpc.FilterValues(e => e.wealthRank == i).Count}"));

        txt += string.Join("\n", Enumerable.Range(0, 6).Select(i =>
             $"世界社会等级为{i}级人数{worldAllNpc.FilterValues(e => e.socialRank == i).Count}"));

        txt += string.Join("\n", Enumerable.Range(0, 6).Select(i =>
                $"世界职务等级为{i}级人数{worldAllNpc.FilterValues(e => e.jobLevel == i).Count}"));
        #endregion
        #region 智识等级监听
        txt += $"世界智力等级为0-20区间的人数为{worldAllNpc.FilterValues(e => e.intellegence > 0 && e.intellegence <= 20).Count}\n" +
    $"世界智力等级为20-40区间的人数为{worldAllNpc.FilterValues(e => e.intellegence > 20 && e.intellegence <= 40).Count}\n" +
    $"世界智力等级为40-60区间的人数为{worldAllNpc.FilterValues(e => e.intellegence > 40 && e.intellegence <= 60).Count}\n" +
    $"世界智力等级为60-80区间的人数为{worldAllNpc.FilterValues(e => e.intellegence > 60 && e.intellegence <= 80).Count}\n" +
    $"世界智力等级为80-100区间的人数为{worldAllNpc.FilterValues(e => e.intellegence > 80 && e.intellegence <= 100).Count}\n" +
    $"世界智力等级为100-120区间的人数为{worldAllNpc.FilterValues(e => e.intellegence > 100 && e.intellegence <= 120).Count}\n" +
    $"世界智力等级为120-140区间的人数为{worldAllNpc.FilterValues(e => e.intellegence > 120 && e.intellegence <= 140).Count}\n" +
    $"世界智力等级为140-160区间的人数为{worldAllNpc.FilterValues(e => e.intellegence > 140 && e.intellegence <= 160).Count}\n";
        #endregion
        #region 学位监听
        txt += OutputDregeeRatio(worldAllNpc);
        #endregion

        txt += ListenEmotion(worldAllNpc);
        txt += ListenEst(worldAllNpc);
        txt += ListenSelfEmploed(worldAllNpc);
        txt += ListenWealth(worldAllNpc);
        eachSocial.Clear();
        return txt;
    }
    #region 学位监听
    private string OutputDregeeRatio(Dictionary<short, NpcBase> worldAllNpc)
    {
        string txt = $"\n世界总计，初中:{worldAllNpc.FilterValues(e => e.degree == Degree.初中).Count}," +
            $"高中{worldAllNpc.FilterValues(e => e.degree == Degree.高中).Count}，" +
            $"学士{worldAllNpc.FilterValues(e => e.degree == Degree.学士).Count}，" +
            $"硕士{worldAllNpc.FilterValues(e => e.degree == Degree.硕士).Count}，" +
            $"博士{worldAllNpc.FilterValues(e => e.degree == Degree.博士).Count}";


        for (int i = 0; i < eachSocial.Count; i++)
        {
            txt += $"\n初中:{eachSocial[i].Filter(e => e.degree == Degree.初中).Count}," +
                $"高中:{eachSocial[i].Filter(e => e.degree == Degree.高中).Count}，" +
                $"学士:{eachSocial[i].Filter(e => e.degree == Degree.学士).Count}" +
                $"硕士:{eachSocial[i].Filter(e => e.degree == Degree.硕士).Count}，" +
                $"博士:{eachSocial[i].Filter(e => e.degree == Degree.博士).Count}在社会阶级{i}中";
        }
        return txt;
    }
    #endregion
    #region 情感关系监听
    private string ListenEmotion(Dictionary<short, NpcBase> worldAllNpc)
    {
        string txt = "";
        txt += $"\n世界npc平均朋友为{worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.friendship >= 30 && e.Value.friendship < 60) / worldAllNpc.Values.Count:F4)},\n" +
            $"世界npc平均知己为{(float)worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.friendship > 60) / worldAllNpc.Values.Count:F4},\n" +
            $"世界npc平均恋人为{(float)worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.personalRelations == Kinship.恋人) / worldAllNpc.Values.Count:F4},\n" +
            $"世界npc平均配偶为{(float)worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.personalRelations == Kinship.配偶) / worldAllNpc.Values.Count:F4},\n" +
            $"世界npc平均情人为{(float)worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.romanticRelations == 1) / worldAllNpc.Values.Count:F4}\n";
        txt += $"npc关系平均值为{(float)worldAllNpc.Values.SelectMany(e => e.relationships).Sum(e => e.Value.friendship) / worldAllNpc.Values.SelectMany(e => e.relationships).Count():F4}\n";



        txt += $"世界恋人对数为{worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.personalRelations == Kinship.恋人) / 2}在{worldAllNpc.Values.Count}中,\n" +
            $"世界配偶对数为{worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.personalRelations == Kinship.配偶) / 2}在{worldAllNpc.Values.Count}中,\n" +
            $"世界情人对数为{worldAllNpc.Values.SelectMany(e => e.relationships).Count(e => e.Value.romanticRelations == 1) / 2}在{worldAllNpc.Values.Count}中,\n";
        return txt;
    }
    #endregion
    #region 房屋监听
    private string ListenEst(Dictionary<short, NpcBase> wordAllNpc)
    {
        string txt = "";
        txt += $"\n当前在租房的npc有{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estIsRent == true)},\n";
        txt += $"\n有住房的npc有{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estIsHold == true && e.estIsLiving == true)},\n";
        txt += $"\n有住房的npc有{wordAllNpc.FilterValues(e => e.estate.Any()).Count},\n";
        for (int i = 0; i < eachSocial.Count; i++)
        {
            txt += $"阶级{i}有房人数为{eachSocial[i].SelectMany(e => e.estate).Count(e => e.estIsHold == true)}\n";
        }
        txt += $"持有1级住房的数量为{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 1 && e.estType == 0)}\n" +
            $"持有2级住房的数量为{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 2 && e.estType == 0)}\n" +
            $"持有3级住房的数量为{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 3 && e.estType == 0)}\n" +
            $"持有4级住房的数量为{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 4 && e.estType == 0)}\n" +
            $"持有5级住房的数量为{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 5 && e.estType == 0)}\n" +

            $"持有1级写字楼的数量为{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 1 && e.estType == 1)}\n" +
            $"持有2级写字楼的数量为{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 2 && e.estType == 1)}\n" +
            $"持有3级写字楼的数量为{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 3 && e.estType == 1)}\n" +
            $"持有4级写字楼的数量为{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 4 && e.estType == 1)}\n" +
            $"持有5级写字楼的数量为{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estRank == 5 && e.estType == 1)}\n";
        txt += $"用于自营的自有房屋有{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estIsCpn == true && e.estIsHold == true)}";
        txt += $"用于自营的租用房屋有{wordAllNpc.Values.SelectMany(e => e.estate).Count(e => e.estIsCpn == true && e.estIsHold == false)}";
        return txt;
    }
    #endregion
    #region 职场监听
    #endregion
    #region 自营企业监听
    public string ListenSelfEmploed(Dictionary<short, NpcBase> worldAllNpc)
    {
        string txt = "";
        int z = 0;
        txt += $"\n世界当前有自营数为{worldAllNpc.Values.Select(e => e.selfEmployed).Count(e => e != null)}\n";
        //新增自营数量
        for (int i = 0; i < eachSocial.Count; i++)
        {
            z = 0;
            txt += $"阶级{i}有自营数为{eachSocial[i].Select(e => e.selfEmployed).Count(e => e != null)}\n";
            for (int j = 0; j < eachSocial[i].Count; j++)
            {
                if (eachSocial[i][j].selfEmployed != null)
                {
                    z += eachSocial[i][j].selfEmployed.equityStructure.Count;
                }
            }
            txt += $"阶级{i}有自营企业的股权持有者有为{z}\n";
        }
        //自营关闭数量
        //自营升级数量
        return txt;
    }
    #endregion
    #region 财务监听
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
            txt += $"社会阶层{i}每个人平均债务为{z / eachSocial[i].Count}\n";
            txt += $"社会阶层{i}每个人平均股票价值为为{f / eachSocial[i].Count}\n";
            txt += $"当前{i}级社会人数为{eachSocial[i].Count}\n";
            txt += $"社会阶层{i}每个人平均现金为{sumMoney / eachSocial[i].Count}\n";
            txt += $"社会阶层{i}每个人平均载具数为{eachSocial[i].Sum(e => e.vehicles.Count) / eachSocial[i].Count}\n";
            txt += $"社会阶层{i}的资产平均为为房屋价值为{estSumprice / eachSocial[i].Count}\n,车辆价值{vehicleSumprice / eachSocial[i].Count}\n,股票价值{f / eachSocial[i].Count},\n总资产平均为{(sumMoney + estSumprice + vehicleSumprice + f) / eachSocial[i].Count}\n";
        }
        return txt;
    }
    #endregion
    #endregion
    #endregion
}
