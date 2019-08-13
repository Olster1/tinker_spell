using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEntityManager : MonoBehaviour
{
	private List<GameObject> entities;
	public bool activate0;
    public bool activate1;
    public bool activate2;
    public bool activate3;

    [Range(0.1f, 1f)] 
    public float timeScale;

    void Awake() {
         entities = new List<GameObject>();
    }
    void Start()
    {
    	activate0 = true;
        activate1 = false;
        activate2 = false;
        activate3 = true;
        timeScale = 1.0f;
       
    }

    public void AddEntity(GameObject obj) {
    	entities.Add(obj);
    }

    void toggleCollisionGeometry(string name) {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(name);
        // Debug.Log("key 2" + objs.Length);
        for(int i = 0; i < objs.Length; ++i) {
            objs[i].GetComponent<SpriteRenderer>().enabled = activate1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
        if(Input.GetKeyDown(KeyCode.F1)) {
            Debug.Log("key 1");
            activate0 = !activate0;
            for(int i = 0; i < entities.Count; ++i) {
                entities[i].SetActive(activate0);

                if(activate0) {
                    RockGullumAI ai = entities[i].GetComponent<RockGullumAI>();
                    if(ai != null) {
                        ai.endAttack();
                    }
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.F2)) {
            activate1 = !activate1;
            toggleCollisionGeometry("WorldGeometryEarth");
            toggleCollisionGeometry("WorldGeometryWater");
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
