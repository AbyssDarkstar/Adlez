using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class ChestBehaviour : MonoBehaviour, IInteractable
    {
        public Sprite OpenSprite;

        private SpriteRenderer spriteRenderer;
        private Chest chest;

        public GameObject SwordPrefab;

        public Chest Chest
        {
            get { return chest; }
            set
            {
                chest = value;
                chest.ChestBehaviour = this;
            }
        }

        public bool IsOpen
        {
            get { return Chest.Open; }
            set { Chest.Open = value; }
        }

        public Vector3 Position
        {
            get { return Chest.Position; }
            set { Chest.Position = value; }
        }

        public ChestBehaviour()
        {
            Chest = new Chest();
        }

        public bool CanInteract(PlayerController player)
        {
            return !IsOpen;
        }

        public void Interact(PlayerController player)
        {
            spriteRenderer.sprite = OpenSprite;
            IsOpen = true;
            if (Chest.Contents != null)
            {
                switch (Chest.Contents.Type)
                {
                    case "Key":
                        bool isBossKey = false; ;

                        var split = Chest.Contents.MetaData.Split(',');
                        foreach (var s in split)
                        {
                            var split2 = s.Split(':');

                            switch(split2[0])
                            {
                                case "IsBossKey":
                                    isBossKey = bool.Parse(split2[1]);
                                    break;
                            }
                        }
                        var spawnPos = transform.position;                        
                        spawnPos.y += 0.5f;

                        var kb = player.GameWorld.SpawnKey(spawnPos, isBossKey, false);

                        kb.KillObject(0.75f);
                        player.AddKey(isBossKey);
                        break;
                    case "Sword":
                        var swordSpawnPos = transform.position;
                        swordSpawnPos.y += 0.5f;

                        var sb = player.GameWorld.SpawnSword(swordSpawnPos);

                        sb.KillObject(0.75f);
                        player.AddSword(SwordPrefab);
                        break;
                    default:
                        break;
                }
            }
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
