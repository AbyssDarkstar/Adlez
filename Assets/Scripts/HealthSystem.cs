using System;
using System.Collections.Generic;

public class HealthSystem
{
    public event EventHandler OnDamaged;
    public event EventHandler OnHealed;
    public event EventHandler OnKilled;

    private const int MaxHeartFragments = 4;

    private List<Heart> hearts;

    public HealthSystem(int heartAmount)
    {
        hearts = new List<Heart>();
        for (int i = 0; i < heartAmount; i++)
        {
            var heart = new Heart(4);

            hearts.Add(heart);
        }
    }

    public List<Heart> GetHearts()
    {
        return hearts;
    }

    public void Damage(int damageAmount)
    {
        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            var heart = hearts[i];
            if(damageAmount > heart.GetHeartFragments())
            {
                damageAmount -= heart.GetHeartFragments();
                heart.Damage(heart.GetHeartFragments());
            }
            else
            {
                heart.Damage(damageAmount);
                break;
            }
        }

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if(IsDead())
        {
            OnKilled?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Heal(int healAmount)
    {
        for (int i = 0; i < hearts.Count; i--)
        {
            var heart = hearts[i];
            int missingFragments = MaxHeartFragments - heart.GetHeartFragments();

            if (healAmount > missingFragments)
            {
                healAmount -= missingFragments;
                heart.Heal(missingFragments);
            }
            else
            {
                heart.Heal(healAmount);
                break;
            }
        }

        OnHealed?.Invoke(this, EventArgs.Empty);
    }

    public bool IsDead()
    {
        return hearts[0].GetHeartFragments() == 0;
    }

    public class Heart
    {
        private int fragments;

        public Heart(int heartFragments)
        {
            fragments = heartFragments;
        }

        public int GetHeartFragments()
        {
            return fragments;
        }

        public void SetHeartFragments(int heartFragments)
        {
            fragments = heartFragments;
        }

        public void Damage(int damageAmount)
        {
            if(damageAmount >= fragments)
            {
                fragments = 0;
            }
            else
            {
                fragments -= damageAmount;
            }
        }

        public void Heal(int healAmount)
        {
            if(fragments + healAmount > MaxHeartFragments)
            {
                fragments = MaxHeartFragments;
            }
            else
            {
                fragments += healAmount;
            }
        }
    }
}