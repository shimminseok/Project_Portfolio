using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class ObjectController : MonoBehaviour
{
    [Header("ObjController")]
    public OBJ_TYPE objType;
    public AnimationController aniCtrl;
    public List<GameObject> effectList;
    public Transform effectRoot;


    public abstract void ObjectGetComponent();
    public virtual void Init() { }
    public virtual void FindEnemy() { }
    public  void ChangeState(OBJ_ANIMATION_STATE _state) 
    {
        aniCtrl.ChangeAnimation(_state);
    }
    public bool IsTargetAngle(GameObject _target, float _angle)
    {
        //타겟의 방향 
        Vector3 targetDir = (_target.transform.position - gameObject.transform.position).normalized;
        float dot = Vector3.Dot(gameObject.transform.forward, targetDir);

        if (dot > 0.99f)
            return true;
        if (dot < 0 && _angle <= 180)
            return false;

        //내적을 이용한 각 계산하기
        // thetha = cos^-1( a dot b / |a||b|)
        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
        return theta <= _angle;
    }
    public List<IHittable> GetInCircleObjects(Transform _start, float _radius)
    {
        List<IHittable> hitObjs = new List<IHittable>();
        Collider[] cols = Physics.OverlapSphere(_start.localPosition + new Vector3(0,0.3f,0), _radius);
        for (int i = 0; i < cols.Length; i++)
        {
            IHittable hitObj = cols[i].GetComponent<IHittable>();
            if (hitObj != null)
            {
                hitObjs.Add(hitObj);
            }
        }
        return hitObjs;
    }
    public List<IHittable> GetInBarObjects(Transform _start, float _width, float _range)
    {
        List<IHittable> hitObjs = new List<IHittable>();
        Vector3 dir = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
        Vector3 skillRange = new Vector3(_width, 0.5f, _range);
        Vector3 boxSenter = _start.localPosition + (_start.forward * skillRange.z * 0.5f);
        Collider[] cols = Physics.OverlapBox(boxSenter, skillRange / 2, Quaternion.LookRotation(_start.forward));
        for (int i = 0; i < cols.Length; i++)
        {
            IHittable hitObj = cols[i].GetComponent<IHittable>();
            if (hitObj != null)
            {
                hitObjs.Add(hitObj);
            }
        }
        return hitObjs;
    }

}
