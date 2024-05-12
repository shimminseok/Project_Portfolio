using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.Util;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public enum TweenType
{
    NONE,
    LOOP,
    PINGPONG
}
public class TweenManager : Singleton<TweenManager>
{
    


    public IEnumerator FadeOut(GameObject _go, float _alpha, float _time, float _delay = 0, UnityAction _action = null)
    {
        CanvasGroup render = _go.GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(_delay);
        float startAlpha = render.alpha;
        float time = 0;
        while(time < _time)
        {
            render.alpha = Mathf.Lerp(startAlpha, _alpha, time / _time);
            time += Time.deltaTime;
            yield return null;
        }
        render.alpha = _alpha;
        _action?.Invoke();
    }
    public IEnumerator FadeIn(GameObject _go, float _alpha,float _time, float _delay = 0, UnityAction _action = null)
    {
        CanvasGroup render = _go.GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(_delay);
        float startAlpha = render.alpha;
        float time = 0;
        while (time < _time)
        {
            render.alpha = Mathf.Lerp(startAlpha, _alpha, time / _time);
            time += Time.deltaTime;
            yield return null;
        }
        render.alpha = _alpha;
        _action?.Invoke();
    }

    public IEnumerator TweenMove(RectTransform _rect,Vector2 _from, Vector2 _to, float _time, float _delay = 0, TweenType _type = TweenType.NONE, UnityAction _action = null)
    {
        yield return new WaitForSeconds(_delay);
        float time = 0;
        switch(_type)
        {
            case TweenType.NONE:
                {
                    while (time < _time)
                    {
                        _rect.anchoredPosition = Vector3.Lerp(_from, _to, time / _time);
                        time += Time.deltaTime;
                        yield return null;
                    }
                    break;
                }
            case TweenType.LOOP:
                {
                    while (true)
                    {
                        while (time < _time)
                        {
                            _rect.anchoredPosition = Vector3.Lerp(_from, _to, time / _time);
                            time += Time.deltaTime;
                            yield return null;
                        }
                        _rect.anchoredPosition = _to;
                        time = 0;
                        _rect.anchoredPosition = _from;
                        yield return null;
                    }
                }
            case TweenType.PINGPONG:
                {
                    while (true)
                    {
                        while (time < _time)
                        {
                            _rect.anchoredPosition = Vector3.Lerp(_from, _to, time / _time);
                            time += Time.deltaTime;
                            yield return null;
                        }
                        _rect.anchoredPosition = _to;
                        time = 0;
                        _to = _from;
                        _from = _rect.anchoredPosition;
                        yield return null;
                    }
                }
        }

        _rect.anchoredPosition = _to;
        _action?.Invoke();
    }
    public IEnumerator TweenFill(Image _image, float _start,float _end, UnityAction _action = null)
    {
        _image.fillAmount = _start;
        float time = 0;
        while(time < 0.5f)
        {
            _image.fillAmount = Mathf.Lerp(_start, _end, time / 0.5f);
            time += Time.deltaTime;
            yield return null;
        }
        _image.fillAmount = _end;
        _action?.Invoke();
    }
}