using System.Collections.Generic;
using UnityEngine;

public class Company
{
    #region 一次加载
    //ID
    public short ID;
    //公司名
    public string Name;
    //所在区域
    public string area;
    //公司等级
    public int companyLevel;
    //公司行业
    public string Industry;
    //细分功能
    public string segmentationFunction;
    //学术偏向
    public string academicBias;
    //是否显示
    public bool isVisible;
    //场景消费
    public short scnCost;
    //可否工作
    public bool canWork;
    //可否兼职
    public bool canPartTime;
    //核定人数
    public short humanAmount;
    //公司地图位置
    public Vector2 comPos;
    //地图图标路径未亮
    public string mapBtnImg;
    //地图图标选中
    public string mpcBtnImgChosen;
    #endregion
    //公司场景集合
    public Dictionary<string, List<PositionCapacity>> companyScene = new();
}