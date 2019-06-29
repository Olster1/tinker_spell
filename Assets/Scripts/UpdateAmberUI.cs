using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyGameManager;

public class UpdateAmberUI : MonoBehaviour
{
    private Text textElm; 
    public Text senitnelHeadElm; 
    public Text fuelCellElm; 
    // Start is called before the first frame update
    void Start()
    {
        textElm = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        textElm.text = GameManager.amberCount.ToString();
        senitnelHeadElm.text = GameManager.senintelHeadCount.ToString();
        fuelCellElm.text = GameManager.fuelCellCount.ToString();
    }
}
