using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DebtItem {

    public Estate contractEst;
    public int Money;
}

public class DebtContract
{//ծ��
    public int creditors;
    //��ծ��
    public short debtor;
    //ծ������
    public DebtItem debtItem;
    //ծ��ʼʱ��
    public long startTime;
    //ծ�����ʱ��
    public long endTime;
    //����ʱ��
    public short overdueTime;
    //ծ����
    public float debtAmount;
    //�ѻ�
    public float areadyPay;
    //��ծ���� 0 ÿ�ڻ� 1 һ���Ի�
    public short payType;
    //ծ��״̬ 0 δ�� 1 �ѻ� 2 ����δ�� 3 �����ѻ�
    public short debtStatus;
    //ÿ�ڻ�����
    public float perRepay; 
    //ծ������
    public float interestRate;
    //ծ����������
    public float overdueInterestRate;
    //ծ������ 0 ���� 1 ����ֱ�ӽ�� 2 ��ҵ���
    public short debtType;
}
public class WorldBank : BaseManger<WorldBank>
{   //��������ID Ŀǰ��������ֻ���� ����ʵ�ʴ洢Ǯ���ܣ�δ��������չ
    private int ID = 771103;
    //ծ��ID
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
        {//ÿ�ڻ���
            if (WorldSceneMgr.Instance.worldAllNpc[worldContract[id].debtor].money < worldContract[id].perRepay)
            { Debug.Log($"ծ����{worldContract[id].debtor}����������");
                worldContract[id].overdueTime++;
                //����δ��6��
                if (worldContract[id].overdueTime >= 6) 
                {
                    if (worldContract[id].debtItem.contractEst != null)
                    {
                        //npc�����Ƴ�
                        WorldSceneMgr.Instance.worldAllNpc[worldContract[id].debtor].estate.Remove(worldContract[id].debtItem.contractEst);
                        //npcծ���Ƴ�
                        WorldSceneMgr.Instance.worldAllNpc[worldContract[id].debtor].debts.Remove(id);
                        //��������������npc�Ѿ�֧����� ���������˻�ʣ�ฺծ������Ǯ����npc
                        WorldSceneMgr.Instance.worldAllNpc[worldContract[id].debtor].money += (int)(worldContract[id].debtItem.contractEst.estSellPrice -
                            (worldContract[id].debtAmount - worldContract[id].areadyPay));
                        Debug.Log($"�ѽ�ծ����{worldContract[id].debtor}�ĸ�ծ���ݵ�ծ");
                        //��ʱ�������м�¼���Ƴ���Լ
                        romoveID.Add(id);
                    }
                }
            }
            //������
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

    #region ��ȡ��ԼĿǰʣ����
    public float GetDebtAmount(long contractID)
    {
        return worldContract[contractID].debtAmount;
    }
    #endregion
}
