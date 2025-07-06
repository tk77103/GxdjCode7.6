using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#region 资源信息类
public abstract class ResInfoBase
{  //引用计数
    public int refCount;
}
/// <summary>
/// 资源信息对象 主要用于存储资源信息 异步加载委托信息 异步加载 协程信息
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResInfo<T> : ResInfoBase
{
    public T asset;
    public UnityAction<T> callBack;
    //用于存储异步加载时开启的协同程序
    public Coroutine coroutine;
    //决定引用计数为0时 是否真正需要移除
    public bool isDel;

    public void AddRefCount() { refCount++; }
    public void SubRefCount()
    {
        refCount--;
        if (refCount < 0)
            Debug.LogError("引用计数小于0了，请检查使用和卸载是否配对执行");
    }
}
#endregion
public class ResourcesMgr : BaseManger<ResourcesMgr>
{//用于存储加载过或者加载中资源的容器
    private Dictionary<string, ResInfoBase> resDic = new Dictionary<string, ResInfoBase>();
    private ResourcesMgr() { }
    #region 异步加载资源方法
    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="callBack">加载结束后回调函数，当异步加载资源结束后才会调用</param>
    public void LoadAsync<T>(string path, UnityAction<T> callBack) where T : UnityEngine.Object
    {//资源的唯一id，通过 路径名_资源类型 拼接而成
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> res;
        if (!resDic.ContainsKey(resName))
        {
            res = new ResInfo<T>();
            res.AddRefCount();
            resDic.Add(resName, res);
            res.callBack += callBack;
            //开启并记录协程用于可能的停止
            res.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        else
        {
            res = resDic[resName] as ResInfo<T>;
            res.AddRefCount();
            if (res.asset == null)
            {
                res.callBack += callBack;
            }
            else
            {
                callBack?.Invoke(res.asset);
            }
        }
    }
    private IEnumerator ReallyLoadAsync<T>(string path) where T : UnityEngine.Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResourceRequest rq = Resources.LoadAsync<T>(path);
        yield return rq;
        if (resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //取出资源信息，并且记录加载完成的资源
            resInfo.asset = rq.asset as T;
            if (resInfo.refCount == 0)
                UnloadAsset<T>(path, resInfo.isDel,null,false);
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);
                //加载完毕 这些引用就可以清空了，避免潜在的内存泄露问题
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }
    [Obsolete("建议使用泛型加载方法，如果实在要用Type加载，一定不能和泛型加载混用去加载同类型同名资源")]
    public void LoadAsync(string path, Type type, UnityAction<UnityEngine.Object> callBack)
    {
        string resName = path + "_" + type.Name;
        ResInfo<UnityEngine.Object> res;
        if (!resDic.ContainsKey(resName))
        {
            res = new ResInfo<UnityEngine.Object>();
            res.AddRefCount();
            resDic.Add(resName, res);
            res.callBack += callBack;
            //开启并记录协程用于可能的停止
            res.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path, type));
        }
        else
        {
            res = resDic[resName] as ResInfo<UnityEngine.Object>;
            res.AddRefCount();
            if (res.asset == null)
            {
                res.callBack += callBack;
            }
            else
            {
                callBack?.Invoke(res.asset);
            }
        }
    }
    private IEnumerator ReallyLoadAsync(string path, Type type)
    {
        string resName = path + "_" + type.Name;
        ResourceRequest rq = Resources.LoadAsync<UnityEngine.Object>(path);
        yield return rq;
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //取出资源信息，并且记录加载完成的资源
            resInfo.asset = rq.asset;
            if (resInfo.refCount == 0)
                UnloadAsset(path, type, resInfo.isDel,null,false);
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);
                //加载完毕 这些引用就可以清空了，避免潜在的内存泄露问题
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }
    #endregion
    #region 同步加载资源方法
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> resInfo;
        if (!resDic.ContainsKey(resName))
        {//直接同步加载 并且记录资源信息到字典中 方便下次取出来用
            T res = Resources.Load<T>(path);
            resInfo = new ResInfo<T>();
            resInfo.asset = res;
            resInfo.AddRefCount();
            resDic.Add(resName, resInfo);
            return res;
        }
        else
        {
            resInfo = resDic[resName] as ResInfo<T>;
            resInfo.AddRefCount();
            if (resInfo.asset == null)
            {
                MonoMgr.Instance.StopCoroutine(resInfo.coroutine);
                T res = Resources.Load<T>(path);
                resInfo.asset = res;
                //把等待异步加载结束的委托直接执行了
                resInfo.callBack?.Invoke(resInfo.asset);
                resInfo.callBack = null;
                resInfo.coroutine = null;
                return res;
            }
            else
            {
                return resInfo.asset;
            }
        }
    }
    #endregion
    #region 资源卸载方法
    /// <summary>
    /// 指定卸载一个资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="isDel">是否马上移除</param>
    /// <param name="callBack"></param>
    /// <param name="isSub">引用计数是否减一</param>
    public void UnloadAsset<T>(string path, bool isDel = false, UnityAction<T> callBack = null,bool isSub=true)
    {
        string resName = path + "_" + typeof(T).Name;
        if (resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            if(isSub)
            resInfo.SubRefCount();
            //记录引用计数为0时 是否马上移除标签
            resInfo.isDel = isDel;
            //资源加载结束 并且引用该资源没有任何引用才去卸载
            if (resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel == true)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }

            else if (resInfo.asset == null)
            {
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }

    }
    public void UnloadAsset(string path, Type type, bool isDel = false, UnityAction<UnityEngine.Object> callBack = null,bool isSub=true)
    {
        string resName = path + "_" + type.Name;
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            if (isSub)
            resInfo.SubRefCount();
            resInfo.isDel = isDel;
            //资源加载结束
            if (resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel == true)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            else if (resInfo.asset == null)
            {//当异步加载不想使用时，应该移除他的回调记录 而不是直接卸载资源
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }

    }
    /// <summary>
    /// 异步卸载对应的没有使用的Resources相关的资源
    /// </summary>
    /// <param name="callBack"></param>
    public void UnloadUnusedAsset(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAsset(callBack));
    }
    private IEnumerator ReallyUnloadUnusedAsset(UnityAction callBack)
    {//在真正移除不使用的资源之前 应该把我们自己记录的那些引用计数为0 并且没有被移除记录的资源移除掉
        List<string> list = new List<string>();
        foreach (string path in resDic.Keys)
        {
            if (resDic[path].refCount == 0)
            {
                list.Add(path);
            }
        }
        foreach (string name in list)
        {
            resDic.Remove(name);
        }
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callBack();
    }
    #endregion
    #region 获取资源引用数
    /// <summary>
    /// 获取当前某个资源的引用数
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public int GetRefCount<T>(string path)
    {
        string resName = path + "_" + typeof(T).Name;
        if (resDic.ContainsKey(resName))
            return (resDic[resName] as ResInfo<T>).refCount;
        return 0;
    }
    #endregion
    #region 清空字典
    /// <summary>
    /// 清空字典
    /// </summary>
    /// <param name="callBack"></param>
    public void ClearDic(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyClearDic(callBack));
    }
    private IEnumerator ReallyClearDic(UnityAction callBack)
    {
        resDic.Clear();
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callBack();
    }
    #endregion
    #region 使用注意
    // 使用资源时候有用就有删
    // 当使用某个资源对象移除是，一定要记得调用移除方法
    // 2.当决定卸载资源功能麻烦时也可以完全不使用卸载相关的方法
    // 加载相关逻辑不会有任何影响，和以前直接使用Resources的用法几乎一样
    // 使用清空字典的方法
    #endregion
}
