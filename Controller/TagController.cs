using NPOI.POIFS.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class TagController : MonoBehaviour
{
    [SerializeField] Image hpGauge;
    [SerializeField] ObjectController targetObj;
    [SerializeField] Vector2 offset;

    RectTransform parent;
    Camera mainCamera;
    void Start()
    {
        parent = transform.parent.GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(targetObj.transform.localPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, mainCamera, out Vector2 localPoint);
        transform.localPosition = localPoint + offset;
    }
    public void InitTag()
    {
        targetObj = null;
        hpGauge.fillAmount = 1;
        hpGauge.color = Color.white;
    }
    public void SetTag(ObjectController _target)
    {
        targetObj = _target;
        switch (targetObj.objType)
        {
            case OBJ_TYPE.PLAYER:
                hpGauge.color = Color.green;
                break;
            case OBJ_TYPE.MONSTER:
                hpGauge.color = Color.red;
                break;
        }
        transform.parent = TagManager.Instance.tagCanvas.transform;
        transform.localScale = Vector3.one;
    }
    public void UpdateHPUI()
    {
        if (targetObj != null)
        {
            switch(targetObj.objType)
            {
                case OBJ_TYPE.PLAYER:
                    {
                        hpGauge.fillAmount  = PlayerController.Instance.CurHP / PlayerController.Instance.MaxHP;
                    }
                    break;
            }
        }
    }

}
