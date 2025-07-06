public class Estate
{
    #region 数据容器
    //物业ID
    public int estId;
    //区域名称
    public string estArea;
    //基准价格
    public int estBasePrice;
    //价格系数
    public float estAreaIndex;
    //X坐标
    public int estX;
    //Y坐标
    public int estY;
    //房产名称
    public string estName;
    //房产类别 0 生活住所 1 商业楼房
    public short estType;
    //房产等级
    public short estRank;
    //房产档次
    public string estRankTag;
    //单元号（A-Z）
    public string estUnit;
    //门牌号（01-99）
    public short estNum;
    //房产面积
    public int estSquare;
    //房产描述
    public string estNote;
    //周围人口
    public int estPopulaiton;
    //平米售价
    public int setMeterSellPrice;
    //平米租价
    public int setMeterRentPrice;
    //售价
    public int estSellPrice;
    //租价
    public int estRentPrice;
    //是否持有
    public bool estIsHold;
    //是否入住
    public bool estIsLiving;
    //是否租赁
    public bool estIsRent;
    //是否租出
    public bool estIsRentout;
    //是否开设
    public bool estIsCpn;
    //是否有房贷
    public bool hasLoan;
    //考虑一个参数“类型系数”，住宅类型系数是1，办公间类型系数是2.基准价格*价格系数*人口系数*类型系数*房产营销行业趋势值=平米单价。将计算出来的结果填入“平米单价”列。平米单价*面积=售价，将计算结果填入售价列。平米租价是平米单价的1/200，租价是售价的1/200。
    public string est;
    #endregion
    #region 函数
    public Estate() { }
    public Estate(Estate estate)
    {
        // Copy all fields from input estate
        estId = estate.estId;
        estArea = estate.estArea;
        estBasePrice = estate.estBasePrice;
        estAreaIndex = estate.estAreaIndex;
        estX = estate.estX;
        estY = estate.estY;
        estName = estate.estName;
        estType = estate.estType;
        estRank = estate.estRank;
        estRankTag = estate.estRankTag;
        estUnit = estate.estUnit;
        estNum = estate.estNum;
        estSquare = estate.estSquare;
        estNote = estate.estNote;
        estPopulaiton = estate.estPopulaiton;
        setMeterSellPrice = estate.setMeterSellPrice;
        setMeterRentPrice = estate.setMeterRentPrice;
        estSellPrice = estate.estSellPrice;
        estRentPrice = estate.estRentPrice;
        estIsHold = estate.estIsHold;
        estIsLiving = estate.estIsLiving;
        estIsRent = estate.estIsRent;
        estIsRentout = estate.estIsRentout;
        estIsCpn = estate.estIsCpn;
        hasLoan = estate.hasLoan;
        est = estate.est;
    }
    #endregion
}