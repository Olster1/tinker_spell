using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;

public enum LevelStateId {
    LEVEL_PORTAL_ROOM,
    LEVEL_1,
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
    LEVEL_MINI_MAP,
    LEVEL_DRILL_ROOM,
    LEVEL_INSTRUCTION_CARD,

    ///////EVERTHING MUST BE ABOVE THIS!!!//////
    LEVEL_COUNT
}

[System.Serializable]
public class LevelState {
    public LevelStateId id;

    public GameObject levelObject;
    public bool turnSkyOff;
    public bool yStuck;
    public bool spellLevel;

    [HideInInspector] public GameObject loadedLevel;

}




public class SceneStateManager : MonoBehaviour
{

    private IDictionary<LevelStateId, LevelState> levels;
    //we use this one to fill it out then fill out the dictionary so doesn't matter what order  
    //they are declared in
    public LevelState[] levelInputs = new LevelState[(int)LevelStateId.LEVEL_COUNT]; 
    //

    private Animator animator;
    public LevelStateId stateToLoad;
    public GameObject player;
    private PlayerMovement playerMovement;
    public GameObject spell;
    private SpellAi spellAi;
    public GameObject cam;
    public ParticleSystem tailPs;
    [HideInInspector] public float defaultOrthoSize;
    [HideInInspector] public float defaultSafeZone;

    public GameObject skyParent;

    private Rigidbody2D camRb;

    private CameraFollowPlayer camBehaviour;

    public EarthMoveValidator earthValidator;

    public GameObject lastGameObject;

    // public CheckGrounded playerGroundedScript;
    [HideInInspector] public bool useSpawnPoint;

    //TODO: Might want this to so we don't loop through all the levels!!!
    // public LevelStateId lastActiveScene;
    //

    // Start is called before the first frame update
    void Start()
    {	
        levels = new Dictionary<LevelStateId, LevelState>();

        for(int i = 0; i < levelInputs.Length; ++i) {
            LevelState s = levelInputs[i];

            if(s.id == LevelStateId.LEVEL_TINKER_LEVEL_UP) {
              s.loadedLevel = transform.parent.Find("TinkerLevelUp").gameObject;
              Assert.IsTrue(s.loadedLevel != null);
              s.loadedLevel.SetActive(false);

            }

            levels.Add(s.id, s);

        }

        defaultOrthoSize = 10.0f;
        defaultSafeZone = 37;
        animator = gameObject.GetComponent<Animator>();
        playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement);

        spellAi = spell.GetComponent<SpellAi>();

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
        playerMovement.ResetPositionFromFall();   
    }

    public bool IsInGame() {
        bool result = !(
            stateToLoad == LevelStateId.LEVEL_QUESTS ||
            stateToLoad == LevelStateId.LEVEL_JOURNAL || 
            stateToLoad == LevelStateId.LEVEL_SKILL_TILES || 
            stateToLoad == LevelStateId.LEVEL_TINKER_LEVEL_UP || 
            stateToLoad == LevelStateId.LEVEL_MINI_MAP || 
            stateToLoad == LevelStateId.LEVEL_INSTRUCTION_CARD);
        return result;
    }

    public bool IsInGameMenu() {
        bool result = (
            stateToLoad == LevelStateId.LEVEL_QUESTS ||
            stateToLoad == LevelStateId.LEVEL_JOURNAL || 
            stateToLoad == LevelStateId.LEVEL_SKILL_TILES || 
            stateToLoad == LevelStateId.LEVEL_MINI_MAP); 
        return result;

    }

    void ResetFromSpawnPoint(Transform spawnPoint, GameObject levelObj) {
        player.transform.position = spawnPoint.position;
        earthValidator.ResetColliders();

        Vector3 spellOffset = new Vector3(0, 1, 0);
        spell.transform.position = spawnPoint.position + spellOffset;

        string nameToFind = stateToLoad.ToString() + "_CAMERA_SPAWN";
        // Debug.Log(nameToFind);
        Transform camSpawnP = levelObj.transform.Find(nameToFind);
        Assert.IsTrue(camSpawnP != null);
        
        if(camSpawnP != null) {
            Vector3 camSpawnPos = camSpawnP.position;
            // Debug.Log(camSpawnPos);
            Vector3 camPos = new Vector3(camSpawnPos.x, camSpawnPos.y, -10);
            cam.transform.position = camPos;
        } else {
            cam.transform.position = spawnPoint.position;
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
        //turn off the last scenes
        // for(int i = 0; i < levelInputs.Length; ++i) {
        //     GameObject obj = levelInputs[i].levelObject;
        //     if(obj != null && levelInputs[i].id != stateToLoad) {
        //         obj.SetActive(false);
        //     }
        // }

        //turn on new scene
        LevelState lvlState = levels[stateToLoad];
        GameObject prefab = lvlState.levelObject;
        GameObject levelObj = lvlState.loadedLevel;


        if(levelObj == null && prefab != null) {
          lvlState.loadedLevel = levelObj = Instantiate(prefab);
        }

        if(lastGameObject != null) {
          lastGameObject.SetActive(false);
        }

        lastGameObject = levelObj;

        if(levelObj != null) {
            levelObj.SetActive(true);
        }
            
        Transform spawnPoint = null;
        if(levelObj) {
            string nameToFind = stateToLoad.ToString() + "_PLAYER_SPAWN";//
            // Debug.Log(nameToFind);
            spawnPoint = levelObj.transform.Find(nameToFind);   
        }
        
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
            if(lvlState.spellLevel) {
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
        
        if(lvlState.yStuck) {
            camBehaviour.SetYStuck(true);
        } else {
            camBehaviour.SetYStuck(false);
        }


        if(lvlState.turnSkyOff) {
            skyParent.SetActive(false);
        } else {
            skyParent.SetActive(true);
        }

        // camRb.constraints |= RigidbodyConstraints2D.FreezeAll;

        if(spawnPoint != null && useSpawnPoint) {
            camBehaviour.StopFollowingEntity();
            ResetFromSpawnPoint(spawnPoint, levelObj);
        } else {
            // camBehaviour.StopFollowingEntity();
            // camBehaviour.FollowEntity();
        }

        useSpawnPoint = true;


    }
}
