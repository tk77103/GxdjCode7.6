using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Entrance : MonoBehaviour
{
    public Button nextMonth;
    public Text worldImge;
    public RectTransform contentTrs;
    private AudioSource btnSource;
    RectTransform textTrs;
    Vector2 v2 = Vector2.zero;
    void Start()
    {
        print($"{WorldSceneMgr.Instance.worldAllNpc.FilterValues(e => e.loveStatus==0).Count}");
        print($"{WorldSceneMgr.Instance.worldAllNpc.FilterValues(e => e.loveStatus == 1).Count}");
        print($"{WorldSceneMgr.Instance.worldAllNpc.FilterValues(e => e.loveStatus == 2).Count}");
        print($"{WorldSceneMgr.Instance.worldAllNpc.FilterValues(e=>e.relationships.Any()).Count}");
  
        isRealtionMatchID();
        //float y = worldImge.preferredHeight;
        ////nextMonth.onClick.AddListener(UpdateWorld);

        //textTrs = worldImge.GetComponent<RectTransform>();
        //textTrs.sizeDelta=new Vector2(textTrs.sizeDelta.x,y);
        //contentTrs.sizeDelta = new Vector2(contentTrs.sizeDelta.x, y);
        //foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        //{
        //    if (EstateMgr.Instance.IsMatchEstate(npc))
        //        EstateMgr.Instance.ForMatchEaste(npc, Wealth.Instance.WealthMinAmount(npc.jobLevel - 1));
        //    if (VehiclesMgr.Instance.IsYourCarMatch(npc))
        //        VehiclesMgr.Instance.ForMatchCar(npc, Wealth.Instance.WealthMinAmount(npc.jobLevel - 1));
        //    npc.socialRank = Wealth.Instance.CurrentSocailRank(npc);
        //    NpcShopingAi.Instance.ShopingAi(npc,true);
        //}
        ////NpcEmotionMgr.Instance.FindTargetNpc(true);
        //StockMgr.Instance.FirstMonthStockChange();

        print("safe");
       // print(WorldSceneMgr.Instance.ListenData(true));
    }
    private void isRealtionMatchID()
    {
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            if (npc.relationships.Any())
            {
                foreach (var key in npc.relationships.Keys)
                {
                    if (key != npc.relationships[key].relatedID)
                        print($"这sbnpc{npc.Id}关系中键与值不符");
                }
            }
        }
    }
    //在按下按时候执行的操作
    //钱为世界npc流动的基础优先保障经济的流动其次情感
    private void UpdateWorld()
    {
        btnSource= nextMonth.AddComponent<AudioSource>();
        btnSource.clip = Resources.Load<AudioClip>("BtnAddio1");
        btnSource.volume = 0.05f;
        btnSource.Play();
        Destroy(btnSource, 1);
        //世界更新
        WorldChange();

        //更新数据
        UpdateText();
        //更新文本大小

        //更新content大小
        UpdateContentSize();
        print(WorldSceneMgr.Instance.ListenData(false));
    }
    #region 数据更新
    private void WorldChange()
    {
        #region 时间
        WorldClock.Instance.NewMonth();
        #endregion
        #region 工作
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            npc.money += (int)npc.basicSalary;
        }
        #endregion
        #region 职场
        #endregion
        #region 购物
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            NpcShopingAi.Instance.ShopingAi(npc, false);
            VehiclesMgr.Instance.BuyVehicle(npc);
        }
        #endregion
        #region 投资
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            //SelfEmployedMgr.Instance.CreatSelfEmployed(npc);
            if (npc.selfEmployed != null)
            {
                SelfEmployedMgr.Instance.SelfEmploedProfi(npc.selfEmployed);
            }
            //  SelfEmployedMgr.Instance.CreatSelfEmployed(npc);

        }
        #region 股票
        foreach(var stock in StockMgr.Instance.allStocks.Values)
        {
            StockMgr.Instance.StockChange(stock);
        }
        foreach(var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            StockMgr.Instance.BuyStock(npc);
            StockMgr.Instance.SellStock(npc);
        }
        #endregion
        // SelfEmployedMgr.Instance.SelfEmploedProfi();
        #endregion
        #region 社交
        //NpcEmotionMgr.Instance.NPCEmotionalInteraction();
        #endregion
        #region 秘闻
        #endregion
        #region 房屋
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            //是否购房
            EstateMgr.Instance.BuyHouseOrNot(npc);
            //租出房屋
            EstateMgr.Instance.RentOutMyHouse(npc);
            //租金收入
            EstateMgr.Instance.RentHouseIncome(npc);
            npc.socialRank = Wealth.Instance.CurrentSocailRank(npc);
        }
        #endregion
        #region 银行
        WorldBank.Instance.PayDebt();
        #endregion
       
    }
    public void ListenData()
    {

    }
    private void UpdateText()
    {
        //worldImge.text += WorldSceneMgr.Instance.ListenData(false);
    }
    private void UpdateContentSize()
    {
        float y = worldImge.preferredHeight;
        textTrs.sizeDelta = new Vector2(textTrs.sizeDelta.x, y);
        contentTrs.sizeDelta = new Vector2(contentTrs.sizeDelta.x, y);
    }
    #endregion

}
