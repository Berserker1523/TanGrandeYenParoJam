using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum FactionType { faction_1 = 0, faction_2 = 1 }
public class RtsEntity : NetworkBehaviour {
    public string entityName;
    public int maxHealth;

    [SyncVar(hook = "healthChange")]
    public int health;
    private void healthChange(int oldValue, int newValue) {
        health = newValue;
    }

    [SyncVar(hook = "factionChange")]
    public FactionType faction;
    private void factionChange(FactionType oldValue, FactionType newValue) {
        faction = newValue;
        SetColor();
    }

    public bool isSelectable = true;

    public Renderer[] renderers;

    public void Start() {
        SetColor();
    }
    public void SetColor() {
        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].material.color = faction == FactionType.faction_1 ? Color.blue : Color.red;
        }
    }

    void OnTriggerEnter(Collider col) {
        if(!hasAuthority)
            return;
        if (col.GetComponent<Proyectile>() != null) {
            Proyectile proyectile = col.GetComponent<Proyectile>();
            CmdTriggerEnter(col.gameObject);
        }
    }

    [Command]
    public void CmdTriggerEnter(GameObject proyectileGameObject) {
        Proyectile proyectile = proyectileGameObject.GetComponent<Proyectile>();
        if (proyectile.faction != faction) {
            health -= proyectile.damage;
            CheckHealth();
            RpcDestroyObject(proyectile.gameObject);
        }
    }

    public void CheckHealth() {
        Debug.Log("CheckHealth");
        if (health > maxHealth)
            health = maxHealth;
        if (health <= 0) {
            Debug.Log("Destroy TANK!!!");
            RpcDestroyObject(gameObject);
        }
    }

    [ClientRpc]
    protected virtual void RpcDestroyObject(GameObject gameObject) {
        Debug.Log("Destroy GameObject: " + gameObject.name);
        Destroy(gameObject);
    }

}
