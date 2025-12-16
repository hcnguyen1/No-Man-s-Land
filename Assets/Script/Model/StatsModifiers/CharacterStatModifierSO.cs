using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is the class that the attack, health, hunger, and thirst modifiers inherit from. We just want to affect the character's inate stats
// using their gameobject and their corresponding float value. 
public abstract class CharacterStatModifierSO : ScriptableObject
{
    public abstract void AffectCharacter(GameObject character, float val);
}
