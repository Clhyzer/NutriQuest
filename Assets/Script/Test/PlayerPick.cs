using UnityEngine;

public class PlayerPick : MonoBehaviour
{
    public float pickupDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public Camera cam;

    private Fooditemm targetedFood;

    void Update()
    {
        DetectFood();
        Interact();
    }

    void DetectFood()
    {
        if (targetedFood != null)
            targetedFood.SetHighlight(false);

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            Fooditemm food = hit.collider.GetComponent<Fooditemm>();
            if (food != null)
            {
                targetedFood = food;
                food.SetHighlight(true);
            }
            else
            {
                targetedFood = null;
            }
        }
    }

    void Interact()
    {
        if (targetedFood != null && Input.GetKeyDown(interactKey))
        {
            GameManager.Instance.CollectFood(targetedFood);
            Destroy(targetedFood.gameObject);
            targetedFood = null;
        }
    }
}
