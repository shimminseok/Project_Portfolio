using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public static Navigation Instance;
    public List<Vector3> monsterSpawnPoints = new List<Vector3>();
    public Node start;
    public GameObject m_MapRoot;

    public Vector2 m_GroundSize;
    public Node[,] m_Map;

    public List<Node> path;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void CreateMap(Map map)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Maps/Map_01"), m_MapRoot.transform);

        m_GroundSize = map.GroundSize;

        m_Map = map.MapNode;
        SetMoveableMapNode();
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (m_Map == null)
            return;

        for (int i = 0; i < m_GroundSize.x; i++)
        {
            for (int j = 0; j < m_GroundSize.y; j++)
            {
                if (m_Map[i, j] == start)
                    Gizmos.color = Color.blue - new Color(0, 0, 0, 0.5f);
                else if (monsterSpawnPoints.Find(x => x == m_Map[i,j].Position) != Vector3.zero)
                    Gizmos.color = Color.magenta - new Color(0, 0, 0, 0.5f);
                else if (m_Map[i, j].Moveable)
                    Gizmos.color = Color.green - new Color(0, 0, 0, 0.5f);
                else
                    Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);

                Gizmos.DrawCube(new Vector3(m_Map[i, j].Position.x, 0, m_Map[i, j].Position.z), Vector3.one);
            }
        }
    }
#endif

    /// <summary>
    /// GameScene상의 맵 위 obstacle의 collider를 확인하여 m_Map의 movealbe값을 변경 
    /// </summary>
    public void SetMoveableMapNode()
    {
        Vector3 originVec = Vector3.zero;

        for (int i = 0; i < m_GroundSize.x; i++)
        {
            for (int j = 0; j < m_GroundSize.y; j++)
            {
                m_Map[i, j] = new Node(i, j, true);
                m_Map[i, j].Position = new Vector3(i - 7, 0, j - 7);
                originVec = new Vector3(m_Map[i, j].Position.x, -5f, m_Map[i, j].Position.z);
                if (Physics.Raycast(originVec, Vector3.up, 10f, LayerMask.GetMask("Obstacle")))
                {
                    m_Map[i, j].Moveable = false;

                }
                else if (Physics.Raycast(originVec, Vector3.up, 10f, LayerMask.GetMask("Start")))
                {
                    start = m_Map[i, j];
                }
                else if (Physics.Raycast(originVec, Vector3.up, 10f, LayerMask.GetMask("MonsterSpawnPoint")))
                {
                    monsterSpawnPoints.Add(m_Map[i, j].Position);
                }
            }
        }
    }
}
