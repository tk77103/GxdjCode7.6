public class IndustyTrend
{
 #region 数据容器
    //趋势ID
    public int id;
    //每月触发概率
    public string coincidence;
    //趋势类型：1-大利好，2-小利好，3-小利空，4-大利空
    public string trendType;
    //趋势名称
    public string trendName;
    //趋势行业
    public string Industry;
    //趋势风向
    public string Note;
    //趋势系数
    public float trendIndex;
 #endregion
}