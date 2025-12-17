using UnityEngine;
using UnityEngine.UI;

public class PlayerFoodPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public float sphereRadius = 0.25f; // buat sedikit longgar supaya mudah kena

    public Transform holdPoint;
    public LayerMask foodLayer;
    public LayerMask plateLayer;

    public Camera cam;
    public GameObject interactIcon; // assign UI Image or GameObject (ikon "E")

    public float loseTargetDelay = 0.12f; // delay kecil untuk menghindari berkedip

    // NPC give settings
    public float npcGiveRange = 2f; // distance for giving plate to NPC
    public LayerMask npcLayer;

    private GameObject heldObject;
    private bool isHolding = false;

    // aim detection
    private GameObject aimedFood;
    private FoodPlate aimedPlate;
    private NPCPlateReceiver aimedNPC;

    private float lostTimer = 0f;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        if (interactIcon != null) interactIcon.SetActive(false);
    }

    void Update()
    {
        DetectAimedFood();

        if (!isHolding)
        {
            TryPickup();
        }
        else
        {
            // jika sedang memegang plate -> coba beri ke NPC, jika memegang makanan -> coba tempatkan ke plate
            if (heldObject != null && heldObject.GetComponent<FoodPlate>() != null)
                TryGivePlateToNPC();
            else
                TryPlaceToPlate();
        }
    }

    void DetectAimedFood()
    {
        if (cam == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        GameObject foundFood = null;
        FoodPlate foundPlate = null;
        NPCPlateReceiver foundNPC = null;

        // SphereCast cek food (abaikan makanan yang sudah di-plate atau yang sedang dipegang)
        if (Physics.SphereCast(ray, sphereRadius, out hit, pickupRange, foodLayer, QueryTriggerInteraction.Ignore))
        {
            FoodItem food = hit.collider.GetComponentInParent<FoodItem>();
            if (food != null && food.GetComponentInParent<FoodPlate>() == null && !IsPartOfHeldObject(food.gameObject))
                foundFood = food.gameObject;
        }

        // cek plate selalu (agar bisa diambil ketika penuh) — abaikan plate yang sedang dipegang pemain
        if (Physics.SphereCast(ray, sphereRadius, out hit, pickupRange, plateLayer, QueryTriggerInteraction.Ignore))
        {
            FoodPlate plate = hit.collider.GetComponentInParent<FoodPlate>();
            if (plate != null && !IsPartOfHeldObject(plate.gameObject))
                foundPlate = plate;
        }

        // jika sedang memegang nampan, cek NPC untuk memberi (gunakan npcLayer dan npcGiveRange)
        if (isHolding && heldObject != null && heldObject.GetComponent<FoodPlate>() != null)
        {
            int npcMask = npcLayer.value == 0 ? ~0 : npcLayer.value;
            if (Physics.SphereCast(ray, sphereRadius, out hit, npcGiveRange, npcMask, QueryTriggerInteraction.Ignore))
            {
                var npc = hit.collider.GetComponentInParent<NPCPlateReceiver>();
                if (npc != null && npc.CanReceivePlate())
                    foundNPC = npc;
            }
        }

        // Prioritas: jika sedang pegang item → plate (untuk ditempatkan), kalau tidak pegang → plate hanya jika bisa diambil, kalau tidak → food
        if (isHolding)
        {
            // if player is holding a plate, prefer targeting NPCs to give plate
            if (heldObject != null && heldObject.GetComponent<FoodPlate>() != null)
            {
                if (foundNPC != null)
                {
                    if (aimedNPC != foundNPC)
                    {
                        aimedNPC = foundNPC;
                        aimedFood = null;
                        aimedPlate = null;
                        lostTimer = 0f;
                        if (interactIcon != null && !interactIcon.activeSelf)
                            interactIcon.SetActive(true);
                    }
                    else
                    {
                        lostTimer = 0f;
                    }
                }
                else
                {
                    if (aimedNPC != null)
                    {
                        lostTimer += Time.deltaTime;
                        if (lostTimer >= loseTargetDelay)
                        {
                            aimedNPC = null;
                            if (interactIcon != null && interactIcon.activeSelf)
                                interactIcon.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                // holding a food item -> target plates for placement (existing behavior)
                if (foundPlate != null)
                {
                    if (aimedPlate != foundPlate)
                    {
                        aimedPlate = foundPlate;
                        aimedFood = null;
                        lostTimer = 0f;

                        if (interactIcon != null && !interactIcon.activeSelf)
                            interactIcon.SetActive(true);
                    }
                    else
                    {
                        lostTimer = 0f;
                    }
                }
                else
                {
                    // nothing / keep previous until lost
                    if (aimedFood != null || aimedPlate != null)
                    {
                        lostTimer += Time.deltaTime;
                        if (lostTimer >= loseTargetDelay)
                        {
                            aimedFood = null;
                            aimedPlate = null;
                            if (interactIcon != null && interactIcon.activeSelf)
                                interactIcon.SetActive(false);
                        }
                    }
                }
            }
        }
        else
        {
            if (foundPlate != null && foundPlate.CanBeTaken())
            {
                if (aimedPlate != foundPlate)
                {
                    aimedPlate = foundPlate;
                    aimedFood = null;
                    aimedNPC = null;
                    lostTimer = 0f;

                    if (interactIcon != null && !interactIcon.activeSelf)
                        interactIcon.SetActive(true);
                }
                else
                {
                    lostTimer = 0f;
                }
            }
            else if (foundFood != null)
            {
                if (aimedFood != foundFood)
                {
                    aimedFood = foundFood;
                    aimedPlate = null;
                    aimedNPC = null;
                    lostTimer = 0f;

                    if (interactIcon != null && !interactIcon.activeSelf)
                        interactIcon.SetActive(true);
                }
                else
                {
                    lostTimer = 0f;
                }
            }
            else
            {
                if (aimedFood != null || aimedPlate != null || aimedNPC != null)
                {
                    lostTimer += Time.deltaTime;
                    if (lostTimer >= loseTargetDelay)
                    {
                        aimedFood = null;
                        aimedPlate = null;
                        aimedNPC = null;
                        if (interactIcon != null && interactIcon.activeSelf)
                            interactIcon.SetActive(false);
                    }
                }
            }
        }
    }

    void TryPickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (aimedFood != null)
            {
                PickUpObject(aimedFood);
                return;
            }

            if (aimedPlate != null && aimedPlate.CanBeTaken())
            {
                PickUpObject(aimedPlate.gameObject);
                aimedPlate = null;
                return;
            }
        }
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

    void TryGivePlateToNPC()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // prefer the aimed NPC (what the player is looking at)
            if (aimedNPC != null)
            {
                aimedNPC.ReceivePlate();
                Destroy(heldObject);
                heldObject = null;
                isHolding = false;

                if (interactIcon != null && interactIcon.activeSelf)
                    interactIcon.SetActive(false);

                aimedNPC = null;
                return;
            }

            Collider[] hits = Physics.OverlapSphere(transform.position, npcGiveRange, npcLayer);

            foreach (Collider col in hits)
            {
                NPCPlateReceiver npc = col.GetComponent<NPCPlateReceiver>();
                if (npc != null && npc.CanReceivePlate())
                {
                    npc.ReceivePlate();
                    Destroy(heldObject);
                    heldObject = null;
                    isHolding = false;

                    if (interactIcon != null && interactIcon.activeSelf)
                        interactIcon.SetActive(false);

                    return;
                }
            }

            Debug.Log("Tidak menemukan NPC yang menerima plate!");
        }
    }

    void PickUpObject(GameObject obj)
    {
        heldObject = obj;
        isHolding = true;

        // jika yang diambil adalah plate, beri tahu plate untuk spawn pengganti
        var plate = heldObject.GetComponent<FoodPlate>();
        if (plate != null)
        {
            plate.OnTakenByPlayer();
        }

        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        if (heldObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // clear aim icon
        if (interactIcon != null && interactIcon.activeSelf)
            interactIcon.SetActive(false);
    }

    void PlaceToPlate(FoodPlate plate)
    {
        if (heldObject == null) return;

        FoodItem food = heldObject.GetComponent<FoodItem>();
        if (food != null)
        {
            plate.AddFood(food);
        }

        // reset held state (food now parented to the plate)
        heldObject = null;
        isHolding = false;
    }

    // helper: apakah objek yang kena raycast merupakan bagian dari objek yang sedang dipegang
    bool IsPartOfHeldObject(GameObject obj)
    {
        if (heldObject == null || obj == null) return false;
        if (obj == heldObject) return true;
        return obj.transform.IsChildOf(heldObject.transform);
    }
}
