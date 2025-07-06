using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr : SingleToAutoMono<MonoMgr>
{
    #region  ��������
    private event UnityAction updateEvent;
    private event UnityAction fixUpdateEvent;
    private event UnityAction lateUpdateEvent;
    #endregion
    #region ����
    #region ����¼�
    /// <summary>
    /// ���֡���º���
    /// </summary>
    /// <param name="listener"></param>
    public void AddUpdateListener(UnityAction listener)
    {
        this.updateEvent += listener;
    }
    /// <summary>
    /// �Ƴ�֡���º���
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveUpdateListener(UnityAction listener) {
        updateEvent -= listener;
    }
    /// <summary>
    /// �������֡���º���
    /// </summary>
    /// <param name="listener"></param>
    public void AddFixUpdateListener(UnityAction listener)
    {
        this.fixUpdateEvent += listener;
    }
    /// <summary>
    /// �Ƴ�����֡���º���
    /// </summary>
    /// <param name="listener"></param>
    #endregion
    #region �Ƴ��¼�
    public void RemoveFixUpdateListener(UnityAction listener)
    {
        fixUpdateEvent -= listener;
    }
    /// <summary>
    /// ���֡���º���º���
    /// </summary>
    /// <param name="listener"></param>
    public void AddLateUpdateListener(UnityAction listener)
    {
        this.lateUpdateEvent += listener;
    }
    /// <summary>
    /// �Ƴ�֡���º���º���
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveLateUpdateListener(UnityAction listener)
    {
        lateUpdateEvent -= listener;
    }
    #endregion
    #region ʵ�������� �߼���ͬ���������
    private void Update()
    {
        updateEvent?.Invoke();
    }
    private void FixedUpdate()
    {
        fixUpdateEvent?.Invoke();
    }
    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }
    #endregion
    #endregion
}
