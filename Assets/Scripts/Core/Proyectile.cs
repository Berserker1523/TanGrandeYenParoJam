using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Proyectile : NetworkBehaviour
{
    public int damage = 30;    
    
    [SyncVar(hook = "factionChange")]
    public FactionType faction;
    private void factionChange(FactionType oldValue, FactionType newValue) {
        faction = newValue;
    }
    public Rigidbody proyectileRigidbody;
}
