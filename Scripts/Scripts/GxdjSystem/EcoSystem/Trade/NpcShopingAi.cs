using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NpcShopingAi : BaseManger<NpcShopingAi>
{

    private Dictionary<int, Item> totalItem;
    private List<int> gifts;
    private System.Random r = new System.Random();
    private NpcShopingAi()
    {
        BinaryDataMgr.Instance.LoadTable<ItemContainer, Item>();

        totalItem = BinaryDataMgr.Instance.GetTable<ItemContainer>().dataDic;
        gifts = totalItem.Values.Where(e => e.type == "�ղ�Ʒ").Select(e => e.id).ToList();

    }
    #region npc����Ai(�˴���ʱ���ڹ��򴦿��Բ�һ��)
    //�����Ʒ������ �����㱾�ȼ�������ʱֹͣ����
    public void ShopingAi(NpcBase npc, bool isFirstShoping)
    {
        int safeMoney = Wealth.Instance.WealthMinAmount(npc.socialRank - 1);
        List<int> giftSameLev = gifts.Where(e => totalItem[e].level == (npc.socialRank==0?1:npc.socialRank)).ToList();
        List<int> giftLowLev = gifts.Where(e => totalItem[e].level == ((npc.socialRank-1)<=0?1: npc.socialRank - 1)).ToList();
        int key = 0;
        int z = r.Next(1, 3); ;
        if (npc.money > safeMoney)
        {
            if (isFirstShoping)
            {
                for (int i = 0; i < 6; i++)
                {
                    switch (z)
                    {
                        case 1:key = giftSameLev.GetRandomItem();
                            if (npc.money - totalItem[key].price > safeMoney)
                            {
                                if (!npc.items.Keys.Contains(key))
                                    npc.items.Add(key, totalItem[key]);
                                else npc.items[key].Num++;
                            }
                            else
                            {
                                key= giftLowLev.GetRandomItem();
                                if (npc.money - totalItem[key].price > safeMoney)
                                {
                                    if (!npc.items.Keys.Contains(key))
                                        npc.items.Add(key, totalItem[key]);
                                    else npc.items[key].Num++;
                                }
                                else return;
                            }
                            break;
                            case 2:
                            key = giftLowLev.GetRandomItem();
                            if (npc.money - totalItem[key].price > safeMoney)
                            {
                                if (!npc.items.Keys.Contains(key))
                                    npc.items.Add(key, totalItem[key]);
                                else npc.items[key].Num++;
                            }
                            else return;
                            break;
                    }
                }
            }
            else
            {//�ж�npc������Ʒ�Ƿ����ʮ��
                if (npc.items.Any())
                    if (npc.items.Values.Sum(e => e.Num) >= 10)
                        return;
                for(int i = 0; i < 3; i++)
                {
                    switch (z)
                    {
                        case 1:
                            key = giftSameLev.GetRandomItem();
                            if (npc.money - totalItem[key].price > safeMoney)
                            {
                                if (!npc.items.Keys.Contains(key))
                                    npc.items.Add(key, totalItem[key]);
                                else npc.items[key].Num++;
                            }
                            else
                            {
                                key = giftLowLev.GetRandomItem();
                                if (npc.money - totalItem[key].price > safeMoney)
                                {
                                    if (!npc.items.Keys.Contains(key))
                                        npc.items.Add(key, totalItem[key]);
                                    else npc.items[key].Num++;
                                }
                                else return;
                            }
                            break;
                        case 2:
                            key = giftLowLev.GetRandomItem();
                            if (npc.money - totalItem[key].price > safeMoney)
                            {
                                if (!npc.items.Keys.Contains(key))
                                    npc.items.Add(key, totalItem[key]);
                                else npc.items[key].Num++;
                            }
                            else return;
                            break;
                    }
                }
            }
        }
    }

    #endregion
}
