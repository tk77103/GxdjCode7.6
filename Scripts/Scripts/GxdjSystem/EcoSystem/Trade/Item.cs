public class Item
{
 #region 数据容器
    //物品ID
    public int id;
    //大类
    public string type;
    //小类
    public string sort;
    //使用及装备的性别限制：1为男，2为女，0为无限制
    public short genderLimit;
    //等级
    public short level;
    //名称
    public string name;
    //出售地点
    public string sellPlace;
    //价格
    public int price;
    //描述（此处的描述是基准值，不考虑涉及浮动系数的情况）
    public string note;
    //每月使用限制次数
    public short useTIme;
    //使用是否消耗
    public bool useUp;
    //数量
    public int Num;
 #endregion
}