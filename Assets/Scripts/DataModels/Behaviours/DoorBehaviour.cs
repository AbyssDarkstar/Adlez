using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class DoorBehaviour : MonoBehaviour, IInteractable
    {
        public Sprite BossSprite;
        public Sprite SpecialSprite;

        [HideInInspector]
        public int navMapX;

        [HideInInspector]
        public int navMapX2;

        [HideInInspector]
        public int navMapY;

        [HideInInspector]
        public int navMapY2;

        public Sprite[] Sprite;

        private Door door;

        private SpriteRenderer spriteRenderer;

        public Door Door
        {
            get { return door; }
            set
            {
                door = value;
                door.DoorBehaviour = this;
            }
        }

        public bool IsBossDoor
        {
            get { return Door.IsBossDoor; }
            set { Door.IsBossDoor = value; }
        }

        public bool IsSpecialDoor
        {
            get { return Door.IsSpecialDoor; }
            set { Door.IsSpecialDoor = value; }
        }

        public Direction Orientation
        {
            get { return Door.Orientation; }
            set { Door.Orientation = value; }
        }

        public Vector3 Position
        {
            get { return Door.Position; }
            set { Door.Position = value; }
        }

        public Vector3 Position2
        {
            get { return Door.Position2; }
            set { Door.Position2 = value; }
        }

        public DoorBehaviour()
        {
            Door = new Door();
            Sprite = new Sprite[4];
        }

        public bool CanInteract(PlayerController player)
        {
            if (IsSpecialDoor) { return false; }
            if (IsBossDoor && player.HasBossKey) { return true; }
            if (!IsBossDoor && player.KeyCount > 0) { return true; }

            return false;
        }

        public void Interact(PlayerController player)
        {
            if (!IsSpecialDoor)
            {
                player.UseKey(IsBossDoor);
                DestroyDoor();
            }
        }

        public void DestroyDoor()
        {
            Destroy(gameObject);
        }

        public void SetOrientation(Direction orientation)
        {
            if (IsSpecialDoor)
            {
                spriteRenderer.sprite = SpecialSprite; return;
            }

            Orientation = orientation;
            if (IsBossDoor)
            {
                spriteRenderer.sprite = BossSprite;
            }
            else
            {
                spriteRenderer.sprite = Sprite[(int)orientation];
            }

            switch (orientation)
            {
                case Direction.Up:
                case Direction.Down:
                    Position2 = new Vector3(Position.x + 1, Position.y, Position.z);
                    break;
                case Direction.Left:
                case Direction.Right:
                    Position2 = new Vector3(Position.x, Position.y + 1, Position.z);
                    break;
            }
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}