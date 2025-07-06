using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr : SingleToAutoMono<MonoMgr>
{
    #region  数据容器
    private event UnityAction updateEvent;
    private event UnityAction fixUpdateEvent;
    private event UnityAction lateUpdateEvent;
    #endregion
    #region 函数
    #region 添加事件
    /// <summary>
    /// 添加帧更新函数
    /// </summary>
    /// <param name="listener"></param>
    public void AddUpdateListener(UnityAction listener)
    {
        this.updateEvent += listener;
    }
    /// <summary>
    /// 移除帧更新函数
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveUpdateListener(UnityAction listener) {
        updateEvent -= listener;
    }
    /// <summary>
    /// 添加物理帧更新函数
    /// </summary>
    /// <param name="listener"></param>
    public void AddFixUpdateListener(UnityAction listener)
    {
        this.fixUpdateEvent += listener;
    }
    /// <summary>
    /// 移除物理帧更新函数
    /// </summary>
    /// <param name="listener"></param>
    #endregion
    #region 移除事件
    public void RemoveFixUpdateListener(UnityAction listener)
    {
        fixUpdateEvent -= listener;
    }
    /// <summary>
    /// 添加帧更新后更新函数
    /// </summary>
    /// <param name="listener"></param>
    public void AddLateUpdateListener(UnityAction listener)
    {
        this.lateUpdateEvent += listener;
    }
    /// <summary>
    /// 移除帧更新后更新函数
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveLateUpdateListener(UnityAction listener)
    {
        lateUpdateEvent -= listener;
    }
    #endregion
    #region 实际作用域 逻辑相同可自行添加
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
