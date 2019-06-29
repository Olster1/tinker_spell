using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timer_namespace;


public class NextSceneTrigger : MonoBehaviour
{
	public Animator animator;
	public LevelStateId levelToLoad;
	public SceneStateManager manager;
    public Vector2 autoMoveDirection;
    public bool withText;
    public string textToShow;
    public Text textUI;
    public PlayerMovement playerMovement;

	
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

        if(gm.name == "Player" && !playerMovement.autoMoveTimer.isOn()) {
            if(!withText) {
                animator.SetTrigger("FadeIn");
            } else {
                textUI.text = textToShow;
                animator.SetTrigger("FadeIn");
                animator.SetTrigger("FadeWithText");
            }
 			
 			manager.stateToLoad = levelToLoad;
            playerMovement.autoMoveDirection = autoMoveDirection;
            playerMovement.canControlPlayer = false;
            playerMovement.autoMoveTimer.turnOn();
 		}
    }
}
