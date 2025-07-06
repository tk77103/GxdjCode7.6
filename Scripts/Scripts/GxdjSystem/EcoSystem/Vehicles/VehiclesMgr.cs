using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VehiclesMgr : BaseManger<VehiclesMgr>
{
    #region 数据容器
    private Dictionary<int, Vehicles> vc ;
    System.Random r = new System.Random();
    #endregion
    #region 函数
    #region 构造函数
    private VehiclesMgr() {
    BinaryDataMgr.Instance.LoadTable<VehiclesContainer, Vehicles>();
        vc = BinaryDataMgr.Instance.GetTable<VehiclesContainer>().dataDic;
    }
    #endregion
    #region 筛选
    #region 筛选车辆
    public List<Vehicles> FilterVehicle(short vcLev)
    {
        return vc.FilterValues(e => e.level == vcLev);
    }
    public Vehicles GetRandomVc(List<Vehicles> vc)
    {
        return vc[r.Next(0, vc.Count)];
    }
    #endregion
    #region  车子是否匹配
    public bool IsYourCarMatch(NpcBase npc)
    {
        if (npc.jobLevel == 0) return true;
        else
        {
            if (npc.vehicles.Any())
            {
                if (npc.vehicles.MaxBy(e => e.level).level >= npc.socialRank) return true;
                else return false;
            }
            else
                return false;
        }
    }
    #endregion
    #endregion
    #region 行为
    #region 初始世界为npc生成车辆
    public Vehicles GetNpcVehicles(short jobLev)
    {
        int i = r.Next(1, 101);
        switch (jobLev)
        {
            case 0:
                if (i <= 70)
                    return null;
                else if (i > 70 && i <= 90)
                    return GetRandomVc(FilterVehicle(1));
                else return GetRandomVc(FilterVehicle(2));
            case 1:
                if (i <= 50)
                    return null;
                else if (i > 50 && i <= 80)
                    return GetRandomVc(FilterVehicle(1));
                else return GetRandomVc(FilterVehicle(2));
            case 2:
                if (i <= 30)
                    return null;
                else if (i > 30 && i <= 50)
                    return GetRandomVc(FilterVehicle(1));
                else if (i > 50 && i <= 90)
                    return GetRandomVc(FilterVehicle(2));
                else return GetRandomVc(FilterVehicle(3));
            case 3:
                if (i <= 50)
                    return GetRandomVc(FilterVehicle(2));
                else if (i > 50 && i <= 80)
                    return GetRandomVc(FilterVehicle(3));
                else return GetRandomVc(FilterVehicle(4));
            case 4:
                if (i <= 50)
                    return GetRandomVc(FilterVehicle(3));
                else if (i > 50 && i <= 95)
                    return GetRandomVc(FilterVehicle(4));
                else return GetRandomVc(FilterVehicle(5));
            case 5:
                if (i <= 50)
                    return GetRandomVc(FilterVehicle(5));
                else return GetRandomVc(FilterVehicle(4));
        }
        return null;
    }
    #region 第一个月的车辆适配
    /// <summary>
    /// 不匹配为npc来匹配车
    /// </summary>
    /// <param name="npc"></param>
    /// <param name="minLevWelath">最低财富等级限度</param>
    public void ForMatchCar(NpcBase npc, int minLevWelath)
    {//有车进行置换
        List<Vehicles> sameLevVes = vc.FilterValues(e => e.level == npc.jobLevel);
        //此处车价从低到高给npc省点米
        sameLevVes?.Sort((a, b) => a.price.CompareTo(b.price));
        if (npc.vehicles.Any())
        {
            for (int i = 0; i < sameLevVes.Count; i++)
            {
                if (npc.money - sameLevVes[i].price >= minLevWelath)
                {
                    npc.vehicles.Add(sameLevVes[i]);
                    npc.money -= sameLevVes[i].price;
                    break;
                }
                //获取npc最贵车辆置换
                else
                {
                    Vehicles myBestVehicle = npc.vehicles.MaxBy(e => e.price);
                    if (npc.money + myBestVehicle.price - sameLevVes[i].price >= minLevWelath)
                    {
                        npc.money += myBestVehicle.price;
                        npc.money -= sameLevVes[i].price;
                        npc.vehicles.Remove(myBestVehicle);
                        npc.vehicles.Add(sameLevVes[i]);
                        break;
                    }
                    else break;
                }
            }
        }
        else
        {
            for (int i = 0; i < sameLevVes.Count; i++)
            {
                if (npc.money - sameLevVes[i].price >= minLevWelath)
                {
                    npc.money -= sameLevVes[i].price;
                    npc.vehicles.Add(sameLevVes[i]);
                    break;
                }
                else break;
            }
        }
    }
    #endregion
    #endregion
    #region 购买车辆 不匹配时购买
    public void BuyVehicle(NpcBase npc)
    {
        if (!IsYourCarMatch(npc))
        {
            int safeMoney = Wealth.Instance.WealthMinAmount(npc.socialRank - 1);
            //获取当前等级的车辆
            List<Vehicles> FilterVehicle = vc.FilterValues(e => e.level == npc.socialRank);
            if (FilterVehicle.Any()) { Vehicles myVehicle = GetRandomVc(FilterVehicle);
                if (myVehicle != null)
                {
                    if (npc.vehicles.Any())
                    {
                        //判断能否全款买
                        if (npc.money - myVehicle.price >= safeMoney)
                        {
                            npc.vehicles.Add(myVehicle);
                            npc.money -= myVehicle.price;
                        }
                        else
                        {
                            int maxPrice = npc.vehicles.MaxBy(e => e.price).price;
                            if (npc.money + maxPrice - myVehicle.price >= safeMoney)
                            {
                                //获取最贵的车进行置换
                                Vehicles myBestVehicle = npc.vehicles.MaxBy(e => e.price);
                                npc.money += myBestVehicle.price;
                                npc.money -= myVehicle.price;
                                npc.vehicles.Remove(myBestVehicle);
                                npc.vehicles.Add(myVehicle);
                            }
                        }
                    }
                    //无车判断能否全款买
                    else
                    {
                        if (npc.money - myVehicle.price >= safeMoney)
                        {
                            npc.vehicles.Add(myVehicle);
                            npc.money -= myVehicle.price;
                        }
                    }
                }
            }
            else  Debug.Log($"无当前等级车{npc.socialRank}"); 
            
        }
    }
    #endregion
    #endregion
    #endregion
}
