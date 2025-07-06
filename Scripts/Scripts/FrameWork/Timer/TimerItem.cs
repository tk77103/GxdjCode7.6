using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerItem : IPoolObject
{
    #region 数据容器
    /// <summary>
    /// 计时器唯一id
    /// </summary>
    public int keyID;
    /// <summary>
    /// 计时结束时候调用的函数
    /// </summary>
    public UnityAction overCallBack;
    /// <summary>
    /// 间隔时间执行的回调
    /// </summary>
    public UnityAction callBack;
    /// <summary>
    /// 毫秒 表示计时器总的计时时间
    /// </summary>
    public int allTime;
    /// <summary>
    /// 记录一开始计时时的时间 用于时间重置
    /// </summary>
    public int maxAllTime;
    /// <summary>
    /// 间隔执行回调的时间
    /// </summary>
    public int intervalTime;
    /// <summary>
    /// 记录一开始的时间 1s=1000ms
    /// </summary>
    public int maxIntervalTime;
    /// <summary>
    /// 是否在进行计时
    /// </summary>
    public bool isRuning;
    #endregion
    #region 函数
    /// <summary>
    /// 初始化计时器数据
    /// </summary>
    /// <param name="keyID">唯一id</param>
    /// <param name="allTime">总的时间</param>
    /// <param name="overCallBack">总时间结束的回调</param>
    /// <param name="intervalTime">间隔执行的时间</param>
    /// <param name="callBack">间隔执行时间结束后的回调</param>
    public void InitInfo(int keyID,int allTime,UnityAction overCallBack,int intervalTime=0,UnityAction callBack=null)
    {
        this.keyID = keyID;
       this.maxAllTime=this.allTime = allTime;
        this.overCallBack = overCallBack;
        this.maxIntervalTime=this.intervalTime = intervalTime;
        this.callBack = callBack;
    }
    /// <summary>
    /// 重置计时器/重新开始计时
    /// </summary>
    public void RestTimer()
    {
        this.allTime=this.maxAllTime;
        this.intervalTime=this.maxIntervalTime;
        this.isRuning=true;
    }
    /// <summary>
    /// 缓存池回收时 清除相关引用数据
    /// </summary>
    public void RestInfo()
    {
        overCallBack=null;
        callBack=null;
    }
    #endregion
}
