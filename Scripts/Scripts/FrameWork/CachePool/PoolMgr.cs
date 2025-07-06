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
            Debug.Log("请为使用缓存池功能的预设体对象挂载PoolObj脚本，用于设置最大数量");
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
#region 存储 数据结构=和逻辑类的容器（不继承mono的类）
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
    #region 数据容器
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    /// <summary>
    /// 用于存储需要经常实例化的不继承mono的数据类
    /// </summary>
    private Dictionary<string, Pool0bjectBase> poolObjectDic = new Dictionary<string, Pool0bjectBase>();
    //池子根对象
    private GameObject poolObj;
    //是否开启布局功能
    public static bool isOpen = true;
    #endregion
    #region 函数
    private PoolMgr() { }
    /// <summary>
    /// 拿东西的方法
    /// </summary>
    /// <param name="name">抽屉容器的名字</param>
    /// <returns>从缓存池取出的对象</returns>
    #region 从池中取东西
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
    /// 有压则取无压创建一个返回出去
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nameSpace">用于在不同命名空间同类名时进行修改</param>
    /// <returns></returns>
    public T GetObj<T>(string nameSpace = "") where T : class, IPoolObject, new()
    {
        string poolName = nameSpace + "_" + typeof(T).Name;
        if (poolObjectDic.ContainsKey(poolName))
        {
            PoolObject<T> pool = poolObjectDic[poolName] as PoolObject<T>;
            //池中是否有可以复用的对象
            if (pool.poolObjs.Count > 0)
            {
                //此处对象应该被重置没有引用
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
    #region 往池中放对象
    /// <summary>
    /// 往缓存池放东西的方法
    /// </summary>
    /// <param name="name">抽屉对象的名字</param>
    /// <param name="obj">希望放入的对象</param>
    public void PushObj(GameObject obj)
    {

        //将对象放入池中时候将其失活，一会再用

        //if (!poolDic.ContainsKey(obj.name))
        //    poolDic.Add(obj.name, new PoolData(poolObj, obj.name));
        poolDic[obj.name].Push(obj);
    }
    /// <summary>
    /// 自定义数据结构类放入池子中
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
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
        //放入前重置数据
        obj.RestInfo();
        pool.poolObjs.Enqueue(obj);
    }
    #endregion
    #region 清空缓存池
    /// <summary>
    /// 用于清除整个柜子中的数据
    /// 使用场景主要是切场景时
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
