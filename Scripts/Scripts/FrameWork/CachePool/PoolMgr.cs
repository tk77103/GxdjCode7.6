using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
public class PoolData
{
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    private List<GameObject> useList = new List<GameObject>();
    private GameObject rootObj;
    private int maxNum;
    public int UsedCount => useList.Count;
    public int Count => dataStack.Count;
    public bool NeedCreate => useList.Count < maxNum;
    public PoolData(GameObject root, string name, GameObject useObj)
    {
        if (PoolMgr.isOpen)
        {
            rootObj = new GameObject();
            rootObj.name = name;
            rootObj.transform.SetParent(root.transform);
        }
        PushUsedList(useObj);
        PooObj pooObj = useObj.GetComponent<PooObj>();
        if (pooObj == null)
        {
            Debug.Log("��Ϊʹ�û���ع��ܵ�Ԥ����������PoolObj�ű������������������");
            return;
        }
        else maxNum = pooObj.maxNum;
    }
    public GameObject Pop()
    {

        GameObject obj;
        if (Count > 0)
        {
            obj = dataStack.Pop();
            useList.Add(obj);
        }
        else
        {
            obj = useList[0];
            useList.RemoveAt(0);
            useList.Add(obj);
        }
        obj.SetActive(true);
        if (PoolMgr.isOpen) { obj.transform.SetParent(null); }

        return obj;
    }
    public void Push(GameObject obj)
    {
        obj.SetActive(false);
        if (PoolMgr.isOpen)
            obj.transform.SetParent(rootObj.transform);
        dataStack.Push(obj);
        useList.Remove(obj);

    }
    public void PushUsedList(GameObject obj) { useList.Add(obj); }
}
#region �洢 ���ݽṹ=���߼�������������̳�mono���ࣩ
public abstract class Pool0bjectBase { };
public class PoolObject<T> : Pool0bjectBase where T : class
{
    public Queue<T> poolObjs = new Queue<T>();
}
public interface IPoolObject
{
    void RestInfo();
}
#endregion
public class PoolMgr : BaseManger<PoolMgr>
{
    #region ��������
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    /// <summary>
    /// ���ڴ洢��Ҫ����ʵ�����Ĳ��̳�mono��������
    /// </summary>
    private Dictionary<string, Pool0bjectBase> poolObjectDic = new Dictionary<string, Pool0bjectBase>();
    //���Ӹ�����
    private GameObject poolObj;
    //�Ƿ������ֹ���
    public static bool isOpen = true;
    #endregion
    #region ����
    private PoolMgr() { }
    /// <summary>
    /// �ö����ķ���
    /// </summary>
    /// <param name="name">��������������</param>
    /// <returns>�ӻ����ȡ���Ķ���</returns>
    #region �ӳ���ȡ����
    public GameObject GetObj(string name)
    {
        if (poolObj == null && PoolMgr.isOpen)
            poolObj = new GameObject("Pool");
        GameObject obj;

        if (!poolDic.ContainsKey(name) || poolDic[name].Count == 0 && poolDic[name].NeedCreate)
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;
            if (!poolDic.ContainsKey(name))
                poolDic.Add(name, new PoolData(poolObj, name, obj));
            else
                poolDic[name].PushUsedList(obj);
        }
        else
            obj = poolDic[name].Pop();

        return obj;
    }
    /// <summary>
    /// ��ѹ��ȡ��ѹ����һ�����س�ȥ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nameSpace">�����ڲ�ͬ�����ռ�ͬ����ʱ�����޸�</param>
    /// <returns></returns>
    public T GetObj<T>(string nameSpace = "") where T : class, IPoolObject, new()
    {
        string poolName = nameSpace + "_" + typeof(T).Name;
        if (poolObjectDic.ContainsKey(poolName))
        {
            PoolObject<T> pool = poolObjectDic[poolName] as PoolObject<T>;
            //�����Ƿ��п��Ը��õĶ���
            if (pool.poolObjs.Count > 0)
            {
                //�˴�����Ӧ�ñ�����û������
                T obj = pool.poolObjs.Dequeue();
                return obj;
            }
            else
            {
                T obj = new T();
                return obj;
            }
        }
        else
        {
            T obj = new T();
            return obj;
        }

    }
    #endregion
    #region �����зŶ���
    /// <summary>
    /// ������طŶ����ķ���
    /// </summary>
    /// <param name="name">������������</param>
    /// <param name="obj">ϣ������Ķ���</param>
    public void PushObj(GameObject obj)
    {

        //������������ʱ����ʧ�һ������

        //if (!poolDic.ContainsKey(obj.name))
        //    poolDic.Add(obj.name, new PoolData(poolObj, obj.name));
        poolDic[obj.name].Push(obj);
    }
    /// <summary>
    /// �Զ������ݽṹ����������
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <returns></returns>
    public void PushObj<T>(T obj, string nameSpace = "") where T : class, IPoolObject
    {if (obj == null)
            return;
        string poolName = nameSpace + "_" + typeof(T).Name;
        PoolObject<T> pool;
        if (poolObjectDic.ContainsKey(poolName))
        {
            pool = poolObjectDic[poolName] as PoolObject<T>;
        }
        else
        {
            pool = new PoolObject<T>();
            poolObjectDic.Add(poolName, pool);
        }
        //����ǰ��������
        obj.RestInfo();
        pool.poolObjs.Enqueue(obj);
    }
    #endregion
    #region ��ջ����
    /// <summary>
    /// ����������������е�����
    /// ʹ�ó�����Ҫ���г���ʱ
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
        poolObjectDic.Clear();
    }
    #endregion
    #endregion
}
