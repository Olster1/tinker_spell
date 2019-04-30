using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NextSceneTrigger : MonoBehaviour
{
	public Animator animator;
	public LevelStateId levelToLoad;
	public SceneStateManager manager;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        GameObject gm = other.gameObject;

        if(gm.name == "Player") {
 			animator.SetTrigger("FadeIn");
 			manager.stateToLoad = levelToLoad;
 		}
    }
}
