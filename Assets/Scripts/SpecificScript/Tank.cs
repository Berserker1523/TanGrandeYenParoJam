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
        CmdInstantiateCannonball();
    }

    [Command]
    protected void CmdInstantiateCannonball() 
    {
        var cb = Instantiate(canonBallPrefab, canon.transform.position, canon.transform.rotation);
        NetworkServer.Spawn(cb.gameObject);
        cb.faction = GetComponent<RtsEntity>().faction;
        RpcLookAtEnemy(cb.gameObject);
    }

    [ClientRpc]
    private void RpcLookAtEnemy(GameObject cbGO)
    {
        Proyectile cb = cbGO.GetComponent<Proyectile>();
        cb.proyectileRigidbody.velocity = cb.transform.forward * 10;
        transform.LookAt(target.transform.position);
    }
    

}
