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

    public AStar m_AStar = new AStar();

    List<Node> CloseList = new List<Node>();
    string STR_OBSTACLE = "Obstacle";
    bool isManualMap = false;

    Vector3 end;
    List<Vector3> hitRay = new List<Vector3>();
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void CreateMap(Map map)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Maps/Map_01"), m_MapRoot.transform);

        isManualMap = false;
        m_GroundSize = map.GroundSize;

        m_Map = map.MapNode;
        //go.transform.localPosition = new Vector3(-2f, -0.5f, -2f);
        //yield return new WaitForEndOfFrame();
        SetMoveableMapNode();
        m_AStar.m_GroundSize = m_GroundSize;
        m_AStar.LoadMapData(m_Map);
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
                if (m_Map[i, j].Moveable)
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
                m_Map[i, j].Position = new Vector3(i - 7, -0.5f, j - 7);
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

    public void CheckMap()
    {
        for (int i = 0; i < m_GroundSize.x; i++)
        {
            for (int j = 0; j < m_GroundSize.y; j++)
            {
                m_Map[i, j].Moveable = true;

                RaycastHit hit;
                if (Physics.Raycast(new Vector3(m_Map[i, j].Position.x, 50.0f, m_Map[i, j].Position.y), Vector3.down, out hit, 100.0f))
                {
                    if (hit.collider.name != "Ground" && hit.collider.enabled)
                    {
                        m_Map[i, j].Moveable = false;
                    }
                    else if (hit.collider.name == "Ground")
                    {
                        RaycastHit obsHit;
                        int layerMask = 1 << LayerMask.NameToLayer("Obstacle");
                        Vector3 originVec = new Vector3(m_Map[i, j].Position.x, -50, m_Map[i, j].Position.y);
                        if (Physics.SphereCast(originVec, 0.5f, Vector3.up, out obsHit, 100.0f, layerMask) && obsHit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle") && obsHit.collider.enabled)
                            m_Map[i, j].Moveable = false;
                    }

                }
                else
                    m_Map[i, j].Moveable = false;
            }
        }

        m_AStar.m_GroundSize = m_GroundSize;
        m_AStar.LoadMapData(m_Map);

        MapManager.Instance.SetMapCollider();
    }

    /// <summary>
    /// pos 위치에 해당하는 m_Map.position의 x,y값 반환
    /// </summary>
    void FindIndex(Vector3 pos, out int x, out int y)
    {
        Vector3 result = pos - new Vector3(m_Map[0, 0].Position.x, 0, m_Map[0, 0].Position.y);

        result = Quaternion.Euler(0, 45, 0) * result;

        x = Mathf.RoundToInt(-result.z);
        y = Mathf.RoundToInt(result.x);

        if (x >= m_GroundSize.x)
            x = (int)m_GroundSize.x - 1;

        if (y >= (int)m_GroundSize.y)
            y = (int)m_GroundSize.y - 1;

        if (x < 0)
            x = 0;

        if (y < 0)
            y = 0;
    }

    /// <summary>
    /// 인자 pos 위치에 근접 노드(m_Map) 중 이동 가능하면서 제일 가까운 노드를 검색하여 반환
    /// </summary>
    Node FindMoveableNode(Vector3 pos, int indexX, int indexY)
    {
        CloseList.Clear();

        // 해당 노드가 이동 불가능이면 검색 시작
        if (!m_Map[indexX, indexY].Moveable)
        {
            int offset = 0;

            while (!m_Map[indexX, indexY].Moveable)
            {
                offset++;
                if (offset >= m_Map.Length / 2)
                    break;

                bool isOn = false;

                for (int i = -offset; i <= offset; i++)
                {
                    for (int j = -offset; j <= offset; j++)
                    {
                        if ((i == 0 && j == 0) || indexX + i < 0 || indexY + j < 0 || indexX + i >= m_GroundSize.x || indexY + j >= m_GroundSize.y || (Mathf.Abs(i) < offset && Mathf.Abs(j) < offset))
                            continue;

                        // 이동 가능한 근접 노드 찾은 후 CloseList에 저장 
                        if (m_Map[indexX + i, indexY + j].Moveable)
                        {
                            indexX = indexX + i;
                            indexY = indexY + j;

                            if (!CloseList.Contains(m_Map[indexX, indexY]))
                                CloseList.Add(m_Map[indexX, indexY]);

                            isOn = true;
                            break;  // check : FindPath()에서 문제생기면 분기처리. // y가 커질수록 거리가 커져서 더 검색할 필요없어서 break 걸음.
                        }
                    }
                }

                // 이동 가능한 근처 노드 리스트에서 거리가 제일 가까운 노드의 x,y값 저장.
                if (isOn)
                {
                    float dist = 9999;

                    for (int i = 0; i < CloseList.Count; i++)
                    {
                        if (Vector2.Distance(new Vector2(CloseList[i].Position.x, CloseList[i].Position.y), pos) < dist)
                        {
                            dist = Vector2.Distance(new Vector2(CloseList[i].Position.x, CloseList[i].Position.y), pos);
                            indexX = CloseList[i].X;
                            indexY = CloseList[i].Y;
                        }
                    }

                    break;
                }
            }
        }

        return m_Map[indexX, indexY];
    }

    public List<Node> FindPath(Vector2 start, Vector2 end)
    {
        int s_x = 0, s_y = 0, e_x = 0, e_y = 0;

        FindIndex(new Vector3(start.x, 0, start.y), out s_x, out s_y);
        FindIndex(new Vector3(end.x, 0, end.y), out e_x, out e_y);

        return m_AStar.FindPath(FindMoveableNode(start, s_x, s_y), FindMoveableNode(end, e_x, e_y), start);
    }

    /// <summary>
    /// 이동 가능한 m_Map.position 반환 : 
    /// </summary>
    /// <returns> pos근처 이동가능+거리가까운 노드의 position 반환 </returns>
    public Vector3 NearNodePos(Vector3 pos)
    {
        int s_x = 0, s_y = 0;

        FindIndex(pos, out s_x, out s_y);

        Node nearNode = FindMoveableNode(pos, s_x, s_y);

        return new Vector3(nearNode.Position.x, pos.y, nearNode.Position.y);
    }
}

public class AStar
{
    public Vector2 m_GroundSize;

    public List<Node> OpenList = new List<Node>();
    public List<Node> ClosedList = new List<Node>();

    Node[,] Map;

    Node StartNode, Destination;
    string STR_OBSTACLE = "Obstacle";

    public void LoadMapData(Node[,] map)
    {
        Map = map;
    }

    public List<Node> FindPath(Node startNode, Node destination, Vector2 startPos)
    {
        StartNode = startNode;
        Destination = destination;

        Init();
        return AStar_Dir8(startPos);
    }

    void Init()
    {
        OpenList.Clear();
        ClosedList.Clear();
    }

    List<Node> AStar_Dir8(Vector2 startPos)
    {
        List<Node> resultNode = new List<Node>();

        Node CurrentNode = new Node(StartNode);

        CurrentNode.G = 0;
        CurrentNode.F = 0;
        CurrentNode.H = 0;
        CurrentNode.Parent = null;

        OpenList.Add(CurrentNode);
        FindNode(resultNode, CurrentNode);  // 돌고나면 최적 경로(m_Map내)

        Vector3 next = Vector3.zero;
        Vector3 end = Vector3.zero;
        Vector3 nomal = Vector3.zero;
        float dist = 0;
        int layerMask;
        RaycastHit obsHit;
        int count = 0;

        // 노드 사이 장애물이 없으면, 중간 경로 노드 제거  
        if (resultNode.Count > 2)
        {
            for (int i = 0; i < resultNode.Count; i++)
            {
                count = resultNode.Count;
                end = new Vector3(resultNode[i].Position.x, 0, resultNode[i].Position.y);

                for (int j = i + 2; j < resultNode.Count;)
                {
                    next = new Vector3(resultNode[j].Position.x, 0, resultNode[j].Position.y);
                    nomal = (end - next).normalized;
                    dist = Vector3.Distance(end, next);
                    layerMask = 1 << LayerMask.NameToLayer(STR_OBSTACLE);
                    if (Physics.SphereCast(origin: next, radius: 0.4f, direction: nomal, out obsHit, maxDistance: dist, layerMask) && obsHit.collider.gameObject.layer == LayerMask.NameToLayer(STR_OBSTACLE) && obsHit.collider.enabled)
                    {
                        break;
                    }
                    else
                    {
                        resultNode.RemoveAt(j - 1);
                    }

                }
                if (count != resultNode.Count)
                {
                    i--;
                }
                if (resultNode.Count < 2)
                {
                    break;
                }
            }
        }

        // startPos와 resultNode 뒤에서 두번째 노드 사이에 장애물과 충돌하지 않는다면 맨뒤 노드 지움 
        if (resultNode.Count >= 2)
        {
            end = new Vector3(resultNode[resultNode.Count - 2].Position.x, 0, resultNode[resultNode.Count - 2].Position.y);
            next = new Vector3(startPos.x, 0, startPos.y);
            nomal = (end - next).normalized;
            dist = Vector3.Distance(end, next);
            layerMask = 1 << LayerMask.NameToLayer(STR_OBSTACLE);
            if (!(Physics.SphereCast(next, 0.4f, nomal, out obsHit, dist, layerMask) && obsHit.collider.gameObject.layer == LayerMask.NameToLayer(STR_OBSTACLE) && obsHit.collider.enabled))
            {
                resultNode.RemoveAt(resultNode.Count - 1);
            }
        }

        return resultNode;
    }

    /// <summary>
    /// m_Map내 최적경로 찾아서 List<node> result 반환
    /// </summary>
    public void FindNode(List<Node> result, Node current)
    {
        // current(마지막 노드)가 도착지면 result에 최적경로 저장 
        if (current.X == Destination.X && current.Y == Destination.Y)
        {
            Node tmp = current;
            while (tmp != null)
            {
                result.Add(tmp);
                tmp = tmp.Parent;
            }
            return;
        }

        Node minNode = null;
        bool isNodeOpenListNode = false;
        // 근접 + 이동 가능한 + closeList에 없는 노드들
        List<Node> adjacents = FindAdjacentNodes(current);

        for (int i = 0; i < adjacents.Count; i++)
        {
            isNodeOpenListNode = false;

            for (int j = 0; j < OpenList.Count; j++)
            {
                if (OpenList[j].X == adjacents[i].X && OpenList[j].Y == adjacents[i].Y)
                {
                    if (OpenList[j].G > current.G)
                    {
                        OpenList[j].Parent = current;

                        if (OpenList[j].X != current.X && OpenList[j].Y != current.Y)
                        {
                            OpenList[j].G = current.G + 14;
                        }
                        else
                        {
                            OpenList[j].G = current.G + 10;
                        }
                        OpenList[j].F = OpenList[j].G + OpenList[j].H;
                    }

                    isNodeOpenListNode = true;
                    break;
                }
            }

            if (!isNodeOpenListNode)
            {
                OpenList.Add(adjacents[i]);
                adjacents[i].Parent = current;
                Vector3 pos = new Vector3(Mathf.Abs(current.Position.x - adjacents[i].Position.x), 0, Mathf.Abs(current.Position.y - adjacents[i].Position.y));

                if (pos.x < pos.z)
                {
                    adjacents[i].G = (pos.x * 14 + (pos.z - pos.x) * 10);
                }
                else
                {
                    adjacents[i].G = (pos.z * 14 + (pos.x - pos.z) * 10);
                }
                pos = new Vector3(Mathf.Abs(Destination.Position.x - adjacents[i].Position.x), 0, Mathf.Abs(Destination.Position.y - adjacents[i].Position.y));

                if (pos.x < pos.z)
                {
                    adjacents[i].H = (pos.x * 14 + (pos.z - pos.x) * 10);
                }
                else
                {
                    adjacents[i].H = (pos.z * 14 + (pos.x - pos.z) * 10);
                }

                adjacents[i].F = adjacents[i].G + adjacents[i].H;
            }
        }

        OpenList.Remove(current);
        ClosedList.Add(current);

        OpenList.Sort(delegate (Node a, Node b)
        {
            if (a.F > b.F) return 1;
            else if (a.F < b.F) return -1;
            else return 0;
        });

        minNode = OpenList[0];

        if (minNode != null)
        {
            FindNode(result, minNode);
        }
    }

    /// <summary>
    /// currentNode의 유효한 근접 노드를 찾아서 리스트로 반환
    /// </summary>
    List<Node> FindAdjacentNodes(Node currentNode)
    {
        int x = currentNode.X;
        int y = currentNode.Y;

        Node[] adjArray = new Node[8];
        adjArray[0] = AddAdjacent(x + 1, y);
        adjArray[1] = AddAdjacent(x, y - 1);
        adjArray[2] = AddAdjacent(x - 1, y);
        adjArray[3] = AddAdjacent(x, y + 1);
        adjArray[4] = AddAdjacent(x + 1, y + 1);
        adjArray[5] = AddAdjacent(x + 1, y - 1);
        adjArray[6] = AddAdjacent(x - 1, y + 1);
        adjArray[7] = AddAdjacent(x - 1, y - 1);

        List<Node> results = new List<Node>();

        //for (int i = 0; i < adjArray.Length; i++)
        //{
        //    if (adjArray[i] != null)
        //        results.Add(new Node(adjArray[i]));
        //}

        return results;
    }

    /// <summary>
    /// 유효한(근접 + 이동 가능한 + closeList에 없는) 노드인지 체크하여 반환
    /// </summary>
    Node AddAdjacent(int x, int y)
    {
        if ((x < 0) || (x >= m_GroundSize.x) || (y < 0) || (y >= m_GroundSize.y))
            return null;

        var adjacent = Map[x, y];

        if (adjacent == null)
            return null;

        if (!adjacent.Moveable)
            return null;

        for (int i = 0; i < ClosedList.Count; i++)
        {
            if (ClosedList[i].X == adjacent.X && ClosedList[i].Y == adjacent.Y)
            {
                return null;
            }
        }

        return adjacent;
    }
}