using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RobotAttack : CombatScript
{
    [SerializeField]
    public int Damage;
    
    [ClientRpc]
    protected override void RpcAttack()
    {
        CmdDealDamage();
    }

    [Command]
    protected void CmdDealDamage() 
    {
        target.health -= Damage;
        Debug.Log("Enemy health: "+ target.health);
        target.CheckHealth();
        RpcLookAtEnemy();
    }

    [ClientRpc]
    private void RpcLookAtEnemy()
    {
        transform.LookAt(target.transform.position);
    }

}
