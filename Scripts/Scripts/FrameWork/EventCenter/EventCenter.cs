using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#region  数据类封装有参无参俩种委托
public abstract class EventInfoBase { }
public class EventInfo<T> : EventInfoBase
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}
public class EventInfo : EventInfoBase
{
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}
#endregion
/// <summary>
/// 为防止内存泄露有委托增加就要在用完时移除他
/// </summary>
public class EventCenter : BaseManger<EventCenter>
{
    #region 数据容器
    //字典中存储 对应事件（枚举类型）存储的委托
    private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();
    #endregion
    #region 函数
    private EventCenter() { }
    #region 事件触发方法（有参与无参委托）
    /// <summary>
    /// 当事件触发时触发关心此事件的方法
    /// </summary>
    /// <typeparam name="T">T为要传出的参数与委托参数一致方便获取信息</typeparam>
    /// <param name="eventName"></param>
    /// <param name="info"></param>
    public void EventTrigger<T>(E_EventType eventName, T info)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
    }
    public void EventTrigger(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions?.Invoke();
        }
    }
    #endregion
    #region 为事件添加监听者（有参 无参）
    /// <summary>
    /// 添加事件监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T>(func));
        }
    }
    public void AddEventListener(E_EventType eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo(func));
        }
    }
    #endregion
    #region 移除事件监听者
    /// <summary>
    /// 移除事件监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName]as EventInfo<T>).actions -= func;
        }
    }
    public void RemoveEventListener(E_EventType eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions -= func;
        }
    }
    #endregion
    #region 清除事件监听
    public void Clear()
    {
        eventDic.Clear();
    }
    public void Clear(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }
    #endregion
    #endregion

}
