using UnityEngine;
using Mirror;

public class CombatScript : NetworkBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float cadence;
    [SerializeField] private bool targetCanBeAlly;

    protected RtsEntity entity;
    protected RtsEntity target;
    private MovileEntity movileEntity;
    
    void Start ()
    {
        movileEntity = GetComponent<MovileEntity>();
        entity = GetComponent<RtsEntity>();
        InvokeRepeating("PrepareToAttack", cadence, cadence);
    }

    void MouseUp(RaycastHit rh)
    {
        if (!entity.isSelectable)
            return;
        if (rh.collider == null)
        {
            target = null;
            return;
        }
        if (rh.collider.GetComponent<RtsEntity>() != null)
        {
            var cs = rh.collider.GetComponent<RtsEntity>();
            if (cs.faction != entity.faction || targetCanBeAlly)
                CmdSetTarget(cs);
            else
                target = null;
        }
        else
        {
            target = null;
        }
    }

    void PrepareToAttack()
    {
        if (!hasAuthority)
            return;

        if (target == null)
            return;
        if (Vector3.Distance(transform.position, target.transform.position) > range)
        {
            movileEntity.target = target.transform.position;
            return;
        }
        CmdAttack();
    }

    [Command]
    void CmdSetTarget(RtsEntity cs)
    {
        target = cs;
        RpcSetTarget(cs);
    }

    [ClientRpc]
    void RpcSetTarget(RtsEntity cs) 
    {
        target = cs;
    }

    [Command]
    protected virtual void CmdAttack() 
    {
        RpcAttack();
    }

    [ClientRpc]
    protected virtual void RpcAttack() { }

}
