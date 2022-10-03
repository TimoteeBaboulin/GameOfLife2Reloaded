using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetButtonDown("Fire1")) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20))
        {
            Vector3 coordinates = hit.transform.position;
            
            Grid.Instance.InverseCell((int)coordinates.x, (int)coordinates.y);
        }

    }
}
