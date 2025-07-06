using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#region  �������װ�в��޲�����ί��
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
/// Ϊ��ֹ�ڴ�й¶��ί�����Ӿ�Ҫ������ʱ�Ƴ���
/// </summary>
public class EventCenter : BaseManger<EventCenter>
{
    #region ��������
    //�ֵ��д洢 ��Ӧ�¼���ö�����ͣ��洢��ί��
    private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();
    #endregion
    #region ����
    private EventCenter() { }
    #region �¼������������в����޲�ί�У�
    /// <summary>
    /// ���¼�����ʱ�������Ĵ��¼��ķ���
    /// </summary>
    /// <typeparam name="T">TΪҪ�����Ĳ�����ί�в���һ�·����ȡ��Ϣ</typeparam>
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
    #region Ϊ�¼���Ӽ����ߣ��в� �޲Σ�
    /// <summary>
    /// ����¼�������
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
    #region �Ƴ��¼�������
    /// <summary>
    /// �Ƴ��¼�������
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
    #region ����¼�����
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
