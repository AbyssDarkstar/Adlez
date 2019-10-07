using Assets.Scripts.DataModels;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int ScrollSpeed = 5;
        
    private Vector3 targetPosition;
    [HideInInspector]
    public bool MoveRequired = false;

    private void LateUpdate()
    {
        var camera = GetComponent<Camera>();

        if (MoveRequired)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * ScrollSpeed);
            if($"{transform.position:#.0}" == $"{targetPosition:#.0}")
            {
                transform.position = targetPosition;
                MoveRequired = false;
            }
        }
    }

    public void TeleportCamera(Vector3 position, bool withFade = false)
    {
        transform.position = position;
    }

    public void MoveCamera(Direction direction)
    {
        if (!MoveRequired)
        {
            switch (direction)
            {
                case Direction.Up:
                    targetPosition = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
                    MoveRequired = true;
                    break;
                case Direction.Left:
                    targetPosition = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
                    MoveRequired = true;
                    break;
                case Direction.Down:
                    targetPosition = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
                    MoveRequired = true;
                    break;
                case Direction.Right:
                    targetPosition = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
                    MoveRequired = true;
                    break;
            }
        }
    }
}