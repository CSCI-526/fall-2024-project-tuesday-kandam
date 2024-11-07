using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] GameObject platform;
    [SerializeField] GameObject start;
    [SerializeField] GameObject end;
    [SerializeField] float speed = 4;
    [SerializeField] float delay = 1f;


    private Vector2 targetPosition;

    private void Start()
    {
        platform.transform.position = start.transform.position;
        targetPosition = end.transform.position;
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        while(true)
        {
            while((targetPosition - (Vector2)platform.transform.position).sqrMagnitude > 0.01f)
            {
                platform.transform.position = Vector2.MoveTowards(platform.transform.position,
                    targetPosition, speed * Time.deltaTime);
                yield return null;
            }
            targetPosition = targetPosition == (Vector2)start.transform.position ?
                end.transform.position : start.transform.position;
            yield return new WaitForSeconds(delay);
        }
    }


    private void OnDrawGizmos()
    {

        
        if(platform != null && start != null && end != null)
        {
            Gizmos.DrawLine(platform.transform.position, start.transform.position);
            Gizmos.DrawLine(platform.transform.position, end.transform.position);
        }
    }
}

