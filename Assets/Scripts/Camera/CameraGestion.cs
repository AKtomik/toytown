using UnityEngine;

public class CameraGestion : MonoBehaviour
{

    public float scrollSpeed;
    [SerializeField]
    private float topBarrier;
    [SerializeField]
    private float botBarrier;
    [SerializeField]
    private float leftBarrier;
    [SerializeField]
    private float rightBarrier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.mousePosition.y >= Screen.height * topBarrier)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * scrollSpeed, Space.World);
        }
        if (Input.mousePosition.y <= Screen.height * botBarrier)
        {
            transform.Translate(Vector3.back * Time.deltaTime * scrollSpeed, Space.World);
        }
        if (Input.mousePosition.x >= Screen.width * rightBarrier)
        {
            transform.Translate(Vector3.right * Time.deltaTime * scrollSpeed, Space.World);
        }
        if (Input.mousePosition.x <= Screen.width * leftBarrier)
        {
            transform.Translate(Vector3.left * Time.deltaTime * scrollSpeed, Space.World);
        }
    }
}
