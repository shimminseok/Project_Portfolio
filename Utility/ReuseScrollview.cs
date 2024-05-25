using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(RectTransform))]
    public class ReuseScrollview<T> : MonoBehaviour
    {
        protected List<T> tableData = new List<T>(); //����Ʈ �׸��� �����͸� ����
        [SerializeField] protected GameObject cellBase = null; // ���� ���� ��
        [SerializeField] RectOffset padding; // ��ũ���� ������ �е�
        [SerializeField] float spacingHeight; // �� ���� ����
        [SerializeField] float spacingWidht; // �� ���� ����
        [SerializeField] RectOffset visibleRectPadding = null; // visibleRect�� �е�



        LinkedList<ReuseCellData<T>> cells = new LinkedList<ReuseCellData<T>>();
        Rect visibleRect;
        Vector2 prevScrollPos;
        public RectTransform CachedRectTransform => GetComponent<RectTransform>();
        public ScrollRect CachedScrollRect => GetComponent<ScrollRect>();
        public List<T> TableData => tableData;
        public LinkedList<ReuseCellData<T>> Cells => cells;

        protected virtual void Start()
        {
            cellBase.SetActive(false);
            CachedScrollRect.onValueChanged.AddListener(OnScrollPosChanged);
        }

        protected void InitTableView()
        {
            UpdateScrollViewSize();
            UpdateVisibleRect();

            if (cells.Count < 1)
            {
                Vector2 cellTop = new Vector2(0f, -padding.top);
                for (int i = 0; i < tableData.Count; i++)
                {
                    float cellHeight = GetCellHeightAtIndex(i);
                    Vector2 cellBottom = cellTop + new Vector2(0f, -cellHeight);

                    if ((cellTop.y <= visibleRect.y && cellTop.y >= visibleRect.y - visibleRect.height) || (cellBottom.y <= visibleRect.y && cellBottom.y >= visibleRect.y - visibleRect.height))
                    {
                        ReuseCellData<T> cell = CreateCellForIndex(i);
                        cell.Top = cellTop;
                        break;
                    }
                    cellTop = cellBottom + new Vector2(0f, spacingHeight);
                }
                //visibleRect�� ������ �� ���� ������ ���� �ۼ�
                SetFillVisibleRectWithCells();
            }
            else
            {
                LinkedListNode<ReuseCellData<T>> node = cells.First;
                UpdateCellForIndex(node.Value, node.Value.Index);
                node = node.Next;

                while (node != null)
                {
                    UpdateCellForIndex(node.Value, node.Previous.Value.Index + 1);
                    node.Value.Top = node.Previous.Value.Bottom + new Vector2(0f, -spacingHeight);
                    node = node.Next;
                }
                SetFillVisibleRectWithCells();

            }
        }

        /// <summary>
        /// ���� ���̰��� �����ϴ� �Լ�
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        protected virtual float GetCellHeightAtIndex(int _index)
        {
            //���� ���� ��ȯ�ϴ� ó���� ����� Ŭ�������� ����
            //������ ũ�Ⱑ �ٸ� ��� ��ӹ��� Ŭ�������� �� ����
            return cellBase.GetComponent<RectTransform>().sizeDelta.y;
        }
        protected virtual float GetCellWidthAtIndex(int _index)
        {
            //���� ���� ��ȯ�ϴ� ó���� ����� Ŭ�������� ����
            //������ ũ�Ⱑ �ٸ� ��� ��ӹ��� Ŭ�������� �� ����
            return cellBase.GetComponent<RectTransform>().sizeDelta.x;
        }
        /// <summary>
        /// ��ũ���� ���� ��ü�� ���̸� �����ϴ� �Լ�
        /// </summary>
        protected void UpdateScrollViewSize()
        {
            float contentHeigth = 0f;
            for (int i = 0; i < tableData.Count; i++)
            {
                contentHeigth += GetCellHeightAtIndex(i);

                if (i > 0)
                {
                    contentHeigth += spacingHeight;
                }
            }

            Vector2 sizeDelta = CachedScrollRect.content.sizeDelta;
            sizeDelta.y = padding.top + contentHeigth + padding.bottom;
            CachedScrollRect.content.sizeDelta = sizeDelta;
        }
        /// <summary>
        /// ���� �����ϴ� �Լ�
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        ReuseCellData<T> CreateCellForIndex(int _index)
        {
            //���� ���� ���� �̿��� ���ο� ���� �����Ѵ�.
            GameObject go = Instantiate(cellBase);
            go.SetActive(true);
            ReuseCellData<T> cell = go.GetComponent<ReuseCellData<T>>();
            Vector3 scale = cell.transform.localScale;
            Vector2 sizeDelta = cell.CachedRectTransform.sizeDelta;
            Vector2 offsetMin = cell.CachedRectTransform.offsetMin;
            Vector2 offsetMax = cell.CachedRectTransform.offsetMax;

            cell.transform.SetParent(cellBase.transform.parent);

            cell.transform.localScale = scale;
            cell.CachedRectTransform.sizeDelta = sizeDelta;
            cell.CachedRectTransform.offsetMin = offsetMin;
            cell.CachedRectTransform.offsetMax = offsetMax;

            //������ �ε����� ���� ����Ʈ �׸� �����ϴ� ���� ������ ����
            UpdateCellForIndex(cell, _index);
            cells.AddLast(cell);

            return cell;
        }
        /// <summary>
        /// ���� ������ �����ϴ� �Լ�
        /// </summary>
        /// <param name="_cell"></param>
        /// <param name="_index"></param>
        void UpdateCellForIndex(ReuseCellData<T> _cell, int _index)
        {
            // ���� �����ϴ� ����Ʈ �׸��� �ε����� �����Ѵ�.
            _cell.Index = _index;

            if (_cell.Index >= 0 && _cell.Index <= tableData.Count - 1)
            {
                //���� �����ϴ� ����Ʈ �׸��� �ִٸ� ���� Ȱ��ȭ�ؼ� ������ �����ϰ� ���̸� ����

                _cell.gameObject.SetActive(true);
                _cell.UpdateContent(tableData[_cell.Index]);
                _cell.Height = GetCellHeightAtIndex(_cell.Index);
                _cell.Widht = GetCellWidthAtIndex(_cell.Index);
            }
            else
            {
                _cell.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// VisibleRect�� �����ϱ� ���� �Լ�
        /// </summary>
        void UpdateVisibleRect()
        {
            visibleRect.x = CachedScrollRect.content.anchoredPosition.x + visibleRectPadding.left;
            visibleRect.y = -CachedScrollRect.content.anchoredPosition.y + visibleRectPadding.top;

            visibleRect.width = CachedRectTransform.rect.width + visibleRectPadding.left + visibleRectPadding.right;
            visibleRect.height = CachedRectTransform.rect.height + visibleRectPadding.top + visibleRectPadding.bottom;
        }
        /// <summary>
        /// VisibleRect ������ ǥ�õ� ��ŭ�� ���� �����Ͽ� ��ġ�ϴ� �Լ�
        /// </summary>
        void SetFillVisibleRectWithCells()
        {
            if (cells.Count < 1)
            {
                return;
            }

            ReuseCellData<T> lastCell = cells.Last.Value;
            int nextCellDataIndex = lastCell.Index + 1;
            Vector2 nextCellTop = lastCell.Bottom + new Vector2(0f, -spacingHeight);

            while (nextCellDataIndex < tableData.Count && nextCellTop.y >= visibleRect.y - visibleRect.height)
            {
                ReuseCellData<T> cell = CreateCellForIndex(nextCellDataIndex);
                cell.Top = nextCellTop;

                lastCell = cell;
                nextCellDataIndex = lastCell.Index + 1;
                nextCellTop = lastCell.Bottom + new Vector2(0f, -spacingHeight);
            }
        }
        /// <summary>
        /// ��ũ�Ѻ䰡 �������� �� ȣ��Ǵ� �Լ�
        /// </summary>
        /// <param name="_scrollPos"></param>
        public void OnScrollPosChanged(Vector2 _scrollPos)
        {
            UpdateVisibleRect();
            UpdateCells((_scrollPos.y < prevScrollPos.y) ? 1 : -1);
            prevScrollPos = _scrollPos;
        }
        void UpdateCells(int _scrollDirection)
        {
            if (cells.Count < 1)
                return;

            if (_scrollDirection > 0)
            {
                // ���� ��ũ���ϰ� ���� ���� visibleRect�� ������ �������� ���� �ִ� ���� �Ʒ��� ���� ������� �̵����� ������ ����
                ReuseCellData<T> firstCell = cells.First.Value;
                while (firstCell.Bottom.y > visibleRect.y)
                {
                    ReuseCellData<T> lastCell = cells.Last.Value;
                    UpdateCellForIndex(firstCell, lastCell.Index + 1);
                    firstCell.Top = lastCell.Bottom + new Vector2(0f, -spacingHeight);
                    cells.AddLast(firstCell);
                    cells.RemoveFirst();
                    firstCell = cells.First.Value;
                }
                SetFillVisibleRectWithCells();
            }
            else if (_scrollDirection < 0)
            {
                // �Ʒ��� ��ũ���ϰ� ���� ���� visibleRect�� ������ �������� �Ʒ��� �ִ� ���� ���� ���� ������� �̵����� ������ ����
                ReuseCellData<T> lastCell = cells.Last.Value;
                while (lastCell.Top.y < visibleRect.y - visibleRect.height)
                {
                    ReuseCellData<T> firstCell = cells.First.Value;
                    UpdateCellForIndex(lastCell, firstCell.Index - 1);
                    lastCell.Bottom = firstCell.Top + new Vector2(0f, spacingHeight);
                    cells.AddFirst(lastCell);
                    cells.RemoveLast();
                    lastCell = cells.Last.Value;
                }
                SetFillVisibleRectWithCells();

            }
        }
    }
}

