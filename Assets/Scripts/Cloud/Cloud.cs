using System;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public static event Action<Transform> PlayerOnCloud;
    //public static event Action<Transform> PlayerOutFromCloud;
    

    [SerializeField] private float speed = 2.5f;
    [SerializeField] private MovementDirection moveHorizontally = MovementDirection.UpAndDown;
    [SerializeField] private Color color;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float vanishCooldown = 5f;
    

    private Vector3 _direction;

    private void Start()
    {
        _direction = (moveHorizontally == MovementDirection.UpAndDown) ?  Vector3.up :Vector3.right;
    }

    private void Update()
    {
        transform.Translate(_direction * speed * Time.deltaTime);
    }
    


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnPlayerOnCloud();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Walls"))
        {
            transform.position = startPosition;
        }
    }

    enum MovementDirection
    {
        SideToSide = 0,
        UpAndDown = 1
    }

    private void OnPlayerOnCloud()
    {
        PlayerOnCloud?.Invoke(this.transform);
    }
    
    
    //TODO - Check if "PlayerOutFromCloud" event is necessary
    /*private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            OnPlayerOutFromCloud();
                  }
    }*/
    /*private void OnPlayerOutFromCloud()
{
    PlayerOutFromCloud?.Invoke(this.transform);
}*/

}