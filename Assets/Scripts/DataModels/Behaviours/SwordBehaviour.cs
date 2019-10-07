using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class SwordBehaviour : MonoBehaviour
    {
        public Sprite[] Sprites;

        private SpriteRenderer spriteRenderer;

        public bool IsAttacking { get; set; }

        public void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetSwordDirection(Direction direction, Vector3 position)
        {
            transform.position = position;

            switch (direction)
            {
                case Direction.Up:
                    spriteRenderer.sprite = Sprites[0];
                    transform.position += new Vector3(0.25f, 0.5f, 1.0f);
                    break;
                case Direction.Left:
                    spriteRenderer.sprite = Sprites[3];
                    transform.position += new Vector3(-0.5f, -0.25f, 1.0f);
                    break;
                case Direction.Down:
                    spriteRenderer.sprite = Sprites[1];
                    transform.position += new Vector3(-0.25f, -0.65f, -1.0f);
                    break;
                case Direction.Right:
                    spriteRenderer.sprite = Sprites[2];
                    transform.position += new Vector3(0.5f, -0.25f, -1.0f);
                    break;
            }
        }

        private Vector3 currentPosition;

        public void StartAttack(Direction direction)
        {
            IsAttacking = true;
            currentPosition = transform.position;

            switch (direction)
            {
                case Direction.Up:
                    spriteRenderer.sprite = Sprites[0];
                    transform.position += new Vector3(0.0f, 0.10f, 0.0f);
                    break;
                case Direction.Left:
                    spriteRenderer.sprite = Sprites[3];
                    transform.position += new Vector3(-0.10f, 0.0f, 0.0f);
                    break;
                case Direction.Down:
                    spriteRenderer.sprite = Sprites[1];
                    transform.position += new Vector3(0.0f, -0.10f, 0.0f);
                    break;
                case Direction.Right:
                    spriteRenderer.sprite = Sprites[2];
                    transform.position += new Vector3(0.10f, 0.0f, 0.0f);
                    break;
            }

            Invoke("EndAttack", 0.25f);
        }

        public void EndAttack()
        {
            transform.position = currentPosition;
            IsAttacking = false;
        }

        public void KillObject(float delay)
        {
            Destroy(gameObject, delay);
        }
    }
}