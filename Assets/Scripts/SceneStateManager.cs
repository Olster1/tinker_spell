using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;

public enum LevelStateId {
    LEVEL_PORTAL_ROOM,
    LEVEL_1,
    LEVEL_SKILL_TREE,
    LEVEL_PORTAL_ROOM_RETURN,
    LEVEL_EARTH_LEDGE,
    LEVEL_1_RETURN,
    LEVEL_JOURNAL,
    LEVEL_SKILL_TILES,
    LEVEL_EARTH_CAVE,
    LEVEL_EARTH_LEDGE_RETURN,
    LEVEL_EARTH_LEDGE_HOWLER_TREE,
    HELICOPTER_LEVEL,
    LEVEL_TINKER_LEVEL_UP,
    LEVEL_QUESTS,

    ///////EVERTHING MUST BE ABOVE THIS!!!//////
    LEVEL_COUNT
}



public class SceneStateManager : MonoBehaviour
{
    public class LevelObject {
        
    }
    private Animator animator;
    public GameObject[] levelObjects;
    public GameObject[] spawnPointObjs;
    public GameObject[] cameraSpawnPoints;
    public bool[] turnSkyOff;
    public bool[] yStuck;
    public float[] cameraZ;
    public bool[] spellLevel;
    private LevelStateId stateToLoad;
    public GameObject player;
    private PlayerMovement playerMovement;
    public GameObject spell;
    private SpellAi spellAi;
    public GameObject cam;
    public ParticleSystem tailPs;

    public GameObject skyParent;

    private Rigidbody2D camRb;

    private CameraFollowPlayer camBehaviour;

    public EarthMoveValidator earthValidator;

    // public CheckGrounded playerGroundedScript;
    [HideInInspector] public bool useSpawnPoint;

    //TODO: Might want this to so we don't loop through all the levels!!!
    // public LevelStateId lastActiveScene;
    //

    // Start is called before the first frame update
    void Start()
    {	
        animator = gameObject.GetComponent<Animator>();
        playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement);

        spellAi = spell.GetComponent<SpellAi>();

        int len = levelObjects.Length;
        Assert.IsTrue(spawnPointObjs.Length == len);
        Assert.IsTrue(cameraSpawnPoints.Length == len);
        Assert.IsTrue(yStuck.Length == len);
        Assert.IsTrue(turnSkyOff.Length == len);
        Assert.IsTrue(spellLevel.Length == len);

        camRb = cam.GetComponent<Rigidbody2D>();

        camBehaviour = cam.GetComponent<CameraFollowPlayer>();
        
        useSpawnPoint = true;
        ChangeScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UnPausePlayer() {
        playerMovement.canControlPlayer = true;
    }

    void PausePlayer() {
        playerMovement.canControlPlayer = false;
    }

    void PauseAutoMoveTimer() {
        playerMovement.autoMoveTimer.paused = true;
    }

    void UnPauseAutoMoveTimer() {
        playerMovement.autoMoveTimer.paused = false;
    }

    void ResetFromFall() {
        // GameObject spawnPoint = spawnPointObjs[(int)stateToLoad];
        // ResetFromSpawnPoint(spawnPoint);
        // playerMovement.autoMoveTimer.turnOn();
        // playerMovement.autoMoveTimer = false;
        playerMovement.ResetPositionFromFall();   
    }

    public bool IsInGame() {
        bool result = !(stateToLoad == LevelStateId.LEVEL_QUESTS || stateToLoad == LevelStateId.LEVEL_JOURNAL || stateToLoad == LevelStateId.LEVEL_SKILL_TILES);
        return result;
    }

    void ResetFromSpawnPoint(GameObject spawnPoint) {
        player.transform.position = spawnPoint.transform.position;
        earthValidator.ResetColliders();

        Vector3 spellOffset = new Vector3(0, 1, 0);
        spell.transform.position = spawnPoint.transform.position + spellOffset;

        GameObject camSpawnP = cameraSpawnPoints[(int)stateToLoad];
        if(camSpawnP) {
            Vector3 camSpawnPos = camSpawnP.transform.position;
            Vector3 camPos = new Vector3(camSpawnPos.x, camSpawnPos.y, cameraZ[(int)stateToLoad]);
            cam.transform.position = camPos;
        } else {
            cam.transform.position = spawnPoint.transform.position;
        }
    }

    public void ChangeSceneWithId(LevelStateId stateId) {
        stateToLoad = stateId;
        ChangeScene();
    }

    public void ChangeSceneWithIdFade(LevelStateId stateId) {
        stateToLoad = stateId;
        animator.SetTrigger("FadeIn");
    }

    public void SetStateToLoad(LevelStateId stateId) {
        stateToLoad = stateId;
    }

    public LevelStateId GetCurrentLevelState() {
        return stateToLoad;
    }

    void ChangeScene() {
        // Debug.Log("loading: " + stateToLoad);
        for(int i = 0; i < levelObjects.Length; ++i) {
            GameObject obj = levelObjects[i];
            if(obj != null && i != (int)stateToLoad) {
                obj.SetActive(false);
            }
        }

        //turn off the last scene
        GameObject levelObj = levelObjects[(int)stateToLoad];
        if(levelObj != null) {
            levelObj.SetActive(true);
        }
            
        GameObject spawnPoint = spawnPointObjs[(int)stateToLoad];


        if(spawnPoint == null) {
            // player.GetComponent<PlayerMovement>().canControlPlayer = false;
            spell.SetActive(false);
            player.SetActive(false);
            // Debug.Log("player not active");
        } else {
            player.GetComponent<PlayerMovement>().canControlPlayer = true;

            spell.SetActive(true);
            player.SetActive(true);
            tailPs.Clear();

            // tailPs.Play();
        }

        if(spawnPoint != null) { 
            if(spellLevel[(int)stateToLoad]) {
                  player.GetComponent<PlayerMovement>().canControlPlayer = false;
                  player.SetActive(false);
                  spellAi.controllingSpell = true;
                  spellAi.isSpellLevel = true;
                  spellAi.rigidBody.simulated = true;
                  spellAi.rbCol.enabled = true;
                  camBehaviour.changeEntityToFollow(BodyToFollow.SPELL_BODY);
                  spellAi.ClearForcesForSpell();
            } else {
                spellAi.ClearForcesForSpell();
                camBehaviour.changeEntityToFollow(BodyToFollow.PLAYER_BODY);
                spellAi.controllingSpell = false;
                spellAi.isSpellLevel = false;
                spellAi.rigidBody.simulated = false;
                spellAi.rbCol.enabled = false;
            }
            camBehaviour.FollowEntity();
        } else {
            camBehaviour.StopFollowingEntity();
        }

        // //clear grounded count to get rid of any unresolved counts
        // playerGroundedScript.groundedCount = 0;
        // //
        
        if(yStuck[(int)stateToLoad]) {
            camBehaviour.SetYStuck(true);
        } else {
            camBehaviour.SetYStuck(false);
        }


        if(turnSkyOff[(int)stateToLoad]) {
            skyParent.SetActive(false);
        } else {
            skyParent.SetActive(true);
        }

        // camRb.constraints |= RigidbodyConstraints2D.FreezeAll;

        if(spawnPoint != null && useSpawnPoint) {
            camBehaviour.StopFollowingEntity();
            ResetFromSpawnPoint(spawnPoint);
        } else {
            // camBehaviour.StopFollowingEntity();
            // camBehaviour.FollowEntity();
        }

        useSpawnPoint = true;


    }
}
