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

#region OnMouseUp

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
            RtsEntity targetEntity = rh.collider.GetComponent<RtsEntity>();
            CmdSetTarget(targetEntity);
        }
        else
        {
            target = null;
        }
    }

    [Command]
    void CmdSetTarget(RtsEntity targetEntity)
    {
        if (targetEntity.faction != entity.faction || targetCanBeAlly){
            RpcSetTarget(targetEntity);
        }
        else
            target = null;
    }

    [ClientRpc]
    void RpcSetTarget(RtsEntity cs) 
    {
        target = cs;
    }

#endregion

#region PrepareAttack

    void PrepareToAttack()
    {
        if(!hasAuthority) 
            return;
        if (target == null)
            return;
        CmdAttack();
    }


    [Command]
    protected virtual void CmdAttack() 
    {
        if (Vector3.Distance(transform.position, target.transform.position) > range)
        {
            RpcMove();
            return;
        }
        RpcAttack();
    }
    
    [ClientRpc]
    protected virtual void RpcMove() { 
        movileEntity.target = target.transform.position;
    }

    [ClientRpc]
    protected virtual void RpcAttack() { }

#endregion
}
