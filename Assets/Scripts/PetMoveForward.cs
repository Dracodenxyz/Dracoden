using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetMoveForward : MonoBehaviour
{
    public bool moveForward = false;
    public GameObject TransformBtn;
    public Transform Camera, littlePet, TransformedPet;
    public bool moveCamera = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moveForward)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 2f);
        }
        if(moveCamera)
        {
            Camera.Translate(Vector3.forward * Time.deltaTime * 4f);
        }
    }
    
    public void MoveForward()
    {
        moveForward = true;
        TransformBtn.SetActive(false);
        Invoke("MoveCamera", 3f);
    }   

    public void MoveCamera()
    {
        Debug.Log("MoveCamera");
        moveCamera = true;
        littlePet.gameObject.SetActive(false);
        Invoke("TurnCamera", 4f);
    }

    public void TurnCamera()
    {
        TransformedPet.gameObject.SetActive(true);
        moveCamera = false;
        Camera.Rotate(0, 180, 0);        
    }
}
