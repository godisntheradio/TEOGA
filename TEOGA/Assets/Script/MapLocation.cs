using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsEqualTo(Vector2 loc)
    {
        return transform.position.x == loc.x && transform.position.y == loc.y;
    }
}
