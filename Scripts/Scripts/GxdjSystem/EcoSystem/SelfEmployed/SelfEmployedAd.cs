using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 股权结构
/// </summary>
public class EquityStructure
{//股权分享者
    public NpcBase Shareholder;
    public float ShareholdingRatio = 0;
    //自营公司id
    public short id;
    public EquityStructure(NpcBase shareholder, int shareholdingRatio, short id)
    {
        Shareholder = shareholder;
        ShareholdingRatio = shareholdingRatio;
        this.id = id;
    }
    public EquityStructure() { }
}
public class SelfEmployedAd : SelfEmployed
{
    #region 数据容器
    //创建时间
    public long establishDate;
    //设立了多久
    public long setTime;
    //持有人
    public short holderID;
    //股权结构
    public List< EquityStructure> equityStructure = new();
    //自营企业的所在房屋
    public Estate selfEmEstate;
    //自营企业公司资金
    public float selfEdFund;
    // 每月收入
    public int monthlyIncome;
    //年收入
    public int annualIncome;
    //总收入
    public int totalIncome;
    #region 企业支出
    //人力成本
    public int huamanCost;
    //物业成本
    public int estCost;
    //税务成本
    public int officalCost;
    //总成本
    public int totalCost;
    #endregion
    #endregion
    #region 函数
    public SelfEmployedAd(SelfEmployed selfEmployed)
    {
        id = selfEmployed.id;
        companyName = selfEmployed.companyName;
        Industry = selfEmployed.Industry;
        rank = selfEmployed.rank;
        employeeNum = selfEmployed.employeeNum;
        profitStandard = selfEmployed.profitStandard;
        floatIndex = selfEmployed.floatIndex;
    }
   public SelfEmployedAd() { }
    #endregion
}
