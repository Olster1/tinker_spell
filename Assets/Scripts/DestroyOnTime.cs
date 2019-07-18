using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
	public float timeLimit;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeLimit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
