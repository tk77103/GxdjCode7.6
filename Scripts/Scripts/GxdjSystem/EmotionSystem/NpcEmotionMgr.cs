using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
//社交进度
public class SocialProgress
{

}
public class NpcEmotionMgr : BaseManger<NpcEmotionMgr>
{
    #region 数据容器
    System.Random r = new();
    List<WorldScene> consumeScene;
    #endregion
    #region 函数
    #region 构造函数
    private NpcEmotionMgr()
    {

    }
    #endregion
    #region 情感生成
    #region 生成npc的情感状态(有时间用switch重写)
    public short GetNpcLoveStatus(short age)
    { //personal婚恋情况 已婚2/恋爱1/单身0
        int i = r.Next(1, 101);
        if (age <= 25)
        {
            if (i <= 30) return 1;
            else return 0;
        }
        else if (age > 25 && age <= 28)
        {
            if (i <= 10) return 2;
            else if (i > 10 && i <= 40) return 1;
            else return 0;
        }
        else if (age > 28 && age <= 32)
        {
            if (i <= 10) return 2;
            else if (i > 10 && i <= 50) return 1;
            else return 0;
        }
        else if (age > 32 && age <= 35)
        {
            if (i <= 30) return 2;
            else if (i > 30 && i <= 70) return 1;
            else return 0;
        }
        else if (age > 35 && age <= 38)
        {
            if (i <= 50) return 2;
            else if (i > 50 && i <= 70) return 1;
            else return 0;
        }
        else if (age > 38 && age <= 42)
        {
            if (i <= 70) return 2;
            else if (i > 70 && i <= 80) return 1;
            else return 0;
        }
        else
        {
            if (i <= 80) return 2;
            else if (i > 80 && i <= 90) return 1;
            else return 0;
        }
    }
    #endregion
    #region 世界配偶生成
    public void GenerateSpouse(Dictionary<short, NpcBase> worldNpc)
    {//已婚男，暂时认为男性已婚数要多于女性
        List<short> mans = worldNpc?.FilterValues(e => e.gender is false && e.loveStatus is 2).Select(e=>e.Id).ToList();
        //已婚女
        List<short> womans = worldNpc?.FilterValues(e => e.gender is true && e.loveStatus is 2).Select(e=>e.Id).ToList();

        //为了倒腾而声明的容器
        List<short> npcTemp = new();
        //女性多于男性
        for (int i = womans.Count - 1; i >= 0; i--)
        {
            npcTemp = mans.Where(e => Mathf.Abs(worldNpc[e].socialRank - worldNpc[ womans[i]].socialRank) <= 2).ToList();
            //判断已婚男性是否有足够的数量
            if (npcTemp.Any())
            {
                npcTemp = npcTemp.Shuffle(npcTemp.Count);
                //女人关系添加
                worldNpc[womans[i]].relationships.Add(npcTemp[0], new()
                {
                    relatedID = npcTemp[0],
                    personalRelations = Kinship.配偶,
                    friendship = r.Next(60, 101)
                });
                //男人关系添加
                worldNpc[npcTemp[0]].relationships.Add(womans[i], new()
                {
                    relatedID = womans[i],
                    personalRelations = Kinship.配偶,
                    friendship = r.Next(60, 101)
                });
                womans.RemoveAt(i);
                mans.Remove(npcTemp[0]);
            }
        }
        if (womans.Any())
            womans.ForEach(e => worldNpc[e].loveStatus = 0);
        if (mans.Any())
            mans.ForEach(e => worldNpc[e].loveStatus = 0);
    }
    #endregion
    #region 世界情侣（包括恋人）生成 (方法冗余有时间拆分重构)
   public void MatchWorldCouple(Dictionary<short,NpcBase> worldAllNpc)
    {
        List<short>womansLove=worldAllNpc.Values.Where(e=>e.loveStatus==1&&e.gender==true).Select(e=>e.Id).ToList();
        List<short>mansLove=worldAllNpc.Values.Where(e=>e.loveStatus==1&&e.gender==false).Select(e=>e.Id).ToList();
        List<short>allNpcKey=worldAllNpc.Keys.ToList();
        List<short> halfMan = mansLove.Shuffle(mansLove.Count / 2);
        HalfManMatchLover(halfMan, womansLove,mansLove,allNpcKey,worldAllNpc);
        //为剩余的一半女人匹配情侣
        List<short> halfWoman = womansLove.Shuffle(womansLove.Count / 2);
        HalfWomanMatchLover(halfWoman, womansLove, mansLove, allNpcKey, worldAllNpc);
        //剩余的男女 与剩下世界 此时allNpcKey 已经将全部已经匹配的男女剔除干净
        ForOtherMatchLove(true, womansLove, mansLove, allNpcKey, worldAllNpc);
        ForOtherMatchLove(false, womansLove, mansLove, allNpcKey, worldAllNpc);
        if (mansLove.Any())
            mansLove.ForEach(e => worldAllNpc[e].loveStatus = 0);
        if (womansLove.Any())
            womansLove.ForEach(e => worldAllNpc[e].loveStatus = 0);
    }
    #region 为一半男人匹配情侣
    public void HalfManMatchLover(List<short>halfMan,List<short> womansLove,List<short> mansLove,List<short>allNpcKey, Dictionary<short, NpcBase> worldAllNpc)
    {
        List<short> womanMatch = new();
        for (int i = 0; i < halfMan.Count; i++)
        {
            womanMatch = womansLove.Where(e => Mathf.Abs(worldAllNpc[e].jobLevel - worldAllNpc[halfMan[i]].jobLevel) <= 2).ToList();
            womanMatch = womanMatch.Shuffle(womanMatch.Count);
            worldAllNpc[halfMan[i]].relationships.Add(womanMatch[0], new()
            {
                relatedID = womanMatch[0],
                friendship = r.Next(30, 71)
            });
            worldAllNpc[womanMatch[0]].relationships.Add(halfMan[i], new()
            {
                relatedID = halfMan[i],
                friendship = r.Next(30, 71)
            });
            //在匹配后在已经有恋爱标签的男女中移除
            womansLove.Remove(womanMatch[0]);
            mansLove.Remove(halfMan[i]);
            allNpcKey.Remove(halfMan[i]);
            allNpcKey.Remove(womanMatch[0]);
        }
        halfMan.Clear();
    }
    #endregion
    #region 为剩余的一半女人匹配情侣
    public void HalfWomanMatchLover(List<short> halfWoman, List<short> womansLove, List<short> mansLove, List<short> allNpcKey, Dictionary<short, NpcBase> worldAllNpc)
    {
        List<short> manMatch = new();
        for (int i = 0; i < halfWoman.Count; i++)
        {
            manMatch = mansLove.Where(e => Mathf.Abs(worldAllNpc[e].jobLevel - worldAllNpc[halfWoman[i]].jobLevel) <= 2).ToList();
            manMatch = manMatch.Shuffle(manMatch.Count);
            worldAllNpc[halfWoman[i]].relationships.Add(manMatch[0], new()
            {
                relatedID = manMatch[0],
                friendship = r.Next(30, 71)
            });
            worldAllNpc[manMatch[0]].relationships.Add(halfWoman[i], new()
            {
                relatedID = halfWoman[i],
                friendship = r.Next(30, 71)
            });
            //在匹配后在已经有恋爱标签的男女中移除
            womansLove.Remove(halfWoman[i]);
            mansLove.Remove(manMatch[0]);
            allNpcKey.Remove(halfWoman[i]);
            allNpcKey.Remove(manMatch[0]);
        }
        halfWoman.Clear();
    }
    #endregion
    #region 为剩余的男女匹配
    public void ForOtherMatchLove(bool isWoman,List<short> womansLove, List<short> mansLove, List<short> allNpcKey, Dictionary<short, NpcBase> worldAllNpc)
    {
        List<short> tempUse = new();
        List<short> tempMatch = new();
        if (isWoman)
        {//此时allNpcKey 已经将全部已经匹配的男女剔除干净, 筛选剩下包括没匹配的男性 以及其他男性
            tempUse = allNpcKey.Where(e => worldAllNpc[e].gender == false).ToList();
            for(int i = womansLove.Count - 1; i >= 0; i--)
            {//筛选剩余男性中符合条件的男性
                tempMatch = tempUse.Where(e => Mathf.Abs(worldAllNpc[womansLove[i]].jobLevel - worldAllNpc[e].jobLevel) <= 2).ToList();
                tempMatch = tempMatch.Shuffle(tempMatch.Count);
                //判断被挑选男性的情感状态
                switch (worldAllNpc[tempMatch[0]].loveStatus)
                {   //单身变更为情侣状态
                    case 0:
                        worldAllNpc[tempMatch[0]].loveStatus = 1;
                        worldAllNpc[womansLove[i]].relationships.Add(tempMatch[0],
                            new()
                            {
                                relatedID = tempMatch[0],
                                friendship = r.Next(30, 71)
                            }
                            );
                        worldAllNpc[tempMatch[0]].relationships.Add(womansLove[i], new()
                        {
                            relatedID = womansLove[i],
                            friendship = r.Next(30, 71)
                        });
                        allNpcKey.Remove(womansLove[i]);
                        womansLove.RemoveAt(i);
                        allNpcKey.Remove(tempMatch[0]);
                        break;
                        case 1:
                        worldAllNpc[womansLove[i]].relationships.Add(tempMatch[0],
                        new()
                        {
                            relatedID = tempMatch[0],
                            friendship = r.Next(30, 71)
                        }
                        );
                        worldAllNpc[tempMatch[0]].relationships.Add(womansLove[i], new()
                        {
                            relatedID = womansLove[i],
                            friendship = r.Next(30, 71)
                        });
                        allNpcKey.Remove(womansLove[i]);
                        womansLove.RemoveAt(i);
                        mansLove.Remove(tempMatch[0]);
                        allNpcKey.Remove(tempMatch[0]);
                        break;
                    case 2:
                        worldAllNpc[womansLove[i]].relationships.Add(tempMatch[0],
                        new()
                        {
                            relatedID = tempMatch[0],
                            friendship = r.Next(30, 71),
                            romanticRelations=1
                        }
                        );
                        worldAllNpc[tempMatch[0]].relationships.Add(womansLove[i], new()
                        {
                            relatedID = womansLove[i],
                            friendship = r.Next(30, 71),
                            romanticRelations = 1
                        });
                        allNpcKey.Remove(womansLove[i]);
                        womansLove.RemoveAt(i);
                        allNpcKey.Remove(tempMatch[0]);
                        break;
                }
            }
        }
        else
        {
            tempUse = allNpcKey.Where(e => worldAllNpc[e].gender == true).ToList();
            for (int i = mansLove.Count - 1; i >= 0; i--)
            {
                //筛选剩余女性中符合条件的女性
                tempMatch = tempUse.Where(e => Mathf.Abs(worldAllNpc[mansLove[i]].jobLevel - worldAllNpc[e].jobLevel) <= 2).ToList();
                tempMatch = tempMatch.Shuffle(tempMatch.Count);
                switch (worldAllNpc[tempMatch[0]].loveStatus)
                {   //单身变更为情侣状态
                    case 0:
                        worldAllNpc[tempMatch[0]].loveStatus = 1;
                        worldAllNpc[mansLove[i]].relationships.Add(tempMatch[0],
                            new()
                            {
                                relatedID = tempMatch[0],
                                friendship = r.Next(30, 71)
                            }
                            );
                        worldAllNpc[tempMatch[0]].relationships.Add(mansLove[i], new()
                        {
                            relatedID = mansLove[i],
                            friendship = r.Next(30, 71)
                        });
                        allNpcKey.Remove(mansLove[i]);
                        mansLove.RemoveAt(i);
                        allNpcKey.Remove(tempMatch[0]);
                        break;
                    case 1:
                        worldAllNpc[mansLove[i]].relationships.Add(tempMatch[0],
                        new()
                        {
                            relatedID = tempMatch[0],
                            friendship = r.Next(30, 71)
                        }
                        );
                        worldAllNpc[tempMatch[0]].relationships.Add(mansLove[i], new()
                        {
                            relatedID = mansLove[i],
                            friendship = r.Next(30, 71)
                        });
                        allNpcKey.Remove(mansLove[i]);
                        mansLove.RemoveAt(i);
                        womansLove.Remove(tempMatch[0]);
                        allNpcKey.Remove(tempMatch[0]);
                        break;
                    case 2:
                        worldAllNpc[mansLove[i]].relationships.Add(tempMatch[0],
                        new()
                        {
                            relatedID = tempMatch[0],
                            friendship = r.Next(30, 71),
                            romanticRelations = 1
                        }
                        );
                        worldAllNpc[tempMatch[0]].relationships.Add(mansLove[i], new()
                        {
                            relatedID = mansLove[i],
                            friendship = r.Next(30, 71),
                            romanticRelations = 1
                        });
                        allNpcKey.Remove(mansLove[i]);
                        mansLove.RemoveAt(i);
                        allNpcKey.Remove(tempMatch[0]);
                        break;
                }
            }
        }
    }
    #endregion
    #region 为剩余的男女匹配

    #endregion
    #endregion
    //#region 确定同事关系值
    //public void GenreatCollugeRealthion(Dictionary<short, NpcBase> allNpc)
    //{
    //    List<NpcBase> npcOldFr = new();
    //    List<NpcBase> otherSamComNpc = new();
    //    foreach (var npc in allNpc.Values)
    //    {
    //        if (npc.relationships.Any())
    //        {
    //            npcOldFr.AddRange(npc.relationships.Values.Where(r => r.npcRelated != null).Select(r => r.npcRelated));
    //            npc.relationships.Values.ToList().Filter(e => e.npcRelated.company == npc.company).ForEach((e) =>
    //            {
    //                switch (npc.jobLevel - e.npcRelated.jobLevel)
    //                {
    //                    case < 0:
    //                        e.colleagueRelations = 3;
    //                        break;
    //                    case 0:
    //                        e.colleagueRelations = 2;
    //                        break;
    //                    case > 0:
    //                        e.colleagueRelations = 1;
    //                        break;
    //                }
    //                switch (e.npcRelated.jobLevel)
    //                {
    //                    case 1:
    //                    case 2:
    //                    case 3:
    //                        e.friendship += 15 + r.Next(5, 26) / e.npcRelated.jobLevel;
    //                        break;
    //                    case 4:
    //                        e.friendship += 10 + r.Next(5, 26) / e.npcRelated.jobLevel;
    //                        break;
    //                    case 5:
    //                        e.friendship += 10 + r.Next(5, 26) / e.npcRelated.jobLevel;
    //                        break;
    //                }
    //            });
    //            otherSamComNpc = allNpc.Values.Except(npcOldFr).ToList();
    //            otherSamComNpc = otherSamComNpc.Filter(e => e.Id != npc.Id && e.company == npc.company);
    //        }
    //        else
    //            otherSamComNpc = allNpc.FilterValues(e => e.Id != npc.Id && e.company == npc.company);
    //        otherSamComNpc.ForEach(e =>
    //        {
    //            NpcRelationships collegueRes = new();
    //            collegueRes.npcRelated = e;
    //            switch (npc.jobLevel - e.jobLevel)
    //            {
    //                case < 0:
    //                    collegueRes.colleagueRelations = 3;
    //                    break;
    //                case 0:
    //                    collegueRes.colleagueRelations = 2;
    //                    break;
    //                case > 0:
    //                    collegueRes.colleagueRelations = 1;
    //                    break;
    //            }
    //            switch (e.jobLevel)
    //            {
    //                case 1:
    //                case 2:
    //                case 3:
    //                    collegueRes.friendship += 15 + r.Next(5, 26) / e.jobLevel;
    //                    break;
    //                case 4:
    //                    collegueRes.friendship += 10 + r.Next(5, 26) / e.jobLevel;
    //                    break;
    //                case 5:
    //                    collegueRes.friendship += 5 + r.Next(5, 26) / e.jobLevel;
    //                    break;
    //            }
    //            npc.relationships.Add(collegueRes.npcRelated.Id, collegueRes);
    //        });

    //    }
    //}
    //#endregion
    //#region 确定npc倾慕对象
    //public void GetAdmirationObject(Dictionary<short, NpcBase> worldAllNpc)
    //{
    //    foreach (var npc in worldAllNpc.Values)
    //    {
    //        if (npc.relationships.Any())
    //        {
    //            if (npc.relationships.Values.Where(e => e.npcRelated.gender != npc.gender).Any())
    //            {
    //                npc.AdmirationObject = npc.relationships.Values.Where(e => e.npcRelated.gender != npc.gender).ToList().
    //                    MaxBy(e => e.npcRelated.charm * e.friendship).npcRelated.Id;
    //            }
    //        }
    //    }
    //}
    //#endregion
    //#endregion
    //#region 情感演化
    //#region 找到社交靶目标 （次年更换）
    ////非职场8人职场10人
    //public void FindTargetNpc(bool isFirst)
    //{
    //    List<short> allNpcKey = WorldSceneMgr.Instance.worldAllNpc.Keys.ToList();
    //    foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
    //    {
    //        if (isFirst)
    //        {
    //            if (npc.isWorkplace)
    //            {
    //                if (npc.AdmirationObject != 0)
    //                {
    //                    npc.targetNpc.Add(npc.AdmirationObject);
    //                    npc.targetNpc.AddRange(allNpcKey.Except(npc.targetNpc).ToList().Shuffle(3));//如果有倾慕对象则添加
    //                }
    //                else
    //                    npc.targetNpc.AddRange(allNpcKey.Except(npc.targetNpc).ToList().Shuffle(4));
    //                //添加职场同事
    //                if (npc.relationships.Any())
    //                {
    //                    if (npc.relationships.Values.ToList().Filter(e => e.colleagueRelations != 0).Any())
    //                    {
    //                        List<NpcRelationships> mycolleague = npc.relationships.Values.ToList().Filter(e => e.colleagueRelations != 0);
    //                        npc.targetNpc.AddRange(mycolleague.Filter(e => e.colleagueRelations == 1 || e.colleagueRelations == 2)
    //                            .Select(e => e.npcRelated.Id).ToList().Shuffle(4));
    //                        npc.targetNpc.AddRange(mycolleague.Filter(e => e.colleagueRelations == 3)
    //                           .Select(e => e.npcRelated.Id).ToList().Shuffle(2));
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                if (npc.AdmirationObject != 0)
    //                {
    //                    npc.targetNpc.Add(npc.AdmirationObject); //如果有倾慕对象则添加
    //                    npc.targetNpc.AddRange(WorldSceneMgr.Instance.worldAllNpc.FilterValues(e => e.gender != npc.gender && Mathf.Abs(e.socialRank - npc.socialRank) <= 1)
    //                .Shuffle(3).Select(e => e.Id).ToList());
    //                }
    //                else
    //                    npc.targetNpc.AddRange(WorldSceneMgr.Instance.worldAllNpc.FilterValues(e => e.gender != npc.gender && Mathf.Abs(e.socialRank - npc.socialRank) <= 1)
    //                    .Shuffle(4).Select(e => e.Id).ToList());
    //                npc.targetNpc.AddRange(WorldSceneMgr.Instance.worldAllNpc.FilterValues(e => e.gender == npc.gender && Mathf.Abs(e.socialRank - npc.socialRank) <= 1)
    //                .Shuffle(4).Select(e => e.Id).ToList());
    //            }
    //        }
    //        else
    //        {
    //            if (npc.targetNpc.Any())
    //            {
    //                npc.targetNpc = npc.targetNpc.Shuffle(npc.targetNpc.Count);
    //                npc.targetNpc = npc.targetNpc.Take(npc.targetNpc.Count / 2).ToList();
    //            }
    //            if (npc.isWorkplace)
    //                npc.targetNpc.AddRange(allNpcKey.Except(npc.targetNpc).ToList().Shuffle(10 - npc.targetNpc.Count / 2));
    //            else
    //                npc.targetNpc.AddRange(allNpcKey.Except(npc.targetNpc).ToList().Shuffle(8 - npc.targetNpc.Count / 2));
    //        }
    //        npc.targetNpc = npc.targetNpc.Distinct().ToList();
    //        if (npc.targetNpc.Contains(npc.Id))
    //            npc.targetNpc.Remove(npc.Id);
    //    }
    //}
    //#endregion
    //#region npc间的情感互动
    //#region 用于情感交往公式
    //#region 相性量化
    //public int GetMoralTag(int moral)
    //{
    //    switch (moral)
    //    {
    //        case >= -100 and < -81:
    //            return 5;
    //        case >= -81 and < -40:
    //            return 4;
    //        case >= -40 and < 40:
    //            return 3;
    //        case >= 40 and < 80:
    //            return 2;
    //        case >= 80:
    //            return 1;
    //        default:
    //            return 3;
    //    }
    //}
    //#endregion
    //#region 阶层系数
    //private float socialModulus(int rank)
    //{
    //    switch (rank)
    //    {
    //        case -5:
    //            return 0.3f;
    //        case -4:
    //            return 0.4f;
    //        case -3:
    //            return 0.5f;
    //        case -2:
    //            return 0.6f;
    //        case -1:
    //            return 0.8f;
    //        case 0:
    //            return 1f;
    //        case 1:
    //            return 1.1f;
    //        case 2:
    //            return 1.2f;
    //        case 3:
    //            return 1.3f;
    //        case 4:
    //            return 1.4f;
    //        case 5:
    //            return 2f;
    //        default:
    //            return 1;
    //    }
    //}
    //#endregion
    //#region 民族系数
    //private float NationModulus(Nation a, Nation b)
    //{
    //    // 同族
    //    if (a == b) return 1.5f;

    //    // 定义夏川、樱山、歌铎的集合
    //    var specialNations = new[] { Nation.夏川, Nation.樱山, Nation.歌铎 };
    //    // 检查是否在夏川、樱山、歌铎之间的组合
    //    if (specialNations.Contains(a) && specialNations.Contains(b))
    //        return 1.3f;

    //    // 检查夏川-泰马 或 樱山-泰马 的组合
    //    if ((a == Nation.夏川 && b == Nation.泰马) || (a == Nation.泰马 && b == Nation.夏川) ||
    //        (a == Nation.樱山 && b == Nation.泰马) || (a == Nation.泰马 && b == Nation.樱山))
    //        return 0.6f;

    //    // 其余组合
    //    return 1.0f;
    //}
    //#endregion
    //#region 性格相性
    //private float CharacterModulus(int a, int b)
    //{
    //    switch (Mathf.Abs(a - b))
    //    {
    //        case 0:
    //            return 1.2f;
    //        case 1:
    //            return 1f;
    //        case 2:
    //            return 0.8f;
    //        case 3:
    //            return 0.6f;
    //        case 4:
    //            return 0.4f;
    //        default:
    //            return 1f;
    //    }
    //}
    //#endregion
    //#endregion
    //#region 世界npc的互动ai
    ///// <summary>
    ///// 模拟世界npc间的互动
    /////  若从来没有与任何人有过交集，在首月先与人互动产生初步印象
    /////  有一部分互动过，则优先认识其余靶对象未认识的
    /////  靶对象全部认识后开始初级的互动 赠礼 ，聚会
    /////  二阶开始增加邀约 并进行其余判断
    /////  目前邀约只是做了邀请去宾馆与好感增加 此处需要细化
    ///// </summary>
    //public void NPCEmotionalInteraction()
    //{
    //    int interactionNum = 0;
    //    foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
    //    {
    //        if (npc.targetNpc.Any())
    //        {  //每次互动消耗互动次数
    //            interactionNum = 12;
    //            if (npc.relationships.Any())
    //            {
    //                //第一次互动 优先与没互动过的人互动
    //                //如果靶对象存在不包含在已交往
    //                if (npc.targetNpc.Except(npc.relationships.Keys).Any())
    //                {
    //                    List<short> strangeID = npc.targetNpc.Except(npc.relationships.Keys).ToList();
    //                    //优先对没有相处过的npc 进行一次谈话
    //                    foreach (var key in strangeID)
    //                    {
    //                        TalkFirst(npc, WorldSceneMgr.Instance.worldAllNpc[key]);
    //                        interactionNum--;
    //                        if (interactionNum == 0) return;
    //                    }
    //                    //此时所有靶对象都进行了互动

    //                }
    //                //全部靶对象已经包含在npc已交往的对象中
    //                else
    //                {
    //                    //首先与谈话次数小于三的进行一次对话
    //                    if (npc.relationships.Values.Where(e => e.isTalkedThree < 3).Any())
    //                    {
    //                        List<short> conless3 = npc.relationships.Values.Where(e => e.isTalkedThree < 3).Select(e => e.npcRelated.Id).ToList();
    //                        foreach (var id in conless3)
    //                        {
    //                            TalkNormal(npc, WorldSceneMgr.Instance.worldAllNpc[id]);
    //                            interactionNum--;
    //                            if (interactionNum == 0) return;
    //                        }
    //                        //目前按照谈话为三次 开启其他活动
    //                    }
    //                    //所有人都交谈大于三次彻底移除交谈这个选项 只进行聚会 邀约
    //                    else
    //                    {
    //                        //关系值低于三十的进行一阶互动
    //                        //大于三十进行二阶互动
    //                        if (npc.targetNpc.All(e => npc.relationships.Keys.Contains(e)))
    //                        {//此处所有的靶目标应该都已经接触过
    //                            List<short> targetContacted = npc.targetNpc.Where(e => npc.relationships.Keys.Contains(e)).ToList();
    //                            List<short> targetFriLow30 = new();
    //                            List<short> targetFriMore30 = new();
    //                            foreach (var id in targetContacted)
    //                            {
    //                                if (npc.relationships[id].friendship < 30)
    //                                    targetFriLow30.Add(id);
    //                                else targetFriMore30.Add(id);
    //                            }
    //                            if (targetFriLow30.Any())
    //                            {
    //                                //进行一阶段判断
    //                                //将关系大于三十的放入二阶判断

    //                            }
    //                            if (targetFriMore30.Any())
    //                            {
    //                                //进行二阶互动
    //                            }
    //                        }
    //                        else
    //                            Debug.Log($"{npc.Id}未与所有靶目标建立联系，分析原因");
    //                    }

    //                }

    //            }
    //            //npc之前没有与别人建立亲密关系
    //            else
    //            {
    //                foreach (var otherID in npc.targetNpc)
    //                {
    //                    TalkFirst(npc, WorldSceneMgr.Instance.worldAllNpc[otherID]);
    //                    interactionNum--;
    //                    if (interactionNum == 0) return;
    //                }
    //            }
    //        }
    //        else Debug.Log($"{npc.Id}社交靶对象为空");
    //    }
    //}
    //#endregion
    //#region 谈话
    //private void TalkFirst(NpcBase npc, NpcBase other)
    //{
    //    npc.relationships.Add(other.Id, new()
    //    {
    //        npcRelated = other,
    //        friendship = (int)Mathf.Ceil(4 * socialModulus(npc.socialRank - other.socialRank) *
    //                      NationModulus(npc.race,
    //                      other.race) *
    //                      npc.charm / 50 * CharacterModulus(npc.moralTag, other.moralTag)),
    //        isTalkedThree = 1
    //    });
    //    if (other.relationships.Keys.Contains(npc.Id))
    //    {
    //        Debug.Log($"{other.Id}已经认识{npc.Id}在第一次谈话前");
    //    }
    //    else
    //    {
    //        other.relationships.Add(npc.Id,
    //new()
    //{
    //    npcRelated = npc,
    //    friendship = (int)Mathf.Ceil(4 * socialModulus(npc.socialRank - other.socialRank) *
    //               NationModulus(npc.race,
    //               other.race) *
    //               npc.charm / 50 * CharacterModulus(npc.moralTag, other.moralTag)),
    //    isTalkedThree = 1
    //}
    //);
    //    }

    //}
    ///// <summary>
    ///// 基于已经认识的人
    ///// </summary>
    ///// <param name="npc"></param>
    ///// <param name="other"></param>
    //private void TalkNormal(NpcBase npc, NpcBase other)
    //{
    //    if (npc.relationships.Keys.Contains(other.Id))
    //    {
    //        npc.relationships[other.Id].isTalkedThree++;
    //        npc.relationships[other.Id].friendship += (int)Mathf.Ceil(4 * socialModulus(npc.socialRank - other.socialRank) *
    //                      NationModulus(npc.race,
    //                      other.race) *
    //                      npc.charm / 50 * CharacterModulus(npc.moralTag, other.moralTag));

    //    }
    //    else
    //        Debug.Log($"主体{npc.Id}未与{other.Id}相识但进行了第二次及以上谈话");
    //    if (other.relationships.Keys.Contains(npc.Id))
    //    {
    //        other.relationships[npc.Id].isTalkedThree++;
    //        other.relationships[npc.Id].friendship += (int)Mathf.Ceil(4 * socialModulus(npc.socialRank - other.socialRank) *
    //                       NationModulus(npc.race,
    //                       other.race) *
    //                       npc.charm / 50 * CharacterModulus(npc.moralTag, other.moralTag));
    //    }
    //    else
    //        Debug.Log($"客体{other.Id}未与{npc.Id}相识但进行了第二次及以上谈话");
    //    //俩次判断 
    //    //判断谈话发发起着的关系队列是否
    //}
    //#endregion
    //#region 一阶段互动
    //private void FirstOrderInteraction(NpcBase npc, List<short> targetFriLow30, List<short> targetFriMore30, ref int interactionNum, bool isFirst)
    //{



    //}

    //#endregion
    //#region 二阶互动

    //private void SecondOrderInteraction(NpcBase npc, NpcBase other, ref int interactionNum)
    //{
    //    int p = r.Next(1, 101);
    //    switch (p)
    //    {
    //        case <= 20:
    //            break;
    //        case > 20 and <= 40:
    //            break;
    //        default:
    //            break;
    //    }
    //}
    //#region 赠礼
    ///// <summary>
    ///// 在次互动次数可能会减少为负数
    ///// </summary>
    ///// <param name="npc"></param>
    ///// <param name="other"></param>
    ///// <param name="interactionNum"></param>
    //private void GiftForEmo(NpcBase npc, NpcBase other, ref int interactionNum, List<short> targetFriMore30 = null, bool isfirstOrder = true)
    //{
    //    if (npc.items.Any())
    //    {

    //    }
    //    else
    //        MeetForEmo(npc, other, ref interactionNum);
    //    if (interactionNum <= 0) return;
    //}
    //#endregion
    //#region 聚会
    //private void MeetForEmo(NpcBase npc, NpcBase other, ref int interactionNum, List<short> targetFriMore30 = null, bool isfirstOrder = true)
    //{

    //}
    //#endregion
    //#region 邀约
    //private void InvitationForEmo(NpcBase npc, NpcBase other, ref int interactionNum)
    //{

    //}
    //#endregion
    //#endregion
    //#endregion
    #endregion
    #endregion

}