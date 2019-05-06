﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum LevelStateId {
    LEVEL_PORTAL_ROOM,
    LEVEL_1,


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
    public CheckGrounded playerGroundedScript;

    //TODO: Might want this to so we don't loop through all the levels!!!
    // public LevelStateId lastActiveScene;
    //

    // Start is called before the first frame update
    void Start()
    {	
        playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement);
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
        player.transform.position = spawnPoint.transform.position;

        //clear grounded count to get rid of any unresolved counts
        playerGroundedScript.groundedCount = 0;
        //

        Vector3 spellOffset = new Vector3(0, 1, 0);
        spell.transform.position = spawnPoint.transform.position + spellOffset;
        Vector3 camPos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + offsetCam, cameraZ[(int)stateToLoad]);
        camera.transform.position = camPos;
    }
}