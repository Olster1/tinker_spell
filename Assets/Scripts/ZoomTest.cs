using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomTest : MonoBehaviour
{

    [Range(0.0f, -20.0f)] 
    public float timeScale;

     [Range(90.0f, 110.0f)] 
    public float fov;
    // Start is called before the first frame update
    void Start()
    {
        	fov = 90.0f;
    }

    // Update is called once per frame
    void Update()
    {
    	Vector3 v = Camera.main.transform.localPosition;
    	v.z = timeScale;
        Camera.main.transform.localPosition = v;

        Camera.main.fieldOfView = fov;
    }
}
