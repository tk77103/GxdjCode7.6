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
        gifts = totalItem.Values.Where(e => e.type == "收藏品").Select(e => e.id).ToList();

    }
    #region npc购买Ai(此处有时间在购买处可以拆一下)
    //四类藏品买三件 当满足本等级有三件时停止购买
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
            {//判断npc持有物品是否大于十件
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
