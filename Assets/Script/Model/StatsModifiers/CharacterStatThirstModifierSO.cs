using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The class inherits from CharacterStatModifierSO and allows the player to have their THIRST changed based on whatever items they consume or equip. 
[CreateAssetMenu]
public class CharacterStatThirstModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        Player player = character.GetComponent<Player>();
        if(player != null)
        {
            // Only restore if not at max thirst
            if (player.thirst < player.maxThirst)
            {
                player.thirst += val;
                player.thirst = Mathf.Clamp(player.thirst, 0, player.maxThirst);
            }
        }
    }
}
