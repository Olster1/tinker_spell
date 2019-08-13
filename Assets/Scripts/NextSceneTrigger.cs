using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timer_namespace;
using ConfigControls_namespace;


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

    public GameObject indicatorPrefab;

    private InteractWithIndicate indicator;
    public bool isAutomatic;
	
    // Start is called before the first frame update
    void Start()
    {
        if(!isAutomatic) {
            indicator = Instantiate(indicatorPrefab, transform.position, Quaternion.identity).GetComponent<InteractWithIndicate>();
            indicator.spRender.flipX = autoMoveDirection.x < 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAutomatic) {
           if(!playerMovement.autoMoveTimer.isOn() && Input.GetButtonDown("Fire2") && indicator.isOn() && !Input.GetButton(ConfigControls.SPELLS_TRIGGER_BTN)) {
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

    public void OnTriggerEnter2D(Collider2D other) {
        GameObject gm = other.gameObject;
      if(isAutomatic && gm.name == "Player" && !playerMovement.autoMoveTimer.isOn()) {
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
