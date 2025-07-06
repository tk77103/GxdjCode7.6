using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EstateMgr : BaseManger<EstateMgr>
{
    #region 数据容器
    public Dictionary<int, Estate> AllEstate;
    private System.Random r = new System.Random();
    #endregion
    #region 函数
    #region 构造函数
    private EstateMgr()
    {
        BinaryDataMgr.Instance.LoadTable<EstateContainer, Estate>();
        AllEstate = BinaryDataMgr.Instance.GetTable<EstateContainer>().dataDic;
    }
    #endregion
    #region 筛选
    #region 为获取的房屋设置单元保证其唯一
    /// <summary>
    /// 为房屋设置单元门票号 为其设置状态
    /// </summary>
    /// <param name="estateM">房产</param>
    /// <param name="isHold">是否持有</param>
    /// <param name="isliving">是否居住</param>
    /// <param name="isRent">是否租入</param>
    /// <param name="isRentout">是否租出</param>
    /// <param name="isCpn">是否开设</param>
    /// <returns></returns>
    public Estate GetRandomEstate(Estate estateM, bool isHold = false, bool isliving = false, bool isRent = false,
        bool isRentout = false, bool isCpn = false)
    {
        Estate estate = new(estateM);
        int z = r.Next(0, 11);
        estate.estUnit = LetterData.Instance.letters[z].ToString();
        estate.estNum = (short)r.Next(1, 100);
        estate.estIsHold = isHold;
        estate.estIsLiving = isliving;
        estate.estIsRent = isRent;
        estate.estIsRentout = isRentout;
        estate.estIsCpn = isCpn;
        return estate;
    }
    #endregion
    #region 筛选指定等级住房/写字楼 
    /// <summary>
    /// 获取匹配npc所需类型的房屋
    /// </summary>
    /// <param name="estType">0 住房 1写字楼</param>
    /// <param name="estRank">房屋所需等级</param>
    /// <returns></returns>
    public List<Estate> GetMatchEstate(short estType, int estRank)
    {//先筛选类型后 select等级
        return AllEstate.FilterValues(e => e.estType == estType).Filter(e => e.estRank == estRank);
    }
    #endregion
    #region 判断npc是否持有房屋
    public bool IsHaveHouse(NpcBase npc)
    {
        if (npc.estate.Any())
        {
            //有先判定是否持有住房
            if (npc.estate.Filter(e => e.estIsHold == true).Any())
                return true;
            else
                return false;
        }
        else return false;
    }
    #endregion
    #region 判断npc租房是否与当前社会等级匹配
    public bool IsMyRentHouseMatch(NpcBase npc)
    {
        if (npc.estate.Any())
        {
            //有先判定是否租住住房
            if (npc.estate.Filter(e => e.estIsRent == true && e.estType == 0 && e.estIsLiving == true).Any())
            {
                //如果租住的住房等级与社会等级匹配则返回true
                if (npc.estate.Filter(e => e.estIsRent == true && e.estType == 0 && e.estIsLiving == true).MaxBy(e => e.estRank).estRank >= npc.socialRank)
                    return true;
                return false;
            }
            else
                return false;
        }
        else
            return false;
    }
    #endregion
    #region 判断目前的住房是否满足当前社会等级
    public bool IsMyHoldHouseMatch(NpcBase npc)
    {
        if (npc.estate.Filter(e => e.estIsHold == true && e.estIsLiving == true && e.estRank >= npc.socialRank).Any()) return true;
        else return false;
    }
    #endregion
    #region 获取有业npc所在区域的同等级住房/写字楼
    public List<Estate> GetSameAreaLev(short estType, short estRank, string area)
    {
        return AllEstate.FilterValues(e => e.estType == estType && e.estRank == estRank && e.estArea == area);
    }
    #endregion
    #region 判断是否要卖房
    public bool ShouldSellMyHouse(NpcBase npc)
    {
        if (!npc.estate.Any())
            return false;
        else
        {
            if (npc.money <= Wealth.Instance.WealthMinAmount(npc.socialRank - 1))
                return true;
            else return false;
        }
    }
    #endregion=
    #endregion
    #region 行为
    #region 生成世界初始住房
    public Estate GnerateOriginEst(short jobLev)
    {
        int i = r.Next(0, 101);
        switch (jobLev)
        {
            case 0:
                switch (i)
                {
                    case <= 70:
                        return null;
                    case > 70 and <= 90:
                        return GetRandomEstate(GetMatchEstate(0, 1).GetRandomItem(), true, true, false, false, false);
                    case > 90:
                        return GetRandomEstate(GetMatchEstate(0, 2).GetRandomItem(), true, true, false, false, false);
                };
            case 1:
                switch (i)
                {
                    case <= 50:
                        return null;
                    case > 50 and <= 70:
                        return GetRandomEstate(GetMatchEstate(0, 1).GetRandomItem(), true, true, false, false, false);
                    case > 70:
                        return GetRandomEstate(GetMatchEstate(0, 2).GetRandomItem(), true, true, false, false, false);
                };
            case 2:
                switch (i)
                {
                    case <= 30:
                        return null;
                    case > 30 and <= 50:
                        return GetRandomEstate(GetMatchEstate(0, 1).GetRandomItem(), true, true, false, false, false);
                    case > 50 and <= 90:
                        return GetRandomEstate(GetMatchEstate(0, 2).GetRandomItem(), true, true, false, false, false);
                    case > 90:
                        return GetRandomEstate(GetMatchEstate(0, 3).GetRandomItem(), true, true, false, false, false);
                };
            case 3:
                switch (i)
                {
                    case <= 50:
                        return GetRandomEstate(GetMatchEstate(0, 2).GetRandomItem(), true, true, false, false, false); ;
                    case > 50 and <= 80:
                        return GetRandomEstate(GetMatchEstate(0, 3).GetRandomItem(), true, true, false, false, false);
                    case > 80:
                        return GetRandomEstate(GetMatchEstate(0, 4).GetRandomItem(), true, true, false, false, false);
                };
            case 4:
                switch (i)
                {
                    case <= 50:
                        return GetRandomEstate(GetMatchEstate(0, 3).GetRandomItem(), true, true, false, false, false); ;
                    case > 50 and <= 95:
                        return GetRandomEstate(GetMatchEstate(0, 4).GetRandomItem(), true, true, false, false, false);
                    case > 95:
                        return GetRandomEstate(GetMatchEstate(0, 5).GetRandomItem(), true, true, false, false, false);
                };
            case 5:
                switch (i)
                {
                    case <= 50:
                        return GetRandomEstate(GetMatchEstate(0, 4).GetRandomItem(), true, true, false, false, false); ;
                    case > 50:
                        return GetRandomEstate(GetMatchEstate(0, 5).GetRandomItem(), true, true, false, false, false);
                };
            default:
                return null;

        }
    }
    #endregion
    #region 确认居住地址
    public void ConfrimNpcLiveAddress(NpcBase npc)
    {
        if (npc.estate.Any())
        {if (npc.estate.Filter(e => e.estIsLiving).Any())
                npc.livingAdress = npc.estate.Filter(e => e.estIsLiving)[0].estName +
                        npc.estate.Filter(e => e.estIsLiving)[0].estUnit + "单元" +
                        npc.estate.Filter(e => e.estIsLiving)[0].estNum + "号";
            else
                Debug.Log(npc.Id + "没有住房");
        }
    }
    #endregion
    #region  第一个月的房屋逻辑
    #region  判断npc房产是否与当前社会等级匹配（在第一个月用的是职务等级）
    public bool IsMatchEstate(NpcBase npc)
    {
        if (npc.jobLevel == 0)
            return true;
        else
        {
            if (npc.estate.Count > 0)
            {
                if (npc.estate.MaxBy(e => e.estRank).estRank >= npc.jobLevel)
                    return true;
                return false;
            }
            else
                return false;
        }
    }
    #endregion
    #region 匹配月为npc匹配房子 同时不做贷款规划只在之后购买产生贷款行为
    /// <summary>
    /// 为npc匹配房子目前用于开始的匹配日
    /// </summary>
    /// <param name="npc"></param>
    /// <param name="minLevWel"></param>
    public void ForMatchEaste(NpcBase npc, int minLevWel)
    {
        List<Estate> sameLevEst = GetMatchEstate(0, npc.jobLevel);
        sameLevEst?.Sort((a, b) => a.estSellPrice.CompareTo(b.estSellPrice));
        List<Estate> sameLevAreaEsT = sameLevEst.Filter(e => e.estArea == npc.companyArea);
        if (npc.estate.Any())
            ThridJudge(npc, sameLevAreaEsT, sameLevEst, minLevWel, true);
        else
            ThridJudge(npc, sameLevAreaEsT, sameLevEst, minLevWel, false);
        ConfrimNpcLiveAddress(npc);
    }
    #region 匹配房子的三层判断
    public void ThridJudge(NpcBase npc, List<Estate> sameLevAreaEsT, List<Estate> sameLevEst, int minLevWel, bool hasEst)
    {
        for (int i = 0; i < sameLevAreaEsT.Count; i++)
        {
            //全款拿下公司附件房子
            if (npc.money - sameLevAreaEsT[i].estSellPrice >= minLevWel)
            {
                npc.estate.ForEach(e => e.estIsLiving = false);
                npc.estate.Add(GetRandomEstate(sameLevAreaEsT[i], true, true, false, false, false));
                npc.money -= sameLevAreaEsT[i].estSellPrice;
                break;
            }
            //不能全款买
            else
            {//如果有房子 判断置后能否买起
                if (hasEst)
                {
                    Estate myBestHouse = npc.estate.MaxBy(e => e.estSellPrice);
                    if (npc.money + myBestHouse.estSellPrice - sameLevAreaEsT[i].estSellPrice >= minLevWel)
                    {
                        npc.money = npc.money + myBestHouse.estSellPrice - sameLevAreaEsT[i].estSellPrice;
                        npc.estate.Remove(myBestHouse);
                        if (npc.estate.Any())
                        {
                            for (int j = 0; j < npc.estate.Count; j++)
                            {
                                npc.estate[j].estIsLiving = false;
                            }
                        }
                        npc.estate.Add(GetRandomEstate(sameLevAreaEsT[i], true, true, false, false, false));
                        break;
                    }
                    //同区域的整不起试试同等级的能不能买不能就退出
                    else
                    {
                        for (int k = 0; k < sameLevEst.Count; k++)
                        {
                            //全款拿下同等级房子
                            if (npc.money - sameLevEst[k].estSellPrice >= minLevWel)
                            {
                                npc.money -= sameLevEst[k].estSellPrice;
                                for (int j = 0; j < npc.estate.Count; j++)
                                {
                                    npc.estate[j].estIsLiving = false;
                                }
                                npc.estate.Add(GetRandomEstate(sameLevEst[k], true, true, false, false, false));
                                break;
                            }
                            //不能全款 则试试置换 已有房屋不用管
                            else
                            {
                                myBestHouse = npc.estate.MaxBy(e => e.estSellPrice);
                                if (npc.money + myBestHouse.estSellPrice - sameLevEst[k].estSellPrice >= minLevWel)
                                {
                                    npc.money = npc.money + myBestHouse.estSellPrice - sameLevEst[k].estSellPrice;
                                    npc.estate.Remove(myBestHouse);
                                    if (npc.estate.Any())
                                    {
                                       npc.estate.ForEach(e=>e.estIsLiving = false);
                                    }
                                    npc.estate.Add(GetRandomEstate(sameLevEst[k], true, true, false, false, false));
                                    break;
                                }
                            }

                        }
                    }
                }
                //如果没房尝试 同区域的房子
                else
                {
                    for (int z = 0; z < sameLevEst.Count; z++)
                    {
                        //全款拿下同等级房子
                        if (npc.money - sameLevEst[z].estSellPrice >= minLevWel)
                        {
                            npc.money -= sameLevEst[z].estSellPrice;
                            npc.estate.Add(GetRandomEstate(sameLevEst[z], true, true, false, false, false));
                            break;
                        }
                    }
                    //操作俩次都买不起就租房
                    if (npc.estate.Count == 0)
                        npc.estate.Add(RentEstate(0, npc.jobLevel));
                }

            }
        }
    }
    #endregion
    #endregion
    #endregion
    #region 房屋部分世界演化行为
    #region 买房 判断与行为并行
    public void BuyHouseOrNot(NpcBase npc)
    {
        int safeMoney = Wealth.Instance.WealthMinAmount(npc.socialRank - 1);
        if (npc.socialRank > 0)
        {//持有房屋
            if (IsHaveHouse(npc))
            {//已经持有同级别房屋考虑 购买写字楼和其他房屋
                if (IsMyHoldHouseMatch(npc))
                {//小于5进行买房
                    if (npc.estate.Count < 5)
                    {
                        switch (r.Next(1, 101))
                        {
                            case <= 20:
                                DoubleJugeForHasHouseLevMatch(npc, safeMoney, npc.socialRank, 1);
                                break;
                            default:
                                DoubleJugeForHasHouseLevMatch(npc, safeMoney, npc.socialRank, 0); break;
                        }

                    }
                }
                //优先将房屋匹配到与当前等级相同
                else
                    DoubleJugeForHasHouse(npc, safeMoney, npc.socialRank);
            }
            //不持有房屋 默认世界上所有人都有地方居住，所以要将他租住的房子去掉
            else
            {
                DoubleJugeForBugHouseNotHoldHouse(npc, safeMoney, npc.socialRank);
                //负担不了同等级房屋的0.5倍价格
                //判断经过一系列操作有无房产
                if (!IsHaveHouse(npc))
                    if (npc.socialRank > 1) DoubleJugeForBugHouseNotHoldHouse(npc, safeMoney, (short)(npc.socialRank - 1));
                //俩层判断仍买不起 租好点的
                if (!IsHaveHouse(npc))
                    if (!IsMyRentHouseMatch(npc))
                    {
                        if (npc.estate.Any())
                        {
                            if (npc.basicSalary > GetMatchEstate(0, npc.socialRank).MinBy(e => e.estRentPrice).estRentPrice)
                            {
                                npc.estate.Clear();
                                npc.estate.Add(RentEstate(0, npc.socialRank));
                            }
                        }
                    }
            }
        }
        ConfrimNpcLiveAddress(npc);
    }
    #region 没房时的双层判断
    public void DoubleJugeForBugHouseNotHoldHouse(NpcBase npc, int safeMoney, short requireRank)
    {//同区域同等级住房
        List<Estate> sameAreaLevEst = AllEstate.FilterValues(e => e.estRank == requireRank && e.estArea == npc.companyArea && e.estType == 0);

        //判断能否在购买本区域最低价房子后仍然高于安全资金
        //如果没有
        if (!sameAreaLevEst.Any())
        {
            Debug.Log("npc所在公司区域无对应房屋");
            sameAreaLevEst = AllEstate.FilterValues(e => e.estRank == requireRank && e.estType == 0);
        }
        //能够负担同等级的房子
        if (npc.money - sameAreaLevEst.MinBy(e => e.estSellPrice).estSellPrice * 0.5 >= safeMoney)
        {//满足了条件只有百分之一的概率购买房子
            if (r.Next(1, 101) <= 1)
            {//优先全款买
                sameAreaLevEst.Sort((a, b) => b.estSellPrice.CompareTo(a.estSellPrice));
                for (int i = 0; i < sameAreaLevEst.Count; i++)
                { //有条件全款买则买贵的给npc设置障碍
                    if (npc.money - sameAreaLevEst[i].estSellPrice >= safeMoney)
                    {
                        if (npc.estate.Any())
                            //如果能买则推掉租房
                            npc.estate.Clear();
                        npc.estate.Add(GetRandomEstate(sameAreaLevEst[i], true, true, false, false, false));
                        npc.money -= sameAreaLevEst[i].estSellPrice;
                        break;
                    }
                }
                //循环完判断是否买到了房子如还没有买到考虑贷款 如果能买则已经买否则进行下级贷款判断
                //判断npc12个月的工资 为授信额度
                //判断有贷款能否买起房子
                for (int j = 0; j < sameAreaLevEst.Count; j++)
                {
                    if (npc.money + npc.basicSalary * 120 - sameAreaLevEst[j].estSellPrice >= safeMoney)
                    {//判断背多少债务 并用 将所有多余资金用于房子剩下为120月贷款
                        npc.debts.Add(WorldBank.Instance.AddContract(new DebtContract()
                        {
                            creditors = 771103,
                            debtor = npc.Id,
                            debtItem = new() { contractEst = sameAreaLevEst[j] },
                            startTime = WorldClock.Instance.TotalTime,
                            endTime = WorldClock.Instance.TotalTime + 120,
                            debtAmount = (sameAreaLevEst[j].estSellPrice - (npc.money - safeMoney)) * 1.5f,
                            payType = 0,
                            debtStatus = 0,
                            perRepay = (sameAreaLevEst[j].estSellPrice - (npc.money - safeMoney)) * 1.5f / 120f,
                            interestRate = 1.5f,
                            overdueInterestRate = 2,
                            debtType = 0
                        }));
                        //如果能买则推掉租房
                        if (npc.estate.Any())
                            npc.estate.Clear();
                        npc.estate.Add(GetRandomEstate(sameAreaLevEst[j], true, true, false, false, false));
                        npc.estate.ForEach(e => e.hasLoan = true);
                        npc.money = safeMoney;
                        break;
                    }
                }
            }
        }
    }
    #endregion
    #region 有房时候的双重判断
    public void DoubleJugeForHasHouse(NpcBase npc, int safeMoney, short requireRank)
    {
        List<Estate> sameAreaLevEst = AllEstate.FilterValues(e => e.estRank == requireRank && e.estArea == npc.companyArea && e.estType == 0);

        //判断能否在购买本区域最低价房子后仍然高于安全资金
        //如果没有
        if (!sameAreaLevEst.Any())
        {
            Debug.Log("npc所在公司区域无对应房屋");
            sameAreaLevEst = AllEstate.FilterValues(e => e.estRank == requireRank && e.estType == 0);
        }
        //能够负担同等级的房子
        if (npc.money - sameAreaLevEst.MinBy(e => e.estSellPrice).estSellPrice * 0.5 >= safeMoney)
        {//满足了条件只有百分之一的概率购买房子
            if (r.Next(1, 101) <= 1)
            {//优先全款买
                sameAreaLevEst.Sort((a, b) => b.estSellPrice.CompareTo(a.estSellPrice));
                for (int i = 0; i < sameAreaLevEst.Count; i++)
                { //有条件全款买则买贵的给npc设置障碍  
                    if (npc.money - sameAreaLevEst[i].estSellPrice >= safeMoney)
                    {
                        if (npc.estate.Any())
                            npc.estate.ForEach(e => e.estIsLiving = false);
                        npc.estate.Add(GetRandomEstate(sameAreaLevEst[i], true, true, false, false, false));
                        npc.money -= sameAreaLevEst[i].estSellPrice;
                        break;
                    }
                }
                //循环完判断是否买到了房子如还没有买到考虑贷款 如果能买则已经买否则进行下级贷款判断
                //判断npc12个月的工资 为授信额度
                //判断有贷款能否买起房子
                for (int j = 0; j < sameAreaLevEst.Count; j++)
                {
                    if (npc.money + npc.basicSalary * 120 - sameAreaLevEst[j].estSellPrice >= safeMoney)
                    {//判断背多少债务 并用 将所有多余资金用于房子剩下为120月贷款
                        npc.debts.Add(WorldBank.Instance.AddContract(new DebtContract()
                        {
                            creditors = 771103,
                            debtor = npc.Id,
                            debtItem = new() { contractEst = sameAreaLevEst[j] },
                            startTime = WorldClock.Instance.TotalTime,
                            endTime = WorldClock.Instance.TotalTime + 120,
                            debtAmount = (sameAreaLevEst[j].estSellPrice - (npc.money - safeMoney)) * 1.5f,
                            payType = 0,
                            debtStatus = 0,
                            perRepay = (sameAreaLevEst[j].estSellPrice - (npc.money - safeMoney)) * 1.5f / 120f,
                            interestRate = 1.5f,
                            overdueInterestRate = 2,
                            debtType=0
                        }));
                        //购买后之前居住的房产不再住
                        if (npc.estate.Any())
                            npc.estate.ForEach(e => e.estIsLiving = false);
                        Estate myNewHouse = GetRandomEstate(sameAreaLevEst[j], true, true, false, false, false);
                        myNewHouse.hasLoan = true;
                        npc.estate.Add(myNewHouse);
                        npc.money = safeMoney;
                        break;
                    }
                }
            }
        }
    }
    #endregion
    #region 有房且房屋满足当前社会等级时候的双层判断买当前等级或者下一级的(20概率购买写字楼,80概率住房）
    public void DoubleJugeForHasHouseLevMatch(NpcBase npc, int safeMoney, short requireRank, short estType)
    {

        List<Estate> sameAreaLevEst = AllEstate.FilterValues(e => e.estRank == requireRank && e.estArea == npc.companyArea && e.estType == estType);

        //判断能否在购买本区域最低价房子后仍然高于安全资金
        //如果没有
        if (!sameAreaLevEst.Any())
        {
            Debug.Log("npc所在公司区域无对应房屋");
            sameAreaLevEst = AllEstate.FilterValues(e => e.estRank == requireRank && e.estType == estType);
        }
        //能够负担同等级的房子
        if (npc.money - sameAreaLevEst.MinBy(e => e.estSellPrice).estSellPrice * 0.5 >= safeMoney)
        {//满足了条件只有百分之一的概率购买房子
            if (r.Next(1, 101) <= 1)
            {//优先全款买
                sameAreaLevEst.Sort((a, b) => b.estSellPrice.CompareTo(a.estSellPrice));
                for (int i = 0; i < sameAreaLevEst.Count; i++)
                { //有条件全款买则买贵的给npc设置障碍  
                    if (npc.money - sameAreaLevEst[i].estSellPrice >= safeMoney)
                    {
                        npc.estate.Add(GetRandomEstate(sameAreaLevEst[i], true, false, false, false, false));
                        npc.money -= sameAreaLevEst[i].estSellPrice;
                        break;
                    }
                }
                //循环完判断是否买到了房子如还没有买到考虑贷款 如果能买则已经买否则进行下级贷款判断
                //判断npc12个月的工资 为授信额度
                //判断有贷款能否买起房子
                for (int j = 0; j < sameAreaLevEst.Count; j++)
                {
                    if (npc.money + npc.basicSalary * 120 - sameAreaLevEst[j].estSellPrice >= safeMoney)
                    {//判断背多少债务 并用 将所有多余资金用于房子剩下为120月贷款
                        npc.debts.Add(WorldBank.Instance.AddContract(new DebtContract()
                        {
                            creditors = 771103,
                            debtor = npc.Id,
                            debtItem = new() { contractEst = sameAreaLevEst[j] },
                            startTime = WorldClock.Instance.TotalTime,
                            endTime = WorldClock.Instance.TotalTime + 120,
                            debtAmount = (sameAreaLevEst[j].estSellPrice - (npc.money - safeMoney)) * 1.5f,
                            payType = 0,
                            debtStatus = 0,
                            perRepay = (sameAreaLevEst[j].estSellPrice - (npc.money - safeMoney)) * 1.5f / 120f,
                            interestRate = 1.5f,
                            overdueInterestRate = 2,
                            debtType=0
                        }));
                        //购买后之前居住的房产不再住
                        Estate myNewHouse = GetRandomEstate(sameAreaLevEst[j], true, true, false, false, false);
                        myNewHouse.hasLoan = true;
                        npc.estate.Add(myNewHouse);
                        npc.money = safeMoney;
                        break;
                    }
                }
            }
        }
    }
    #endregion
    #endregion
    #region 租房用于大世界行为(在外层判断是否满足租入条件)
    /// <summary>
    /// 租入 
    /// </summary>
    /// <param name="estType">0,为住房，1为写字楼 同时从此处判断使用用途</param>
    /// <param name="rentEstateRank">租住房屋的等级</param>
    public Estate RentEstate(short estType, short rentEstateRank = 1)
    {
        rentEstateRank = (short)(rentEstateRank <= 0 ? 1 : rentEstateRank);
        List<Estate> estatesSameLev = GetMatchEstate(estType, rentEstateRank);
        if (estatesSameLev.Count > 0 && estType == 0)
        {
            Estate est = GetRandomEstate(estatesSameLev[r.Next(0, estatesSameLev.Count)],
                false, true, true, false, false);
            return est;
        }
        //npc租房
        if (estatesSameLev.Count > 0 && estType == 1)
        {
            Estate est = GetRandomEstate(estatesSameLev[r.Next(0, estatesSameLev.Count)],
                false, false, true, false, true);
            return est;
        }
        Debug.Log("没有匹配的租房信息" + estType + " " + rentEstateRank);
        return null;
    }
    #endregion
    #region 租房收支
    public void RentHouseIncome(NpcBase npc)
    {
        if (npc.estate.Any())
        {
            if (npc.estate.Filter(e => e.estIsRent == true&&e.estIsCpn==false).Any())
            {
                List<Estate> rentHoues = npc.estate.Filter(e => e.estIsRent == true && e.estIsCpn == false);
                for (int i = 0; i < rentHoues.Count; i++) {
                    npc.money -= rentHoues[i].estRentPrice;
                }
            }
            if(npc.estate.Filter(e => e.estIsRentout == true).Any())
            {
                npc.estate.Filter(e => e.estIsRentout == true).ForEach(e => { npc.money += e.estRentPrice; });
            }
        }
    }
    #endregion
    #region 租出房子
    public void RentOutMyHouse(NpcBase npc)
    {
        if (npc.estate.Any())
            if (npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estIsLiving == false && e.estIsRentout == false).Any())
                npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estIsLiving == false && e.estIsRentout == false).ForEach(e => e.estIsRentout = true);
    }
    #endregion 
    #region 卖房 世界演化行为 
    public void SellMyHouseAi(NpcBase npc)
    {
        int safeMonry = (int)(Wealth.Instance.WealthMinAmount(npc.socialRank - 1) * 0.5f);
        if (npc.estate.Any())
        {//优先处理最便宜的 非住 非开设 非贷款 房子 多余的房子正常情况下是租出的但不考虑租出给谁，所以此处，粗暴的将所有在租房子售出
            if (npc.estate.Filter(e => e.estIsHold == true && e.estIsLiving == false && e.estIsCpn == false && e.hasLoan == false).Any())
            {
                List<Estate> estSell = npc.estate.Filter(e => e.estIsHold == true && e.estIsLiving == false && e.estIsCpn == false && e.hasLoan == false);
                estSell.Sort((a, b) => b.estSellPrice.CompareTo(a.estSellPrice));
                for (int i = estSell.Count; i >= 0; i--)
                {
                    if (npc.money + estSell[i].estSellPrice >= safeMonry)
                    {
                        npc.money += estSell[i].estSellPrice;
                        npc.estate.Remove(estSell[i]);
                        break;
                    }
                    else
                    {//如果卖完后仍然小于安全资金则继续卖
                        npc.money += estSell[i].estSellPrice;
                        npc.estate.Remove(estSell[i]);
                    }
                }
            }
            if (ShouldSellMyHouse(npc))
            {  //第二步判断是否有持有的用于企业开设的写字楼 无贷款房子
                if (npc.selfEmployed != null && npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == true && e.hasLoan == false).Any())
                {
                    //出售持有的写字楼 改为租用（此处如果往细可能会判断是否能租房此处不再讨论，为npc租一个对应等级的房屋）
                    List<Estate> estSell = npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == true && e.hasLoan == false);
                    //因为默认npc的自营只会有一处（此处又埋下了坑 npc只有一处自营是否合理）
                    npc.money += estSell[0].estSellPrice;
                    npc.selfEmployed.selfEmEstate = RentEstate(1, npc.selfEmployed.rank);
                    npc.estate.Remove(estSell[0]);
                }
            }
            if (ShouldSellMyHouse(npc))
            {
                //第三步此时将多余的房与没有贷款的房全部卖掉了，剩下的为有贷款的房子，其中可能包括贷款在住房，贷款在开设企业房屋
            }
        }
    }
    #endregion
    #region npc同居
    public void LiveTogther(NpcBase npc)
    {    //调取npc为配偶的npc
        if (npc.relationships.Count > 0)
        {
            List<NpcRelationships> listspouse = npc.relationships.Values.ToList().Filter(r => r.personalRelations == Kinship.配偶);
            // 此处先会为所有npc匹配一处住房 或者一处房屋租住
            //所以只考虑npc持有或是租住即可
            //if (npc.estate.Count > 0 && listspouse[0].npcRelated.estate.Count > 0)
            //{

            //}
        }
    }
    #endregion
    #endregion
    #endregion
    #endregion

}