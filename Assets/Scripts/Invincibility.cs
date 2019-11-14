using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Invincibility : MonoBehaviour
{
    private ColorBlock originalButtonColorBlock;
    private ColorBlock greenButtonColorBlock;
    public bool isInvincible = false;
    public float nextInvincibility = 0;
    public float invincibilityCooldown = 30;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
        originalButtonColorBlock = GetComponent<Button>().colors;
        greenButtonColorBlock = GetComponent<Button>().colors;
        greenButtonColorBlock.normalColor = Color.green;
        greenButtonColorBlock.highlightedColor = Color.green;
    }


    void TaskOnClick()
    {
        Debug.Log("START INVINCIBILITY");
        ActivateInvincibility();
    }

    void ActivateInvincibility()
    {
        if (Time.time > nextInvincibility)
        {
            GetComponentInParent<PlayerController>().CmdActivateInvincibility();
            nextInvincibility = Time.time + invincibilityCooldown;
            isInvincible = true;
        }

    }

    void DeactivateInvincibility()
    {
        if (nextInvincibility - Time.time < 25)
        {
            GetComponentInParent<PlayerController>().CmdDeactivateInvincibility();
            isInvincible = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isInvincible)
        {
            GetComponent<Button>().colors = greenButtonColorBlock;
            DeactivateInvincibility();
        }
        else
        {
            GetComponent<Button>().colors = originalButtonColorBlock;
        }
    }

}
