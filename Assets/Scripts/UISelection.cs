using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum UICurrentSelection{
	UI_QUESTS,
	UI_BEASTERY,
	UI_SPELL_SKILL,
	UI_MAP,
	UI_INVETORY,

	////////
	UI_COUNT,


}

public class UISelection : MonoBehaviour
{
	

	[HideInInspector] public UICurrentSelection currentSelection;
	[HideInInspector] public bool controlling;
	[HideInInspector] private bool isActive;
	private Animator anim;

	public SpriteRenderer[] glowSprites = new SpriteRenderer[5]; 

	private bool yIsNew;
	private bool xIsNew;

	public AudioSource moveAudio;

	private int yCoord;

	[HideInInspector] public LevelStateId gameWorldLevelState;

	public BeastryJournal beastJournal;
	public SkillSectiom spellSkillery;
    public Journal questLog;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void Display(UICurrentSelection s_, bool control) {
    	int s = (int)s_;
    	if(isActive) {

		} else {
			for(int i = 0; i < glowSprites.Length; ++i) {
				glowSprites[i].enabled = false;
			}
			isActive = true;
			anim.SetTrigger("Show");
		}

		currentSelection = s_;
		Assert.IsTrue(s < (int)UICurrentSelection.UI_COUNT);

		controlling = control;
		if(control) {
			glowSprites[s].enabled = true;	
		}
		yCoord = s;
    }

    public void Hide() {
    	Assert.IsTrue(isActive);
    	isActive = false;
    	anim.SetTrigger("Hide");
    }

    public void EndControlling() {
    	controlling = false;
    	glowSprites[yCoord].enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(controlling) {
        	Assert.IsTrue(isActive);

        	float yAxis = Input.GetAxis("Vertical");
        	float xAxis = Input.GetAxis("Horizontal");
        	float threshold = 0.5f;
        	float lowerThreshold = 0.25f;
        	
        	bool upKeyDown = Input.GetKeyDown(KeyCode.UpArrow);
        	bool downKeyDown = Input.GetKeyDown(KeyCode.DownArrow);
        	bool rightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);

        	if(yAxis < -threshold || downKeyDown) {
        		if(yIsNew || downKeyDown) {
        			yCoord++;
        			if(yCoord >= glowSprites.Length) {
        				yCoord--;
        			} else {
        				glowSprites[yCoord - 1].enabled = false;
        				glowSprites[yCoord].enabled = true;
        				moveAudio.Play();
        			}
        			yIsNew = false;
        		}
        	} 

        	if(yAxis > threshold || upKeyDown) {
        		if(yIsNew || upKeyDown) {
        			yCoord--;
        			if(yCoord < 0) {
        				yCoord = 0;
        			} else {
        				glowSprites[yCoord + 1].enabled = false;
        				glowSprites[yCoord].enabled = true;
        				moveAudio.Play();
        			}
        			yIsNew = false;
        		}
        	} 

        	if(xAxis > threshold || rightKeyDown) {
        		if(xIsNew || rightKeyDown) {
    				EndControlling();
    				moveAudio.Play();
        			xIsNew = false;
        			if(currentSelection == UICurrentSelection.UI_BEASTERY) {
        				beastJournal.GetFocus();
        			} else if(currentSelection == UICurrentSelection.UI_SPELL_SKILL) {
        				spellSkillery.GetFocus();
        			} else if(currentSelection == UICurrentSelection.UI_QUESTS) {
                        questLog.GetFocus();
                    }
        		}
        	} 

        	if(yAxis < lowerThreshold && yAxis > -lowerThreshold) {
        		yIsNew = true;
        	}

        	if(xAxis < lowerThreshold  && xAxis > -lowerThreshold) {
        		
        		xIsNew = true;
        	}

        	if(Input.GetButtonDown("Jump") && currentSelection != (UICurrentSelection)yCoord) {
        		EndControlling();

        		if(currentSelection == UICurrentSelection.UI_BEASTERY) {
        			beastJournal.ExitMenu();
        		} else if(currentSelection == UICurrentSelection.UI_SPELL_SKILL) {
        			spellSkillery.ExitMenu();
        		} else if(currentSelection == UICurrentSelection.UI_QUESTS) {
                    questLog.ExitMenu();
                }

        		if((UICurrentSelection)yCoord == UICurrentSelection.UI_BEASTERY) {
        			beastJournal.EnterMenu();
        		} else if((UICurrentSelection)yCoord == UICurrentSelection.UI_SPELL_SKILL) {
        			spellSkillery.EnterMenu();
        		} else if((UICurrentSelection)yCoord == UICurrentSelection.UI_QUESTS) {
                    questLog.EnterMenu();
                }
        		
        		currentSelection = (UICurrentSelection)yCoord;
        	}


        }
    }
}
