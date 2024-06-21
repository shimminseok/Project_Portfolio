using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenScale : MonoBehaviour
{
    [SerializeField] Vector3 startScale = Vector3.one;
    [SerializeField] Vector3 endScale;
    [SerializeField] float time;
    [SerializeField] float delayTime;
    [SerializeField] TweenType type;

    void Start()
    {
        transform.localScale = startScale;
    }

    private void OnEnable()
    {
        StartCoroutine(Scale(transform, startScale, endScale, time, delayTime, type));
    }

    public IEnumerator Scale(Transform _target, Vector3 _startScale, Vector3 _endScale, float _time, float _delayTime, TweenType _type = TweenType.NONE)
    {
        yield return new WaitForSeconds(_delayTime);
        _startScale = _target.localScale;
        do
        {
            float time = 0;
            while (time < _time)
            {
                Vector3 scale = new Vector3();
                scale.x = Mathf.Lerp(_startScale.x, _endScale.x, time / _time);
                scale.y = Mathf.Lerp(_startScale.y, _endScale.y, time / _time);
                _target.localScale = scale;
                time += Time.deltaTime;
                yield return null;
            }
            if (_type == TweenType.PINGPONG)
            {
                time = 0;
                while (time < _time)
                {
                    Vector3 scale = new Vector3();
                    scale.x = Mathf.Lerp(_endScale.x, _startScale.x, time / _time);
                    scale.y = Mathf.Lerp(_endScale.y, _startScale.y, time / _time);
                    _target.localScale = scale;
                    time += Time.deltaTime;
                    yield return null;
                }
            }
        } while (_type == TweenType.LOOP || _type == TweenType.PINGPONG);
    }
}
