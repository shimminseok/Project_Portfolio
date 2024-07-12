using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class SkillListReuseScrollRect : ReuseScrollview<SkillListCellData>
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CreateSkillListSlot();
    }

    public void CreateSkillListSlot()
    {
        tableData = new List<SkillListCellData>();
        int index = 0;
        var skillDic = Tables.Skill.data.Values.ToList().OrderBy(x => x.SkillGrade);
        foreach (var skill in skillDic)
        {
            SkillListCellData cell = new SkillListCellData();
            cell.Index = index++;
            cell.m_skill = skill;
            cell.isSelected = false;
            tableData.Add(cell);
        }

        InitTableView();
    }
}
