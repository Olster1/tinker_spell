using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    public float movementSize;
    private float tAt;
    private Vector3 startPos;
    private Transform objTransform;
    // Start is called before the first frame update
    void Start()
    {
        objTransform = gameObject.GetComponent<Transform>();
        startPos = objTransform.position;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void FixedUpdate()
    {
        tAt += Time.fixedDeltaTime;
        objTransform.position = startPos + movementSize*Mathf.Sin(tAt)*Vector3.up;
    }
}
