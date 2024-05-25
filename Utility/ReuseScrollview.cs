using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(RectTransform))]
    public class ReuseScrollview<T> : MonoBehaviour
    {
        protected List<T> tableData = new List<T>(); //리스트 항목의 데이터를 저장
        [SerializeField] protected GameObject cellBase = null; // 복사 원본 셀
        [SerializeField] RectOffset padding; // 스크롤할 내용의 패딩
        [SerializeField] float spacingHeight; // 각 셀의 간격
        [SerializeField] float spacingWidht; // 각 셀의 간격
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
                //visibleRect의 범위에 빈 곳이 있으면 셀을 작성
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
        /// 셀의 높이값을 리턴하는 함수
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        protected virtual float GetCellHeightAtIndex(int _index)
        {
            //실제 값을 반환하는 처리는 상속한 클래스에서 구현
            //셀마다 크기가 다를 경우 상속받은 클래스에서 재 구현
            return cellBase.GetComponent<RectTransform>().sizeDelta.y;
        }
        protected virtual float GetCellWidthAtIndex(int _index)
        {
            //실제 값을 반환하는 처리는 상속한 클래스에서 구현
            //셀마다 크기가 다를 경우 상속받은 클래스에서 재 구현
            return cellBase.GetComponent<RectTransform>().sizeDelta.x;
        }
        /// <summary>
        /// 스크롤할 내용 전체의 높이를 갱신하는 함수
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
        /// 셀을 생성하는 함수
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        ReuseCellData<T> CreateCellForIndex(int _index)
        {
            //복사 원본 셀을 이용해 새로운 셀을 생성한다.
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

            //지정된 인덱스가 붙은 리스트 항목에 대응하는 셀로 내용을 갱신
            UpdateCellForIndex(cell, _index);
            cells.AddLast(cell);

            return cell;
        }
        /// <summary>
        /// 셀의 내용을 갱신하는 함수
        /// </summary>
        /// <param name="_cell"></param>
        /// <param name="_index"></param>
        void UpdateCellForIndex(ReuseCellData<T> _cell, int _index)
        {
            // 셀에 대응하는 리스트 항목의 인덱스를 설정한다.
            _cell.Index = _index;

            if (_cell.Index >= 0 && _cell.Index <= tableData.Count - 1)
            {
                //셀에 대응하는 리스트 항목이 있다면 셀을 활성화해서 내용을 갱신하고 높이를 설정

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
        /// VisibleRect를 갱신하기 위한 함수
        /// </summary>
        void UpdateVisibleRect()
        {
            visibleRect.x = CachedScrollRect.content.anchoredPosition.x + visibleRectPadding.left;
            visibleRect.y = -CachedScrollRect.content.anchoredPosition.y + visibleRectPadding.top;

            visibleRect.width = CachedRectTransform.rect.width + visibleRectPadding.left + visibleRectPadding.right;
            visibleRect.height = CachedRectTransform.rect.height + visibleRectPadding.top + visibleRectPadding.bottom;
        }
        /// <summary>
        /// VisibleRect 범위에 표시될 만큼의 셀을 생성하여 배치하는 함수
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
        /// 스크롤뷰가 움직였을 때 호출되는 함수
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
                // 위로 스크롤하고 있을 때는 visibleRect에 지정된 범위보다 위에 있는 셀을 아래를 향해 순서대로 이동시켜 내용을 갱신
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
                // 아래로 스크롤하고 있을 때는 visibleRect에 지정된 범위보다 아래에 있는 셀을 위를 향해 순서대로 이동시켜 내용을 갱신
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

