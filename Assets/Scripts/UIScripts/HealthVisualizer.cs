using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIScripts
{
    public class HealthVisualizer : MonoBehaviour
    {
        public Sprite[] HeartSprites = new Sprite[5];
        public PlayerController PlayerController;

        private List<HeartImage> heartImages;
        private HealthSystem healthSystem;
        

        private HeartImage CreateHeartImage(Vector2 anchoredPosition)
        {
            var heartObject = new GameObject("Heart", typeof(Image));

            heartObject.transform.parent = transform;
            heartObject.transform.localPosition = Vector3.zero;

            heartObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
            heartObject.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);

            var heartUiImage = heartObject.GetComponent<Image>();
            heartUiImage.sprite = HeartSprites[4];

            var heartImage = new HeartImage(this, heartUiImage);
            heartImages.Add(heartImage);

            return heartImage;
        }

        private void Awake()
        {
            heartImages = new List<HeartImage>();
        }

        private void Start()
        {
            SetHealthSystem(PlayerController.HealthSystem);
        }

        public void SetHealthSystem(HealthSystem newHealthSystem)
        {
            healthSystem = newHealthSystem;

            var heartAnchor = new Vector2(0, 0);
            foreach (var heart in healthSystem.GetHearts())  
            {
                CreateHeartImage(heartAnchor).SetHeartFragments(heart.GetHeartFragments());
                heartAnchor += new Vector2(25, 0);
            }

            healthSystem.OnDamaged += HealthSystem_OnDamaged;
            healthSystem.OnHealed += HealthSystem_OnHealed;
            healthSystem.OnKilled += HealthSystem_OnKilled;
        }

        private void HealthSystem_OnKilled(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HealthSystem_OnHealed(object sender, EventArgs e)
        {
            RefreshAllHearts();
        }

        private void HealthSystem_OnDamaged(object sender, EventArgs e)
        {
            RefreshAllHearts();
        }

        private void RefreshAllHearts()
        {
            var hearts = healthSystem.GetHearts();

            for (int i = 0; i < heartImages.Count; i++)
            {
                heartImages[i].SetHeartFragments(hearts[i].GetHeartFragments());
            }
        }
    }

    public class HeartImage
    {
        private Image heartImage;
        private HealthVisualizer healthVisualizer;

        public HeartImage(HealthVisualizer visualizer, Image heartUiImage)
        {
            healthVisualizer = visualizer;
            heartImage = heartUiImage;
        }

        public void SetHeartFragments(int fragments)
        {
            heartImage.sprite = healthVisualizer.HeartSprites[fragments];
        }
    }
}