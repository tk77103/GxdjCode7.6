using System.Collections.Generic;
using UnityEngine;

public class Company
{
    #region һ�μ���
    //ID
    public short ID;
    //��˾��
    public string Name;
    //��������
    public string area;
    //��˾�ȼ�
    public int companyLevel;
    //��˾��ҵ
    public string Industry;
    //ϸ�ֹ���
    public string segmentationFunction;
    //ѧ��ƫ��
    public string academicBias;
    //�Ƿ���ʾ
    public bool isVisible;
    //��������
    public short scnCost;
    //�ɷ���
    public bool canWork;
    //�ɷ��ְ
    public bool canPartTime;
    //�˶�����
    public short humanAmount;
    //��˾��ͼλ��
    public Vector2 comPos;
    //��ͼͼ��·��δ��
    public string mapBtnImg;
    //��ͼͼ��ѡ��
    public string mpcBtnImgChosen;
    #endregion
    //��˾��������
    public Dictionary<string, List<PositionCapacity>> companyScene = new();
}