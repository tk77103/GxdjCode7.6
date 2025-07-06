using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DebtItem {

    public Estate contractEst;
    public int Money;
}

public class DebtContract
{//债主
    public int creditors;
    //负债人
    public short debtor;
    //债务主体
    public DebtItem debtItem;
    //债务开始时间
    public long startTime;
    //债务结束时间
    public long endTime;
    //逾期时间
    public short overdueTime;
    //债务金额
    public float debtAmount;
    //已还
    public float areadyPay;
    //负债类型 0 每期还 1 一次性还
    public short payType;
    //债务状态 0 未还 1 已还 2 逾期未还 3 逾期已还
    public short debtStatus;
    //每期还款金额
    public float perRepay; 
    //债务利率
    public float interestRate;
    //债务逾期利率
    public float overdueInterestRate;
    //债务类型 0 房产 1 人物直接借贷 2 企业借贷
    public short debtType;
}
public class WorldBank : BaseManger<WorldBank>
{   //世界银行ID 目前世界银行只虚拟 不做实际存储钱功能，未来可能拓展
    private int ID = 771103;
    //债务ID
    private long contractID=0;
    Dictionary<long, DebtContract> worldContract=new();
    private WorldBank()
    {
    }
    public long AddContract(DebtContract newDebtContract)
    {
        contractID++;
        worldContract.Add(contractID, newDebtContract);
        return contractID;
    }
    public void RemoveContract(long contractID)
    {
       worldContract.Remove(contractID);
    }
    public void PayDebt()
    {
        List<long> romoveID = new();
        foreach (var id in worldContract.Keys)
        {//每期还款
            if (WorldSceneMgr.Instance.worldAllNpc[worldContract[id].debtor].money < worldContract[id].perRepay)
            { Debug.Log($"债务人{worldContract[id].debtor}已无力还款");
                worldContract[id].overdueTime++;
                //逾期未还6月
                if (worldContract[id].overdueTime >= 6) 
                {
                    if (worldContract[id].debtItem.contractEst != null)
                    {
                        //npc房产移除
                        WorldSceneMgr.Instance.worldAllNpc[worldContract[id].debtor].estate.Remove(worldContract[id].debtItem.contractEst);
                        //npc债务移除
                        WorldSceneMgr.Instance.worldAllNpc[worldContract[id].debtor].debts.Remove(id);
                        //将房产变卖返回npc已经支付金额 将房子卖了还剩余负债还有余钱还给npc
                        WorldSceneMgr.Instance.worldAllNpc[worldContract[id].debtor].money += (int)(worldContract[id].debtItem.contractEst.estSellPrice -
                            (worldContract[id].debtAmount - worldContract[id].areadyPay));
                        Debug.Log($"已将债务人{worldContract[id].debtor}的负债房屋抵债");
                        //在时间银行中记录并移除合约
                        romoveID.Add(id);
                    }
                }
            }
            //正常还
            else { WorldSceneMgr.Instance.worldAllNpc[worldContract[id].debtor].money -= (int)worldContract[id].perRepay;
                worldContract[id].areadyPay = worldContract[id].perRepay;
                worldContract[id].overdueTime = 0;
            }
            if (worldContract[id].areadyPay >= worldContract[id].debtAmount)
                romoveID.Add(id);
        }
        foreach (var id in romoveID)
        {
            WorldSceneMgr.Instance.worldAllNpc[worldContract[id].debtor].debts.Remove(id);
            RemoveContract(id);
        }
    }

    #region 获取合约目前剩余金额
    public float GetDebtAmount(long contractID)
    {
        return worldContract[contractID].debtAmount;
    }
    #endregion
}
