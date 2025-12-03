using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupSystem : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public float trayRange = 2f;       // jarak untuk bisa menaruh ke nampan
    public Transform handPoint;        // tempat barang di tangan
    public Transform trayPoint;        // tempat barang di nampan
    public LayerMask pickupLayer;      // layer object yang bisa diambil
    public float throwForce = 6f;

    [Header("UI")]
    public GameObject interactText;    // tulisan "E" muncul saat bisa diambil

    private GameObject heldObject;
    private bool isOnTray = false;

    void Update()
    {
        HandleInteraction();
    }

    void HandleInteraction()
    {
        // jika belum memegang barang
        if (heldObject == null)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
            {
                interactText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpObject(hit.collider.gameObject);
                }
            }
            else
            {
                interactText.SetActive(false);
            }
        }
        else
        {
            // jika sedang memegang barang
            interactText.SetActive(true);

            // tekan E untuk letakkan ke nampan (jika cukup dekat)
            if (Input.GetKeyDown(KeyCode.E))
            {
                float distanceToTray = Vector3.Distance(transform.position, trayPoint.position);

                if (!isOnTray && distanceToTray <= trayRange)
                {
                    PlaceOnTray();
                }
                else if (isOnTray)
                {
                    DropFromTray();
                }
                else
                {
                    // jika terlalu jauh dari nampan, beri notifikasi (opsional)
                    Debug.Log("Terlalu jauh dari nampan!");
                }
            }

            // tekan G untuk buang / lempar
            if (Input.GetKeyDown(KeyCode.G))
            {
                ThrowObject();
            }
        }
    }

    void PickUpObject(GameObject obj)
    {
        heldObject = obj;
        isOnTray = false;

        obj.transform.SetParent(handPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider col = obj.GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        interactText.SetActive(false);
    }

    void PlaceOnTray()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(trayPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
        isOnTray = true;

        interactText.SetActive(true);
    }

    void DropFromTray()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);
        isOnTray = false;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider col = heldObject.GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        interactText.SetActive(false);
        heldObject.layer = LayerMask.NameToLayer("Pickup");
        heldObject = null;
    }

    void ThrowObject()
    {
        if (heldObject == null) return;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        Collider col = heldObject.GetComponent<Collider>();

        heldObject.transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        }

        if (col != null)
            col.enabled = true;

        heldObject.layer = LayerMask.NameToLayer("Pickup");
        heldObject = null;
        interactText.SetActive(false);
        isOnTray = false;
    }
}
