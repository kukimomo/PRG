using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testUnityFunction : MonoBehaviour
{
    private Transform[] myChildObject;
    // Start is called before the first frame update
    void Start()
    {
        myChildObject = gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < myChildObject.Length ; i++)
        {
            
            Debug.Log(myChildObject[i].name+"     "+myChildObject[i].GetSiblingIndex());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
