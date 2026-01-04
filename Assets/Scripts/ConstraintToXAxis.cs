using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ConstrainToXAxis : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Transform interactorTransform;
    private Vector3 offset;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactorTransform = args.interactorObject.transform;
        offset = transform.position - interactorTransform.position;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        interactorTransform = null;
    }

    private void Update()
    {
        if (interactorTransform != null)
        {
            float newX = interactorTransform.position.x + offset.x;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}