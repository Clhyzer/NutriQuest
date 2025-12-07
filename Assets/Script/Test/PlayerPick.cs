// ...existing code...
using UnityEngine;

public class PlayerPick : MonoBehaviour
{
    public float pickupDistance = 3f;
    public float sphereRadius = 0.18f;           // lebih longgar dari ray biasa
    public KeyCode interactKey = KeyCode.E;
    public Camera cam;
    public LayerMask interactLayer = ~0;         // default: semua layer
    public float loseTargetDelay = 0.12f;        // delay kecil untuk mencegah berkedip

    public GameObject interactIcon; 

    private Fooditemm targetedFood;
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
        DetectFood();
        Interact();
    }

    void DetectFood()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        // Default tidak menemukan food
        Fooditemm foundFood = null;

        // Gunakan SphereCast agar sedikit longgar; ignore trigger untuk konsistensi
        if (Physics.SphereCast(ray, sphereRadius, out hit, pickupDistance, interactLayer, QueryTriggerInteraction.Ignore))
        {
            foundFood = hit.collider.GetComponentInParent<Fooditemm>();
        }

        // Jika target berubah
        if (foundFood != targetedFood)
        {
            if (foundFood == null)
            {
                // mulai timer sebelum benar-benar kehilangan target (hindari berkedip singkat)
                lostTimer += Time.deltaTime;
                if (lostTimer >= loseTargetDelay)
                {
                    targetedFood = null;
                    if (interactIcon != null && interactIcon.activeSelf)
                        interactIcon.SetActive(false);
                }
            }
            else
            {
                // langsung set target baru
                targetedFood = foundFood;
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

    void Interact()
    {
        if (targetedFood != null && Input.GetKeyDown(interactKey))
        {
            GameManager.Instance.CollectFood(targetedFood);
            Destroy(targetedFood.gameObject);
            targetedFood = null;

            if (interactIcon != null)
                interactIcon.SetActive(false);
        }
    }
}
// ...existing code...