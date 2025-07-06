using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class TextUtil : MonoBehaviour
{
    #region 函数
    #region 字符串拆分相关
    #region 拆分字符串方法
    /// <summary>
    /// 拆分字符串方法（采用类型避免输入错误）
    /// </summary>
    /// <param name="str">要拆分的字符串</param>
    /// <param name="type">分割符类型：1--; 2--, 3--% 4--: 5--空格 6--| 7--_</param>
    /// <returns></returns>
    public static string[] SplitStr(string str, int type = 1)
    {
        if (str == "")
            return new string[0];
        string newStr = str;
        if (type == 1)
        {//为避免英文符号填成中文符号先进行替换
            while (newStr.IndexOf("；") != -1)
                newStr = newStr.Replace("；", ";");
            return newStr.Split(';');
        }
        else if (type == 2)
        {
            while (newStr.IndexOf("，") != -1)
                newStr = newStr.Replace("，", ",");
            return newStr.Split(',');
        }
        else if (type == 3)
        {
            return newStr.Split('%');
        }
        else if (type == 4)
        {
            while (newStr.IndexOf("：") != -1)
                newStr = newStr.Replace("：", ":");
            return newStr.Split(':');
        }
        else if (type == 5)
        {
            return newStr.Split(' ');
        }
        else if (type == 6)
        {
            return newStr.Split('|');
        }
        else if (type == 7)
        {
            return newStr.Split('_');
        }
        return new string[0];
    }
    #endregion
    #region 拆分字符串返回int数组
    /// <summary>
    /// 拆分字符串方法（采用类型避免输入错误）
    /// </summary>
    /// <param name="str">要拆分的字符串</param>
    /// <param name="type">分割符类型：1--; 2--, 3--% 4--: 5--空格 6--| 7--_</param>
    /// <returns></returns>
    public static int[] SplitStrToIntArr(string str, int type = 1)
    {
        string[] strs = SplitStr(str, type);
        if (strs.Length == 0)
            return new int[0];
        //把字符串数组转换成int数组
        return Array.ConvertAll<string, int>(strs, (str) =>
        {
            return int.Parse(str);
        });
    }
    #endregion
    #region  俩合并项分割
    /// <summary>
    ///  俩int合并项分割
    /// </summary> 俩int合并项拆分,必须满足类似"12_1,21_21"类似格式且不能有残项
    /// <param name="str">待拆分的字符串</param>
    /// <param name="typeOne">合并项目间的分隔符：1--; 2--, 3--% 4--: 5--空格 6--| 7--_</param>
    /// <param name="typeTwo">俩合并项间的分隔符：1--; 2--, 3--% 4--: 5--空格 6--| 7--_</param>
    /// <param name="callBack">使用拆分出的俩个int数据的方法</param>
    public static void SplitStrToIntArrTwice(string str, int typeOne, int typeTwo, UnityAction<int, int> callBack)
    {
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0) return;
        int[] ints;
        for (int i = 0; i < strs.Length; i++)
        {
            ints = SplitStrToIntArr(strs[i], typeTwo);
            if (ints.Length == 0) continue;
            callBack.Invoke(ints[0], ints[1]);
        }
    }
    #endregion
    #region  string合并项分割
    /// <summary>
    ///  string合并项分割
    /// </summary> 俩string合并项拆分,必须满足类似"12_1,21_21"类似格式且不能有残项
    /// <param name="str">待拆分的字符串</param>
    /// <param name="typeOne">合并项目间的分隔符：1--; 2--, 3--% 4--: 5--空格 6--| 7--_</param>
    /// <param name="typeTwo">俩合并项间的分隔符：1--; 2--, 3--% 4--: 5--空格 6--| 7--_</param>
    /// <param name="callBack">使用拆分出的俩个string数据的方法</param>
    public static void SplitStrTwice(string str, int typeOne, int typeTwo, UnityAction<string, string> callBack)
    {
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0) return;
        string[] strs2;
        for (int i = 0; i < strs.Length; i++)
        {
            strs2 = SplitStr(strs[i], typeTwo);
            if (strs2.Length == 0) continue;
            callBack.Invoke(strs2[0], strs2[1]);
        }
    }
    #endregion
    #endregion
    #region 数字转字符串相关
    #region 数字转字符添0
    /// <summary>
    /// 数字转字符添0
    /// </summary>
    /// <param name="value">数值</param>
    /// <param name="length">长度</param>
    /// <returns></returns>
    public static string GetNumStr(int value, int length)
    {   //tostring中传入一个Dn的字符串
        //代表将数字转换为长度为n的字符串 如果长度不够会在前面补0 超过没有用
        return value.ToString($"D{length}");
    }
    #endregion
    #region 浮点数转字符保留n位小数
    /// <summary>
    /// 让浮点输保留小数点后n位
    /// </summary>
    /// <param name="value">具体的浮点数</param>
    /// <param name="len">保留位数</param>
    /// <returns></returns>
    public static string GetDecimalStr(float value, int len)
    {//tostring 传入一个fn的字符串 代表想要保留小数点后几位数
        return value.ToString($"F{len}");
    }
    #endregion
    #region 大数值转换字符串
    public static string GetBigDataToString(long value)
    {//大于亿显示几亿几千万
        if (value >= 100000000)
        {
            return BigDataChange(value, 100000000, "亿", "千万");
        }
        //大于万显示几万几千
        else if (value >= 10000)
        {
            return BigDataChange(value, 10000, "万", "千");
        }
        else return value.ToString();
    }
    /// <summary>
    /// 大数据转换成对应的字符串拼接
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="company">转换最大单位int</param>
    /// <param name="bigCompany">最大单位 如 亿</param>
    /// <param name="littleCompany">次级单位如 千万</param>
    /// <returns></returns>
    public static string BigDataChange(long value, int company,
        string bigCompany, string littleCompany)
    {
        resultStr.Clear();
        resultStr.Append(value / company);
        resultStr.Append(bigCompany);
        int tmpNum = (int)(value % company);
        tmpNum /= (company / 10);
        if (tmpNum != 0)
        {
            resultStr.Append(tmpNum);
            resultStr.Append(littleCompany);
        }
        return resultStr.ToString();

    }
    #endregion
    #endregion
    #region 时间转换相关
    #region 秒数转时分秒
    private static StringBuilder resultStr = new StringBuilder("");
    /// <summary>
    /// 秒数转时分秒
    /// </summary>
    /// <param name="s"></param>
    /// <param name="egZero">是否忽略0</param>
    /// <param name="isKeepLen">是否保留俩位</param>
    /// <param name="hourStr">小时拼接字符</param>
    /// <param name="minuteStr">分钟拼接字符</param>
    /// <param name="secondStr">秒拼接字符</param>
    /// <returns></returns>
    public static string SecondToHMS(int s, bool egZero = false, bool isKeepLen = false,
        string hourStr = "时", string minuteStr = "分", string secondStr = "秒")
    {
        if (s < 0)
            s = 0;
        int hour = s / 3600;
        int second = s % 3600;
        int minute = second / 60;
        second = s % 60;
        resultStr.Clear();
        //如果小时不为0 或者不忽略0
        if (hour != 0 || !egZero)
        {
            resultStr.Append(isKeepLen ? GetNumStr(hour, 2) : hour);
            resultStr.Append(hourStr);
        }
        if (minute != 0 || !egZero || hour != 0)
        {
            resultStr.Append(isKeepLen ? GetNumStr(minute, 2) : minute);
            resultStr.Append(minuteStr);
        }
        if (second != 0 || !egZero || hour != 0 || minute != 0)
        {
            resultStr.Append(isKeepLen ? GetNumStr(second, 2) : second);
            resultStr.Append(secondStr);
        }
        if (resultStr.Length == 0)
        {
            resultStr.Append(0);
            resultStr.Append(secondStr);
        }
        return resultStr.ToString();
    }
    #endregion
    #region 秒转00:00:00的方法
    /// <summary>
    /// 秒转00:00:00的方法
    /// </summary>
    /// <param name="s"></param>
    /// <param name="egZero"></param>
    /// <returns></returns>
    public static string SecondToHMS2(int s, bool egZero = false)
    {
        return SecondToHMS(s, egZero, true, ":", ":", "");
    }
    #endregion 
    #endregion

    #endregion
}
