using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public LevelStateId stateToLoad;
    public GameObject player;
    public GameObject spell;

    //TODO: Might want this to so we don't loop through all the levels!!!
    // public LevelStateId lastActiveScene;
    //

    // Start is called before the first frame update
    void Start()
    {	
        ChangeScene();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        Vector3 spellOffset = new Vector3(0, 1, 0);
        spell.transform.position = spawnPoint.transform.position + spellOffset;
    }
}
