using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class ButtonBehaviour : MonoBehaviour
    {
        private Button button;

        private bool hasTriggered = false;

        public Button Button
        {
            get { return button; }
            set
            {
                button = value;
                button.ButtonBehaviour = this;
            }
        }

        public Vector3 Position
        {
            get { return Button.Position; }
            set { Button.Position = value; }
        }

        public ButtonBehaviour()
        {
            button = new Button();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (hasTriggered) { return; }
            var pc = collision.gameObject.GetComponent<PlayerController>();

            if (pc != null)
            {
                pc.GameWorld.TriggerButton(Button.Action);

                hasTriggered = true;
            }
        }
    }
}