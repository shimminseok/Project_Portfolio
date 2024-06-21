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
        foreach (var skill in Tables.Skill.data.Values)
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
