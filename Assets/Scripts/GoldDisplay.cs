using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldDisplay : MonoBehaviour
{
    public GameObject GameState;
    private Main main;
    // Start is called before the first frame update
    void Start()
    {
        main = GameState.GetComponent<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("Text").GetComponent<Text>().text = "Gold Remaining:\n" + main.p1_remainingCost.ToString();
    }
}
