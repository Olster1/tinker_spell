using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GoToSkillTree : MonoBehaviour
{
	
	public HoverMovement hover;

    private ExperienceManager skillSection;

    // Start is called before the first frame update
    void Start()
    {
        skillSection = Camera.main.GetComponent<ExperienceManager>();
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
        Assert.IsTrue(skillSection != null);
    	if(Input.GetButtonDown("Fire1") && gm.name == "Player" && !skillSection.controller.isActive) {
            skillSection.GoToTinkerLevelUp();

        }

    }

}
