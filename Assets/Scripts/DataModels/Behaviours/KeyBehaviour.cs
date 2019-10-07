using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class KeyBehaviour : MonoBehaviour
    {
        public Sprite BossKeySprite;

        private Key key;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public Key Key
        {
            get { return key; }
            set
            {
                key = value;
                key.KeyBehaviour = this;
            }
        }

        public bool IsBossKey
        {
            get { return Key.IsBossKey; }
            set
            {
                Key.IsBossKey = value;
                if (value)
                {
                    spriteRenderer.sprite = BossKeySprite;
                }
            }
        }

        public KeyBehaviour()
        {
            Key = new Key();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var pc = collision.gameObject.GetComponent<PlayerController>();

            if (pc != null)
            {
                Key.OnPickUp(pc);
                KillObject(0.0f);
            }
        }

        public void KillObject(float delay)
        {
            Destroy(gameObject, delay);
        }
    }
}