using UnityEngine;

public class CombatScript : MonoBehaviour
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
                target = cs;
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
        if (target == null)
            return;
        if (Vector3.Distance(transform.position, target.transform.position) > range)
        {
            movileEntity.target = target.transform.position;
            return;
        }
        Attack();
    }

    protected virtual void Attack() { }
}
