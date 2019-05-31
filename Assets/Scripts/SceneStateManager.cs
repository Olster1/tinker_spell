using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum LevelStateId {
    LEVEL_PORTAL_ROOM,
    LEVEL_1,
    LEVEL_SKILL_TREE,


    ///////EVERTHING MUST BE ABOVE THIS!!!//////
    LEVEL_COUNT
}



public class SceneStateManager : MonoBehaviour
{
    public class LevelObject {
        
    }

    public GameObject[] levelObjects;
    public GameObject[] spawnPointObjs;
    public float[] cameraZ;
    public LevelStateId stateToLoad;
    public GameObject player;
    private PlayerMovement playerMovement;
    public GameObject spell;
    public GameObject camera;
    public float offsetCam;
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
        } else {
            // player.GetComponent<PlayerMovement>().canControlPlayer = true;
            spell.SetActive(true);
            player.SetActive(true);
        }

        // //clear grounded count to get rid of any unresolved counts
        // playerGroundedScript.groundedCount = 0;
        // //
        if(spawnPoint != null && useSpawnPoint) {
            player.transform.position = spawnPoint.transform.position;

            Vector3 spellOffset = new Vector3(0, 1, 0);
            spell.transform.position = spawnPoint.transform.position + spellOffset;
            Vector3 camPos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + offsetCam, cameraZ[(int)stateToLoad]);
            camera.transform.position = camPos;
        } 

        useSpawnPoint = true;
    }
}
