using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogQuestionEvent : MonoBehaviour
{
	public NextSceneTrigger nextScene; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAccept() {
    	nextScene.GoToNextScene();
    }

    public void OnDecline() {

    }
}

