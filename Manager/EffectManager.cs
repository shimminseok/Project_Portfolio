using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    [SerializeField] List<GameObject> runEffectList = new List<GameObject>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void PlayEffect(GameObject _go, float _time = 1)
    {
        if (_go.TryGetComponent(out ObjectMove moveGo))
        {
            moveGo.Set();
        }

        ParticleSystem[] partycles = _go.GetComponentsInChildren<ParticleSystem>();
        float longestDurating = 0f;
        for (int i = 0; i < partycles.Length; i++)
        {
            
            var main = partycles[i].main;
            main.simulationSpeed = (GameManager.Instance.GameSpeed / 1.5f) * _time;

            if (longestDurating < main.duration)
                longestDurating = main.duration;
        }
        runEffectList.Add(_go);
        StartCoroutine(PushEffectObj(_go, longestDurating));
    }
    public GameObject GetEffect(string _name)
    {
        GameObject go = PoolManager.Instance.GetObj(_name, POOL_TYPE.EFFECT);
        return go;
    }
    public IEnumerator PushEffectObj(GameObject _obj, float _delay = 0)
    {
        yield return new WaitForSeconds(_delay);
        PoolManager.Instance.PushObj(_obj.name, POOL_TYPE.EFFECT, _obj);
        runEffectList.Remove(_obj);
    }

}
