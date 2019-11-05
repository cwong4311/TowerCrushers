using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Catapult : MonoBehaviour
{
    enum TutorialStage {RotateLeft,RotateRight,ForceIncrease,ForceDecrease,Fire,Reload,Goal,Done};

    public GameObject tutorialText;
    public GameObject myBall;
    private GameObject curBall = null;
    private float multiplier = 0f;
    private float ballForce = 0f;
    private TutorialStage tutorialStage = TutorialStage.RotateLeft;
    private bool isReloading = false;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        UpdateTutorialText();
        setBall(Instantiate(myBall, transform.Find("Spawn").position, transform.Find("Spawn").rotation));
    }

    private string CalcTutorialText()
    {
        switch (tutorialStage)
        {
            case TutorialStage.RotateLeft:
                return "Press D to rotate the catapult to the left";
            case TutorialStage.RotateRight:
                return "Press A to rotate the catapult to the right";
            case TutorialStage.ForceIncrease:
                return "Press W to increase the force of the catapult";
            case TutorialStage.ForceDecrease:
                return "Press S to decrease the force of the catapult";
            case TutorialStage.Fire:
                return "Press Space to fire";
            case TutorialStage.Reload:
                return "Press R to reload the catapult, you cannot move while reloading";
            case TutorialStage.Goal:
                return "Destroy the enemy towers!";
            case TutorialStage.Done:
                return "";
        }
        return "";
    }
    private void UpdateTutorialText()
    {
        tutorialText.GetComponent<Text>().text = CalcTutorialText();
    }
    private IEnumerator AdvanceTutorialStageCoroutine()
    {
        if (tutorialStage < TutorialStage.Done)
        {
            tutorialStage = tutorialStage + 1;
        }
        UpdateTutorialText();
        if (tutorialStage == TutorialStage.Goal)
        {
            yield return new WaitForSeconds(5);
            AdvanceTutorialStage();
        }
    }
    private void AdvanceTutorialStage()
    {
        StartCoroutine(AdvanceTutorialStageCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Launch());
            if (tutorialStage == TutorialStage.Fire) AdvanceTutorialStage();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reel());
            if (tutorialStage == TutorialStage.Reload) AdvanceTutorialStage();
        }
        if (!isReloading)
        {
            if (Input.GetKey(KeyCode.D)) {
                if (transform.parent.rotation.y > -0.30f) {
                    transform.parent.Rotate(0, -1f, 0);
                }
                if (tutorialStage == TutorialStage.RotateLeft) AdvanceTutorialStage();
            }
            if (Input.GetKey(KeyCode.A)) {
                if (transform.parent.rotation.y < 0.30f) {
                    transform.parent.Rotate(0, 1f, 0);
                }
                if (tutorialStage == TutorialStage.RotateRight) AdvanceTutorialStage();
            }
            if (Input.GetKey(KeyCode.W)) {
                if (multiplier < 3) {
                    multiplier += 0.01f;
                    slider.value = multiplier;
                }
                if (tutorialStage == TutorialStage.ForceIncrease) AdvanceTutorialStage();
            }
            if (Input.GetKey(KeyCode.S)) {
                if (multiplier > 1) {
                    multiplier -= 0.01f;
                    slider.value = multiplier;
                }
                if (tutorialStage == TutorialStage.ForceDecrease) AdvanceTutorialStage();
            }
        }
    }

    IEnumerator Launch()
    {
        Vector3 forward = transform.Find("Spawn").forward;
        if (curBall != null) {
            GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, -90f);
            yield return new WaitForSeconds(.3f);
            curBall.GetComponent<Rigidbody>().AddForce(forward * multiplier * ballForce, ForceMode.Impulse);
            yield return new WaitForSeconds(.2f);
            GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0f);
            curBall = null;
        }
    }    

    IEnumerator Reel()
    {
        if (!isReloading && curBall == null)
        {
            isReloading = true;
            GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 20f);
            yield return new WaitForSeconds(.5f);
            GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0f);
            yield return new WaitForSeconds(1.5f);
            setBall(Instantiate(myBall, transform.Find("Spawn").position, Quaternion.identity));
            yield return new WaitForSeconds(.5f);
            isReloading = false;
        }
    }

    private void setBall(GameObject newBall) {
        curBall = newBall;
        //ballForce = curBall.GetComponent<ConstantForce>().force.y;
        ballForce = 1f;     //Placeholder: Each ball should be different
        multiplier = slider.value;
    }
}
