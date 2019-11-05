using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildTower : MonoBehaviour
{
    public GameObject my_tower_prefab;
    public int my_tower_cost;

    // Start is called before the first frame update
    void Start()
    {
		GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

	void TaskOnClick(){
        GetComponentInParent<PlayerController>().selectedTower = my_tower_prefab;
        GetComponentInParent<PlayerController>().selectedCost = my_tower_cost;
	}
}
