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


    ///////EVERTHING MUST BE ABOVE THIS!!!//////
    LEVEL_COUNT
}



public class SceneStateManager : MonoBehaviour
{
    public class LevelObject {
        
    }

    public GameObject[] levelObjects;
    public GameObject[] spawnPointObjs;
    public Vector2[] offsetCams;
    public bool[] yStuck;
    public float[] cameraZ;
    public LevelStateId stateToLoad;
    public GameObject player;
    private PlayerMovement playerMovement;
    public GameObject spell;
    public GameObject camera;
    public ParticleSystem tailPs;

    // public CheckGrounded playerGroundedScript;
    [HideInInspector] public bool useSpawnPoint;

    //TODO: Might want this to so we don't loop through all the levels!!!
    // public LevelStateId lastActiveScene;
    //

    // Start is called before the first frame update
    void Start()
    {	
        playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement);

        int len = levelObjects.Length;
        Assert.IsTrue(spawnPointObjs.Length == len);
        Assert.IsTrue(offsetCams.Length == len);
        Assert.IsTrue(yStuck.Length == len);
        
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

    void ChangeScene() {
        for(int i = 0; i < levelObjects.Length; ++i) {
            GameObject obj = levelObjects[i];
            if(obj != null && i != (int)stateToLoad) {
                obj.SetActive(false);
            }
        }
        //turn off the last scene
        levelObjects[(int)stateToLoad].SetActive(true);    
        GameObject spawnPoint = spawnPointObjs[(int)stateToLoad];


        if(spawnPoint == null) {
            // player.GetComponent<PlayerMovement>().canControlPlayer = false;
            spell.SetActive(false);
            player.SetActive(false);
            Debug.Log("player not active");
        } else {
            player.GetComponent<PlayerMovement>().canControlPlayer = true;
            spell.SetActive(true);
            player.SetActive(true);
            tailPs.Clear();

            // tailPs.Play();
        }

        // //clear grounded count to get rid of any unresolved counts
        // playerGroundedScript.groundedCount = 0;
        // //
        
        // camera. yStuck[(int)stateToLoad];


        if(spawnPoint != null && useSpawnPoint) {
            player.transform.position = spawnPoint.transform.position;

            Vector2 offsetCam = offsetCams[(int)stateToLoad];
            Vector3 spellOffset = new Vector3(0, 1, 0);
            spell.transform.position = spawnPoint.transform.position + spellOffset;
            Vector3 camPos = new Vector3(spawnPoint.transform.position.x + offsetCam.x, spawnPoint.transform.position.y + offsetCam.y, cameraZ[(int)stateToLoad]);
            camera.transform.position = camPos;
        } 

        useSpawnPoint = true;


    }
}
