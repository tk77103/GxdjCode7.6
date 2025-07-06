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
                        print($"��sbnpc{npc.Id}��ϵ�м���ֵ����");
                }
            }
        }
    }
    //�ڰ��°�ʱ��ִ�еĲ���
    //ǮΪ����npc�����Ļ������ȱ��Ͼ��õ�����������
    private void UpdateWorld()
    {
        btnSource= nextMonth.AddComponent<AudioSource>();
        btnSource.clip = Resources.Load<AudioClip>("BtnAddio1");
        btnSource.volume = 0.05f;
        btnSource.Play();
        Destroy(btnSource, 1);
        //�������
        WorldChange();

        //��������
        UpdateText();
        //�����ı���С

        //����content��С
        UpdateContentSize();
        print(WorldSceneMgr.Instance.ListenData(false));
    }
    #region ���ݸ���
    private void WorldChange()
    {
        #region ʱ��
        WorldClock.Instance.NewMonth();
        #endregion
        #region ����
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            npc.money += (int)npc.basicSalary;
        }
        #endregion
        #region ְ��
        #endregion
        #region ����
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            NpcShopingAi.Instance.ShopingAi(npc, false);
            VehiclesMgr.Instance.BuyVehicle(npc);
        }
        #endregion
        #region Ͷ��
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            //SelfEmployedMgr.Instance.CreatSelfEmployed(npc);
            if (npc.selfEmployed != null)
            {
                SelfEmployedMgr.Instance.SelfEmploedProfi(npc.selfEmployed);
            }
            //  SelfEmployedMgr.Instance.CreatSelfEmployed(npc);

        }
        #region ��Ʊ
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
        #region �罻
        //NpcEmotionMgr.Instance.NPCEmotionalInteraction();
        #endregion
        #region ����
        #endregion
        #region ����
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            //�Ƿ񹺷�
            EstateMgr.Instance.BuyHouseOrNot(npc);
            //�������
            EstateMgr.Instance.RentOutMyHouse(npc);
            //�������
            EstateMgr.Instance.RentHouseIncome(npc);
            npc.socialRank = Wealth.Instance.CurrentSocailRank(npc);
        }
        #endregion
        #region ����
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
