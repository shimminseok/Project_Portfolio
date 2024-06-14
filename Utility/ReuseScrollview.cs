using NPOI.SS.UserModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(RectTransform))]
    public class ReuseScrollview<T> : MonoBehaviour
    {
        protected List<T> tableData = new List<T>(); // 리스트 항목의 데이터를 저장
        [SerializeField] protected GameObject cellBase = null; // 복사 원본 셀
        [SerializeField] RectOffset padding; // 스크롤할 내용의 패딩
        [SerializeField] float spacingHeight; // 각 셀의 세로 간격
        [SerializeField] float spacingWidth; // 각 셀의 가로 간격
        [SerializeField] RectOffset visibleRectPadding = null; // visibleRect의 패딩

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
            if (CachedScrollRect.vertical)
            {
                UpdateScrollViewSizeVertical();
            }
            else
            {
                UpdateScrollViewSizeHorizontal();
            }

            UpdateVisibleRect();

            if (cells.Count < 1)
            {
                Vector2 cellStart = CachedScrollRect.vertical ? new Vector2(0f, -padding.top) : new Vector2(padding.left, 0f);
                for (int i = 0; i < tableData.Count; i++)
                {
                    float cellSize = CachedScrollRect.vertical ? GetCellHeightAtIndex(i) : GetCellWidthAtIndex(i);
                    Vector2 cellEnd = CachedScrollRect.vertical ? cellStart + new Vector2(0f, -cellSize) : cellStart + new Vector2(cellSize, 0f);

                    if (IsWithinVisibleRect(cellStart, cellEnd))
                    {
                        ReuseCellData<T> cell = CreateCellForIndex(i);
                        if (CachedScrollRect.vertical)
                        {
                            cell.Top = cellStart;
                            break;
                        }
                        else
                        {
                            cell.Left = cellStart;
                            break;
                        }

                    }
                        cellStart = CachedScrollRect.vertical
                            ? cellEnd + new Vector2(0f, spacingHeight)
                            : cellEnd + new Vector2(spacingWidth, 0f);
                }
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
                    if (CachedScrollRect.vertical)
                    {
                        node.Value.Top = node.Previous.Value.Bottom + new Vector2(0f, -spacingHeight);
                    }
                    else
                    {
                        node.Value.Left = node.Previous.Value.Right + new Vector2(spacingWidth, 0f);
                    }
                    node = node.Next;
                }
                SetFillVisibleRectWithCells();
            }
        }

        protected void ResetCell()
        {
            int i = 0;
            foreach (var cell in cells)
            {
                UpdateCellForIndex(cell, i++);
            }
        }

        protected virtual float GetCellHeightAtIndex(int _index)
        {
            return cellBase.GetComponent<RectTransform>().sizeDelta.y;
        }

        protected virtual float GetCellWidthAtIndex(int _index)
        {
            return cellBase.GetComponent<RectTransform>().sizeDelta.x;
        }
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
        protected void UpdateScrollViewSizeVertical()
        {
            float contentHeight = 0f;
            for (int i = 0; i < tableData.Count; i++)
            {
                contentHeight += GetCellHeightAtIndex(i);

                if (i > 0)
                {
                    contentHeight += spacingHeight;
                }
            }

            Vector2 sizeDelta = CachedScrollRect.content.sizeDelta;
            sizeDelta.y = padding.top + contentHeight + padding.bottom;
            CachedScrollRect.content.sizeDelta = sizeDelta;
        }

        protected void UpdateScrollViewSizeHorizontal()
        {
            float contentWidth = 0f;
            for (int i = 0; i < tableData.Count; i++)
            {
                contentWidth += GetCellWidthAtIndex(i);

                if (i > 0)
                {
                    contentWidth += spacingWidth;
                }
            }

            Vector2 sizeDelta = CachedScrollRect.content.sizeDelta;
            sizeDelta.x = padding.left + contentWidth + padding.right;
            CachedScrollRect.content.sizeDelta = sizeDelta;
        }

        ReuseCellData<T> CreateCellForIndex(int _index)
        {
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

            UpdateCellForIndex(cell, _index);
            cells.AddLast(cell);

            return cell;
        }

        protected void UpdateCellForIndex(ReuseCellData<T> _cell, int _index)
        {
            _cell.Index = _index;

            if (_cell.Index >= 0 && _cell.Index <= tableData.Count - 1)
            {
                _cell.gameObject.SetActive(true);
                _cell.UpdateContent(tableData[_cell.Index]);
                if (CachedScrollRect.vertical)
                {
                    _cell.Height = GetCellHeightAtIndex(_cell.Index);
                }
                else
                {
                    _cell.Width = GetCellWidthAtIndex(_cell.Index);
                }
                _cell.m_data = tableData[_cell.Index];
            }
            else
            {
                _cell.gameObject.SetActive(false);
            }
        }
        protected void UpdateAllCells()
        {
            foreach (var cell in cells)
            {
                cell.UpdateContent(tableData[cell.Index]);
            }
        }
        void UpdateVisibleRect()
        {
            visibleRect.x = -CachedScrollRect.content.anchoredPosition.x + visibleRectPadding.left;
            visibleRect.y = -CachedScrollRect.content.anchoredPosition.y + visibleRectPadding.top;

            visibleRect.width = CachedRectTransform.rect.width + visibleRectPadding.left + visibleRectPadding.right;
            visibleRect.height = CachedRectTransform.rect.height + visibleRectPadding.top + visibleRectPadding.bottom;
        }

        bool IsWithinVisibleRect(Vector2 start, Vector2 end)
        {
            if (CachedScrollRect.vertical)
            {
                return (start.y <= visibleRect.y && start.y >= visibleRect.y - visibleRect.height) || (end.y <= visibleRect.y && end.y >= visibleRect.y - visibleRect.height);
            }
            else
            {
                return (start.x <= visibleRect.x + visibleRect.width && start.x >= visibleRect.x) || (end.x <= visibleRect.x + visibleRect.width && end.x >= visibleRect.x);
            }
        }

        void SetFillVisibleRectWithCells()
        {
            if (cells.Count < 1)
            {
                return;
            }

            ReuseCellData<T> lastCell = cells.Last.Value;
            int nextCellDataIndex = lastCell.Index + 1;
            Vector2 nextCellStart = CachedScrollRect.vertical ? lastCell.Bottom + new Vector2(0f, -spacingHeight)
                : lastCell.Right + new Vector2(spacingWidth, 0f);

            while (nextCellDataIndex < tableData.Count && IsWithinVisibleRect(nextCellStart, nextCellStart))
            {
                ReuseCellData<T> cell = CreateCellForIndex(nextCellDataIndex);
                if (CachedScrollRect.vertical)
                {
                    cell.Top = nextCellStart;
                }
                else
                {
                    cell.Left = nextCellStart;
                }

                lastCell = cell;
                nextCellDataIndex = lastCell.Index + 1;
                nextCellStart = CachedScrollRect.vertical
                    ? lastCell.Bottom + new Vector2(0f, -spacingHeight)
                    : lastCell.Right + new Vector2(spacingWidth, 0f);
            }
        }

        public void OnScrollPosChanged(Vector2 _scrollPos)
        {
            UpdateVisibleRect();
            if (CachedScrollRect.vertical)
            {

                UpdateVirtical((_scrollPos.y < prevScrollPos.y) ? 1 : -1);
            }
            else
            {
                UpdateHorizontal((_scrollPos.x < prevScrollPos.x) ? 1 : -1);
            }
            prevScrollPos = _scrollPos;
        }
        void UpdateVirtical(int _scrollDirection)
        {
            if (cells.Count < 1)
                return;

            if (_scrollDirection > 0)
            {
                // 위로 스크롤하고 있을 때는 visibleRect에 지정된 범위보다 위에 있는 셀을 아래를 향해 순서대로 이동시켜 내용을 갱신
                ReuseCellData<T> firstCell = cells.First.Value;
                while (IsBeyondVisibleRect(firstCell))
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
                // 아래로 스크롤하고 있을 때는 visibleRect에 지정된 범위보다 아래에 있는 셀을 위를 향해 순서대로 이동시켜 내용을 갱신
                ReuseCellData<T> lastCell = cells.Last.Value;
                while (IsBeyondVisibleRect(lastCell))
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
        void UpdateHorizontal(int _scrollDirection)
        {
            if (cells.Count < 1)
                return;

            if (_scrollDirection > 0)
            {
                ReuseCellData<T> lastCell = cells.Last.Value;
                while (IsBeyondVisibleRect(lastCell))
                {
                    ReuseCellData<T> firstCell = cells.First.Value;
                    UpdateCellForIndex(lastCell, firstCell.Index - 1);
                    lastCell.Right = firstCell.Left + new Vector2(-spacingWidth, 0f);
                    cells.AddFirst(lastCell);
                    cells.RemoveLast();
                    lastCell = cells.Last.Value;
                }
                SetFillVisibleRectWithCells();
            }
            else if (_scrollDirection < 0)
            {
                ReuseCellData<T> firstCell = cells.First.Value;
                while (IsBeyondVisibleRect(firstCell))
                {
                    ReuseCellData<T> lastCell = cells.Last.Value;
                    UpdateCellForIndex(firstCell, lastCell.Index + 1);
                    firstCell.Left = lastCell.Right + new Vector2(spacingWidth, 0f);
                    cells.AddLast(firstCell);
                    cells.RemoveFirst();
                    firstCell = cells.First.Value;
                }
                SetFillVisibleRectWithCells();
            }

        }
        bool IsBeyondVisibleRect(ReuseCellData<T> cell)
        {
            if (CachedScrollRect.vertical)
            {
                if (cell.Bottom.y > visibleRect.y || cell.Top.y < visibleRect.y - visibleRect.height)
                    return true;
                else
                    return false;
            }
            else
            {

                if (cell.Right.x < visibleRect.x || cell.Left.x > visibleRect.x + visibleRect.width)
                {
                    return true;
                }
                else
                    return false;
            }
        }
    }
}
