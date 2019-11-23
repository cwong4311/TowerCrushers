using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpireTime : MonoBehaviour
{
    private float lifetime = 1f;
    private bool primed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (primed) {
            StartCoroutine(Decay(lifetime));
            primed = false;
        }
    }

    IEnumerator Decay(float wait) 
    {
        yield return new WaitForSeconds(wait);
        Destroy(gameObject);
    }

    public void StartLife(float my_life) {
        primed = true;
        lifetime = my_life;
    }
}
