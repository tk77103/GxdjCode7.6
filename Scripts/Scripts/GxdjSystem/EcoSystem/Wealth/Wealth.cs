using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;

public class Wealth : BaseManger<Wealth>
{
    #region 数据容器
    System.Random r = new();
    #endregion
    #region 函数
    private Wealth() { }
    #region npc 相关
    #region 生成npc初始资金
    public int BaseJobLevelToMoney(short jobLev)
    {
        switch (jobLev)
        {
            case 0:
                return r.Next(12, 19) / 10 * 5000;
            case 1:
                return r.Next(12, 19) / 10 * 25000 ;
            case 2:
                return r.Next(12, 19) / 10 * 125000;
            case 3:
                return r.Next(12, 19) / 10 * 625000;
            case 4:
                return r.Next(12, 19) / 10 * 3125000;
            case 5:
                return r.Next(12, 19) / 10 * 15625000;
            default:
                return 5000;
        }
    }
    #endregion
    #region 获取财富等级
    public short GetWealthRank(int i)
    {
        if (i < 5000)
            return 0;
        else if (i >= 5000 & i < 25000)
            return 1;
        else if (i >= 25000 & i < 125000)
            return 2;
        else if (i >= 125000 & i < 625000)
            return 3;
        else if (i >= 625000 & i < 3125000)
            return 4;
        else return 5;
    }
    #endregion
    #region 获取npc财富等级标签

    public string GetWealthRankTag(int i)
    {
        if (i < 5000)
            return "贫困";
        else if (i >= 5000 & i < 25000)
            return "拮据";
        else if (i >= 25000 & i < 125000)
            return "小康";
        else if (i >= 125000 & i < 625000)
            return "中产";
        else if (i >= 625000 & i < 3125000)
            return "富有";
        else return "豪门";
    }
    #endregion
    #region 获取npc安全资金（当前社会等级-1）
    /// <summary>
    /// 此处值应该是一个动态的值
    /// </summary>
    /// <param name="npc"></param>
    /// <returns></returns>
    public int WealthMinAmount(int wealthRank)
    {
        return wealthRank == 0 ? 5000 :
                wealthRank == 1 ? 25000 :
                wealthRank == 2 ? 125000 :
                wealthRank == 3 ? 625000 :
                wealthRank == 4 ? 3125000 :
                wealthRank == 5 ? 15625000 :
                5000;
    }
    #endregion
    #region 获得npc的薪资
    #endregion
    #endregion
    #region 企业相关
    #region 自营企业创立资金
    public int GetLevFund(int npcSocialRank)
    {
        switch (npcSocialRank)
        {
            case 1:
                return 10000;
            case 2:
                return 100000;
            case 3:
                return 500000;
            case 4:
                return 300000;
            case 5:
                return 1500000;
            default:
                return 10000;
        }
    }
    #endregion
    #region 自营收入
    public int selfEmployedIncome(short empLev)
    {
        switch (empLev)
        {
            case 1:
                return 5000;
            case 2:
                return 25000;
            case 3:
                return 125000;
            case 4:
                return 625000;
            case 5:
                return 3250000;
            default:
                return 5000;
        }
    }
    #endregion
    #endregion


    #region 目前把npc的社会等级放到这里
    public short CurrentSocailRank(NpcBase npc)
    {
        npc.wealthRank = GetWealthRank(npc.money);
        short a = (short)(npc.estate.Any() ? (npc.estate.Filter(e => e.estIsHold == true).Any() ?
            npc.estate.Filter(e => e.estIsHold == true).MaxBy(e => e.estRank).estRank : 0) : 0);
        short b = (short)(npc.vehicles.Any()?npc.vehicles.MaxBy(e=>e.level).level : 0);
        return (short)Mathf.Round((npc.jobLevel + a + b + npc.wealthRank) / 4f);
    }
    #endregion

    #endregion
}
