using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
//�罻����
public class SocialProgress
{

}
public class NpcEmotionMgr : BaseManger<NpcEmotionMgr>
{
    #region ��������
    System.Random r = new();
    List<WorldScene> consumeScene;
    #endregion
    #region ����
    #region ���캯��
    private NpcEmotionMgr()
    {

    }
    #endregion
    #region �������
    #region ����npc�����״̬(��ʱ����switch��д)
    public short GetNpcLoveStatus(short age)
    { //personal������� �ѻ�2/����1/����0
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
    #region ������ż����
    public void GenerateSpouse(Dictionary<short, NpcBase> worldNpc)
    {//�ѻ��У���ʱ��Ϊ�����ѻ���Ҫ����Ů��
        List<short> mans = worldNpc?.FilterValues(e => e.gender is false && e.loveStatus is 2).Select(e=>e.Id).ToList();
        //�ѻ�Ů
        List<short> womans = worldNpc?.FilterValues(e => e.gender is true && e.loveStatus is 2).Select(e=>e.Id).ToList();

        //Ϊ�˵��ڶ�����������
        List<short> npcTemp = new();
        //Ů�Զ�������
        for (int i = womans.Count - 1; i >= 0; i--)
        {
            npcTemp = mans.Where(e => Mathf.Abs(worldNpc[e].socialRank - worldNpc[ womans[i]].socialRank) <= 2).ToList();
            //�ж��ѻ������Ƿ����㹻������
            if (npcTemp.Any())
            {
                npcTemp = npcTemp.Shuffle(npcTemp.Count);
                //Ů�˹�ϵ���
                worldNpc[womans[i]].relationships.Add(npcTemp[0], new()
                {
                    relatedID = npcTemp[0],
                    personalRelations = Kinship.��ż,
                    friendship = r.Next(60, 101)
                });
                //���˹�ϵ���
                worldNpc[npcTemp[0]].relationships.Add(womans[i], new()
                {
                    relatedID = womans[i],
                    personalRelations = Kinship.��ż,
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
    #region �������£��������ˣ����� (����������ʱ�����ع�)
   public void MatchWorldCouple(Dictionary<short,NpcBase> worldAllNpc)
    {
        List<short>womansLove=worldAllNpc.Values.Where(e=>e.loveStatus==1&&e.gender==true).Select(e=>e.Id).ToList();
        List<short>mansLove=worldAllNpc.Values.Where(e=>e.loveStatus==1&&e.gender==false).Select(e=>e.Id).ToList();
        List<short>allNpcKey=worldAllNpc.Keys.ToList();
        List<short> halfMan = mansLove.Shuffle(mansLove.Count / 2);
        HalfManMatchLover(halfMan, womansLove,mansLove,allNpcKey,worldAllNpc);
        //Ϊʣ���һ��Ů��ƥ������
        List<short> halfWoman = womansLove.Shuffle(womansLove.Count / 2);
        HalfWomanMatchLover(halfWoman, womansLove, mansLove, allNpcKey, worldAllNpc);
        //ʣ�����Ů ��ʣ������ ��ʱallNpcKey �Ѿ���ȫ���Ѿ�ƥ�����Ů�޳��ɾ�
        ForOtherMatchLove(true, womansLove, mansLove, allNpcKey, worldAllNpc);
        ForOtherMatchLove(false, womansLove, mansLove, allNpcKey, worldAllNpc);
        if (mansLove.Any())
            mansLove.ForEach(e => worldAllNpc[e].loveStatus = 0);
        if (womansLove.Any())
            womansLove.ForEach(e => worldAllNpc[e].loveStatus = 0);
    }
    #region Ϊһ������ƥ������
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
            //��ƥ������Ѿ���������ǩ����Ů���Ƴ�
            womansLove.Remove(womanMatch[0]);
            mansLove.Remove(halfMan[i]);
            allNpcKey.Remove(halfMan[i]);
            allNpcKey.Remove(womanMatch[0]);
        }
        halfMan.Clear();
    }
    #endregion
    #region Ϊʣ���һ��Ů��ƥ������
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
            //��ƥ������Ѿ���������ǩ����Ů���Ƴ�
            womansLove.Remove(halfWoman[i]);
            mansLove.Remove(manMatch[0]);
            allNpcKey.Remove(halfWoman[i]);
            allNpcKey.Remove(manMatch[0]);
        }
        halfWoman.Clear();
    }
    #endregion
    #region Ϊʣ�����Ůƥ��
    public void ForOtherMatchLove(bool isWoman,List<short> womansLove, List<short> mansLove, List<short> allNpcKey, Dictionary<short, NpcBase> worldAllNpc)
    {
        List<short> tempUse = new();
        List<short> tempMatch = new();
        if (isWoman)
        {//��ʱallNpcKey �Ѿ���ȫ���Ѿ�ƥ�����Ů�޳��ɾ�, ɸѡʣ�°���ûƥ������� �Լ���������
            tempUse = allNpcKey.Where(e => worldAllNpc[e].gender == false).ToList();
            for(int i = womansLove.Count - 1; i >= 0; i--)
            {//ɸѡʣ�������з�������������
                tempMatch = tempUse.Where(e => Mathf.Abs(worldAllNpc[womansLove[i]].jobLevel - worldAllNpc[e].jobLevel) <= 2).ToList();
                tempMatch = tempMatch.Shuffle(tempMatch.Count);
                //�жϱ���ѡ���Ե����״̬
                switch (worldAllNpc[tempMatch[0]].loveStatus)
                {   //������Ϊ����״̬
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
                //ɸѡʣ��Ů���з���������Ů��
                tempMatch = tempUse.Where(e => Mathf.Abs(worldAllNpc[mansLove[i]].jobLevel - worldAllNpc[e].jobLevel) <= 2).ToList();
                tempMatch = tempMatch.Shuffle(tempMatch.Count);
                switch (worldAllNpc[tempMatch[0]].loveStatus)
                {   //������Ϊ����״̬
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
    #region Ϊʣ�����Ůƥ��

    #endregion
    #endregion
    //#region ȷ��ͬ�¹�ϵֵ
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
    //#region ȷ��npc��Ľ����
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
    //#region ����ݻ�
    //#region �ҵ��罻��Ŀ�� �����������
    ////��ְ��8��ְ��10��
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
    //                    npc.targetNpc.AddRange(allNpcKey.Except(npc.targetNpc).ToList().Shuffle(3));//�������Ľ���������
    //                }
    //                else
    //                    npc.targetNpc.AddRange(allNpcKey.Except(npc.targetNpc).ToList().Shuffle(4));
    //                //���ְ��ͬ��
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
    //                    npc.targetNpc.Add(npc.AdmirationObject); //�������Ľ���������
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
    //#region npc�����л���
    //#region ������н�����ʽ
    //#region ��������
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
    //#region �ײ�ϵ��
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
    //#region ����ϵ��
    //private float NationModulus(Nation a, Nation b)
    //{
    //    // ͬ��
    //    if (a == b) return 1.5f;

    //    // �����Ĵ���ӣɽ������ļ���
    //    var specialNations = new[] { Nation.�Ĵ�, Nation.ӣɽ, Nation.���� };
    //    // ����Ƿ����Ĵ���ӣɽ������֮������
    //    if (specialNations.Contains(a) && specialNations.Contains(b))
    //        return 1.3f;

    //    // ����Ĵ�-̩�� �� ӣɽ-̩�� �����
    //    if ((a == Nation.�Ĵ� && b == Nation.̩��) || (a == Nation.̩�� && b == Nation.�Ĵ�) ||
    //        (a == Nation.ӣɽ && b == Nation.̩��) || (a == Nation.̩�� && b == Nation.ӣɽ))
    //        return 0.6f;

    //    // �������
    //    return 1.0f;
    //}
    //#endregion
    //#region �Ը�����
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
    //#region ����npc�Ļ���ai
    ///// <summary>
    ///// ģ������npc��Ļ���
    /////  ������û�����κ����й������������������˻�����������ӡ��
    /////  ��һ���ֻ���������������ʶ����ж���δ��ʶ��
    /////  �ж���ȫ����ʶ��ʼ�����Ļ��� ���� ���ۻ�
    /////  ���׿�ʼ������Լ �����������ж�
    /////  Ŀǰ��Լֻ����������ȥ������ø����� �˴���Ҫϸ��
    ///// </summary>
    //public void NPCEmotionalInteraction()
    //{
    //    int interactionNum = 0;
    //    foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
    //    {
    //        if (npc.targetNpc.Any())
    //        {  //ÿ�λ������Ļ�������
    //            interactionNum = 12;
    //            if (npc.relationships.Any())
    //            {
    //                //��һ�λ��� ������û���������˻���
    //                //����ж�����ڲ��������ѽ���
    //                if (npc.targetNpc.Except(npc.relationships.Keys).Any())
    //                {
    //                    List<short> strangeID = npc.targetNpc.Except(npc.relationships.Keys).ToList();
    //                    //���ȶ�û���ദ����npc ����һ��̸��
    //                    foreach (var key in strangeID)
    //                    {
    //                        TalkFirst(npc, WorldSceneMgr.Instance.worldAllNpc[key]);
    //                        interactionNum--;
    //                        if (interactionNum == 0) return;
    //                    }
    //                    //��ʱ���аж��󶼽����˻���

    //                }
    //                //ȫ���ж����Ѿ�������npc�ѽ����Ķ�����
    //                else
    //                {
    //                    //������̸������С�����Ľ���һ�ζԻ�
    //                    if (npc.relationships.Values.Where(e => e.isTalkedThree < 3).Any())
    //                    {
    //                        List<short> conless3 = npc.relationships.Values.Where(e => e.isTalkedThree < 3).Select(e => e.npcRelated.Id).ToList();
    //                        foreach (var id in conless3)
    //                        {
    //                            TalkNormal(npc, WorldSceneMgr.Instance.worldAllNpc[id]);
    //                            interactionNum--;
    //                            if (interactionNum == 0) return;
    //                        }
    //                        //Ŀǰ����̸��Ϊ���� ���������
    //                    }
    //                    //�����˶���̸�������γ����Ƴ���̸���ѡ�� ֻ���оۻ� ��Լ
    //                    else
    //                    {
    //                        //��ϵֵ������ʮ�Ľ���һ�׻���
    //                        //������ʮ���ж��׻���
    //                        if (npc.targetNpc.All(e => npc.relationships.Keys.Contains(e)))
    //                        {//�˴����еİ�Ŀ��Ӧ�ö��Ѿ��Ӵ���
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
    //                                //����һ�׶��ж�
    //                                //����ϵ������ʮ�ķ�������ж�

    //                            }
    //                            if (targetFriMore30.Any())
    //                            {
    //                                //���ж��׻���
    //                            }
    //                        }
    //                        else
    //                            Debug.Log($"{npc.Id}δ�����а�Ŀ�꽨����ϵ������ԭ��");
    //                    }

    //                }

    //            }
    //            //npc֮ǰû������˽������ܹ�ϵ
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
    //        else Debug.Log($"{npc.Id}�罻�ж���Ϊ��");
    //    }
    //}
    //#endregion
    //#region ̸��
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
    //        Debug.Log($"{other.Id}�Ѿ���ʶ{npc.Id}�ڵ�һ��̸��ǰ");
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
    ///// �����Ѿ���ʶ����
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
    //        Debug.Log($"����{npc.Id}δ��{other.Id}��ʶ�������˵ڶ��μ�����̸��");
    //    if (other.relationships.Keys.Contains(npc.Id))
    //    {
    //        other.relationships[npc.Id].isTalkedThree++;
    //        other.relationships[npc.Id].friendship += (int)Mathf.Ceil(4 * socialModulus(npc.socialRank - other.socialRank) *
    //                       NationModulus(npc.race,
    //                       other.race) *
    //                       npc.charm / 50 * CharacterModulus(npc.moralTag, other.moralTag));
    //    }
    //    else
    //        Debug.Log($"����{other.Id}δ��{npc.Id}��ʶ�������˵ڶ��μ�����̸��");
    //    //�����ж� 
    //    //�ж�̸���������ŵĹ�ϵ�����Ƿ�
    //}
    //#endregion
    //#region һ�׶λ���
    //private void FirstOrderInteraction(NpcBase npc, List<short> targetFriLow30, List<short> targetFriMore30, ref int interactionNum, bool isFirst)
    //{



    //}

    //#endregion
    //#region ���׻���

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
    //#region ����
    ///// <summary>
    ///// �ڴλ����������ܻ����Ϊ����
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
    //#region �ۻ�
    //private void MeetForEmo(NpcBase npc, NpcBase other, ref int interactionNum, List<short> targetFriMore30 = null, bool isfirstOrder = true)
    //{

    //}
    //#endregion
    //#region ��Լ
    //private void InvitationForEmo(NpcBase npc, NpcBase other, ref int interactionNum)
    //{

    //}
    //#endregion
    //#endregion
    //#endregion
    #endregion
    #endregion

}