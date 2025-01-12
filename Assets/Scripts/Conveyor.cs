using UnityEngine;
using System.Threading.Tasks;
using System;

public class Conveyor : MonoBehaviour
{
    public bool enablePLC = false;
    public string tagName;
    public float speed = 0;
    public bool run = false;

    PLC plc;

    Vector3 startPos = new();
    Rigidbody rb;

    Guid id = Guid.NewGuid();
    void Start()
    {
        if (enablePLC)
        {
            plc = GameObject.Find("PLC").GetComponent<PLC>();
            plc.Connect(tagName, 1, id);
            InvokeRepeating(nameof(ScanTag), 0, (float)plc.ScanTime / 1000f);
        }

        rb = GetComponentInChildren<Rigidbody>();

        startPos = rb.transform.position;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }
    void Update()
    {
        if (run)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            rb.velocity = transform.TransformDirection(Vector3.left) * speed;
            rb.transform.position = startPos;
        }
        else
        {
            if(rb.velocity != Vector3.zero && rb.velocity.y == 0)
            {
                rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                rb.transform.position = startPos;
                rb.velocity = Vector3.zero;
            }
            //when you stop, keep updating startPos for elevator
            startPos = transform.position;
        }
    }

    async Task ScanTag()
    {
        speed = await plc.Read(id);
    }
}
