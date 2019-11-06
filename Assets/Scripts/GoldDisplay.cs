using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var currency = GetComponentInParent<PlayerController>().currency;
        transform.Find("Text").GetComponent<Text>().text = "Gold Remaining:\n" + currency.ToString();
    }
}
