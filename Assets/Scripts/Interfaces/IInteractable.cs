namespace Assets.Scripts.Interfaces
{
    public interface IInteractable
    {
        void Interact(PlayerController player);

        bool CanInteract(PlayerController player);
    }
}
