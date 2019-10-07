using Assets.Scripts.DataModels;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera Camera;

    public float MovementSpeed = 0.25f;

    public WorldController GameWorld;
    public GameObject PauseMenu;

    public Sprite[] MovementSprites = new Sprite[4];

    private SpriteRenderer spriteRenderer;
    private int lastSprite = 0;
    private Direction? movePending = null;
    public Vector3? teleportPending = null;
    private bool isMoving = false;

    public event EventHandler OnKeyAdded;
    public event EventHandler OnKeyRemoved;

    public HealthSystem HealthSystem = new HealthSystem(4);

    private string currentRoomName;

    public int KeyCount { get; set; }
    public bool HasBossKey { get; set; }
    public GameObject Weapon { get; set; }

    private Direction currentDirection = Direction.Up;

    public string Name;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentRoomName = GameWorld.GetRoomAtPosition(transform.position).Name;
    }

    private Sprite GetNextSprite(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                switch (lastSprite)
                {
                    case 0:
                        lastSprite++;
                        break;
                    case 1:
                        lastSprite--;
                        break;
                    case 2:
                        lastSprite = 1;
                        break;
                    case 3:
                        lastSprite = 0;
                        break;
                    default:
                        lastSprite = 0;
                        break;
                }
                break;
            case Direction.Right:
                switch (lastSprite)
                {
                    case 4:
                        lastSprite++;
                        break;
                    case 5:
                        lastSprite--;
                        break;
                    case 6:
                        lastSprite = 5;
                        break;
                    case 7:
                        lastSprite = 4;
                        break;
                    default:
                        lastSprite = 4;
                        break;
                }
                break;
            case Direction.Left:
                switch (lastSprite)
                {
                    case 4:
                        lastSprite = 7;
                        break;
                    case 5:
                        lastSprite = 6;
                        break;
                    case 6:
                        lastSprite++;
                        break;
                    case 7:
                        lastSprite--;
                        break;
                    default:
                        lastSprite = 6;
                        break;
                }
                break;
            case Direction.Down:
                switch (lastSprite)
                {
                    case 0:
                        lastSprite = 3;
                        break;
                    case 1:
                        lastSprite = 2;
                        break;
                    case 2:
                        lastSprite++;
                        break;
                    case 3:
                        lastSprite--;
                        break;
                    default:
                        lastSprite = 2;
                        break;
                }
                break;
        }

        return MovementSprites[lastSprite];
    }

    private float moveDelay;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameState.GamePaused)
            {
                PauseMenu.SetActive(false);
                GameState.GamePaused = false;
                return;
            }
            else
            {
                GameState.GamePaused = true;
                PauseMenu.SetActive(true);
                return;
            }
        }

        if (GameState.GamePaused) { return; }

        if (movePending != null) { return; }
        if (Weapon != null && Weapon.GetComponent<SwordBehaviour>().IsAttacking) { return; }

        Vector3 newPos = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.Space) && Weapon != null)
        {
            Weapon.GetComponent<SwordBehaviour>().StartAttack(currentDirection);

            switch (currentDirection)
            {
                case Direction.Up:
                    newPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                    break;
                case Direction.Left:
                    newPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
                    break;
                case Direction.Down:
                    newPos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                    break;
                case Direction.Right:
                    newPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                    break;
                default:
                    newPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                    break;
            }
            GameWorld.CanEnterTile(newPos.x, newPos.y, true);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentDirection != Direction.Up)
            {
                currentDirection = Direction.Up;
                Weapon?.GetComponent<SwordBehaviour>().SetSwordDirection(currentDirection, transform.position);
                spriteRenderer.sprite = GetNextSprite(Direction.Up);
            }
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentDirection != Direction.Left)
            {
                currentDirection = Direction.Left;
                Weapon?.GetComponent<SwordBehaviour>().SetSwordDirection(currentDirection, transform.position);
                spriteRenderer.sprite = GetNextSprite(Direction.Left);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentDirection != Direction.Down)
            {
                currentDirection = Direction.Down;
                Weapon?.GetComponent<SwordBehaviour>().SetSwordDirection(currentDirection, transform.position);
                spriteRenderer.sprite = GetNextSprite(Direction.Down);
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentDirection != Direction.Right)
            {
                currentDirection = Direction.Right;
                Weapon?.GetComponent<SwordBehaviour>().SetSwordDirection(currentDirection, transform.position);
                spriteRenderer.sprite = GetNextSprite(Direction.Right);
            }
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (moveDelay > 0) { moveDelay -= (1 * Time.deltaTime); return; }

            newPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

            isMoving = true;

            if (GameWorld.CanEnterTile(newPos.x, newPos.y))
            {
                var newRoomName = GameWorld.GetRoomAtPosition(newPos).Name;

                if (!newRoomName.Equals(currentRoomName))
                {
                    Camera.GetComponent<CameraController>().MoveCamera(Direction.Up);
                    movePending = Direction.Up;
                    currentRoomName = newRoomName;
                    return;
                }

                transform.position = newPos;
            }

            spriteRenderer.sprite = GetNextSprite(Direction.Up);
            isMoving = false;
            moveDelay = MovementSpeed;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (moveDelay > 0) { moveDelay -= (1 * Time.deltaTime); return; }

            newPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);

            isMoving = true;

            if (GameWorld.CanEnterTile(newPos.x, newPos.y))
            {
                var newRoomName = GameWorld.GetRoomAtPosition(newPos).Name;

                if (!newRoomName.Equals(currentRoomName))
                {
                    Camera.GetComponent<CameraController>().MoveCamera(Direction.Left);
                    movePending = Direction.Left;
                    currentRoomName = newRoomName;
                    return;
                }

                transform.position = newPos;
            }

            spriteRenderer.sprite = GetNextSprite(Direction.Left);
            isMoving = false;
            moveDelay = MovementSpeed;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (moveDelay > 0) { moveDelay -= (1 * Time.deltaTime); return; }

            newPos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

            isMoving = true;

            if (GameWorld.CanEnterTile(newPos.x, newPos.y))
            {
                var newRoomName = GameWorld.GetRoomAtPosition(newPos).Name;

                if (!newRoomName.Equals(currentRoomName))
                {
                    Camera.GetComponent<CameraController>().MoveCamera(Direction.Down);
                    movePending = Direction.Down;
                    currentRoomName = newRoomName;
                    return;
                }

                transform.position = newPos;
            }

            spriteRenderer.sprite = GetNextSprite(Direction.Down);
            isMoving = false;
            moveDelay = MovementSpeed;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (moveDelay > 0) { moveDelay -= (1 * Time.deltaTime); return; }

            newPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

            isMoving = true;

            if (GameWorld.CanEnterTile(newPos.x, newPos.y))
            {
                var newRoomName = GameWorld.GetRoomAtPosition(newPos).Name;

                if (!newRoomName.Equals(currentRoomName))
                {
                    Camera.GetComponent<CameraController>().MoveCamera(Direction.Right);
                    movePending = Direction.Right;
                    currentRoomName = newRoomName;
                    return;
                }

                transform.position = newPos;
            }

            spriteRenderer.sprite = GetNextSprite(Direction.Right);
            isMoving = false;
            moveDelay = MovementSpeed;
        }

        if (Input.GetKeyUp(KeyCode.W) ||
            Input.GetKeyUp(KeyCode.UpArrow) ||
            Input.GetKeyUp(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.S) ||
            Input.GetKeyUp(KeyCode.DownArrow) ||
            Input.GetKeyUp(KeyCode.D) ||
            Input.GetKeyUp(KeyCode.RightArrow))
        {
            moveDelay = MovementSpeed;
        }

        if (Application.isEditor)
        {
            // Debug Keys
            if (Input.GetKeyDown(KeyCode.R))
            {
                newPos = new Vector3(25, 2, transform.position.z);
                var newCameraPos = new Vector3(25.5f, 5.5f, Camera.transform.position.z);
                Teleport(newPos, newCameraPos);
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                AddKey(false);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                AddKey(true);
            }
        }
    }

    public void Teleport(Vector3 destination, Vector3 cameraDestination)
    {
        transform.position = destination;
        Camera.GetComponent<CameraController>().TeleportCamera(cameraDestination);
        currentRoomName = GameWorld.GetRoomAtPosition(destination).Name;
    }

    private void LateUpdate()
    {
        if (movePending != null && !Camera.GetComponent<CameraController>().MoveRequired)
        {
            Vector3 newPos;

            switch (movePending.Value)
            {
                case Direction.Up:
                    newPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

                    transform.position = newPos;

                    spriteRenderer.sprite = GetNextSprite(Direction.Up);
                    movePending = null;
                    break;
                case Direction.Left:
                    newPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);

                    transform.position = newPos;
                    spriteRenderer.sprite = GetNextSprite(Direction.Left);
                    movePending = null;
                    break;
                case Direction.Down:
                    newPos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

                    transform.position = newPos;
                    spriteRenderer.sprite = GetNextSprite(Direction.Down);
                    movePending = null;
                    break;
                case Direction.Right:
                    newPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

                    transform.position = newPos;
                    spriteRenderer.sprite = GetNextSprite(Direction.Right);
                    movePending = null;
                    break;
            }
            isMoving = false;
        }

        if (teleportPending != null && !isMoving)
        {
            var room = GameWorld.GetRoomAtPosition(teleportPending.Value);
            var newCamPos = new Vector3(5.5f + ((room.Position.x - 1) * 10), 5.5f + ((room.Position.y - 1) * 10), Camera.transform.position.z);
            Teleport(teleportPending.Value, newCamPos);

            teleportPending = null;
        }
    }

    public void AddKey(bool bossKey = false)
    {
        if (!bossKey) { KeyCount++; }
        else { HasBossKey = true; }

        OnKeyAdded?.Invoke(this, EventArgs.Empty);
    }

    public void UseKey(bool bossKey = false)
    {
        if (!bossKey) { KeyCount--; }
        else { HasBossKey = false; }

        OnKeyRemoved?.Invoke(this, EventArgs.Empty);
    }

    public void AddSword(GameObject sword)
    {
        Weapon = Instantiate(sword);
        var sb = Weapon.GetComponent<SwordBehaviour>();
        Weapon.transform.parent = gameObject.transform;
        sb.SetSwordDirection(currentDirection, transform.position);
    }

    public void Damage(int amount)
    {
        HealthSystem.Damage(amount);
    }

    public void Heal(int amount)
    {
        HealthSystem.Heal(amount);
    }
}