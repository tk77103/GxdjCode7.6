using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

/// <summary>
/// 2进制数据管理器 在加载表配置数据时候 先加载表 再调用 GetTable 获取表的容器
/// </summary>
public class BinaryDataMgr
{
    #region 数据容器
    /// <summary>
    /// 用于存储每张excle表数据的容器
    /// </summary>
    private Dictionary<string, object> tableDic = new Dictionary<string, object>();
    /// <summary>
    /// 存储数据的位置
    /// </summary>
    private static string SAVE_PATH = Application.persistentDataPath + "/Data/";
    /// <summary>
    /// 统一的二进制文件存放位置 在只读的streamingAssets下
    /// </summary>
    public static string DATA_BINARY_PATH = Application.streamingAssetsPath + "/BinaryData/";
    //常规的单例声明并配套 私有构造函数 继承小框架基类可去除
    //为防止单例声明对静态变量造成影响声明在静态变量后
    private static BinaryDataMgr instance = new BinaryDataMgr();
    public static BinaryDataMgr Instance => instance;
    #endregion
    #region 函数
    private BinaryDataMgr()
    {

    }
    public void InitData()
    {
    }
    /// <summary>
    /// 加载excel表的二进制数据到内存当中 注意规定了 读取文件的后缀一定是.hmzs 在加载表配置数据时候 先加载表 再调用 GetTable 获取表的容器
    /// </summary>
    /// <typeparam name="T">数据容器类</typeparam>
    /// <typeparam name="K">数据结构体类 类名于表名相同</typeparam>
    
    public void LoadTable<T, K>()
    {
        //读取excel生成的二进制文件来解析
        using (FileStream fs = File.Open(DATA_BINARY_PATH + typeof(K).Name + ".hmzs", FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            //用于记录当前读取了多少字节
            int index = 0;
            //二进制数据配置规则为;先记录了要读多少行数据 2.记录了excle表的key名
            //读取要读多少行excel行
            int count = BitConverter.ToInt32(bytes, index);
            index += sizeof(int);
            //读取主键名
            int keyNameLength = BitConverter.ToInt32(bytes, index);
            index += sizeof(int);
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
            index += keyNameLength;
            //通过反射创建excel数据容器类的实例
            Type contaninerType = typeof(T);
            object contaniner = Activator.CreateInstance(contaninerType);
            //得到数据结构类 通过反射可得到其所有变量
            Type classType = typeof(K);
            FieldInfo[] infos = classType.GetFields();
            //相当于每行构建一个数据结构类对象 并通过指定的键值存储到数据容器中 至此一张表的数据全部读取完成
            for (int i = 0; i < count; i++)
            {//实例化一个数据结构对象 并通过BinaryFormatter类反序列化出来
                object dataObj = Activator.CreateInstance(classType);
                //遍历excel数据结构类的所有字段
                foreach (FieldInfo info in infos)
                {   //判断字段类 将读取到的二进制字段转为对应类型值 并赋值给数据类型对象实例
                    if (info.FieldType == typeof(short))
                    {
                        info.SetValue(dataObj, BitConverter.ToInt16(bytes, index));
                        index += sizeof(short);
                    }
                    if (info.FieldType == typeof(int))
                    {
                        info.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                        index += sizeof(int);
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        info.SetValue(dataObj, BitConverter.ToSingle(bytes, index));
                        index += sizeof(float);
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        info.SetValue(dataObj, BitConverter.ToBoolean(bytes, index));
                        index += sizeof(bool);
                    }
                    else if (info.FieldType == typeof(string))
                    {
                        int strLength = BitConverter.ToInt32(bytes, index);
                        index += sizeof(int);
                        info.SetValue(dataObj, Encoding.UTF8.GetString(bytes, index, strLength));
                        index += strLength;
                    }
                }
                //为实例化的数据结构类赋值后将其存入声明好的数据容器对象中
                //获取某一确定字段(字典)信息 后获取到使用type实例化对象的 指定该字段
                object dicObject = contaninerType.GetField("dataDic").GetValue(contaniner);
                //得到该实例字典变量的add方法
                MethodInfo minfo = dicObject.GetType().GetMethod("Add");
                //第一步得到数据结构类信息中人为规定的做为key的字段（此处为id可为其他）的信息 并为绑定对象 获得其具体的值
                object keyValue = classType.GetField(keyName).GetValue(dataObj);
                //调用add方法 第一参为确定好对象字典变量 第二参中的数组可理解为该方法所传的参数 即字典的add方法
                minfo.Invoke(dicObject, new object[] { keyValue, dataObj });
            }
            //把读取出的一张表记录到记录所有表数据的字典中
            tableDic.Add(typeof(T).Name, contaniner);
            fs.Close();
        }
    }
    /// <summary>
    /// 得到一张表的数据容器类（含有字典容器）
    /// </summary>
    /// <typeparam name="T">表数据容器类名</typeparam>
    /// <returns></returns>
    public T GetTable<T>() where T : class
    {
        string tableName = typeof(T).Name;
        if (tableDic.ContainsKey(tableName))
            //为了能直接转换为数据容器类 故在tableDic处直接使用数据容器名做键
            return tableDic[tableName] as T;
        return null;
    }

    /// <summary>
    /// 存储类对象数据
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fileName"></param>
    public void Save(object obj, string fileName)
    {
        //先判断路径文件夹有没有
        if (!Directory.Exists(SAVE_PATH))
            Directory.CreateDirectory(SAVE_PATH);

        using (FileStream fs = new FileStream(SAVE_PATH + fileName + ".hmzs", FileMode.OpenOrCreate, FileAccess.Write))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
            fs.Close();
        }
    }

    /// <summary>
    /// 读取2进制数据转换成对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public T Load<T>(string fileName) where T : class
    {
        //如果不存在这个文件 就直接返回泛型对象的默认值
        if (!File.Exists(SAVE_PATH + fileName + ".hmzs")) {
           Debug.Log("未找到指定文件");
            return default(T);
        }
            
            

        T obj;
        Debug.Log("存在文件");
        using (FileStream fs = File.Open(SAVE_PATH + fileName + ".hmzs", FileMode.Open, FileAccess.Read))
        {
            BinaryFormatter bf = new BinaryFormatter();
            obj = bf.Deserialize(fs) as T;
            fs.Close();
        }

        return obj;
    }
    #endregion
}
