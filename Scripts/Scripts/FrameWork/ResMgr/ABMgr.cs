using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 让外部更方便的进行ab包资源加载
/// </summary>
public class ABMgr : SingleToAutoMono<ABMgr>
{
    #region 数据容器
    //AB包不能重复加载 重复会报错
    //用字典存储加载过的ab包
    //主包
    private AssetBundle mainAB = null;
    //依赖包配置文件
    private AssetBundleManifest mainfest = null;
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();
    /// <summary>
    /// ab包存放路径 方便修改
    /// </summary>
    private string PathUrl
    {
        get { return Application.streamingAssetsPath + "/"; }
    }
    /// <summary>
    /// 主包名方便修改
    /// </summary>
    private string MainABName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }
    #endregion
    #region 方法
    #region 前置主包依赖包加载
    /// <summary>
    /// 加载ab包
    /// </summary>
    public void LoadAB(string abName)
    {
        AssetBundle ab;

        LoadMainAB();
        //获取依赖包相关信息
        string[] strs = mainfest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i], ab);
            }
        }
        //加载资源来源包
        //如果没有加载过再加载
        if (!abDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            abDic.Add(abName, ab);
        }

    }
    private void LoadMainAB()
    {
        //加载主包 加载主包关键配置文件 获取依赖包 加载依赖包
        if (mainfest == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            mainfest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }
    //已与异步方法融合
    #endregion
    #region 异步加载与同步加载融合
    #region 根据名字异步加载资源
    public void LoadResAsync(string abName, string resName, UnityAction<UnityEngine.Object> callBack, bool isSync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callBack, isSync));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<UnityEngine.Object> callBack, bool isSync)
    {
        LoadMainAB();
        //获取依赖包相关信息
        string[] strs = mainfest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                else
                {
                    abDic.Add(strs[i], null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    abDic[strs[i]] = req.assetBundle;
                }
            }
            //证明字典中已经存在含有依赖包信息的内容，但不确定是否加载完成
            else
            {
                while (abDic[strs[i]] == null)
                {
                    //只要发现内容为空就不停的进行等待
                    yield return 0;
                }
            }
        }
        //加载资源来源包
        //如果没有加载过再加载
        if (!abDic.ContainsKey(abName))
        {
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                abDic[abName] = req.assetBundle;
            }

        }
        else
        {
            while (abDic[abName] == null)
            {
                //只要发现内容为空就不停的进行等待
                yield return 0;
            }
        }
        if (isSync)
        {
            UnityEngine.Object res = abDic[abName].LoadAsset(resName);
            callBack(res);
        }
        else
        { //异步加载包
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
            yield return abr;
            callBack(abr.asset);
        }
    }
    #endregion
    #region 根据Type异步加载资源
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<UnityEngine.Object> callBack, bool isAsync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callBack, isAsync));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<UnityEngine.Object> callBack, bool isAsync)
    {
        LoadMainAB();
        //获取依赖包相关信息
        string[] strs = mainfest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                if (isAsync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                else
                {
                    abDic.Add(strs[i], null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    abDic[strs[i]] = req.assetBundle;
                }
            }
            //证明字典中已经存在含有依赖包信息的内容，但不确定是否加载完成
            else
            {
                while (abDic[strs[i]] == null)
                {
                    //只要发现内容为空就不停的进行等待
                    yield return 0;
                }
            }
        }
        //加载资源来源包
        //如果没有加载过再加载
        if (!abDic.ContainsKey(abName))
        {
            if (isAsync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                abDic[abName] = req.assetBundle;
            }

        }
        else
        {
            while (abDic[abName] == null)
            {
                //只要发现内容为空就不停的进行等待
                yield return 0;
            }
        }
        if (isAsync)
        {
            UnityEngine.Object res = abDic[abName].LoadAsset(resName,type);
            callBack(res);
        }
        else
        { //异步加载包
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName,type);
            yield return abr;
            callBack(abr.asset);
        }

    }
    #endregion
    #region 根据泛型异步加载资源
    /// <summary>
    /// 泛型异步加载资源
    /// </summary>
    /// <typeparam name="T">加载资源类型</typeparam>
    /// <param name="abName">包名</param>
    /// <param name="resName">资源名</param>
    /// <param name="callBack">加载完成后做什么</param>
    /// <param name="isSync">是否同步加载</param>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T : UnityEngine.Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack, isSync));
    }
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync) where T : UnityEngine.Object
    {
        LoadMainAB();
        //获取依赖包相关信息
        string[] strs = mainfest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                else
                {
                    abDic.Add(strs[i], null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    abDic[strs[i]] = req.assetBundle;
                }
            }
            //证明字典中已经存在含有依赖包信息的内容，但不确定是否加载完成
            else
            {
                while (abDic[strs[i]] == null)
                {
                    //只要发现内容为空就不停的进行等待
                    yield return 0;
                }
            }
        }
        //加载资源来源包
        //如果没有加载过再加载
        if (!abDic.ContainsKey(abName))
        {
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                abDic[abName] = req.assetBundle;
            }

        }
        else
        {
            while (abDic[abName] == null)
            {
                //只要发现内容为空就不停的进行等待
                yield return 0;
            }
        }
        //加载资源 由于同步异步精炼为一个函数所以也要为同步加载传入回调函数
        if (isSync)
        {
            T res = abDic[abName].LoadAsset<T>(resName);
            callBack(res);
        }
        else
        { //异步加载包
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abr;
            callBack(abr.asset as T);
        }
    }
    #endregion
    #endregion
    #region 单个包卸载
    public void Unload(string abName, UnityAction<bool> callBackResult)
    {
        if (abDic.ContainsKey(abName))
        {
            if (abDic[abName] == null)
            {//正在异步加载还未成功
                callBackResult(false);
                return;
            }
            abDic[abName].Unload(false);
            abDic.Remove(abName);
            //卸载成功
            callBackResult(true);
        }
    }
    #endregion
    #region 所有包卸载
    public void ClearAB()
    {//由于AB包都是异步加载了，因此在清理前停止协同程序
        StopAllCoroutines();
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        mainfest = null;
    }
    #endregion
    #endregion
}
