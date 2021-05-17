using Mirror;
using UnityEngine;

public class Tank : CombatScript
{
    [SerializeField]
    private Proyectile canonBallPrefab;
    [SerializeField]
    private Transform canon;

    [Command]
    protected override void CmdAttack()
    {
        Attack();
        base.CmdAttack();
    }

    [ClientRpc]
    protected override void RpcAttack()
    {
        Attack();
    }

    private void Attack()
    {
        Debug.Log(target);
        transform.LookAt(target.transform.position);
        var cb = Instantiate(canonBallPrefab, canon.transform.position, canon.transform.rotation);
        cb.faction = hasAuthority? FactionType.faction_1 : FactionType.faction_2;
        cb.proyectileRigidbody.velocity = cb.transform.forward * 10;
    }

}
