using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("rendering enabled");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("rendering...");
    }

    void OnEnable()
    {
        
        
    }

    void OnDisable()
    {
        Debug.Log("rendering disabled");
    }
}
