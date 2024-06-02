using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class TagController : MonoBehaviour
{
    [SerializeField] Image hpGauge;
    [SerializeField] ObjectController targetObj;
    [SerializeField] Vector2 offset;
    [SerializeField] List<TextMeshProUGUI> damageFontList;


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
        damageFontList.ForEach(x => x.gameObject.SetActive(false));
    }
    public void SetTag(ObjectController _target)
    {
        InitTag();
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
        transform.SetParent(TagManager.Instance.tagCanvas.transform,false);
        transform.localScale = Vector3.one;
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
    public void UpdateHPUI(double _max, double _cur)
    {
        if (targetObj != null)
        {
            hpGauge.fillAmount = (float)(_cur / _max);
        }
    }

    public void SetDamageFontText(double _damage)
    {
        TextMeshProUGUI text = damageFontList.Find(x => x.gameObject.activeSelf == false);
        if (text == null)
        {
            text = Instantiate(damageFontList[0],transform);
            damageFontList.Add(text);
        }

        text.gameObject.SetActive(true);
        text.text = Utility.ToCurrencyString(_damage);
        switch(targetObj.objType)
        {
            case OBJ_TYPE.PLAYER:
                text.color = Color.red;
                break;
            case OBJ_TYPE.MONSTER:
                text.color = Color.yellow;
                break;
        }
        if(gameObject.activeSelf)
        {
            StartCoroutine(TweenManager.Instance.TweenMove(text.rectTransform, text.rectTransform.localPosition, text.rectTransform.localPosition + new Vector3(0, 30, 0), 1, 0, TweenType.NONE, () =>
            {
                text.gameObject.SetActive(false);
                text.rectTransform.localPosition = Vector3.zero;
                text.alpha = 1f;
            }));
            StartCoroutine(TweenManager.Instance.TweenAlpha(text, 1, 0, 0.5f));
        }
    }


}
