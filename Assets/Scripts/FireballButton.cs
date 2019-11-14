using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FireballButton : MonoBehaviour
{
    public Camera camera;
    public GraphicRaycaster graphicRaycaster;
    private ColorBlock originalButtonColorBlock;
    private ColorBlock redButtonColorBlock;
    private bool isFireballTargeting = false;

    private float nextFireBall = 0;
    private float fireballCooldown = 10;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
        originalButtonColorBlock = GetComponent<Button>().colors;
        redButtonColorBlock = GetComponent<Button>().colors;
        redButtonColorBlock.normalColor = Color.red;
        redButtonColorBlock.highlightedColor = Color.red;
    }

    void SetFireballTargeting(bool value)
    {
        isFireballTargeting = value;
        if (isFireballTargeting)
        {
            GetComponent<Button>().colors = redButtonColorBlock;
        }
        else
        {
            GetComponent<Button>().colors = originalButtonColorBlock;
        }
    }

    void TaskOnClick()
    {
        SetFireballTargeting(!isFireballTargeting);
    }

    bool IsMouseOverSelf()
    {
        // https://answers.unity.com/questions/1526663/detect-click-on-canvas.html
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();
        pointerData.position = Input.mousePosition;
        this.graphicRaycaster.Raycast(pointerData, results);
        return results.Exists((RaycastResult result) => result.gameObject == gameObject);
    }

    bool GetHitPosition(out Vector3 hitPos)
    {
        // https://docs.unity3d.com/Manual/CameraRays.html
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        hitPos = ray.GetPoint(40);
        return true;

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextFireBall)
        {
            GetComponent<Button>().interactable = true;
            GetComponentInChildren<Text>().text = "Fireball";
        } else
        {
            GetComponent<Button>().interactable = false;
            GetComponentInChildren<Text>().text = (nextFireBall - Time.time).ToString("F2");
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (isFireballTargeting)
            {
                if (IsMouseOverSelf()) return;
                Vector3 hit;
                if (GetHitPosition(out hit))
                {
                    GetComponentInParent<PlayerController>().ShootFireball(camera.transform.position, hit - camera.transform.position);
                    SetFireballTargeting(false);

                    nextFireBall = Time.time + fireballCooldown;
                }
            }
        }

    }
}
