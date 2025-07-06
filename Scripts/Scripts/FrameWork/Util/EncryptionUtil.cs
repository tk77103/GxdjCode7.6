using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 加密工具 实现加密功能
/// </summary>
public class EncryptionUtil
{
    #region 函数
    #region 获取随机密钥
    /// <summary>
    /// 返回一个整型密值
    /// </summary>
    /// <returns></returns>
    public static int GetRandomKey()
    {
        return Random.Range(1, 10000);
    }
    #endregion
    #region 加密数据
    /// <summary>
    /// 自己在这写一套带入key的算法即可
    /// </summary>
    /// <param name="value"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static int LockValue(int value, int key)
    {
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        value += key;
        return value;
    }
    public static long LockValue(long value, int key)
    {
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        value += key;
        return value;
    }
    #endregion
    #region 解密数据
    public static int UnLockValue(int value, int key)
    {//有可能还没有加密过 没有初始化过数据 直接想要获取就不用解密了
        if (value == 0) return value;
        value -= key;
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        return value;
    }
    public static long UnLockValue(long value, int key)
    {//value为0可能为其初始值没有初始化过 直接想要获取就不用解密了
        if (value == 0) return value;
        value -= key;
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        return value;
    }
    #endregion
    #endregion
}
