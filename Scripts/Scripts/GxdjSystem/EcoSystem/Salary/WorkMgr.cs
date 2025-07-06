using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WorkMgr : BaseManger<WorkMgr>
{
    #region ����
    #endregion
    Dictionary<short, Salary>     worldSalary;
    #region ����
    private WorkMgr()
    {
        BinaryDataMgr.Instance.LoadTable<SalaryContainer, Salary>();
        worldSalary = BinaryDataMgr.Instance.GetTable<SalaryContainer>().dataDic;
    }
    #region нˮ���
    public float MatchSalaryFirstAmendment(NpcBase npc)
    {
        //ƥ����ڹ�˾������ҵ��npcְ��ȼ� npcְ���ν
        if (npc.jobLevel >= 1)
        {
            if (worldSalary.FilterValues(e => e.rank == npc.jobLevel && e.sort == npc.industry && e.oralName == npc.jobTitle).Any())
                return worldSalary.FilterValues(e => e.rank == npc.jobLevel && e.sort == npc.industry && e.oralName == npc.jobTitle)[0].income*DegreeForSalary(npc.degree);
            else
            {
                Debug.Log($"δ��Ϊnpc{npc.Id}��{npc.jobLevel},{npc.industry},{npc.jobTitle}�ҵ���Ӧ��н��");
                return 0;
            }
        }
        else
            //��ҵ��������
            return 1000;
    }
    #region
    #region ѧ��н��ϵ��
    private float DegreeForSalary(Degree degree)
    {
        switch (degree){
            case Degree.����:
                return 1;
            case Degree.����:
                return 1.25f;
            case Degree.ѧʿ:
                return 1.5f;
            case Degree.˶ʿ:
                return 2f;
            case Degree.��ʿ:
                return 2.5f;
            default:
                return 1;
                
        }
    }
    #endregion
    #endregion
    #endregion
    #region ��ְ���
    #endregion
    #endregion
}
