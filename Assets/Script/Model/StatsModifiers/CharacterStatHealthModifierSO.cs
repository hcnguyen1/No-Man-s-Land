using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        Health health = character.GetComponent<Health>();
        if(health != null)
        {
            // Only heal if not at max health
            if (health.GetCurrentHealth() < health.GetMaxHealth())
            {
                health.AddHealth((int) val);
            }
        }
    }
}
