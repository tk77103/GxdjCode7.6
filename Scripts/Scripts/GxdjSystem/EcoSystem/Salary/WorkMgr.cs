using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WorkMgr : BaseManger<WorkMgr>
{
    #region 容器
    #endregion
    Dictionary<short, Salary>     worldSalary;
    #region 函数
    private WorkMgr()
    {
        BinaryDataMgr.Instance.LoadTable<SalaryContainer, Salary>();
        worldSalary = BinaryDataMgr.Instance.GetTable<SalaryContainer>().dataDic;
    }
    #region 薪水相关
    public float MatchSalaryFirstAmendment(NpcBase npc)
    {
        //匹配基于公司所处行业，npc职务等级 npc职务称谓
        if (npc.jobLevel >= 1)
        {
            if (worldSalary.FilterValues(e => e.rank == npc.jobLevel && e.sort == npc.industry && e.oralName == npc.jobTitle).Any())
                return worldSalary.FilterValues(e => e.rank == npc.jobLevel && e.sort == npc.industry && e.oralName == npc.jobTitle)[0].income*DegreeForSalary(npc.degree);
            else
            {
                Debug.Log($"未能为npc{npc.Id}，{npc.jobLevel},{npc.industry},{npc.jobTitle}找到对应的薪资");
                return 0;
            }
        }
        else
            //无业保障收入
            return 1000;
    }
    #region
    #region 学历薪酬系数
    private float DegreeForSalary(Degree degree)
    {
        switch (degree){
            case Degree.初中:
                return 1;
            case Degree.高中:
                return 1.25f;
            case Degree.学士:
                return 1.5f;
            case Degree.硕士:
                return 2f;
            case Degree.博士:
                return 2.5f;
            default:
                return 1;
                
        }
    }
    #endregion
    #endregion
    #endregion
    #region 入职相关
    #endregion
    #endregion
}
