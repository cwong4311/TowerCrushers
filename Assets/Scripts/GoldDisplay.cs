using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldDisplay : MonoBehaviour
{
    void Update()
    {
        // update the currency viewer for the current player
        var currency = GetComponentInParent<PlayerController>().currency;
        transform.Find("Text").GetComponent<Text>().text = currency.ToString();
    }
}
