using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Complex = System.Numerics.Complex;

public class DragArea : MonoBehaviour
{
    public float rotateSpeed;

    private bool _dragging = false;
    private Vector3 _dragStartPosition;
    private Vector3 _dragPosition;
    private Quaternion _dragStartRotation;
    private Camera _camera;
    private Transform _sphereTransform;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _sphereTransform = GameObject.Find("Sphere").transform;

        _dragStartPosition = new Vector3(0, 0, 0);
        _dragPosition = new Vector3(0, 0, 0);
        _dragStartRotation = _sphereTransform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (_dragging)
        {
            var dragVector = _dragPosition - _dragStartPosition;
            var dragZ = new Complex(dragVector.x, dragVector.y);

            // 回転軸
            var axisZ = dragZ * new Complex(0, -1);
            var axisVector = new Vector3((float) axisZ.Real, (float) axisZ.Imaginary, 0);

            // axisVector を軸に dragVector の大きさだけ回転させる
            var rotation = Quaternion.AngleAxis(dragVector.magnitude * rotateSpeed, axisVector);
            // _dragStartRotation に rotation を追加する
            // https://virtualcast.jp/blog/2019/11/quaternion/
            _sphereTransform.rotation = rotation * _dragStartRotation;
        }
    }

    private void OnMouseDown()
    {
        _dragging = true;

        var cursorPosition = GetMousePointerPosition();
        _dragStartPosition = cursorPosition;
        _dragPosition = cursorPosition;
        _dragStartRotation = _sphereTransform.rotation;
    }

    private void OnMouseDrag()
    {
        _dragPosition = GetMousePointerPosition();
    }

    private void OnMouseUp()
    {
        _dragging = false;
    }

    private Vector3 GetMousePointerPosition()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        throw new InvalidOperationException();
    }
}