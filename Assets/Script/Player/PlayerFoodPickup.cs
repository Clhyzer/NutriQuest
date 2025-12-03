using UnityEngine;

public class PlayerFoodPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public Transform holdPoint;
    public LayerMask foodLayer;
    public LayerMask plateLayer;

    private GameObject heldObject;
    private bool isHolding = false;

    void Update()
    {
        if (!isHolding)
            TryPickup();
        else
            TryPlaceToPlate();
    }

    void TryPickup()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, foodLayer))
        {
            if (Input.GetKeyDown(KeyCode.E))
                PickUpObject(hit.collider.gameObject);
        }
    }

    void TryPlaceToPlate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 3f, plateLayer);

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
    }

    void PlaceToPlate(FoodPlate plate)
    {
        FoodItem food = heldObject.GetComponent<FoodItem>();

        if (food != null)
            plate.AddFood(food);

        heldObject = null;
        isHolding = false;
    }
}
