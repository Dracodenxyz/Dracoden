using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCheck : MonoBehaviour
{
public GameObject pet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void DebugCheckk()
    {
        Debug.Log("DebugCheckk");
        pet.transform.localScale = new Vector3(2f, 2f, 2f);
    }
}
