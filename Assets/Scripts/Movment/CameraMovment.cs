using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraMovment : MonoBehaviour
{
    private Transform _target;
    private Vector3 _offset;

    public float smoothSpeed = 0.1f;

    private bool isInit = false;

    public void Init(Transform target)
    {
        _target = target;
        transform.position = new Vector3(target.position.x + 5, 30, target.position.y);
        _offset = transform.position - _target.position;
        isInit = true;
    }

    private void LateUpdate()
    {
        if(isInit)
            SmoothFollow();
    }

    private void SmoothFollow()
    {
        Vector3 targetPos = _target.position + _offset;
        Vector3 smoothFollow = Vector3.Lerp(transform.position, targetPos, smoothSpeed);

        transform.position = smoothFollow;
        transform.LookAt(_target);
    }
}
