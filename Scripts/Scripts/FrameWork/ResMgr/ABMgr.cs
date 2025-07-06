using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ���ⲿ������Ľ���ab����Դ����
/// </summary>
public class ABMgr : SingleToAutoMono<ABMgr>
{
    #region ��������
    //AB�������ظ����� �ظ��ᱨ��
    //���ֵ�洢���ع���ab��
    //����
    private AssetBundle mainAB = null;
    //�����������ļ�
    private AssetBundleManifest mainfest = null;
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();
    /// <summary>
    /// ab�����·�� �����޸�
    /// </summary>
    private string PathUrl
    {
        get { return Application.streamingAssetsPath + "/"; }
    }
    /// <summary>
    /// �����������޸�
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
    #region ����
    #region ǰ����������������
    /// <summary>
    /// ����ab��
    /// </summary>
    public void LoadAB(string abName)
    {
        AssetBundle ab;

        LoadMainAB();
        //��ȡ�����������Ϣ
        string[] strs = mainfest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i], ab);
            }
        }
        //������Դ��Դ��
        //���û�м��ع��ټ���
        if (!abDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            abDic.Add(abName, ab);
        }

    }
    private void LoadMainAB()
    {
        //�������� ���������ؼ������ļ� ��ȡ������ ����������
        if (mainfest == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            mainfest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }
    //�����첽�����ں�
    #endregion
    #region �첽������ͬ�������ں�
    #region ���������첽������Դ
    public void LoadResAsync(string abName, string resName, UnityAction<UnityEngine.Object> callBack, bool isSync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callBack, isSync));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<UnityEngine.Object> callBack, bool isSync)
    {
        LoadMainAB();
        //��ȡ�����������Ϣ
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
            //֤���ֵ����Ѿ����ں�����������Ϣ�����ݣ�����ȷ���Ƿ�������
            else
            {
                while (abDic[strs[i]] == null)
                {
                    //ֻҪ��������Ϊ�վͲ�ͣ�Ľ��еȴ�
                    yield return 0;
                }
            }
        }
        //������Դ��Դ��
        //���û�м��ع��ټ���
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
                //ֻҪ��������Ϊ�վͲ�ͣ�Ľ��еȴ�
                yield return 0;
            }
        }
        if (isSync)
        {
            UnityEngine.Object res = abDic[abName].LoadAsset(resName);
            callBack(res);
        }
        else
        { //�첽���ذ�
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
            yield return abr;
            callBack(abr.asset);
        }
    }
    #endregion
    #region ����Type�첽������Դ
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<UnityEngine.Object> callBack, bool isAsync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callBack, isAsync));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<UnityEngine.Object> callBack, bool isAsync)
    {
        LoadMainAB();
        //��ȡ�����������Ϣ
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
            //֤���ֵ����Ѿ����ں�����������Ϣ�����ݣ�����ȷ���Ƿ�������
            else
            {
                while (abDic[strs[i]] == null)
                {
                    //ֻҪ��������Ϊ�վͲ�ͣ�Ľ��еȴ�
                    yield return 0;
                }
            }
        }
        //������Դ��Դ��
        //���û�м��ع��ټ���
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
                //ֻҪ��������Ϊ�վͲ�ͣ�Ľ��еȴ�
                yield return 0;
            }
        }
        if (isAsync)
        {
            UnityEngine.Object res = abDic[abName].LoadAsset(resName,type);
            callBack(res);
        }
        else
        { //�첽���ذ�
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName,type);
            yield return abr;
            callBack(abr.asset);
        }

    }
    #endregion
    #region ���ݷ����첽������Դ
    /// <summary>
    /// �����첽������Դ
    /// </summary>
    /// <typeparam name="T">������Դ����</typeparam>
    /// <param name="abName">����</param>
    /// <param name="resName">��Դ��</param>
    /// <param name="callBack">������ɺ���ʲô</param>
    /// <param name="isSync">�Ƿ�ͬ������</param>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T : UnityEngine.Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack, isSync));
    }
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync) where T : UnityEngine.Object
    {
        LoadMainAB();
        //��ȡ�����������Ϣ
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
            //֤���ֵ����Ѿ����ں�����������Ϣ�����ݣ�����ȷ���Ƿ�������
            else
            {
                while (abDic[strs[i]] == null)
                {
                    //ֻҪ��������Ϊ�վͲ�ͣ�Ľ��еȴ�
                    yield return 0;
                }
            }
        }
        //������Դ��Դ��
        //���û�м��ع��ټ���
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
                //ֻҪ��������Ϊ�վͲ�ͣ�Ľ��еȴ�
                yield return 0;
            }
        }
        //������Դ ����ͬ���첽����Ϊһ����������ҲҪΪͬ�����ش���ص�����
        if (isSync)
        {
            T res = abDic[abName].LoadAsset<T>(resName);
            callBack(res);
        }
        else
        { //�첽���ذ�
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abr;
            callBack(abr.asset as T);
        }
    }
    #endregion
    #endregion
    #region ������ж��
    public void Unload(string abName, UnityAction<bool> callBackResult)
    {
        if (abDic.ContainsKey(abName))
        {
            if (abDic[abName] == null)
            {//�����첽���ػ�δ�ɹ�
                callBackResult(false);
                return;
            }
            abDic[abName].Unload(false);
            abDic.Remove(abName);
            //ж�سɹ�
            callBackResult(true);
        }
    }
    #endregion
    #region ���а�ж��
    public void ClearAB()
    {//����AB�������첽�����ˣ����������ǰֹͣЭͬ����
        StopAllCoroutines();
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        mainfest = null;
    }
    #endregion
    #endregion
}
