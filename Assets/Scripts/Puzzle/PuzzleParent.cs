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

    int wrapIndex(int inVal) {
    	int outVal = inVal;

    	if(outVal >= puzzleCount) {
    		outVal = 0;
    	}
    	return outVal;

    }

    public void solvePuzzle() {
    	// int[] puzzleToCheck = new int[puzzle.Length];

    	// int startPuzzleIndex = wrapIndex(puzzleAt + 1);
    	// int offset = 0;
    	// while(startPuzzleIndex != puzzleAt) {
    	// 	for(int i = 0; i < puzzle.Length; ++i) {
    	// 		puzzleToCheck[i] = puzzleSoFar[wrapIndex(i + startPuzzleIndex)];
    	// 	}
	    // 	bool found = true;
	    // 	for(int i = 0; i < puzzle.Length; ++i) {
	    // 		int val1 = puzzle[i];
	    // 		int val2 = puzzleToCheck[i];
	    // 		if(val2 != val1) {
	    // 			found = false;
	    // 			break;
	    // 		}
	    // 	}
	    // 	offset++;
	    // }
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
