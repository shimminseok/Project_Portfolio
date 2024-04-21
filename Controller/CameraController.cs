using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform targetTransform;

    [SerializeField] Vector3 offset;
    void Start()
    {
        if(mainCamera == null)
            mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        mainCamera.transform.position = targetTransform.position + offset;
        mainCamera.transform.LookAt(targetTransform.position);
    }
}
