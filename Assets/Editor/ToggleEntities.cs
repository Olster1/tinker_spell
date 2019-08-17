using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleEntities : MonoBehaviour
{
    public bool activate1;
    public bool activate2;
    public bool activate3;

    void toggleCollisionGeometry(string name) {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(name);
        // Debug.Log("key 2" + objs.Length);
        for(int i = 0; i < objs.Length; ++i) {
            objs[i].GetComponent<SpriteRenderer>().enabled = activate1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if(Input.GetKeyDown(KeyCode.F2)) {
            activate1 = !activate1;
            toggleCollisionGeometry("WorldGeometryEarth");
            toggleCollisionGeometry("WorldGeometryWater");
            toggleCollisionGeometry("WorldGeometrySlope");
        }
        if(Input.GetKeyDown(KeyCode.F3)) {
           activate2 = !activate2;
            GameObject[] objs = GameObject.FindGameObjectsWithTag("CameraCollision");
            for(int i = 0; i < objs.Length; ++i) {
                objs[i].GetComponent<SpriteRenderer>().enabled = activate2;
            }
        }
        if(Input.GetKeyDown(KeyCode.F4)) {
            activate3 = !activate3;
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Scenery");
            for(int i = 0; i < objs.Length; ++i) {
                objs[i].GetComponent<SpriteRenderer>().enabled = activate3;
            }
        }
    }
}