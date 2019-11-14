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
    public float invincibilityDuration = 3; //invuln is broken, lOl

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
        originalButtonColorBlock = GetComponent<Button>().colors;
        greenButtonColorBlock = GetComponent<Button>().colors;
        greenButtonColorBlock.disabledColor = new Color(0f, 0.90f, 0.1f, 0.50f);
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
            GetComponent<Button>().interactable = false;
        }

    }

    void DeactivateInvincibility()
    {
        if ((nextInvincibility - Time.time) < (invincibilityCooldown - invincibilityDuration))
        {
            GetComponentInParent<PlayerController>().CmdDeactivateInvincibility();
            isInvincible = false;
        }

    }

    // Update is called once per frame
    void Update()
    {


        if (Time.time > nextInvincibility)
        {
            GetComponent<Button>().interactable = true;
            GetComponentInChildren<Text>().text = "Invincibility";
        } else
        {
            GetComponent<Button>().interactable = false;
            GetComponentInChildren<Text>().text = (nextInvincibility - Time.time).ToString("F2");
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

}
