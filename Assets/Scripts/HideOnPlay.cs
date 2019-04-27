using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    private SpriteRenderer spRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        spRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    
    // Update is called once per frame
    void Update()
    {
        //this is basically to hide the collision geomtry when we are playing the game. 
        //Might want to make this into a debug feature later. 
        spRenderer.enabled = false;
    }
}
