using UnityEngine;

public class Guy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Created capsule object!\n");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 disp = new Vector3(1.0f * Time.deltaTime, 0.0f, 0.0f);
        transform.position += disp;
    }
}
