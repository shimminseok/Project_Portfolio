using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public static Navigation Instance;
    public List<Vector3> monsterSpawnPoints = new List<Vector3>();
    public Node start;
    public Node boss;
    public GameObject m_MapRoot;

    public Vector2 m_GroundSize;

    public Node[,] m_Map;
    public List<Node> path;

    private Dictionary<string, List<Node>> nodePathCache = new Dictionary<string, List<Node>>();
    private const int MaxCacheSize = 100; // ĳ�� ũ�� ����

    Coroutine findPathCo;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void CreateMap(Map map)
    {
        m_GroundSize = map.mapSize;
        m_Map = map.mapNode;
        start = map.start;
        boss = map.boss;
        monsterSpawnPoints = map.monsterSpawnPoint;
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
                if (m_Map[i, j].walkable)
                    Gizmos.color = new Color(0, 0, 0, 0);
                else
                    Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);

                Gizmos.DrawCube(new Vector3(m_Map[i, j].worldPos.x, 0, m_Map[i, j].worldPos.z), Vector3.one);
            }
        }
    }
    #endif
    /// <summary>
    /// ���ִ� Position�� ���� Node�� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="_worldPos"></param>
    /// <returns></returns>
    public Node NodeFromWorldPoint(Vector3 _worldPos)
    {
        float percentX = (_worldPos.x + m_GroundSize.x / 2) / m_GroundSize.x;
        float percentY = (_worldPos.z + m_GroundSize.y / 2) / m_GroundSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((m_GroundSize.x) * percentX);
        int y = Mathf.RoundToInt((m_GroundSize.y) * percentY);

        return m_Map[x, y];
    }
    /// <summary>
    /// Ư�� ����� �̿������� ã�� �ִ� ��� �ĺ� ��� ����Ʈ�� ��ȯ
    /// </summary>
    /// <param name="_node"></param>
    /// <returns>�ִ� ��� �ĺ� ��� ����Ʈ</returns>
    public List<Node> GetNeighbours(Node _node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //���� ��带 ������ ���� ��带 Ž��
                if (x == 0 && y == 0)
                    continue;

                int checkX = _node.gridX + x;
                int checkY = _node.gridY + y;

                //�� �ȿ� �ִ��� Ȯ��
                if (checkX >= 0 && checkX < m_GroundSize.x && checkY >= 0 && checkY < m_GroundSize.y)
                {
                    neighbours.Add(m_Map[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
    /// <summary>
    /// ���� ���ִ� ��ġ�� ���� Ÿ�ٱ����� A*�� ã�� �Լ�
    /// </summary>
    /// <param name="_startPos"></param>
    /// <param name="_targetPos"></param>
    /// <param name="_callback"></param>
    public void RequestPath(Vector3 _startPos, Vector3 _targetPos, System.Action<List<Node>, bool> _callback)
    {
        string pathKey = GetPathKey(NodeFromWorldPoint(_startPos), NodeFromWorldPoint(_targetPos));
        if(nodePathCache.TryGetValue(pathKey,out List<Node> cachedPath))
        {
            _callback(cachedPath, true);
        }
        if (findPathCo != null)
            StopCoroutine(findPathCo);

        findPathCo = StartCoroutine(FindPath(_startPos, _targetPos, _callback));
    }
    /// <summary>
    /// Ÿ�ٱ����� ���� ����� �ڽ��� ã�� �Լ�
    /// </summary>
    /// <param name="_startPos"></param>
    /// <param name="_targetPos"></param>
    /// <param name="_callback"></param>
    /// <returns></returns>
    IEnumerator FindPath (Vector3 _startPos, Vector3 _targetPos, System.Action<List<Node>, bool> _callback)
    {
        List<Node> waypoints = new List<Node>();
        bool pathSuccess = false;

        Node startNode = NodeFromWorldPoint(_startPos);
        Node targetNode = NodeFromWorldPoint(_targetPos);

        //���� ������Ʈ�� �̵� �Ұ� ������ ��ġ�� ��찡 �����Ͽ�, ���� Ư�� ��尡 �̵� �Ұ� �����̶��, ��ó�� �̵� ������ ���� ��带 ��ä
        if(!startNode.walkable)
        {
            startNode = GetClosestWalkableNode(startNode);
        }
        if(!targetNode.walkable)
        {
            targetNode = GetClosestWalkableNode(startNode);
        }

        if (startNode.walkable && targetNode.walkable)
        {
            List<Node> openSet = new List<Node>(100); // �ʱ� �뷮 ����
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }

                if (openSet.Count % 100 == 0)
                {
                    yield return null;
                }
            }
        }
        else
        {
            Debug.Log("Not Walkable");
        }
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            string pathKey = GetPathKey(NodeFromWorldPoint(_startPos), NodeFromWorldPoint(_targetPos));
            if (nodePathCache.Count >= MaxCacheSize) // ĳ�� ũ�� ����
            {
                nodePathCache.Remove(nodePathCache.Keys.First());
            }
            nodePathCache[pathKey] = waypoints;
        }
        _callback(waypoints, pathSuccess);
    }
    /// <summary>
    /// ���� Node�� �̵� �Ұ� �����̶��, ��ó�� �̵� ���� ������ ã�� �Լ�
    /// </summary>
    /// <param name="_node"></param>
    /// <returns></returns>
    private Node GetClosestWalkableNode(Node _node)
    {
        Queue<Node> nodeQueue = new Queue<Node>();
        HashSet<Node> visitedNodes = new HashSet<Node>();

        nodeQueue.Enqueue(_node);
        visitedNodes.Add(_node);

        while (nodeQueue.Count > 0)
        {
            Node currentNode = nodeQueue.Dequeue();

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (neighbour.walkable)
                {
                    return neighbour;
                }

                if (!visitedNodes.Contains(neighbour))
                {
                    visitedNodes.Add(neighbour);
                    nodeQueue.Enqueue(neighbour);
                }
            }
        }

        return _node; // ��� ��带 Ž���������� walkable�� ��尡 ������ ���� ��带 ��ȯ (��ǻ� ���� ó��)
    }
    /// <summary>
    /// ���� ��θ� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <returns></returns>
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
/// <summary>
/// ����� �̵� ����� ��� �ϴ� �Լ�
/// </summary>
/// <param name="_nodeA"></param>
/// <param name="_nodeB"></param>
/// <returns></returns>
    int GetDistance(Node _nodeA, Node _nodeB)
    {
        int dstX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
        int dstY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
    /// <summary>
    /// Fixed Update���� �Ź� ��θ� Ž���ϱ⶧����, ������ ����� �߻��Ǿ��, ���� �Ȱ��� ��忡 ���ִٸ�, ĳ�̵� ��θ� ��ȯ�ϱ� ����
    /// ���� �Լ�
    /// </summary>
    /// <param name="_startNode"></param>
    /// <param name="_targetPos"></param>
    /// <returns></returns>
    private string GetPathKey(Node _startNode, Node _targetPos)
    {
        string returnStr = string.Format("SNode : {0},{1}, TNode :{2},{3}",_startNode.gridX,_startNode.gridY,_targetPos.gridX,_targetPos.gridY);
        return returnStr;
    }
}
