using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The class inherits from CharacterStatModifierSO and allows the player to have their HUNGER changed based on whatever items they consume or equip. 
[CreateAssetMenu]
public class CharacterStatHungerModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        Player player = character.GetComponent<Player>();
        if(player != null)
        {
            // Only restore if not at max hunger
            if (player.hunger < player.maxHunger)
            {
                player.hunger += val;
                player.hunger = Mathf.Clamp(player.hunger, 0, player.maxHunger);
            }
        }
    }
}
