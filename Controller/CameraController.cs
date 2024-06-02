using UnityEngine;
using static UnityEditor.SceneView;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform targetTransform;
    [SerializeField] Transform winCameraPos;
    [SerializeField] Vector3 offset;
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (GameManager.Instance.GameState == GAME_STATE.WIN)
        {
            CameraSetting(winCameraPos.position ,targetTransform.position);
        }
        else if (GameManager.Instance.GameState == GAME_STATE.BOSS)
        {
            CameraSetting(MonsterManager.instance.BossMon.transform.position + new Vector3(0, 3, 5), MonsterManager.instance.BossMon.transform.position,20);
        }
        else
        {
            CameraSetting(targetTransform.position + offset, targetTransform.position);
        }


    }
    void CameraSetting(Vector3 _pos, Vector3 _lookPos, float _field = 30)
    {
        mainCamera.transform.position = _pos;
        mainCamera.transform.LookAt(_lookPos);
    }
}
