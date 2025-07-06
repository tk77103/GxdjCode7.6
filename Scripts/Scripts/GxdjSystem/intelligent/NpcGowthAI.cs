using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGowthAI : BaseManger<NpcGowthAI>
{
    #region 数据容器
    System.Random r = new();
    #endregion
    #region 函数
    private NpcGowthAI() { }
    #region 时间演化初的npc技能生成

    public void GenerateSkill(NpcBase npc)
    {
        //npc.myJobSkill.skillPoint = (int)npc.intellegence;
        switch (npc.companyAcademy)
        {
            case "理工":
                break;
            case "文传":
                break;
            case "艺术":
                break;
            case "商业":
                break;
            case "身心":
                break;
            case "任意":
                break;
        }
    }
    #endregion
    #region npc智能成长(过年判断)
    public void NpcIntelligentGrow()
    {
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            npc.npcAge++;
            if (npc.npcAge > 40)
            {
                switch (npc.growType)
                {
                    case 1:
                    case 3:
                        if (r.Next(1, 101) <= 10)
                            npc.isStopGrow = true;
                        break;
                    case 2:
                        if (r.Next(1, 101) <= 5)
                            npc.isStopGrow = true;
                        break;
                }
            }
            if (!npc.isStopGrow)
            {
                switch (npc.growType)
                {
                    case 1:
                        npc.intellegence += 0.5f;
                        break;
                    case 2:
                        npc.intellegence += 1;
                        break;
                    case 3:
                        npc.intellegence += r.Next(1, 3) > 1 ? 2 : 1;
                        break;
                }
            }
            if (npc.industry == "黑社会")
                continue;
                DegreeUp(npc);
        }
    }
    #endregion
    #region 学位提升 （不为博士时有机会进入）
    public void DegreeUp(NpcBase npc)
    {
        if (npc.degree >= Degree.博士)
            return;
        else
        {
            if (r.Next(1, 101) <= 5)
            {
                switch (npc.degree)
                {
                    case Degree.初中:
                        if (npc.intellegence > 10)
                        {
                            npc.degree = Degree.高中;
                            npc.myJobSkill.skillPoint += 10;
                        }
                        break;
                    case Degree.高中:
                        if (npc.intellegence > 20)
                        {
                            npc.degree = Degree.学士;
                            npc.myJobSkill.skillPoint += 20;
                        }
                        break;
                    case Degree.学士:
                        if (npc.intellegence > 30)
                        {
                            npc.degree = Degree.硕士;
                            npc.myJobSkill.skillPoint += 50;
                        }
                        break;
                    case Degree.硕士:
                        if (npc.intellegence > 40)
                        {
                            npc.degree = Degree.博士;
                            npc.myJobSkill.skillPoint += 100;
                        }
                        break;
                }
            }

        }
    }
    #endregion
    #endregion
}
