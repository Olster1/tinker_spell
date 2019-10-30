using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using EasyGameManager;

public class TinkerSkillController : MonoBehaviour
{
	private bool xIsNew;
	private bool yIsNew;

	public AudioSource moveAudio;
    public AudioSource selectAudio;

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

    [HideInInspector] public bool isActive;

    private SoundMixer.MusicId lastSoundId;

    private SoundMixer soundMixer;

    [System.Serializable]
     public class Row
     {
         public Image[] i = new Image[10];
     }

	public Row[] sprites = new Row[4];
    public Row[] filledSprites = new Row[4];
    private SceneStateManager sceneManager;

	private ExperienceManager xpManager;
    private LevelStateId lastLevelToLoad;
    // Start is called before the first frame update
    void Start()
    {
        soundMixer = Camera.main.GetComponent<SoundMixer>();
        xCoord = 0;
        yCoord = 0;

        sceneManager = Camera.main.GetComponent<MySceneManager>().sceneStateManager;
        xpManager = Camera.main.GetComponent<ExperienceManager>();

        for(int y = 0; y < 4; ++y) {
            for(int x = 0; x < 10; ++x) {
                if(y == 0 && x == 0) {

                } else {
                    sprites[y].i[x].enabled = false;
                }
            }
        }

        for(int y = 0; y < 4; ++y) {
            for(int x = 0; x < 10; ++x) {
                if(!xpManager.tiles[y, x]) {
                    filledSprites[y].i[x].enabled = false;
                }
            }
        }

    }

    public void GoToTinkerLevelUp() {
        isActive = true;
        lastLevelToLoad = sceneManager.GetCurrentLevelState();

        sceneManager.useSpawnPoint = false;
        sceneManager.ChangeSceneWithIdFade(LevelStateId.LEVEL_TINKER_LEVEL_UP);

        skillPointsText.text = xpManager.skillPointsAvailable.ToString();
        //@Speed: string builder
        string currentLevelAsString = xpManager.currentLevel.ToString();
        levelText.text = "LV: " + currentLevelAsString;
        strengthText.text = xpManager.strength.ToString();
        shieldText.text = xpManager.defence.ToString();
        xpText.text =  xpManager.luck.ToString();
        healthText.text = xpManager.maxHealth.ToString();
        nextLevelText.text = "LV " + ((xpManager.currentLevel + 1).ToString()) + ": ";

        UpdateInfoText();

        lastSoundId = soundMixer.currentId;
        soundMixer.SetSound(SoundMixer.MusicId.SKILL_SECTION);

    }

    public void ExitLevelUp() {
        isActive = false;
        sceneManager.useSpawnPoint = false;
        soundMixer.SetSound(lastSoundId);
        Debug.Log(lastLevelToLoad);
        sceneManager.ChangeSceneWithIdFade(lastLevelToLoad);
    }

    private float GetUpdateForTile() {
        float result = 0;
        switch(yCoord) {
            case 0: { //health
                //25, 25, 50, 50, 100, 100, 100, 100, 200, 200
                if(xpManager.tiles[yCoord, 1]) {
                    if(xpManager.tiles[yCoord, 3]) {
                         if(xpManager.tiles[yCoord, 7]) {
                            result += 200;
                        } else {
                            result += 100;
                        }
                    } else {
                        result += 50;
                    }
                } else {
                    result += 25;
                }

                
            } break;
             case 1: { //strength
                //5, 5, 5, 5, 10, 10, 10, 10, 20, 20
                if(xpManager.tiles[yCoord, 3]) {
                    if(xpManager.tiles[yCoord, 7]) {
                        result += 10;
                    } else {
                        result += 20;
                    }
                } else {
                    result += 5;
                }

            } break;
             case 2: { //defence
                //all 5% max 50%
                result += 5;
            } break;
             case 3: { //luck
                //5% for each one max of 50%
                result += 5;
            } break;
            default: {
                Assert.IsTrue(false);
            } break;
        }
        return result;
    }

    void UpdateInfoText() {
        if(xpManager.tiles[yCoord, xCoord]) {
            nextStrengthText.text = "";
            nextShieldText.text = "";
            nextExperienceText.text = "";
            nextHealthText.text = "";
        } else {
            float increment = GetUpdateForTile();
            switch(yCoord) {
                case 0: {
                    //@Speed: use string builder
                    nextHealthText.text = "+" + increment.ToString();

                    //@Speed: use string builder
                   descriptionText.text = "INCREASES TINKERS HEALTH POINTS BY " + nextHealthText.text;

                   nextStrengthText.text = "";
                   nextShieldText.text = "";
                   nextExperienceText.text = "";



                } break;
                case 1: {
                    nextStrengthText.text = "+" + increment.ToString();

                     //@Speed: use string builder
                    descriptionText.text = "INCREASES TINKERS STRENGTH POINTS BY " + nextStrengthText.text;

                    nextHealthText.text = "";
                    nextShieldText.text = "";
                    nextExperienceText.text = "";

                   
                } break;
                case 2: {
                    nextShieldText.text = "+" + increment.ToString();

                   //@Speed: use string builder
                  descriptionText.text = "INCREASES TINKERS DEFENCE POINTS BY " + nextShieldText.text;

                  nextHealthText.text = "";
                  nextStrengthText.text = "";
                  nextExperienceText.text = "";
                } break;
                case 3: {
                    nextExperienceText.text = "+" + increment.ToString();

                     //@Speed: use string builder
                    descriptionText.text = "INCREASES TINKERS LUCK POINTS BY " + nextExperienceText.text;

                    nextHealthText.text = "";
                    nextStrengthText.text = "";
                    nextShieldText.text = "";
                } break;
                default: {
                    Assert.IsTrue(false);
                } break;
            }
        }
    }

    bool IsAllowedToMove() {
        bool result = false;
        if(yCoord >= 0 && yCoord < 4) {
            if(xCoord <= 0) {
                Assert.IsTrue(xCoord == 0);
                result = true;
            } else {
                result = xpManager.tiles[yCoord, xCoord - 1];    
            }
        }
        
        return result;
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
        		if(xCoord >= 10 || !xpManager.tiles[yCoord, xCoord - 1]) {
        			xCoord--;
        		} else {
        			moveAudio.Play();
        			sprites[yCoord].i[xCoord].enabled = true;
        			sprites[yCoord].i[xCoord - 1].enabled = false;

                    UpdateInfoText();
        		}

        		xIsNew = false;
        	}
        } 

        
        if(yAxis < -threshold || downKeyDown) {
        	if(yIsNew || downKeyDown) {
        		yCoord++;
        		if(yCoord >= 4 ) {
        			yCoord--;
        		} else if (IsAllowedToMove()){
                    
        			moveAudio.Play();
        			sprites[yCoord].i[xCoord].enabled = true;
        			sprites[yCoord - 1].i[xCoord].enabled = false;

                    UpdateInfoText();
        		} else {
                    yCoord--;
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

                    UpdateInfoText();
        		}
        		xIsNew = false;
        	}
        } 

        
        if(yAxis > threshold || upKeyDown) {
        	if(yIsNew || upKeyDown) {
        		yCoord--;
        		if(yCoord < 0) {
        			yCoord = 0;
        		} else if(IsAllowedToMove()) {
                    moveAudio.Play();
                    sprites[yCoord].i[xCoord].enabled = true;
                    sprites[yCoord + 1].i[xCoord].enabled = false;

                    UpdateInfoText();
        		} else {
                    yCoord++;
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
            if(xpManager.skillPointsAvailable > 0) { //has skill points available
                if(!xpManager.tiles[yCoord, xCoord]) {
                    float increment = GetUpdateForTile();
                    switch(yCoord) {
                        case 0: {
                            xpManager.maxHealth += increment;
                            GameManager.playerHealth = xpManager.maxHealth; //restore health
                            GameManager.updateHealth = true;  

                            healthText.text = xpManager.maxHealth.ToString();
                        } break;
                        case 1: {
                            xpManager.strength += increment;
                            strengthText.text = xpManager.strength.ToString();
                            
                        } break;
                        case 2: {
                            xpManager.defence += increment;
                            shieldText.text = xpManager.defence.ToString();
                           
                        } break;
                        case 3: {
                            xpManager.luck += increment;
                             xpText.text =  xpManager.luck.ToString();
                        } break;
                        default: {
                            Assert.IsTrue(false);
                        } break;
                    }
                    
                    selectAudio.Play();
                    xpManager.tiles[yCoord, xCoord] = true;
                    filledSprites[yCoord].i[xCoord].enabled = true;
                    xpManager.skillPointsAvailable--;
                    skillPointsText.text = xpManager.skillPointsAvailable.ToString();

                    if(xCoord < 9) {
                        //move across one automaticall
                        xCoord++;
                        sprites[yCoord].i[xCoord].enabled = true;
                        sprites[yCoord].i[xCoord - 1].enabled = false;
                    }

                    UpdateInfoText();

                    //NOTE: Probably want to do something when we return to the game screen to show what updated, have some 
                    //particle effects etc. 

                }
            }
        }

        if(Input.GetButtonDown("Fire1") && isActive) {
            ExitLevelUp();
            Debug.Log("exiting");
        }
    }

    


}
