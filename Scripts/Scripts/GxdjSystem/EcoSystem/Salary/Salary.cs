public class Salary
{
 #region 数据容器
    //ID
    public short id;
    //行业分类
    public string sort;
    //职务等级
    public short rank;
    //职称
    public string oralName;
    //职场基于场景的细化称呼
    public string visibleName;
    //基准收入
    public int income;
    //主角可否从事
    public bool available;
    //从业等级要求
    public short levelNeed;
    //从业专精要求
    public short masterNeed;
    //从业魅力要求
    public short charmNeed;
 #endregion
}