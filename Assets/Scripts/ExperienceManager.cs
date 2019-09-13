using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
	public float maxHealth = 50;
	public float strength = 15;
	public float defence = 0;
	public float luck = 0;

	public bool[,] tiles = new bool[4, 10];

	public TinkerSkillController controller;

	public int currentLevel;
	public int skillPointsAvailable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GoToTinkerLevelUp() {
    	controller.GoToTinkerLevelUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
