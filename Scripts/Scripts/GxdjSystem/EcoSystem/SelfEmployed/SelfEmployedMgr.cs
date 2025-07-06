using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SelfEmployedMgr : BaseManger<SelfEmployedMgr>
{
    #region 数据容器
    System.Random r = new();
    Dictionary<short, SelfEmployed> AllSelfEmploed;
    //键为自营企业的id
    Dictionary<short, SelfEmployedAd> CurrentSelfEmploed = new();
    #endregion
    #region 函数
    #region 构造函数
    private SelfEmployedMgr()
    {
        BinaryDataMgr.Instance.LoadTable<SelfEmployedContainer, SelfEmployed>();
        AllSelfEmploed = BinaryDataMgr.Instance.GetTable<SelfEmployedContainer>().dataDic;
    }
    #endregion
    #region 筛选
    #region 从自营企业池中获取需要的自营企业
    public SelfEmployedAd GetSelfEmployed(short selfEmdLev)
    {//获取所需等级的自营并从总字典中移除
        Dictionary<short, SelfEmployed> selfEmployeds = AllSelfEmploed.FilterValuesToDic(e => e.Value.rank == selfEmdLev);
        if (selfEmployeds.Count > 0)
        {
            var se = selfEmployeds.GetRandomItem();
            AllSelfEmploed.Remove(se.Key);
            return new SelfEmployedAd(se.Value);
        }
        return null;
    }
    #endregion
    #region 用于生成的自营企业获得
    public SelfEmployedAd GetMySelfEmployed(short estLev, bool hasEst, Estate myEst = null)
    {
        SelfEmployedAd myEmp = GetSelfEmployed(estLev);
        if (myEmp != null)
        {
            myEmp.establishDate = WorldClock.Instance.TotalTime; //设置创建时间
            if (!hasEst)
            {
                myEmp.selfEmEstate = EstateMgr.Instance.RentEstate(1, estLev);
                myEmp.estCost = myEmp.selfEmEstate.estRentPrice;
            }
            else myEmp.selfEmEstate = myEst;
            myEmp.selfEdFund = Wealth.Instance.GetLevFund(estLev);
            myEmp.monthlyIncome = Wealth.Instance.selfEmployedIncome(estLev);
            myEmp.officalCost = (int)(myEmp.monthlyIncome * r.NextFloat(0.05f, 0.02f));
            myEmp.huamanCost = myEmp.employeeNum * 5000;
            return myEmp;
        }
        else
        {
            Debug.Log("目前自营池不足等级为：" + estLev + "的自营企业");
            return null;
        }

    }
    #endregion
    #region 生成股权匹配
    public List<EquityStructure> EquityDistribution(NpcBase hoder, SelfEmployedAd selAd, Dictionary<short, NpcBase> allnpc)
    {
        NpcBase npcHoder;
        List<NpcBase> samLevNpc = allnpc.FilterValues(e => e.jobLevel == selAd.rank && e.Id != hoder.Id);
        samLevNpc.Add(hoder);
        List<EquityStructure> hoderList = new();
        EquityStructure eqHodlder = new EquityStructure()
        {
            Shareholder = hoder,
            ShareholdingRatio = 51,
            id = selAd.id
        };
        hoderList.Add(eqHodlder);
        for (int i = 0; i < 7; i++)
        {
            npcHoder = samLevNpc.GetRandomItem();
            if (hoderList.Filter(e => e.id == npcHoder.Id).Any())
            {
                hoderList.Filter(e => e.id == npcHoder.Id)[0].ShareholdingRatio += 7;
            }
            //
            else
            {
                EquityStructure eqOther = new EquityStructure()
                {
                    Shareholder = npcHoder,
                    ShareholdingRatio = 7,
                    id = selAd.id
                };
            }
        }
        hoderList.ForEach(e =>
        {
            e.Shareholder.equityStructures.Add(e);
        });
        return hoderList;

    }
    #endregion
    #endregion
    #region 行为
    #region 自营企业生成 世界生成
    public void GenerateSelfEmployed(Dictionary<short, NpcBase> allNpc)
    {
        int i = r.Next(1, 101);
        List<NpcBase> hasSelfEmployed = allNpc.FilterValues(e => e.jobLevel >= 3);
        hasSelfEmployed = hasSelfEmployed.Shuffle((int)(hasSelfEmployed.Count * 0.05f));
        hasSelfEmployed.ForEach((e) =>
        {
            switch (i)
            {
                case <= 50:
                    e.selfEmployed = GetMySelfEmployed((short)(e.jobLevel - 1), false);
                    break;
                case > 50:
                    e.selfEmployed = GetMySelfEmployed((short)(e.jobLevel - 2), false);
                    break;
            }
            e.selfEmployed.holderID = e.Id; // 设置持有人ID
            e.selfEmployed.equityStructure = EquityDistribution(e, e.selfEmployed, allNpc);
            CurrentSelfEmploed.Add(e.selfEmployed.id, e.selfEmployed);
        });
    }
    #endregion
    #region 世界演化 
    //#region 创立自营 判断与创立同步
    //public void CreatSelfEmployed(NpcBase npc)
    //{
    //    int currentLevSelfFund;
    //    int nextLevSelfFund;
    //    int safeMoney;
    //    if (r.Next(1, 101) <= 1)
    //    {
    //        currentLevSelfFund = Wealth.Instance.GetLevFund(npc.socialRank - 1);
    //        nextLevSelfFund = Wealth.Instance.GetLevFund(npc.socialRank - 2);
    //        safeMoney = Wealth.Instance.WealthMinAmount(npc.socialRank - 1);
    //        //社会等级大于三且无自营
    //        if (npc.socialRank >= 3 && npc.selfEmployed == null)
    //        {
    //            //判断有无资金创立 判断有无对应房产 判断有无朋友 三个参数 俩层判断
    //            //有朋友凑钱开企业
    //            if (npc.relationships.Values.ToList().Filter(e => e.friendship >= 30).Any())
    //            {
    //                List<NpcBase> myFriends = npc.relationships.Values.ToList().Filter(e => e.friendship >= 30).Select(e => e.npcRelated).ToList();
    //                //如果有能创立企业的房产 直接用 有创建一级企业的房产
    //                if (npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 1).Any())
    //                {
    //                    if (npc.money - currentLevSelfFund * 0.51f >= safeMoney)
    //                    {
    //                        //判断能否全款置办
    //                        if (npc.money - currentLevSelfFund >= safeMoney)
    //                        {  //判断能否全款置办
    //                           //简单判断下npc工资能否负担房租
    //                            if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 1).GetRandomItem().estRentPrice)
    //                            {
    //                                SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 1), true,
    //                                    npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 1)[0]);
    //                                npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 1)[0].estIsCpn = true;
    //                                if (myEmployed != null)
    //                                {
    //                                    npc.selfEmployed = myEmployed;
    //                                    npc.money -= currentLevSelfFund;
    //                                    //分配企业股权
    //                                    myEmployed.equityStructure.Add(new EquityStructure()
    //                                    {
    //                                        Shareholder = npc,
    //                                        ShareholdingRatio = 100,
    //                                        id = myEmployed.id
    //                                    });
    //                                    npc.equityStructures.Add(new EquityStructure()
    //                                    {
    //                                        Shareholder = npc,
    //                                        ShareholdingRatio = 100,
    //                                        id = myEmployed.id
    //                                    });
    //                                }
    //                                else
    //                                    Debug.Log($"此等级自营池不足{npc.socialRank - 1}");
    //                            }
    //                        }
    //                        //朋友来凑
    //                        else
    //                        {//如果工资能负担房租进行租
    //                            if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 1).GetRandomItem().estRentPrice)
    //                            {
    //                                int restMoney = currentLevSelfFund - (npc.money - safeMoney);
    //                                int npcPay = npc.money - safeMoney;
    //                                int frsCanAfford = myFriends.
    //                                    Where(e => e.socialRank > 0 && e.money > Wealth.Instance.WealthMinAmount(e.socialRank - 1))
    //                                    .Sum(e => e.money - Wealth.Instance.WealthMinAmount(e.socialRank - 1));
    //                                List<NpcBase> moenySafes = myFriends.Filter(e => e.money > Wealth.Instance.WealthMinAmount(e.socialRank - 1));
    //                                //能够负担则创立 钱多的先借
    //                                if (frsCanAfford >= restMoney)
    //                                {
    //                                    SelfEmployedAd mySelfemploed = GetMySelfEmployed((short)(npc.socialRank - 1), true,
    //                                        npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 1)[0]);
    //                                    npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 1)[0].estIsCpn = true;
    //                                    moenySafes.Sort((a, b) => a.money.CompareTo(b.money));
    //                                    //首先为npc分配股份
    //                                    if (mySelfemploed != null)
    //                                    {
    //                                        npc.selfEmployed = mySelfemploed;
    //                                        npc.money = safeMoney;
    //                                        mySelfemploed.equityStructure.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                        npc.equityStructures.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                        for (int i = 0; i < moenySafes.Count; i++)
    //                                        {
    //                                            if (restMoney <= 0)
    //                                                break;
    //                                            restMoney -= moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;
    //                                            moenySafes[i].money = Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;

    //                                            mySelfemploed.equityStructure.Add(new EquityStructure()
    //                                            {
    //                                                Shareholder = moenySafes[i],
    //                                                ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                id = mySelfemploed.id
    //                                            });
    //                                            moenySafes[i].equityStructures.Add(new EquityStructure()
    //                                            {
    //                                                Shareholder = moenySafes[i],
    //                                                ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                id = mySelfemploed.id
    //                                            });
    //                                        }
    //                                    }
    //                                    else
    //                                        Debug.Log($"自营池不足{npc.socialRank - 1}");
    //                                }
    //                                //进行下级企业判断
    //                                else
    //                                {//判断有无下级的房产
    //                                    if (npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2).Any())
    //                                    { //判断能否支付下级企业的百分之五十一
    //                                        if (npc.money - nextLevSelfFund * 0.51f >= safeMoney)
    //                                        {
    //                                            if (npc.money - nextLevSelfFund >= safeMoney)
    //                                            {  //判断能否全款置办
    //                                               //简单判断下npc工资能否负担房租
    //                                                if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                                {
    //                                                    SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 2), true,
    //                                                        npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0]);
    //                                                    npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0].estIsCpn = true;
    //                                                    if (myEmployed != null)
    //                                                    {
    //                                                        npc.selfEmployed = myEmployed;
    //                                                        npc.money -= nextLevSelfFund;
    //                                                        //分配企业股权
    //                                                        myEmployed.equityStructure.Add(new EquityStructure()
    //                                                        {
    //                                                            Shareholder = npc,
    //                                                            ShareholdingRatio = 100,
    //                                                            id = myEmployed.id
    //                                                        });
    //                                                        npc.equityStructures.Add(new EquityStructure()
    //                                                        {
    //                                                            Shareholder = npc,
    //                                                            ShareholdingRatio = 100,
    //                                                            id = myEmployed.id
    //                                                        });
    //                                                    }
    //                                                    else
    //                                                        Debug.Log($"此等级自营池不足{npc.socialRank - 2}");
    //                                                }
    //                                            }
    //                                            //不能全款买
    //                                            else
    //                                            {//如果工资能负担房租进行租
    //                                                if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                                {
    //                                                    //能够负担则创立 钱多的先借
    //                                                    if (frsCanAfford > restMoney)
    //                                                    {
    //                                                        SelfEmployedAd mySelfemploed = GetMySelfEmployed((short)(npc.socialRank - 2), true,
    //                                                            npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0]);
    //                                                        npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0].estIsCpn = true;
    //                                                        //首先为npc分配股份
    //                                                        if (mySelfemploed != null)
    //                                                        {
    //                                                            npc.selfEmployed = mySelfemploed;
    //                                                            npc.money = safeMoney;
    //                                                            mySelfemploed.equityStructure.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                            npc.equityStructures.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                            for (int i = 0; i < moenySafes.Count; i++)
    //                                                            {
    //                                                                if (restMoney <= 0)
    //                                                                    break;
    //                                                                restMoney -= moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;
    //                                                                moenySafes[i].money = Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;

    //                                                                mySelfemploed.equityStructure.Add(new EquityStructure()
    //                                                                {
    //                                                                    Shareholder = moenySafes[i],
    //                                                                    ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                                    id = mySelfemploed.id
    //                                                                });
    //                                                                moenySafes[i].equityStructures.Add(new EquityStructure()
    //                                                                {
    //                                                                    Shareholder = moenySafes[i],
    //                                                                    ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                                    id = mySelfemploed.id
    //                                                                });
    //                                                            }
    //                                                        }
    //                                                        else
    //                                                            Debug.Log($"自营池不足{npc.socialRank - 2}");
    //                                                    }
    //                                                    //朋友帮忙来处理声音部分
    //                                                }
    //                                            }
    //                                        }
    //                                    }
    //                                    //进行租房
    //                                    else
    //                                    {//判断能否支付下级企业的百分之五十一
    //                                        if (npc.money - nextLevSelfFund * 0.51f >= safeMoney)
    //                                        {
    //                                            if (npc.money - nextLevSelfFund >= safeMoney)
    //                                            {  //判断能否全款置办
    //                                               //简单判断下npc工资能否负担房租
    //                                                if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                                {
    //                                                    SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 2), false);
    //                                                    if (myEmployed != null)
    //                                                    {
    //                                                        npc.selfEmployed = myEmployed;
    //                                                        npc.money -= nextLevSelfFund;
    //                                                        //分配企业股权
    //                                                        myEmployed.equityStructure.Add(new EquityStructure()
    //                                                        {
    //                                                            Shareholder = npc,
    //                                                            ShareholdingRatio = 100,
    //                                                            id = myEmployed.id
    //                                                        });
    //                                                        npc.equityStructures.Add(new EquityStructure()
    //                                                        {
    //                                                            Shareholder = npc,
    //                                                            ShareholdingRatio = 100,
    //                                                            id = myEmployed.id
    //                                                        });
    //                                                    }
    //                                                    else
    //                                                        Debug.Log($"此等级自营池不足{npc.socialRank - 2}");
    //                                                }
    //                                            }
    //                                            //不能全款买
    //                                            else
    //                                            {//如果工资能负担房租进行租
    //                                                if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                                {
    //                                                    //能够负担则创立 钱多的先借
    //                                                    if (frsCanAfford > restMoney)
    //                                                    {
    //                                                        SelfEmployedAd mySelfemploed = GetMySelfEmployed((short)(npc.socialRank - 2), false);
    //                                                        moenySafes.Sort((a, b) => a.money.CompareTo(b.money));
    //                                                        //首先为npc分配股份
    //                                                        if (mySelfemploed != null)
    //                                                        {
    //                                                            npc.selfEmployed = mySelfemploed;
    //                                                            npc.money = safeMoney;
    //                                                            mySelfemploed.equityStructure.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                            npc.equityStructures.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                            for (int i = 0; i < moenySafes.Count; i++)
    //                                                            {
    //                                                                if (restMoney <= 0)
    //                                                                    break;
    //                                                                restMoney -= moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;
    //                                                                //此处判断朋友出钱为其安全线俩倍所有的钱 当朋友出资多余创立资金时候，可能会出现股权相加大于100
    //                                                                moenySafes[i].money = Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;

    //                                                                mySelfemploed.equityStructure.Add(new EquityStructure()
    //                                                                {
    //                                                                    Shareholder = moenySafes[i],
    //                                                                    ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                                    id = mySelfemploed.id
    //                                                                });
    //                                                                moenySafes[i].equityStructures.Add(new EquityStructure()
    //                                                                {
    //                                                                    Shareholder = moenySafes[i],
    //                                                                    ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                                    id = mySelfemploed.id
    //                                                                });
    //                                                            }
    //                                                        }
    //                                                        else
    //                                                            Debug.Log($"自营池不足{npc.socialRank - 2}");
    //                                                    }
    //                                                    //朋友帮忙来处理声音部分
    //                                                }
    //                                            }
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //                //没有能创立一级企业的房子
    //                else
    //                {//有先判断 有无51的资金
    //                    if (npc.money - currentLevSelfFund * 0.51f >= safeMoney)
    //                    {
    //                        //判断能否全款置办
    //                        if (npc.money - currentLevSelfFund >= safeMoney)
    //                        {  //判断能否全款置办
    //                           //简单判断下npc工资能否负担房租
    //                            if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 1).GetRandomItem().estRentPrice)
    //                            {
    //                                SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 1), false);
    //                                if (myEmployed != null)
    //                                {
    //                                    npc.selfEmployed = myEmployed;
    //                                    npc.money -= currentLevSelfFund;
    //                                    //分配企业股权
    //                                    myEmployed.equityStructure.Add(new EquityStructure()
    //                                    {
    //                                        Shareholder = npc,
    //                                        ShareholdingRatio = 100,
    //                                        id = myEmployed.id
    //                                    });
    //                                    npc.equityStructures.Add(new EquityStructure()
    //                                    {
    //                                        Shareholder = npc,
    //                                        ShareholdingRatio = 100,
    //                                        id = myEmployed.id
    //                                    });
    //                                }
    //                                else
    //                                    Debug.Log($"此等级自营池不足{npc.socialRank - 1}");
    //                            }
    //                        }
    //                        //朋友来凑
    //                        else
    //                        {//如果工资能负担房租进行租
    //                            if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 1).GetRandomItem().estRentPrice)
    //                            {
    //                                int restMoney = currentLevSelfFund - (npc.money - safeMoney);
    //                                int npcPay = npc.money - safeMoney;
    //                                int frsCanAfford = myFriends.
    //                                    Where(e => e.socialRank > 0 && e.money > Wealth.Instance.WealthMinAmount(e.socialRank - 1))
    //                                    .Sum(e => e.money - Wealth.Instance.WealthMinAmount(e.socialRank - 1));
    //                                List<NpcBase> moenySafes = myFriends.Filter(e => e.money > Wealth.Instance.WealthMinAmount(e.socialRank - 1));
    //                                //能够负担则创立 钱多的先借
    //                                if (frsCanAfford > restMoney)
    //                                {
    //                                    SelfEmployedAd mySelfemploed = GetMySelfEmployed((short)(npc.socialRank - 1), false);
    //                                    moenySafes.Sort((a, b) => a.money.CompareTo(b.money));
    //                                    //首先为npc分配股份
    //                                    if (mySelfemploed != null)
    //                                    {
    //                                        npc.selfEmployed = mySelfemploed;
    //                                        npc.money = safeMoney;
    //                                        mySelfemploed.equityStructure.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                        npc.equityStructures.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                        for (int i = 0; i < moenySafes.Count; i++)
    //                                        {
    //                                            if (restMoney <= 0)
    //                                                break;
    //                                            restMoney -= moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;
    //                                            //此处判断朋友出钱为其安全线俩倍所有的钱 当朋友出资多余创立资金时候，可能会出现股权相加大于100
    //                                            moenySafes[i].money = Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;

    //                                            mySelfemploed.equityStructure.Add(new EquityStructure()
    //                                            {
    //                                                Shareholder = moenySafes[i],
    //                                                ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                id = mySelfemploed.id
    //                                            });
    //                                            moenySafes[i].equityStructures.Add(new EquityStructure()
    //                                            {
    //                                                Shareholder = moenySafes[i],
    //                                                ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                id = mySelfemploed.id
    //                                            });
    //                                        }
    //                                    }
    //                                    else
    //                                        Debug.Log($"自营池不足{npc.socialRank - 1}");
    //                                }
    //                                //进行下级企业判断
    //                                else
    //                                {//判断有无下级的房产
    //                                    if (npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2).Any())
    //                                    { //判断能否支付下级企业的百分之五十一
    //                                        if (npc.money - nextLevSelfFund * 0.51f >= safeMoney)
    //                                        {
    //                                            if (npc.money - nextLevSelfFund >= safeMoney)
    //                                            {  //判断能否全款置办
    //                                               //简单判断下npc工资能否负担房租
    //                                                if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                                {
    //                                                    SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 2), true,
    //                                                        npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0]);
    //                                                    npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0].estIsCpn = true;
    //                                                    if (myEmployed != null)
    //                                                    {
    //                                                        npc.selfEmployed = myEmployed;
    //                                                        npc.money -= nextLevSelfFund;
    //                                                        //分配企业股权
    //                                                        myEmployed.equityStructure.Add(new EquityStructure()
    //                                                        {
    //                                                            Shareholder = npc,
    //                                                            ShareholdingRatio = 100,
    //                                                            id = myEmployed.id
    //                                                        });
    //                                                        npc.equityStructures.Add(new EquityStructure()
    //                                                        {
    //                                                            Shareholder = npc,
    //                                                            ShareholdingRatio = 100,
    //                                                            id = myEmployed.id
    //                                                        });
    //                                                    }
    //                                                    else
    //                                                        Debug.Log($"此等级自营池不足{npc.socialRank - 2}");
    //                                                }
    //                                            }
    //                                            //不能全款买
    //                                            else
    //                                            {//如果工资能负担房租进行租
    //                                                if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                                {
    //                                                    //能够负担则创立 钱多的先借
    //                                                    if (frsCanAfford > restMoney)
    //                                                    {
    //                                                        SelfEmployedAd mySelfemploed = GetMySelfEmployed((short)(npc.socialRank - 2), true,
    //                                                            npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0]);
    //                                                        npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0].estIsCpn = true;
    //                                                        moenySafes.Sort((a, b) => a.money.CompareTo(b.money));
    //                                                        //首先为npc分配股份
    //                                                        if (mySelfemploed != null)
    //                                                        {
    //                                                            npc.selfEmployed = mySelfemploed;
    //                                                            npc.money = safeMoney;
    //                                                            mySelfemploed.equityStructure.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                            npc.equityStructures.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                            for (int i = 0; i < moenySafes.Count; i++)
    //                                                            {
    //                                                                if (restMoney <= 0)
    //                                                                    break;
    //                                                                restMoney -= moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;
    //                                                                //此处判断朋友出钱为其安全线俩倍所有的钱 当朋友出资多余创立资金时候，可能会出现股权相加大于100
    //                                                                moenySafes[i].money = Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;

    //                                                                mySelfemploed.equityStructure.Add(new EquityStructure()
    //                                                                {
    //                                                                    Shareholder = moenySafes[i],
    //                                                                    ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                                    id = mySelfemploed.id
    //                                                                });
    //                                                                moenySafes[i].equityStructures.Add(new EquityStructure()
    //                                                                {
    //                                                                    Shareholder = moenySafes[i],
    //                                                                    ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                                    id = mySelfemploed.id
    //                                                                });
    //                                                            }
    //                                                        }
    //                                                        else
    //                                                            Debug.Log($"自营池不足{npc.socialRank - 2}");
    //                                                    }
    //                                                    //朋友帮忙来处理声音部分
    //                                                }
    //                                            }
    //                                        }
    //                                    }
    //                                    //进行租房
    //                                    else
    //                                    {//判断能否支付下级企业的百分之五十一
    //                                        if (npc.money - nextLevSelfFund * 0.51f >= safeMoney)
    //                                        {
    //                                            if (npc.money - nextLevSelfFund >= safeMoney)
    //                                            {  //判断能否全款置办
    //                                               //简单判断下npc工资能否负担房租
    //                                                if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                                {
    //                                                    SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 2), false);
    //                                                    if (myEmployed != null)
    //                                                    {
    //                                                        npc.selfEmployed = myEmployed;
    //                                                        npc.money -= nextLevSelfFund;
    //                                                        //分配企业股权
    //                                                        myEmployed.equityStructure.Add(new EquityStructure()
    //                                                        {
    //                                                            Shareholder = npc,
    //                                                            ShareholdingRatio = 100,
    //                                                            id = myEmployed.id
    //                                                        });
    //                                                        npc.equityStructures.Add(new EquityStructure()
    //                                                        {
    //                                                            Shareholder = npc,
    //                                                            ShareholdingRatio = 100,
    //                                                            id = myEmployed.id
    //                                                        });
    //                                                    }
    //                                                    else
    //                                                        Debug.Log($"此等级自营池不足{npc.socialRank - 2}");
    //                                                }
    //                                            }
    //                                            //不能全款买
    //                                            else
    //                                            {//如果工资能负担房租进行租
    //                                                if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                                {
    //                                                    //能够负担则创立 钱多的先借
    //                                                    if (frsCanAfford > restMoney)
    //                                                    {
    //                                                        SelfEmployedAd mySelfemploed = GetMySelfEmployed((short)(npc.socialRank - 2), false);
    //                                                        moenySafes.Sort((a, b) => a.money.CompareTo(b.money));
    //                                                        //首先为npc分配股份
    //                                                        if (mySelfemploed != null)
    //                                                        {
    //                                                            npc.selfEmployed = mySelfemploed;
    //                                                            npc.money = safeMoney;
    //                                                            mySelfemploed.equityStructure.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                            npc.equityStructures.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                            for (int i = 0; i < moenySafes.Count; i++)
    //                                                            {
    //                                                                if (restMoney <= 0)
    //                                                                    break;
    //                                                                restMoney -= moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;
    //                                                                //此处判断朋友出钱为其安全线俩倍所有的钱 当朋友出资多余创立资金时候，可能会出现股权相加大于100
    //                                                                moenySafes[i].money = Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;

    //                                                                mySelfemploed.equityStructure.Add(new EquityStructure()
    //                                                                {
    //                                                                    Shareholder = moenySafes[i],
    //                                                                    ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                                    id = mySelfemploed.id
    //                                                                });
    //                                                                moenySafes[i].equityStructures.Add(new EquityStructure()
    //                                                                {
    //                                                                    Shareholder = moenySafes[i],
    //                                                                    ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                                    id = mySelfemploed.id
    //                                                                });
    //                                                            }
    //                                                        }
    //                                                        else
    //                                                            Debug.Log($"自营池不足{npc.socialRank - 2}");
    //                                                    }
    //                                                    //朋友帮忙来处理声音部分
    //                                                }
    //                                            }
    //                                        }
    //                                    }
    //                                }

    //                            }
    //                        }

    //                    }
    //                    //开始创建下级企业
    //                    else
    //                    {//判断有无下级的房产
    //                        if (npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2).Any())
    //                        { //判断能否支付下级企业的百分之五十一
    //                            if (npc.money - nextLevSelfFund * 0.51f >= safeMoney)
    //                            {
    //                                if (npc.money - nextLevSelfFund >= safeMoney)
    //                                {  //判断能否全款置办
    //                                   //简单判断下npc工资能否负担房租
    //                                    if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                    {
    //                                        SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 2), true,
    //                                            npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0]);
    //                                        npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0].estIsCpn = true;
    //                                        if (myEmployed != null)
    //                                        {
    //                                            npc.selfEmployed = myEmployed;
    //                                            npc.money -= nextLevSelfFund;
    //                                            //分配企业股权
    //                                            myEmployed.equityStructure.Add(new EquityStructure()
    //                                            {
    //                                                Shareholder = npc,
    //                                                ShareholdingRatio = 100,
    //                                                id = myEmployed.id
    //                                            });
    //                                            npc.equityStructures.Add(new EquityStructure()
    //                                            {
    //                                                Shareholder = npc,
    //                                                ShareholdingRatio = 100,
    //                                                id = myEmployed.id
    //                                            });
    //                                        }
    //                                        else
    //                                            Debug.Log($"此等级自营池不足{npc.socialRank - 2}");
    //                                    }
    //                                }
    //                                //不能全款买
    //                                else
    //                                {//如果工资能负担房租进行租
    //                                    if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                    {
    //                                        int restMoney = nextLevSelfFund - (npc.money - safeMoney);
    //                                        int npcPay = npc.money - safeMoney;
    //                                        int frsCanAfford = myFriends.
    //                                            Where(e => e.socialRank > 0 && e.money > Wealth.Instance.WealthMinAmount(e.socialRank - 1))
    //                                            .Sum(e => e.money - Wealth.Instance.WealthMinAmount(e.socialRank - 1) );
    //                                        List<NpcBase> moenySafes = myFriends.Filter(e => e.money > Wealth.Instance.WealthMinAmount(e.socialRank - 1));
    //                                        //能够负担则创立 钱多的先借
    //                                        if (frsCanAfford > restMoney)
    //                                        {
    //                                            SelfEmployedAd mySelfemploed = GetMySelfEmployed((short)(npc.socialRank - 2), true,
    //                                                npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0]);
    //                                            npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0].estIsCpn = true;
    //                                            moenySafes.Sort((a, b) => a.money.CompareTo(b.money));
    //                                            //首先为npc分配股份
    //                                            if (mySelfemploed != null)
    //                                            {
    //                                                npc.selfEmployed = mySelfemploed;
    //                                                npc.money = safeMoney;
    //                                                mySelfemploed.equityStructure.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                npc.equityStructures.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                for (int i = 0; i < moenySafes.Count; i++)
    //                                                {
    //                                                    if (restMoney <= 0)
    //                                                        break;
    //                                                    restMoney -= moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;
    //                                                    //此处判断朋友出钱为其安全线俩倍所有的钱 当朋友出资多余创立资金时候，可能会出现股权相加大于100
    //                                                    moenySafes[i].money = Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;

    //                                                    mySelfemploed.equityStructure.Add(new EquityStructure()
    //                                                    {
    //                                                        Shareholder = moenySafes[i],
    //                                                        ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                        id = mySelfemploed.id
    //                                                    });
    //                                                    moenySafes[i].equityStructures.Add(new EquityStructure()
    //                                                    {
    //                                                        Shareholder = moenySafes[i],
    //                                                        ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                        id = mySelfemploed.id
    //                                                    });
    //                                                }
    //                                            }
    //                                            else
    //                                                Debug.Log($"自营池不足{npc.socialRank - 2}");
    //                                        }
    //                                        //朋友帮忙来处理声音部分
    //                                    }
    //                                }
    //                            }
    //                        }
    //                        //进行租房
    //                        else
    //                        {//判断能否支付下级企业的百分之五十一
    //                            if (npc.money - nextLevSelfFund * 0.51f >= safeMoney)
    //                            {
    //                                if (npc.money - nextLevSelfFund >= safeMoney)
    //                                {  //判断能否全款置办
    //                                   //简单判断下npc工资能否负担房租
    //                                    if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                    {
    //                                        SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 2), false);
    //                                        if (myEmployed != null)
    //                                        {
    //                                            npc.selfEmployed = myEmployed;
    //                                            npc.money -= nextLevSelfFund;
    //                                            //分配企业股权
    //                                            myEmployed.equityStructure.Add(new EquityStructure()
    //                                            {
    //                                                Shareholder = npc,
    //                                                ShareholdingRatio = 100,
    //                                                id = myEmployed.id
    //                                            });
    //                                            npc.equityStructures.Add(new EquityStructure()
    //                                            {
    //                                                Shareholder = npc,
    //                                                ShareholdingRatio = 100,
    //                                                id = myEmployed.id
    //                                            });
    //                                        }
    //                                        else
    //                                            Debug.Log($"此等级自营池不足{npc.socialRank - 2}");
    //                                    }
    //                                }
    //                                //不能全款买
    //                                else
    //                                {//如果工资能负担房租进行租
    //                                    if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                    {
    //                                        int restMoney = nextLevSelfFund - (npc.money - safeMoney);
    //                                        int npcPay = npc.money - safeMoney;
    //                                        int frsCanAfford = myFriends.
    //                                            Where(e => e.socialRank > 0 && e.money > Wealth.Instance.WealthMinAmount(e.socialRank - 1) )
    //                                            .Sum(e => e.money - Wealth.Instance.WealthMinAmount(e.socialRank - 1) );
    //                                        List<NpcBase> moenySafes = myFriends.Filter(e => e.money > Wealth.Instance.WealthMinAmount(e.socialRank - 1));
    //                                        //能够负担则创立 钱多的先借
    //                                        if (frsCanAfford > restMoney)
    //                                        {
    //                                            SelfEmployedAd mySelfemploed = GetMySelfEmployed((short)(npc.socialRank - 2), false);
    //                                            moenySafes.Sort((a, b) => a.money.CompareTo(b.money));
    //                                            //首先为npc分配股份
    //                                            if (mySelfemploed != null)
    //                                            {
    //                                                npc.selfEmployed = mySelfemploed;
    //                                                npc.money = safeMoney;
    //                                                mySelfemploed.equityStructure.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                npc.equityStructures.Add(new EquityStructure() { Shareholder = npc, ShareholdingRatio = npcPay * 100 / nextLevSelfFund, id = mySelfemploed.id });
    //                                                for (int i = 0; i < moenySafes.Count; i++)
    //                                                {
    //                                                    if (restMoney <= 0)
    //                                                        break;
    //                                                    restMoney -= moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;
    //                                                    //此处判断朋友出钱为其安全线俩倍所有的钱 当朋友出资多余创立资金时候，可能会出现股权相加大于100
    //                                                    moenySafes[i].money = Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2;

    //                                                    mySelfemploed.equityStructure.Add(new EquityStructure()
    //                                                    {
    //                                                        Shareholder = moenySafes[i],
    //                                                        ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                        id = mySelfemploed.id
    //                                                    });
    //                                                    moenySafes[i].equityStructures.Add(new EquityStructure()
    //                                                    {
    //                                                        Shareholder = moenySafes[i],
    //                                                        ShareholdingRatio = (moenySafes[i].money - Wealth.Instance.WealthMinAmount(moenySafes[i].socialRank - 1) * 2) * 100 / nextLevSelfFund,
    //                                                        id = mySelfemploed.id
    //                                                    });
    //                                                }
    //                                            }
    //                                            else
    //                                                Debug.Log($"自营池不足{npc.socialRank - 2}");
    //                                        }
    //                                        //朋友帮忙来处理声音部分
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            //没有朋友
    //            else
    //            {//有一级房产
    //                if (npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 1).Any())
    //                {
    //                    if (npc.money - currentLevSelfFund >= safeMoney)
    //                    {
    //                        //判断能否全款置办
    //                        npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 1)[0].estIsCpn = true;
    //                        SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 1), true,
    //                           npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 1)[0]);
    //                        if (myEmployed != null)
    //                        {
    //                            npc.selfEmployed = myEmployed;
    //                            npc.money -= currentLevSelfFund;
    //                            //分配企业股权
    //                            myEmployed.equityStructure.Add(new EquityStructure()
    //                            {
    //                                Shareholder = npc,
    //                                ShareholdingRatio = 100,
    //                                id = myEmployed.id
    //                            });
    //                            npc.equityStructures.Add(new EquityStructure()
    //                            {
    //                                Shareholder = npc,
    //                                ShareholdingRatio = 100,
    //                                id = myEmployed.id
    //                            });
    //                        }

    //                        else
    //                            Debug.Log($"目前的自营池不足{npc.socialRank - 1}");
    //                    }
    //                    //判断npc是否有足够的资金创立下级企业
    //                    else
    //                    {//有下级房产
    //                        if (npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2).Any())
    //                        {
    //                            if (npc.money - nextLevSelfFund >= safeMoney)
    //                            {
    //                                SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 2), true,
    //                                    npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0]);
    //                                npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0].estIsCpn = true;
    //                                if (myEmployed != null)
    //                                {
    //                                    npc.selfEmployed = myEmployed;
    //                                    npc.money -= nextLevSelfFund;
    //                                    //分配企业股权
    //                                    myEmployed.equityStructure.Add(new EquityStructure()
    //                                    {
    //                                        Shareholder = npc,
    //                                        ShareholdingRatio = 100,
    //                                        id = myEmployed.id
    //                                    });
    //                                    npc.equityStructures.Add(new EquityStructure()
    //                                    {
    //                                        Shareholder = npc,
    //                                        ShareholdingRatio = 100,
    //                                        id = myEmployed.id
    //                                    });
    //                                }
    //                                else
    //                                    Debug.Log($"此等级自营池不足{npc.socialRank - 2}");
    //                            }
    //                        }
    //                        //没有下级房产
    //                        else
    //                        {
    //                            if (npc.money - nextLevSelfFund >= safeMoney)
    //                            {
    //                                //简单判断下npc工资能否负担房租
    //                                if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                {
    //                                    SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 2), false);
    //                                    if (myEmployed != null)
    //                                    {
    //                                        npc.selfEmployed = myEmployed;
    //                                        npc.money -= nextLevSelfFund;
    //                                        //分配企业股权
    //                                        myEmployed.equityStructure.Add(new EquityStructure()
    //                                        {
    //                                            Shareholder = npc,
    //                                            ShareholdingRatio = 100,
    //                                            id = myEmployed.id
    //                                        });
    //                                        npc.equityStructures.Add(new EquityStructure()
    //                                        {
    //                                            Shareholder = npc,
    //                                            ShareholdingRatio = 100,
    //                                            id = myEmployed.id
    //                                        });
    //                                    }
    //                                    else
    //                                        Debug.Log($"此等级自营池不足{npc.socialRank - 2}");
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //                //无一级房产
    //                else
    //                {
    //                    if (npc.money - currentLevSelfFund >= safeMoney)
    //                    {
    //                        //判断能否全款置办
    //                        //简单判断下npc工资能否负担房租
    //                        if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 1).GetRandomItem().estRentPrice)
    //                        {
    //                            SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 1), false);
    //                            if (myEmployed != null)
    //                            {
    //                                npc.selfEmployed = myEmployed;
    //                                npc.money -= nextLevSelfFund;
    //                                //分配企业股权
    //                                myEmployed.equityStructure.Add(new EquityStructure()
    //                                {
    //                                    Shareholder = npc,
    //                                    ShareholdingRatio = 100,
    //                                    id = myEmployed.id
    //                                });
    //                                npc.equityStructures.Add(new EquityStructure()
    //                                {
    //                                    Shareholder = npc,
    //                                    ShareholdingRatio = 100,
    //                                    id = myEmployed.id
    //                                });
    //                            }
    //                            else
    //                                Debug.Log($"目前的自营池不足{npc.socialRank - 1}");
    //                        }
    //                    }
    //                    //判断npc是否有足够的资金创立下级企业
    //                    //判断有无下级房产
    //                    else
    //                    {//有下级房产
    //                        if (npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2).Any())
    //                        {
    //                            if (npc.money - nextLevSelfFund >= safeMoney)
    //                            {
    //                                SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 2), true,
    //                                    npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0]);
    //                                npc.estate.Filter(e => e.estIsHold == true && e.estIsCpn == false && e.estType == 1 && e.estRank == npc.socialRank - 2)[0].estIsCpn = true;
    //                                if (myEmployed != null)
    //                                {
    //                                    npc.selfEmployed = myEmployed;
    //                                    npc.money -= nextLevSelfFund;
    //                                    //分配企业股权
    //                                    myEmployed.equityStructure.Add(new EquityStructure()
    //                                    {
    //                                        Shareholder = npc,
    //                                        ShareholdingRatio = 100,
    //                                        id = myEmployed.id
    //                                    });
    //                                    npc.equityStructures.Add(new EquityStructure()
    //                                    {
    //                                        Shareholder = npc,
    //                                        ShareholdingRatio = 100,
    //                                        id = myEmployed.id
    //                                    });
    //                                }
    //                                else
    //                                    Debug.Log($"此等级自营池不足{npc.socialRank - 2}");
    //                            }
    //                        }
    //                        //没有下级房产
    //                        else
    //                        {
    //                            if (npc.money - nextLevSelfFund >= safeMoney)
    //                            {
    //                                //简单判断下npc工资能否负担房租
    //                                if (npc.basicSalary > EstateMgr.Instance.GetMatchEstate(1, npc.socialRank - 2).GetRandomItem().estRentPrice)
    //                                {
    //                                    SelfEmployedAd myEmployed = GetMySelfEmployed((short)(npc.socialRank - 2), false);
    //                                    if (myEmployed != null)
    //                                    {
    //                                        npc.selfEmployed = myEmployed;
    //                                        npc.money -= nextLevSelfFund;
    //                                        //分配企业股权
    //                                        myEmployed.equityStructure.Add(new EquityStructure()
    //                                        {
    //                                            Shareholder = npc,
    //                                            ShareholdingRatio = 100,
    //                                            id = myEmployed.id
    //                                        });
    //                                        npc.equityStructures.Add(new EquityStructure()
    //                                        {
    //                                            Shareholder = npc,
    //                                            ShareholdingRatio = 100,
    //                                            id = myEmployed.id
    //                                        });
    //                                    }
    //                                    else
    //                                        Debug.Log($"此等级自营池不足{npc.socialRank - 2}");
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            //再次判断npc是否创立自营 若创建 加入到时间自营中
    //            if (npc.selfEmployed != null)
    //            {
    //                npc.selfEmployed.holderID = npc.Id;
    //                CurrentSelfEmploed.Add(npc.selfEmployed.id, npc.selfEmployed);
    //            }
    //        }
    //    }


    //}
    //#endregion
    #region 自营盈利
    public void SelfEmploedProfi(SelfEmployedAd mySelfemploed)
    {
        mySelfemploed.setTime = WorldClock.Instance.TotalTime - mySelfemploed.establishDate;
        if (mySelfemploed.setTime >= 36)
        {
            Upgrade(mySelfemploed);
        }

        mySelfemploed.selfEdFund += r.NextFloat(-0.5f, 1.6f) * mySelfemploed.monthlyIncome -
                (mySelfemploed.huamanCost + mySelfemploed.estCost + mySelfemploed.officalCost);
        if (mySelfemploed.selfEdFund <= 0)
        {
            SelfemploedBreak(mySelfemploed);
        }


    }
    #endregion
    #region 自营分红 (过年触发)
    public void Dividend​​()
    {
        int setFund;
        foreach (var worldSelfEmploed in CurrentSelfEmploed.Values)
        {
            setFund = Wealth.Instance.GetLevFund(worldSelfEmploed.rank);
            if (worldSelfEmploed.selfEdFund > setFund)
                worldSelfEmploed.equityStructure.ForEach(e => { e.Shareholder.money += (int)((e.ShareholdingRatio / 100) * (worldSelfEmploed.selfEdFund - setFund)); });
        }
    }
    #endregion
    #region 自营升级
    public void Upgrade(SelfEmployedAd selfEmployedAd)
    {
        if (selfEmployedAd.rank < 5)
        {
            if (r.Next(1, 101) > 3) return;
            else
            {
                //判断持有人资金能否超过
                NpcBase hoder = WorldSceneMgr.Instance.worldAllNpc[selfEmployedAd.holderID];
            }
        }

    }
    #endregion
    #region 自营破产
    private void SelfemploedBreak(SelfEmployedAd selfEmployedAd)
    {
        //改变企业房屋状态
        if (selfEmployedAd.selfEmEstate.estIsRent = false)
            selfEmployedAd.selfEmEstate.estIsHold = false;
        selfEmployedAd.selfEmEstate.estIsCpn = false;
        selfEmployedAd.selfEmEstate.estIsRent = false;
        //清理股权
        if (selfEmployedAd.equityStructure.Any())
        {
            //selfEmployedAd.equityStructure.ForEach(e => e.Shareholder.equityStructures?.RemoveAll(e => e.id == selfEmployedAd.id));
            List<NpcBase> hodes = selfEmployedAd.equityStructure.Select(e => e.Shareholder).ToList();
            if (hodes.Any())
            {
                hodes.ForEach(e =>
                {
                    if (e.equityStructures.Any())
                    {
                        if (e.equityStructures.Filter(e => e.id == selfEmployedAd.id).Any())
                        {
                            e.equityStructures.RemoveAll(e => e.id == selfEmployedAd.id);
                        }
                    }
                });
            }
            selfEmployedAd.equityStructure.Clear();
        }
        else
        {
            Debug.Log("当前自营未分配股权");
        }
        //从当前执行企业移除
        CurrentSelfEmploed.Remove(selfEmployedAd.id);
        WorldSceneMgr.Instance.worldAllNpc[selfEmployedAd.holderID].selfEmployed = null;
        selfEmployedAd = null;
        Debug.Log("有自营企业破产");
        Debug.Log($"破产后世界自营数为{CurrentSelfEmploed.Count}");
    }
    #endregion
    #endregion
    #endregion
    #endregion
}
