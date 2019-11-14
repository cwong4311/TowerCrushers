using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildTower : MonoBehaviour
{
    public string my_tower_name;
    public int my_tower_cost;

    // Start is called before the first frame update
    void Start()
    {
		  GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

	void TaskOnClick(){
      GetComponentInParent<PlayerController>().SetMyTower(my_tower_name, my_tower_cost);
	}
}
