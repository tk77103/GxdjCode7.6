using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class ExtensionsFunc
{

    #region 函数
    #region 用于字典
    #region 洗牌，实现字典项的序列打乱 返回n项的字典值
    #endregion
    #region 为字典添加的扩展方法，返回符合条件的值list
    /// <summary>
    /// 为字典添加的扩展方法，返回符合条件的值list
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="dict"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<T> FilterValues<TKey, T>(this Dictionary<TKey, T> dict, Func<T, bool> predicate)
    {
        return dict.Values.Where(predicate).ToList();
    }
    #endregion
    #region 返回满足条件的字典
    /// <summary>
    /// 返回满足条件的字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Dictionary<TKey, TValue> FilterValuesToDic<TKey, TValue>(
    this Dictionary<TKey, TValue> source,
    Func<KeyValuePair<TKey, TValue>, bool> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return source
            .Where(predicate)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    #endregion
    #region 返回字典中的随机一项
    public static KeyValuePair<TKey, TValue> GetRandomItem<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        if (dictionary == null || dictionary.Count == 0)
            throw new InvalidOperationException("字典为空或未初始化");

        var keys = dictionary.Keys.ToList();
        var random = new System.Random(); // 创建 Random 实例
        int randomIndex = random.Next(keys.Count); // 使用实例方法

        TKey key = keys[randomIndex];
        return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
    }
    #endregion
    #endregion

    #region 用于列表
    #region （洗牌）实现list项的序列打乱 返回一个前n项的list
    /// <summary>
    /// （洗牌）实现list项的序列打乱 返回一个前n项的list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="myList">要打乱的list</param>
    /// <param name="n">返回多少项，若长度不足，则返回全部</param>
    /// <returns></returns>
    public static List<T> Shuffle<T>(this List<T> myList, int n)
    {
        var shuffleList = new List<T>(myList);
        System.Random r = new();
        int j = 0;
        for (int i = shuffleList.Count - 1; i > 0; i--)
        {
            j = r.Next(i + 1);
            T temp = shuffleList[j];
            shuffleList[j] = shuffleList[i];
            shuffleList[i] = temp;
        }
        return shuffleList.Take(n).ToList();
    }
    #endregion
    #region 为list实现的筛选方法 返回满足条件的list
    public static List<T> Filter<T>(this List<T> list, Func<T, bool> predicate)
    {
        return list.Where(predicate).ToList();
    }
    #endregion
    #region  获取列表中根据指定字段比较的最大对象
    /// <summary>
    /// 获取列表中根据指定字段比较的最大对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TKey">比较字段类型，必须实现 IComparable</typeparam>
    /// <param name="list">目标列表</param>
    /// <param name="keySelector">选择比较字段的 lambda 表达式</param>
    /// <returns>最大对象，若列表为空则返回 default(T)</returns>
    public static T MaxBy<T, TKey>(this List<T> list, Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        if (list == null || !list.Any())
            return default(T);

        return list.Aggregate((max, current) =>
            keySelector(max).CompareTo(keySelector(current)) > 0 ? max : current);
    }
    #endregion
    #region 获取列表中根据指定字段比较的最小对象
    /// <summary>
    /// 获取列表中根据指定字段比较的最小对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TKey">比较字段类型，必须实现 IComparable</typeparam>
    /// <param name="list">目标列表</param>
    /// <param name="keySelector">选择比较字段的 lambda 表达式</param>
    /// <returns>最小对象，若列表为空则返回 default(T)</returns>
    public static T MinBy<T, TKey>(this List<T> list, Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        if (list == null || !list.Any())
            return default(T);

        return list.Aggregate((min, current) =>
            keySelector(min).CompareTo(keySelector(current)) < 0 ? min : current);
    }
    #endregion
    #region 获取列表中随机的一项
    public static T GetRandomItem<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("List cannot be null or empty.");

        System.Random random = new System.Random();
        return list[random.Next(list.Count)];
    }
    #endregion
    #endregion

    #region 用于random
    #region 封装并可以返回一个随机的float
    public static float NextFloat(this System.Random random, float min, float max)
    {
        return min + (max - min) * (float)random.NextDouble();
    }
    #endregion
    #endregion

    #endregion
}
