using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
        int indexAt = wrapIndex(puzzleAt + 1);
        
        bool solvedPuzzle = false;
        bool stillPossible = true;
        while(stillPossible) {
            bool found = true;
            for(int i = 0; i < puzzle.Length; i++) {
                int tempIndex = wrapIndex(indexAt + i);
                if(tempIndex == puzzleAt) {
                    //not enought to fill the puzzle
                    Assert.IsTrue(!solvedPuzzle);
                    stillPossible = false;
                    break;
                } else {
                    int val1 = puzzle[i];
                    int val2 = puzzleSoFar[tempIndex];
                    if(val2 != val1) {
                        found = false;
                        break;
                    }
                }
            }
            if(stillPossible && found) {
                solvedPuzzle = true;
                stillPossible = false;
            }
            
            if((indexAt + 1) == puzzleAt) {
                //shouldn't ever get here
                Assert.IsTrue(false);
                stillPossible = false;
            } else {
                //increment index
                indexAt = wrapIndex(indexAt + 1);
            }
        }

        if(solvedPuzzle) {
            Debug.Log("solved puzzle");
        }
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
