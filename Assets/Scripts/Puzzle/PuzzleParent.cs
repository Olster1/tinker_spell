using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleParent : MonoBehaviour
{
	public int[] puzzle;
	private int[] puzzleSoFar;
	private int puzzleAt;
	private int puzzleCount;
	public Animator animator;
	public string triggerName;
    // Start is called before the first frame update
    void Start()
    {
        puzzleSoFar = new int[16];
    }

    public solvePuzzle() {
    	int indexToStart = puzzleAt + 1;
    	if(indexToStart >= puzzle.Length) {
    		indexToStart = 0;
    	}
    	for()
    	animator.SetTrigger(triggerName);
    }

    public void addValue(int valueToAdd) {
    	if(puzzleCount < puzzle.Length) {
    		puzzleCount++;	
    	}
    	//wrap running buffer
    	puzzleAt++;
    	if(puzzleAt >= puzzle.Length) {
    		puzzleAt = 0;
    	}

    	puzzleSoFar[puzzleAt] = valueToAdd;
    	solvePuzzle();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
