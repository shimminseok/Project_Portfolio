using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public static Navigation Instance;
    //public List<Vector3> monsterSpawnPoints = new List<Vector3>();
    public Dictionary<Vector3, int> monsterSpawnPoints = new Dictionary<Vector3, int>();
    public Node start;
    public GameObject m_MapRoot;

    public Vector2 m_GroundSize;
    public Node[,] m_Map;

    public List<Node> path;


    public List<Node> OpenList = new List<Node>();
    public List<Node> ClosedList = new List<Node>();

    Node Destination;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void CreateMap(Map map)
    {
        m_GroundSize = map.GroundSize;
        m_Map = map.MapNode;
        start = map.start;
        monsterSpawnPoints = map.monsterSpawnPointDic;

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

                Gizmos.DrawCube(new Vector3(m_Map[i, j].Position.x, 0, m_Map[i, j].Position.y), Vector3.one);
            }
        }
    }
#endif
    void Init()
    {
        OpenList.Clear();
        ClosedList.Clear();
    }
    public List<Node> FindPath(Vector2 _start, Vector2 _end)
    {
        FindIndex(new Vector3(_start.x, 0, _start.y), out int s_x, out int s_y);
        FindIndex(new Vector3(_end.x, 0, _end.y), out int e_x, out int e_y);

        return FindPath(FindMoveableNode(_start, s_x, s_y), FindMoveableNode(_end, e_x, e_y));
    }
    void FindIndex(Vector3 _pos, out int _x, out int _y)
    {
        Vector3 result = _pos - new Vector3(m_Map[0, 0].Position.x, 0, m_Map[0, 0].Position.y);

        result = Quaternion.Euler(0, 45, 0) * result;

        _x = Mathf.RoundToInt(-result.z);
        _y = Mathf.RoundToInt(result.x);

        if (_x >= m_GroundSize.x)
            _x = (int)m_GroundSize.x - 1;

        if (_y >= (int)m_GroundSize.y)
            _y = (int)m_GroundSize.y - 1;

        if (_x < 0)
            _x = 0;

        if (_y < 0)
            _y = 0;
    }

    Node FindMoveableNode(Vector3 _pos, int _indexX, int _indexY)
    {
        ClosedList.Clear();
        // �ش� ��尡 �̵� �Ұ����̸� �˻� ����
        if (!m_Map[_indexX, _indexY].Moveable)
        {
            int offset = 0;

            while (!m_Map[_indexX, _indexY].Moveable)
            {
                offset++;
                if (offset >= m_Map.Length / 2)
                    break;

                bool isOn = false;

                for (int i = -offset; i <= offset; i++)
                {
                    for (int j = -offset; j <= offset; j++)
                    {
                        if ((i == 0 && j == 0) || _indexX + i < 0 || _indexY + j < 0 || _indexX + i >= m_GroundSize.x || _indexY + j >= m_GroundSize.y || (Mathf.Abs(i) < offset && Mathf.Abs(j) < offset))
                            continue;

                        // �̵� ������ ���� ��� ã�� �� CloseList�� ���� 
                        if (m_Map[_indexX + i, _indexY + j].Moveable)
                        {
                            _indexX += i;
                            _indexY += j;

                            if (!ClosedList.Contains(m_Map[_indexX, _indexY]))
                                ClosedList.Add(m_Map[_indexX, _indexY]);

                            isOn = true;
                            break;  // check : FindPath()���� ��������� �б�ó��. // y�� Ŀ������ �Ÿ��� Ŀ���� �� �˻��� �ʿ��� break ����.
                        }
                    }
                }

                // �̵� ������ ��ó ��� ����Ʈ���� �Ÿ��� ���� ����� ����� x,y�� ����.
                if (isOn)
                {
                    float dist = 9999;

                    for (int i = 0; i < ClosedList.Count; i++)
                    {
                        if (Vector2.Distance(new Vector2(ClosedList[i].Position.x, ClosedList[i].Position.y), _pos) < dist)
                        {
                            dist = Vector2.Distance(new Vector2(ClosedList[i].Position.x, ClosedList[i].Position.y), _pos);
                            _indexX = ClosedList[i].X;
                            _indexY = ClosedList[i].Y;
                        }
                    }

                    break;
                }
            }
        }

        return m_Map[_indexX, _indexY];
    }


    public List<Node> FindPath(Node _startNode, Node _destination)
    {
        Destination = _destination;

        Init();
        return AStar_Dir8(_startNode);
    }
    List<Node> AStar_Dir8(Node _startNode)
    {
        List<Node> resultNode = new List<Node>();

        Node currentNode = new Node(_startNode);

        currentNode.G = 0;
        currentNode.F = 0;
        currentNode.H = 0;
        currentNode.Parent = null;

        OpenList.Add(currentNode);
        FindNode(resultNode, currentNode);  // ������ ���� ���(m_Map��)

        return resultNode;
    }

    public void FindNode(List<Node> result, Node current)
    {
        // current(������ ���)�� �������� result�� ������� ���� 
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
        // ���� + �̵� ������ + closeList�� ���� ����
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

        for (int i = 0; i < adjArray.Length; i++)
        {
            if (adjArray[i] != null)
                results.Add(new Node(adjArray[i]));
        }

        return results;
    }

    Node AddAdjacent(int x, int y)
    {
        if ((x < 0) || (x >= m_GroundSize.x) || (y < 0) || (y >= m_GroundSize.y))
            return null;

        var adjacent = m_Map[x, y];

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
