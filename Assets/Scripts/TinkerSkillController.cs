using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TinkerSkillController : MonoBehaviour
{
	private bool xIsNew;
	private bool yIsNew;

	public AudioSource moveAudio;

	private int xCoord;
	private int yCoord;

    public Text skillPointsText;
    public Text levelText;
    public Text strengthText;
    public Text shieldText;
    public Text xpText;
    public Text healthText;
    public Text descriptionText;
    public Text nextLevelText;
    public Text nextHealthText;
    public Text nextStrengthText;
    public Text nextShieldText;
    public Text nextExperienceText;

    [System.Serializable]
     public class Row
     {
         public Image[] i = new Image[10];
     }

	public Row[] sprites = new Row[4];
    public SceneStateManager sceneManager;

	private ExperienceManager xpManager;
    private LevelStateId lastLevelToLoad;
    // Start is called before the first frame update
    void Start()
    {
        xCoord = 0;
        yCoord = 0;

        xpManager = Camera.main.GetComponent<ExperienceManager>();

        for(int y = 0; y < 4; ++y) {
            for(int x = 0; x < 10; ++x) {
                if(y == 0 && x == 0) {

                } else {
                    sprites[y].i[x].enabled = false;
                }
                
            }

        }

    }

    public void GoToTinkerLevelUp() {
        lastLevelToLoad = sceneManager.stateToLoad;

        sceneManager.useSpawnPoint = false;
        sceneManager.ChangeSceneWithId(LevelStateId.LEVEL_TINKER_LEVEL_UP);

       

        skillPointsText.text = xpManager.skillPointsAvailable.ToString();
        levelText.text = xpManager.currentLevel.ToString();
        strengthText.text = xpManager.strength.ToString();
        shieldText.text = xpManager.defence.ToString();
        xpText.text =  xpManager.luck.ToString();
        healthText.text = xpManager.maxHealth.ToString();
        descriptionText.text = "";
        nextLevelText.text = "";
        nextHealthText.text = "";
        nextStrengthText.text = "";
        nextShieldText.text = "";
        nextExperienceText.text = "";

    }

    public void ExitLevelUp() {
        sceneManager.useSpawnPoint = true;
        sceneManager.ChangeSceneWithId(lastLevelToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        float threshold = 0.5f;
        float lowerThreshold = 0.25f;

        
        bool leftKeyDown = Input.GetKeyDown(KeyCode.LeftArrow);
        bool rightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);
        bool upKeyDown = Input.GetKeyDown(KeyCode.UpArrow);
        bool downKeyDown = Input.GetKeyDown(KeyCode.DownArrow);
        
        if(xAxis > threshold || rightKeyDown) {
        	if(xIsNew || rightKeyDown) {
        		xCoord++;
        		if(xCoord >= 10) {
        			xCoord--;
        		} else {
        			moveAudio.Play();
        			sprites[yCoord].i[xCoord].enabled = true;
        			sprites[yCoord].i[xCoord - 1].enabled = false;
        		}

        		xIsNew = false;
        	}
        } 

        
        if(yAxis < -threshold || downKeyDown) {
        	if(yIsNew || downKeyDown) {
        		yCoord++;
        		if(yCoord >= 10) {
        			yCoord--;
        		} else {
        			moveAudio.Play();
        			sprites[yCoord].i[xCoord].enabled = true;
        			sprites[yCoord - 1].i[xCoord].enabled = false;
        		}
        		yIsNew = false;
        	}
        } 

        if(xAxis < -threshold || leftKeyDown) {
        	if(xIsNew || leftKeyDown) {
        		xCoord--;
        		if(xCoord < 0) {
        			xCoord = 0;
        		} else {
        			moveAudio.Play();
        			sprites[yCoord].i[xCoord].enabled = true;
					sprites[yCoord].i[xCoord + 1].enabled = false;
        		}
        		xIsNew = false;
        	}
        } 

        
        if(yAxis > threshold || upKeyDown) {
        	if(yIsNew || upKeyDown) {
        		yCoord--;
        		if(yCoord < 0) {
        			yCoord = 0;
        		} else {
        			moveAudio.Play();
        			sprites[yCoord].i[xCoord].enabled = true;
        			sprites[yCoord + 1].i[xCoord].enabled = false;
        		}
        		yIsNew = false;
        	}
        } 

        //Reset the stick values
        if(xAxis < lowerThreshold  && xAxis > -lowerThreshold) {
        	xIsNew = true;
        } 

        if(yAxis < lowerThreshold && yAxis > -lowerThreshold) {
        	yIsNew = true;
        }
        if(Input.GetButtonDown("Jump")) {

        }

        if(Input.GetButtonDown("Fire1")) {
            ExitLevelUp();
        }
    }

    


}
