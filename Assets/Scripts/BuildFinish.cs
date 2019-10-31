using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildFinish : MonoBehaviour
{
    public GameObject GameState;
    private Main main;
    // Start is called before the first frame update
    void Start()
    {
        main = GameState.GetComponent<Main>();
		GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

	void TaskOnClick(){
		main.phase = "build_done";
	}
}
