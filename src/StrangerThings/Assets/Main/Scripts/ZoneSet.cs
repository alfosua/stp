using UnityEngine;

public class ZoneSet : MonoBehaviour
{
    [SerializeField]
    private ZoneController zoneController;

    [SerializeField]
    private Camera targetCamera;

    public Vector3 forward { get; private set; } = Vector3.forward;

    public Vector3 right { get; private set; } = Vector3.right;

    void Start()
    {
        targetCamera.enabled = false;
        targetCamera.GetComponent<AudioListener>().enabled = false;

        right = targetCamera.transform.right;
        forward = Vector3.Cross(targetCamera.transform.right, Vector3.up);
    }

    public void Activate()
    {
        targetCamera.enabled = true;
        targetCamera.GetComponent<AudioListener>().enabled = true;
    }

    public void Unactivate()
    {
        targetCamera.enabled = false;
        targetCamera.GetComponent<AudioListener>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            zoneController.PrepareNextZoneSet(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            zoneController.SwitchZoneSet();
        }
    }

    private void Reset()
    {
        zoneController = transform.parent.GetComponent<ZoneController>();
        targetCamera = GetComponentInChildren<Camera>();
    }

    public Camera GetCamera() => targetCamera;
}
