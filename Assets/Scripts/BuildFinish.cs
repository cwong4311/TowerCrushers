using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildFinish : MonoBehaviour
{
  public int hi;
    // Start is called before the first frame update
    void Start()
    {
		GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick(){
        GetComponentInParent<PlayerController>().phase = Phases.PLAY;
	}
}
