using Mirror;
using UnityEngine;

public enum FactionType { faction_1, faction_2 }

public class RtsEntity : NetworkBehaviour
{
    public string entityName;
    public int maxHealth;
    [SyncVar(hook = "healthChange")]
    public int health;
    private void healthChange(int oldValue, int newValue)
    {
        health = newValue;
    }
    public FactionType faction;
    public bool isSelectable = true;

    public Renderer[] renderers;    

    void Start()
    {
        faction = hasAuthority ? FactionType.faction_1 : FactionType.faction_2;
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
