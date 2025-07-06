using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#region ��Դ��Ϣ��
public abstract class ResInfoBase
{  //���ü���
    public int refCount;
}
/// <summary>
/// ��Դ��Ϣ���� ��Ҫ���ڴ洢��Դ��Ϣ �첽����ί����Ϣ �첽���� Э����Ϣ
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResInfo<T> : ResInfoBase
{
    public T asset;
    public UnityAction<T> callBack;
    //���ڴ洢�첽����ʱ������Эͬ����
    public Coroutine coroutine;
    //�������ü���Ϊ0ʱ �Ƿ�������Ҫ�Ƴ�
    public bool isDel;

    public void AddRefCount() { refCount++; }
    public void SubRefCount()
    {
        refCount--;
        if (refCount < 0)
            Debug.LogError("���ü���С��0�ˣ�����ʹ�ú�ж���Ƿ����ִ��");
    }
}
#endregion
public class ResourcesMgr : BaseManger<ResourcesMgr>
{//���ڴ洢���ع����߼�������Դ������
    private Dictionary<string, ResInfoBase> resDic = new Dictionary<string, ResInfoBase>();
    private ResourcesMgr() { }
    #region �첽������Դ����
    /// <summary>
    /// �첽������Դ�ķ���
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    /// <param name="callBack">���ؽ�����ص����������첽������Դ������Ż����</param>
    public void LoadAsync<T>(string path, UnityAction<T> callBack) where T : UnityEngine.Object
    {//��Դ��Ψһid��ͨ�� ·����_��Դ���� ƴ�Ӷ���
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> res;
        if (!resDic.ContainsKey(resName))
        {
            res = new ResInfo<T>();
            res.AddRefCount();
            resDic.Add(resName, res);
            res.callBack += callBack;
            //��������¼Э�����ڿ��ܵ�ֹͣ
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
            //ȡ����Դ��Ϣ�����Ҽ�¼������ɵ���Դ
            resInfo.asset = rq.asset as T;
            if (resInfo.refCount == 0)
                UnloadAsset<T>(path, resInfo.isDel,null,false);
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);
                //������� ��Щ���þͿ�������ˣ�����Ǳ�ڵ��ڴ�й¶����
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }
    [Obsolete("����ʹ�÷��ͼ��ط��������ʵ��Ҫ��Type���أ�һ�����ܺͷ��ͼ��ػ���ȥ����ͬ����ͬ����Դ")]
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
            //��������¼Э�����ڿ��ܵ�ֹͣ
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
            //ȡ����Դ��Ϣ�����Ҽ�¼������ɵ���Դ
            resInfo.asset = rq.asset;
            if (resInfo.refCount == 0)
                UnloadAsset(path, type, resInfo.isDel,null,false);
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);
                //������� ��Щ���þͿ�������ˣ�����Ǳ�ڵ��ڴ�й¶����
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }
    #endregion
    #region ͬ��������Դ����
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> resInfo;
        if (!resDic.ContainsKey(resName))
        {//ֱ��ͬ������ ���Ҽ�¼��Դ��Ϣ���ֵ��� �����´�ȡ������
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
                //�ѵȴ��첽���ؽ�����ί��ֱ��ִ����
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
    #region ��Դж�ط���
    /// <summary>
    /// ָ��ж��һ����Դ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="isDel">�Ƿ������Ƴ�</param>
    /// <param name="callBack"></param>
    /// <param name="isSub">���ü����Ƿ��һ</param>
    public void UnloadAsset<T>(string path, bool isDel = false, UnityAction<T> callBack = null,bool isSub=true)
    {
        string resName = path + "_" + typeof(T).Name;
        if (resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            if(isSub)
            resInfo.SubRefCount();
            //��¼���ü���Ϊ0ʱ �Ƿ������Ƴ���ǩ
            resInfo.isDel = isDel;
            //��Դ���ؽ��� �������ø���Դû���κ����ò�ȥж��
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
            //��Դ���ؽ���
            if (resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel == true)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            else if (resInfo.asset == null)
            {//���첽���ز���ʹ��ʱ��Ӧ���Ƴ����Ļص���¼ ������ֱ��ж����Դ
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }

    }
    /// <summary>
    /// �첽ж�ض�Ӧ��û��ʹ�õ�Resources��ص���Դ
    /// </summary>
    /// <param name="callBack"></param>
    public void UnloadUnusedAsset(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAsset(callBack));
    }
    private IEnumerator ReallyUnloadUnusedAsset(UnityAction callBack)
    {//�������Ƴ���ʹ�õ���Դ֮ǰ Ӧ�ð������Լ���¼����Щ���ü���Ϊ0 ����û�б��Ƴ���¼����Դ�Ƴ���
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
    #region ��ȡ��Դ������
    /// <summary>
    /// ��ȡ��ǰĳ����Դ��������
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
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
    #region ����ֵ�
    /// <summary>
    /// ����ֵ�
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
    #region ʹ��ע��
    // ʹ����Դʱ�����þ���ɾ
    // ��ʹ��ĳ����Դ�����Ƴ��ǣ�һ��Ҫ�ǵõ����Ƴ�����
    // 2.������ж����Դ�����鷳ʱҲ������ȫ��ʹ��ж����صķ���
    // ��������߼��������κ�Ӱ�죬����ǰֱ��ʹ��Resources���÷�����һ��
    // ʹ������ֵ�ķ���
    #endregion
}
