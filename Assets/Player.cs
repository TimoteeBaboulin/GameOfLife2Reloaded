using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        Move();
        if (!Input.GetButtonDown("Fire1")) return;

        Vector3 coordinates = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(coordinates);
        CellGrid.Instance.InverseCell((int)Math.Floor(coordinates.x), (int)Math.Floor(coordinates.y));
    }

    private void Move()
    {
        return;
    }
}
