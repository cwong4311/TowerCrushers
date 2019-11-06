using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Catapult : NetworkBehaviour 
{
    enum TutorialStage {RotateLeft,RotateRight,ForceIncrease,ForceDecrease,Fire,Reload,Goal,Done};

    public GameObject tutorialText;
    public GameObject myBall;
    public Transform ballSpawn;
    private GameObject curBall = null;
    private float ballForce = 0f;
    public float multiplier = 2f;
    //private TutorialStage tutorialStage = TutorialStage.RotateLeft;
    private bool isReloading = false;

    private Quaternion maxRightRot = Quaternion.Euler(0f, -60f, 0f);
    private Quaternion maxLeftRot = Quaternion.Euler(0f, 60f, 0f);
    private readonly float catapultRotStep = 1.0f;
    private readonly float multiplierStep = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        if (!isServer && hasAuthority) 
        {
            transform.Rotate(0f, 180f, 0f);
            maxRightRot *= Quaternion.Euler(0f, 180f, 0f);
            maxLeftRot *= Quaternion.Euler(0f, 180f, 0f);
        }
        CmdSpawnBall();
     //   UpdateTutorialText();
    }

    /*
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
    */

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdLaunch();
     //       if (tutorialStage == TutorialStage.Fire) AdvanceTutorialStage();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdReel();
      //      if (tutorialStage == TutorialStage.Reload) AdvanceTutorialStage();
        }
        //       if (!isReloading)
        //       {
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, maxRightRot, catapultRotStep);
            //         if (tutorialStage == TutorialStage.RotateLeft) AdvanceTutorialStage();
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, maxLeftRot, catapultRotStep);
            //        if (tutorialStage == TutorialStage.RotateRight) AdvanceTutorialStage();
        }

        /*
        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("up" + multiplier);
            //       if (tutorialStage == TutorialStage.ForceIncrease) AdvanceTutorialStage();
        }
        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("down" + multiplier);
            //      if (tutorialStage == TutorialStage.ForceDecrease) AdvanceTutorialStage();
        }
        */
        //       }
    }

    [Command]
    void CmdLaunch()
    {
        StartCoroutine(Launch());
    }

    [Command]
    void CmdReel()
    {
        StartCoroutine(Reel());
    }

    IEnumerator Launch()
    {
        if (curBall != null)
        {
            Vector3 forward = ballSpawn.forward;
            RpcChangePivotAngularVelocity(new Vector3(0, 0, -90f));
            yield return new WaitForSeconds(.3f);
            curBall.GetComponent<Rigidbody>().AddForce(forward * multiplier * ballForce, ForceMode.Impulse);
            yield return new WaitForSeconds(.2f);
            RpcChangePivotAngularVelocity(new Vector3(0, 0, 0f));

            curBall = null;
        }
    }

    IEnumerator Reel()
    {
        if (!isReloading)
        {
            isReloading = true;
            RpcChangePivotAngularVelocity(new Vector3(0, 0, 20f));
            yield return new WaitForSeconds(.5f);
            RpcChangePivotAngularVelocity(new Vector3(0, 0, 0f));
            yield return new WaitForSeconds(1.5f);

            CmdSpawnBall();
            isReloading = false;
        }
    }

    [ClientRpc]
    void RpcChangePivotAngularVelocity(Vector3 v)
    {
        var pipe = transform.Find("Pivot").Find("Pipe").gameObject;
        pipe.GetComponent<Rigidbody>().angularVelocity = v;
    }

    [Command]
    public void CmdSpawnBall() {
        curBall = Instantiate(myBall, ballSpawn.position, Quaternion.identity);
        ballForce = 1f;

        NetworkServer.Spawn(curBall);
    }
}
