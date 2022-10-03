using System;
using System.Collections;
using System.Collections.Generic;
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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20))
        {
            Vector3 coordinates = hit.transform.position;
            
            Grid.Instance.InverseCell((int)coordinates.x, (int)coordinates.y);
        }

    }

    private void Move()
    {
        return;
    }
}
