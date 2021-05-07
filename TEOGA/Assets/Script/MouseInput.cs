using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MouseInput : MonoBehaviour
{

    public UnityEvent<GameObject> ClickedObject;
    void Start()
    {
        
    }

    void Update()
    {
        GameObject HitObject = null;
        HitObject = Ray();
        if (HitObject)
        {
            var mapLocation = HitObject.GetComponent<MapLocation>();
            if (mapLocation != null)
            {
                ClickedObject.Invoke(HitObject);
            }
        }
    }

    private GameObject Ray()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }
}
