using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GoToSkillTree : MonoBehaviour
{
	
	public CameraFollowPlayer cam;
	public HoverMovement hover;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter2D(Collider2D other) {
    	hover.fadeIn();
    }

    void OnTriggerExit2D(Collider2D other) {
    	hover.fadeOut();
    }
	
    void OnTriggerStay2D(Collider2D other) {
    	// Assert.IsTrue(false);
    	GameObject gm = other.gameObject;
    	if(Input.GetButtonDown("Fire1") && gm.name == "Player") {
    		cam.moveUpTimer.turnOn();
    		cam.startMovePos = cam.transform.position;
    		cam.levelToLoad = LevelStateId.LEVEL_SKILL_TREE;
    		PlayerMovement playerMovement = gm.GetComponent<PlayerMovement>();
            playerMovement.canControlPlayer = false;
    	}
    }

}
