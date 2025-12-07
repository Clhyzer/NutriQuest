// ...existing code...
using UnityEngine;
using UnityEngine.UI;

public class PlayerFoodPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public float sphereRadius = 0.25f;          // buat sedikit longgar supaya mudah kena
    public Transform holdPoint;
    public LayerMask foodLayer;
    public LayerMask plateLayer;
    public Camera cam;
    public GameObject interactIcon;             // assign UI Image or GameObject (ikon "E")
    public float loseTargetDelay = 0.12f;       // delay kecil untuk menghindari berkedip

    private GameObject heldObject;
    private bool isHolding = false;

    // aim detection
    private GameObject aimedFood;
    private float lostTimer = 0f;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        if (interactIcon != null)
            interactIcon.SetActive(false);
    }

    void Update()
    {
        DetectAimedFood();

        if (!isHolding)
            TryPickup();
        else
            TryPlaceToPlate();
    }

    void DetectAimedFood()
    {
        if (cam == null)
            return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        GameObject found = null;

        // SphereCast agar lebih mudah mengenai objek yang sedikit meleset
        if (Physics.SphereCast(ray, sphereRadius, out hit, pickupRange, foodLayer, QueryTriggerInteraction.Ignore))
        {
            FoodItem food = hit.collider.GetComponentInParent<FoodItem>();
            if (food != null)
                found = food.gameObject;
        }

        if (found != aimedFood)
        {
            if (found == null)
            {
                // tunda sebelum benar-benar menghilangkan target (hindari flicker singkat)
                lostTimer += Time.deltaTime;
                if (lostTimer >= loseTargetDelay)
                {
                    aimedFood = null;
                    if (interactIcon != null && interactIcon.activeSelf)
                        interactIcon.SetActive(false);
                }
            }
            else
            {
                // set target baru langsung
                aimedFood = found;
                lostTimer = 0f;
                if (interactIcon != null && !interactIcon.activeSelf)
                    interactIcon.SetActive(true);
            }
        }
        else
        {
            // jika masih sama, reset timer
            lostTimer = 0f;
        }
    }

    void TryPickup()
    {
        if (aimedFood == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            PickUpObject(aimedFood);
    }

    void TryPlaceToPlate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, plateLayer);

            foreach (Collider col in hits)
            {
                FoodPlate plate = col.GetComponent<FoodPlate>();
                if (plate != null)
                {
                    PlaceToPlate(plate);
                    return;
                }
            }

            Debug.Log("Tidak menemukan Plate di radius!");
        }
    }

    void PickUpObject(GameObject obj)
    {
        heldObject = obj;
        isHolding = true;

        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        if (heldObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (interactIcon != null && interactIcon.activeSelf)
            interactIcon.SetActive(false);
    }

    void PlaceToPlate(FoodPlate plate)
    {
        if (heldObject == null)
            return;

        FoodItem food = heldObject.GetComponent<FoodItem>();

        if (food != null)
            plate.AddFood(food);

        // detach dan reset state
        heldObject.transform.SetParent(null);
        heldObject = null;
        isHolding = false;
    }
}