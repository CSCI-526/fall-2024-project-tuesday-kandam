using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    private Transform door;  
    private Transform player;  
    private Vector3 direction;  
    private float lastAngle = 0f;  
    private float angleThreshold = 1f;

    private Vector3 doorOffset = new Vector3(130f, -7f, 0f);  // Offset on the Door final (reasons unknown but works)

    void Start()
    {
        GameObject doorObject = GameObject.FindWithTag("Door"); 
        if (doorObject != null)
        {
            door = doorObject.transform;  
        }
        else
        {
            Debug.LogWarning("No door object found with tag 'Door'. Please assign the correct tag.");
        }

        player = Camera.main.transform;  
    }

    void Update()
    {
        if (door != null)
        {
            Vector3 adjustedDoorPos = door.position + doorOffset;

            Vector3 playerPos = player.position;

            direction = (adjustedDoorPos - playerPos).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (Mathf.Abs(angle - lastAngle) > angleThreshold)
            {
                transform.rotation = Quaternion.Euler(0, 0, angle);
                lastAngle = angle;
            }
        }
    }
}
