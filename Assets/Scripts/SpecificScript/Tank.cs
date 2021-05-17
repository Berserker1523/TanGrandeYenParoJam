using Mirror;
using UnityEngine;

public class Tank : CombatScript
{
    [SerializeField]
    private Proyectile canonBallPrefab;
    [SerializeField]
    private Transform canon;

    [ClientRpc]
    protected override void RpcAttack()
    {
        Attack();
    }

    private void Attack()
    {
        transform.LookAt(target.transform.position);
        var cb = Instantiate(canonBallPrefab, canon.transform.position, canon.transform.rotation);
        cb.faction = GetComponent<RtsEntity>().faction;
        cb.proyectileRigidbody.velocity = cb.transform.forward * 10;
    }

}
