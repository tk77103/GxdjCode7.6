using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 计时器管理器主要用于开启，停止，重置等等操作
/// </summary>
public class TimerMgr : BaseManger<TimerMgr>
{
    #region 数据容器

    /// <summary>
    /// 计时器的初始id 
    /// </summary>
    private int TIMER_KEY = 0;
    /// <summary>
    /// 存储所有计时器的字典容器
    /// </summary>
    private Dictionary<int, TimerItem> timeDic = new Dictionary<int, TimerItem>();
    /// <summary>
    /// 不受timeScale影响的计时器
    /// </summary>
    private Dictionary<int, TimerItem> realTimeDic = new Dictionary<int, TimerItem>();
    private List<TimerItem> delList = new List<TimerItem>();
    private Coroutine timer;
    private Coroutine realTimer;
    /// <summary>
    /// 计时管理器中的执行的间隔时间相当于秒针每多长时间转一次
    /// </summary>
    private const float intervalTime = 0.1f;

    //为了避免每次循环创建对象 直接声明在外部节约性能
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime);
    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTime);
    #endregion
    #region 函数
    /// <summary>
    /// 默认在调用时候开启
    /// </summary>
    private TimerMgr() { Start(); }
    #region 开启计时器
    public void Start()
    {
        timer = MonoMgr.Instance.StartCoroutine(StartTiming(false, timeDic));
        realTimer = MonoMgr.Instance.StartCoroutine(StartTiming(true, realTimeDic));

    }
    IEnumerator StartTiming(bool isRealTime, Dictionary<int, TimerItem> timeDic)
    {
        while (true)
        {
            if (isRealTime)
                yield return waitForSecondsRealtime;
            else
                //100ms进行一次计时
                yield return waitForSeconds;
            foreach (TimerItem item in timeDic.Values)
            {//是否需要执行
                if (!item.isRuning)
                    continue;
                //判断是否有间隔时间执行的需求
                if (item.callBack != null)
                {//如果间隔执行的回调函数不为空 每次循环减去最低检测计时时间
                    item.intervalTime -= (int)(intervalTime * 1000);
                    //满足间隔时间后执行对应的间隔回调函数
                    if (item.intervalTime <= 0)
                    {
                        item.callBack.Invoke();
                        item.intervalTime = item.maxIntervalTime;
                    }
                }
                item.allTime -= (int)(intervalTime * 1000);
                if (item.allTime <= 0)
                {
                    item.overCallBack.Invoke();
                    //调用完后可以移除了但在循环语句中移除会报错
                    //将需要移除的计时器暂时记录在容器中在循环结束后移除
                    delList.Add(item);
                }
            }
            //移除带移除的列表中的计时器
            for (int i = 0; i < delList.Count; i++)
            {
                timeDic.Remove(delList[i].keyID);
                //计时器可能会经常创建销毁为节约性能放入到缓存池中
                PoolMgr.Instance.PushObj<TimerItem>(delList[i]);
            }
            //移除结束后将待移除列表清空
            delList.Clear();
        }
    }
    #endregion
    #region 关闭计时器
    public void Stop()
    {
        MonoMgr.Instance.StopCoroutine(timer);
        MonoMgr.Instance.StopCoroutine(realTimer);
    }
    #endregion
    #region 创建单个计时器
    /// <summary>
    /// 创建单个计时器
    /// </summary>
    /// <param name="isRealTime">是否启用忽略timeScale的时间</param>
    /// <param name="allTime">计时器存在时间 单位ms 1000ms=1s</param>
    /// <param name="overCallBack">计时器计时结束函数</param>
    /// <param name="intervalTime">期间间隔时间调用时间</param>
    /// <param name="overCaLLBack">若存在间隔时间做什么事</param>
    /// <returns></returns>
    public int CreateTimer(bool isRealTime, int allTime, UnityAction overCallBack, int intervalTime = 0, UnityAction callBack = null)
    {

        //构建唯一的ID
        int keyID = TIMER_KEY++;
        TimerItem timerItem = PoolMgr.Instance.GetObj<TimerItem>();
        timerItem.InitInfo(keyID, allTime, overCallBack, intervalTime, callBack);
        //记录到计时器字典中
        if (isRealTime)
            realTimeDic.Add(keyID, timerItem);
        else
            timeDic.Add(keyID, timerItem);
        return keyID;
    }
    #endregion
    #region 移除单个计时器
    public void RemoveTimer(int keyID)
    {
        if (timeDic.ContainsKey(keyID))
        {
            //移除id计时器放入缓冲池中
            PoolMgr.Instance.PushObj(timeDic[keyID]);
            timeDic.Remove(keyID);
        }
        else if (realTimeDic.ContainsKey(keyID))
        {
            //移除id计时器放入缓冲池中
            PoolMgr.Instance.PushObj(realTimeDic[keyID]);
            realTimeDic.Remove(keyID);
        }
    }
    #endregion 
    #region 重置单个计时器
    public void RestTimer(int keyID)
    {
        if (timeDic.ContainsKey(keyID))
        {
            timeDic[keyID].RestTimer();
        }
        else if (realTimeDic.ContainsKey(keyID))
        {
            realTimeDic[keyID].RestTimer();
        }
    }
    #endregion
    #region 开启单个计时器
    /// <summary>
    /// 开启单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一id</param>
    public void StartTimer(int keyID)
    {
        if (timeDic.ContainsKey(keyID))
        {
            timeDic[keyID].isRuning = true;
        }
        else if (realTimeDic.ContainsKey(keyID))
        {
            realTimeDic[keyID].isRuning = true;
        }
    }
    #endregion
    #region 停止单个计时器
    public void StopTimer(int keyID)
    {
        if (timeDic.ContainsKey(keyID))
        {
            timeDic[keyID].isRuning = false;
        }
        else if (timeDic.ContainsKey(keyID))
        {
            realTimeDic[keyID].isRuning = false;
        }
    }
    #endregion
    #endregion
}
