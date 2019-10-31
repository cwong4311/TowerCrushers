using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildTower : MonoBehaviour
{
    public GameObject my_tower_prefab;
    public GameObject GameState;
    private Main main;
    // Start is called before the first frame update
    void Start()
    {
        main = GameState.GetComponent<Main>();
		GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

	void TaskOnClick(){
		main.p1_selectedTower = my_tower_prefab;
	}
}
