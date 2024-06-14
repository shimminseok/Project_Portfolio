using System.Collections.Generic;
using Tables;
using UI;

public class WorldMapGenMonsterReuseScrollRect : ReuseScrollview<WorldMapGenMonsterCellData>
{
    protected override void Start()
    {
        base.Start();
    }

    public void CreateSlot(Tables.Stage _st)
    {
        tableData = new List<WorldMapGenMonsterCellData>();
        int index = 0;

        Spawn spawnTb = Spawn.Get(UIWorldMap.instance.SelectStageTb.SpawnGroup);
        if (spawnTb != null)
        {
            for (int i = 0; i < spawnTb.MonsterIndex.Length; i++)
            {
                if (tableData.Find(x => x.m_MonsterTb.key == spawnTb.MonsterIndex[i]) == null)
                {
                    WorldMapGenMonsterCellData cell = new WorldMapGenMonsterCellData();
                    cell.Index = index++;
                    cell.m_MonsterTb = Monster.Get(spawnTb.MonsterIndex[i]);
                    tableData.Add(cell);
                }
            }
        }
        WorldMapGenMonsterCellData bossCell = new WorldMapGenMonsterCellData();
        bossCell.Index = index;
        bossCell.m_MonsterTb = Monster.Get(_st.BossIndex);
        tableData.Add(bossCell);
        InitTableView();
    }
}
