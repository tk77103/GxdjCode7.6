public class Stock
{
 #region 数据容器
    //股票id
    public int stockId;
    //股票名称
    public string stockName;
    //板块门类
    public string stockCat;
    //是否是第一个月入市
    public bool isFirstMoonIn;
    //年初价格
    public float initStockPrice;
    //初始价格
    public float originalPrice;
    //标准价格
    public float baseStockPrice;
    //购入价格
    public float purchasePrice;
    //当年涨跌幅度
    public float priceChgRt;
    //累计涨幅
    public float cumulativeIncrease;
    //预期涨跌幅度
    public float stockPreview;
    //当前价格
    public float currStockPrice;
    //是否持有
    public bool isStockHeld;
    //当前持股
    public int currShares;
    //交易数量
    public int stockTradeQty;
    //交易金额
    public int stockTradeAmt;
    //证券价值
    public int stockValue;
    //累计支出
    public int stockTotalExp;
    //累计收入
    public int stockTotalInc;
    //累计收益
    public int stockTotalProfit;
 #endregion
}