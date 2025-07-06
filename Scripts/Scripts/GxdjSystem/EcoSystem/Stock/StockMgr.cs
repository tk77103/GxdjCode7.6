using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class StockTrade
{
    public int stockId;
    public float stockCount;
}
public class StockMgr : BaseManger<StockMgr>
{
    #region 数据容器
    //这个表当有自营企业上市时会改变
    public Dictionary<int, Stock> allStocks = new();
    List<int> stockIds = new();
    System.Random r = new();
    #endregion
    #region 函数
    private StockMgr()
    {
        BinaryDataMgr.Instance.LoadTable<StockContainer, Stock>();
        allStocks = BinaryDataMgr.Instance.GetTable<StockContainer>().dataDic;
        allStocks.Values.ToList().ForEach(e => e.originalPrice = e.initStockPrice);
        stockIds = allStocks.Keys.ToList();
    }
    #region 初始生成npc持有股票
    public List<StockTrade> StockOriginGenerate(short npcLev, string companyName)
    {
        int i = r.Next(0, 101);
        List<StockTrade> t = new();
        switch (npcLev)
        {
            case 0:
            case 1:
                return null;
            case 2:
                t.Add(new StockTrade()
                {
                    stockId = stockIds.GetRandomItem(),
                    stockCount = (int)(10 * r.NextFloat(0.8f, 1.2f))
                });
                t.Add(new StockTrade()
                {
                    stockId = stockIds.Filter(e => e != t[0].stockId).GetRandomItem(),
                    stockCount = (int)(10 * r.NextFloat(0.8f, 1.2f))
                });
                return t;
            case 3:
                t.Add(new StockTrade()
                {
                    stockId = stockIds.GetRandomItem(),
                    stockCount = (int)(100 * r.NextFloat(0.8f, 1.2f))
                });
                t.Add(new StockTrade()
                {
                    stockId = stockIds.Filter(e => e != t[0].stockId).GetRandomItem(),
                    stockCount = (int)(100 * r.NextFloat(0.8f, 1.2f))
                });
                return t;
            case 4:
                if (allStocks.FilterValues(e => e.stockName == companyName).Any())
                    t.Add(new StockTrade()
                    {
                        stockId = allStocks.FilterValues(e => e.stockName == companyName)[0].stockId,
                        stockCount = (int)(250 * r.NextFloat(0.8f, 1.2f))
                    });
                else
                {
                    t.Add(new StockTrade()
                    {
                        stockId = stockIds.GetRandomItem(),
                        stockCount = (int)(250 * r.NextFloat(0.8f, 1.2f))
                    });
                }
                t.Add(new StockTrade()
                {
                    stockId = stockIds.Filter(e => e != t[0].stockId).GetRandomItem(),
                    stockCount = (int)(250 * r.NextFloat(0.8f, 1.2f))
                });
                t.Add(new StockTrade()
                {
                    stockId = stockIds.Filter(e => e != t[0].stockId && e != t[1].stockId).GetRandomItem(),
                    stockCount = (int)(250 * r.NextFloat(0.8f, 1.2f))
                });
                return t;
            case 5:
                if (allStocks.FilterValues(e => e.stockName == companyName).Any())
                    t.Add(new StockTrade()
                    {
                        stockId = allStocks.FilterValues(e => e.stockName == companyName)[0].stockId,
                        stockCount = (int)(1000 * r.NextFloat(0.8f, 1.2f))
                    });
                else
                {
                    t.Add(new StockTrade()
                    {
                        stockId = stockIds.GetRandomItem(),
                        stockCount = (int)(1000 * r.NextFloat(0.8f, 1.2f))
                    });
                }
                t.Add(new StockTrade()
                {
                    stockId = stockIds.Filter(e => e != t[0].stockId).GetRandomItem(),
                    stockCount = (int)(1000 * r.NextFloat(0.8f, 1.2f))
                });
                t.Add(new StockTrade()
                {
                    stockId = stockIds.Filter(e => e != t[0].stockId && e != t[1].stockId).GetRandomItem(),
                    stockCount = (int)(1000 * r.NextFloat(0.8f, 1.2f))
                });
                return t;
            default:
                return null;
        }
    }
    #endregion
    #region 股市变动初次
    public void FirstMonthStockChange()
    {
        allStocks.Values.ToList().ForEach(stock => { stock.priceChgRt += r.NextFloat(-0.1f, 0.1f); });
    }
    #endregion
    #region 股市变动
    /// <summary>
    /// 模拟股市的变动
    /// </summary>
    /// <param name="stock"></param>
    public void StockChange(Stock stock)
    {
        int i = UnityEngine.Random.Range(1, 101);

        if (stock.isFirstMoonIn)
        {
            stock.originalPrice = stock.initStockPrice;
            stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0.1f);
            stock.isFirstMoonIn = false;
        }
        else
        {
            switch (stock.priceChgRt)
            {

                case > -0.1f and < -0.05f:
                    if (i <= 55)
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    break;

                case > -0.15f and < -0.1f:
                    if (i <= 60)
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    break;

                case > -0.2f and < -0.15f:
                    if (i <= 65)
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    break;

                case > -0.25f and < -0.2f:
                    if (i <= 70)
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    break;

                case > -0.3f and < -0.25f:
                    if (i <= 75)
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    break;

                case > -0.35f and < -0.3f:
                    if (i <= 80)
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    break;

                case > -0.4f and < -0.35f:
                    if (i <= 90)
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    break;

                case <= -0.4f:
                    if (i <= 90)
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    break;
                case >0 and < 0.1f:
                    if (i <= 55)
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    break;

                case > 0.1f and < 0.15f:
                    if (i <= 55)
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    break;

                case > 0.15f and < 0.2f:
                    if (i <= 60)
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    break;

                case > 0.2f and < 0.25f:
                    if (i <= 65)
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    break;

                case > 0.25f and < 0.3f:
                    if (i <= 70)
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    break;

                case > 0.3f and < 0.35f:
                    if (i <= 75)
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    break;

                case > 0.35f and < 0.4f:
                    if (i <= 85)
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    break;


                case >= 0.4f:
                    if (i <= 90)
                        stock.priceChgRt += UnityEngine.Random.Range(-0.1f, 0);
                    else
                        stock.priceChgRt += UnityEngine.Random.Range(0, 0.1f);
                    break;

            }

        }
        //当前价格
        stock.currStockPrice = stock.priceChgRt * stock.initStockPrice;
        //累计涨幅
        stock.cumulativeIncrease = (stock.currStockPrice - stock.originalPrice) / stock.originalPrice;
    }
    #endregion
    #region npc购入股票 判断与行为同行
    public void BuyStock(NpcBase npc)
    {
        int safeMoney = Wealth.Instance.WealthMinAmount(npc.socialRank - 1);
        int i = r.Next(1, 101);
        int investMoney = 0;
        if (npc.money > safeMoney)
        {
            Stock iWillBuy = allStocks.GetRandomItem().Value;
            switch (npc.investTendency)
            {
                case 1:
                    if (i <= 10) investMoney = (int)((npc.money - safeMoney) * 0.05f);
                    break;
                case 2:
                    if (i <= 15) investMoney = (int)((npc.money - safeMoney) * 0.1f);
                    break;
                case 3:
                    if (i <= 25) investMoney = (int)((npc.money - safeMoney) * 0.15f);
                    break;
                case 4:
                    if (i <= 30) investMoney = (int)((npc.money - safeMoney) * 0.20f);
                    break;
                case 5:
                    if (i <= 35) investMoney = (int)((npc.money - safeMoney) * 0.25f);
                    break;
            }
            if (investMoney > 0)
            {
                if (iWillBuy != null)
                {
                    npc.money -= investMoney;
                    if (npc.stock.Any())
                    {
                        if (npc.stock.Filter(e => e.stockId == iWillBuy.stockId).Any())
                        {
                            npc.stock.Filter(e => e.stockId == iWillBuy.stockId)[0].stockCount +=
                                investMoney / allStocks[iWillBuy.stockId].currStockPrice;
                        }
                        else
                        {
                            npc.stock.Add(new StockTrade()
                            {
                                stockId = iWillBuy.stockId,
                                stockCount = investMoney / allStocks[iWillBuy.stockId].currStockPrice
                            });
                        }
                    }
                    else
                    {
                        npc.stock.Add(new StockTrade()
                        {
                            stockId = iWillBuy.stockId,
                            stockCount = investMoney / allStocks[iWillBuy.stockId].currStockPrice
                        });
                    }
                }
            }
        }
    }
    #endregion
    #region npc卖出股票 判断与行为同行
    public void SellStock(NpcBase npc)
    {
        int safeMoney = Wealth.Instance.WealthMinAmount(npc.socialRank - 1);
        if (npc.stock.Any())
        {
            if (npc.money < safeMoney)
            {
                //此处未对卖出股票做规定 只好随机模拟对股票一头雾水的大众
                npc.stock = npc.stock.Shuffle(npc.stock.Count);
                for (int i = npc.stock.Count - 1; i >= 0; i--)
                {
                    if (npc.money >= safeMoney)
                        break;
                    npc.money += (int)(allStocks[npc.stock[i].stockId].currStockPrice * npc.stock[i].stockCount);
                    npc.stock.RemoveAt(i);
                }

            }
            else
            {//百分之五概率卖出投资收益大于0.3的股票
                for (int i = npc.stock.Count - 1; i >= 0; i--)
                {
                    if (allStocks[npc.stock[i].stockId].cumulativeIncrease >= 0.3f && r.Next(1, 101) <= 5)
                    {
                        npc.money += (int)(allStocks[npc.stock[i].stockId].currStockPrice * npc.stock[i].stockCount);
                        npc.stock.RemoveAt(i);
                    }
                }
            }
        }
    }
    #endregion
    #region 每年总计
    public void YearEndSummary()
    {
        allStocks.Values.ToList().ForEach(e => e.initStockPrice = e.currStockPrice);
    }
    #endregion
    #region 获取股票价值
    public float GetStockValue(StockTrade myStock)
    {
        return allStocks[myStock.stockId].currStockPrice * myStock.stockCount;
    }
    #endregion
    #endregion
}
