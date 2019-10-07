using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class BushBehaviour : MonoBehaviour
    {
        private Bush bush;

        public Bush Bush
        {
            get { return bush; }
            set
            {
                bush = value;
                bush.BushBehaviour = this;
            }
        }

        public Vector3 Position
        {
            get { return Bush.Position; }
            set { Bush.Position = value; }
        }

        public BushBehaviour()
        {
            bush = new Bush();
        }

        public void Break()
        {
            Destroy(gameObject);
        }
    }
}