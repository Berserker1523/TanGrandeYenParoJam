using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum FactionType { faction_1=0, faction_2=1 }
public class RtsEntity : NetworkBehaviour 
{
    public string entityName;
    public int maxHealth;
    public int health;

    [SyncVar(hook="factionChange")]
    public FactionType faction;
    private void factionChange(FactionType oldValue, FactionType newValue)
    {
        faction = newValue;
        SetColor();
    }

    public bool isSelectable = true;

    public Renderer[] renderers;    

    public void Start()
    {
        SetColor();
    }
    public void SetColor()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = faction == FactionType.faction_1 ? Color.blue : Color.red;
        }
    }

     void OnTriggerEnter(Collider col)
     {
         if (col.GetComponent<Proyectile>() != null)
         {
             var pro = col.GetComponent<Proyectile>();
             if (pro.faction != faction)
             {
                health -= pro.damage;
                CheckHealth();
                Debug.Log("Recibio Bala");
                Destroy(pro.gameObject);
             }
         }
     }

     public void CheckHealth()
     {
         if (health > maxHealth)
             health = maxHealth;
         if (health <= 0)
         {
            Debug.Log("Destruido");
            Destroy(gameObject);
         }
     }

}
