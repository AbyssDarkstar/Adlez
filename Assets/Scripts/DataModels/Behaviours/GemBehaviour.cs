using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class GemBehaviour : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var pc = collision.gameObject.GetComponent<PlayerController>();

            if (pc != null)
            {
                SceneLoader.Load(SceneLoader.Scene.VictoryScene);
            }
        }
    }
}