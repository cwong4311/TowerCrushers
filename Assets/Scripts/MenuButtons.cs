using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public GameObject GameState;
    private Main main;
    public string my_phase_name;
    // Start is called before the first frame update
    void Start()
    {
        main = GameState.GetComponent<Main>();
		GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

	void TaskOnClick(){
		main.phase = my_phase_name;
	}
}
