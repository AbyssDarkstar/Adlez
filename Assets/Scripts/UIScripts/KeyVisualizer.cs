using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIScripts
{
    public class KeyVisualizer : MonoBehaviour
    {
        public Sprite[] KeySprites = new Sprite[2];

        private List<KeyImage> keyImages;
        public PlayerController PlayerController;

        private KeyImage CreateKeyImage(Vector2 anchorPosition)
        {
            var keyObject = new GameObject("Key", typeof(Image));

            keyObject.transform.parent = transform;
            keyObject.transform.localPosition = Vector3.zero;

            keyObject.GetComponent<RectTransform>().anchoredPosition = anchorPosition;
            keyObject.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);

            var keyUiImage = keyObject.GetComponent<Image>();
            keyUiImage.sprite = KeySprites[0];

            var heartImage = new KeyImage(this, keyUiImage);
            keyImages.Add(heartImage);

            return heartImage;
        }

        private void Awake()
        {
            keyImages = new List<KeyImage>();
        }

        private void Start()
        {
            var keyAnchor = new Vector2(0, 0);

            for (int i = 0; i < 4; i++)
            {
                CreateKeyImage(keyAnchor).SetKeyImage(0);
                keyAnchor += new Vector2(25, 0);
            }

            PlayerController.OnKeyAdded += PlayerController_OnKeyAdded;
            PlayerController.OnKeyRemoved += PlayerController_OnKeyRemoved;
        }

        private void PlayerController_OnKeyRemoved(object sender, EventArgs e)
        {
            RefreshAllKeys();
        }

        private void PlayerController_OnKeyAdded(object sender, EventArgs e)
        {
            RefreshAllKeys();
        }

        private void RefreshAllKeys()
        {
            var keys = PlayerController.KeyCount;

            for (int i = 0; i < keyImages.Count - 1; i++)
            {
                if (keys > 0)
                {
                    keyImages[i].SetKeyImage(1);
                    keys--;
                }
                else
                {
                    keyImages[i].SetKeyImage(0);
                }
            }

            keyImages.Last().SetKeyImage(PlayerController.HasBossKey ? 2 : 0);
        }

        public class KeyImage
        {
            private Image keyImage;
            private KeyVisualizer keyVisualizer;

            public KeyImage(KeyVisualizer visualizer, Image keyUiImage)
            {
                keyVisualizer = visualizer;
                keyImage = keyUiImage;
            }

            public void SetKeyImage(int keyType)
            {
                keyImage.sprite = keyVisualizer.KeySprites[keyType];
            }
        }
    }
}