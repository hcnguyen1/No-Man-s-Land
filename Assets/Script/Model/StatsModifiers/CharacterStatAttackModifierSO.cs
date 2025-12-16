using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The class inherits from CharacterStatModifierSO and allows the player to have their ATTACK changed based on whatever items they consume or equip. 
[CreateAssetMenu]
public class CharacterStatAttackModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        Entity entity = character.GetComponent<Entity>();
        if (entity != null)
        {
            // Add or remove attack power bonus
            entity.ModifyAttackPower((int)val);
        }
    }
}
