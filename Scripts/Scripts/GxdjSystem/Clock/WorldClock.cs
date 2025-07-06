using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldClock : BaseManger<WorldClock>
{
    #region 数据容器
    //期世界变化基本单位
    public long period;
    //当前月
    public int currentMonth = 0;
    //当前年
    public int currentYear = 0;
    //期为节点
    public long TotalTime = 0;
    public string clockExpress;
    public bool isNewYear=false;
    //是否为订单时间
    private bool isOrderTime = false;
    //是否为订单完成时间
    private bool isOrderCompletedTime = false;
    #endregion
    #region 函数

    private WorldClock()
    {
    }
    //期可以考虑用字节来存储更加灵活
    public void NextPeriod()
    {
        period++;
    }
    public void NewMonth()
    {//年月 是否过年及传给外部的时间文本
        TotalTime++;
        currentYear = (int)TotalTime / 12;
        currentMonth = (int)TotalTime % 12;
        isOrderTime=(TotalTime-1)%3>0?false:true;
        isOrderCompletedTime=TotalTime%3==0?true:false;
        if (TotalTime % 12 == 0)
            currentMonth = 0;
        if (TotalTime > 12 && currentMonth == 1)
        {
            isNewYear = true;
        }
        else isNewYear = false;
        if (isNewYear)
        {
            SelfEmployedMgr.Instance.Dividend();
            StockMgr.Instance.YearEndSummary();
            NpcGowthAI.Instance.NpcIntelligentGrow();
            //NpcEmotionMgr.Instance.GetAdmirationObject(WorldSceneMgr.Instance.worldAllNpc);
            //NpcEmotionMgr.Instance.FindTargetNpc(false);
        }
        if (isOrderTime) { }
        if(isOrderCompletedTime) { }
        clockExpress = currentYear + "年" + currentMonth + "月";
        //过月事件触发 其他人去订阅
        EventCenter.Instance.EventTrigger(E_EventType.E_WorldClock_NewMonth);
    }
    #endregion
}
