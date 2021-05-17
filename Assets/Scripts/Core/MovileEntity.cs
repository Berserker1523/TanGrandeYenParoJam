using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class MovileEntity : NetworkBehaviour
{
    public bool isSelected;
    public GameObject selectionSphere;
    Vector3 Target;
    public Vector3 target
    {
        get { return Target; }
        set {
            Target = value;
            SetPlayerDestination(Target);
        }
    }

    public void SetPlayerDestination(Vector3 argPosition)
    {
        nv.SetDestination(argPosition);
    }

    NavMeshAgent nv;
    public RtsEntity entity;
    RaycastHit rh;

    void Start()
    {
        nv = GetComponent<NavMeshAgent>();
        entity = GetComponent<RtsEntity>();
        target = transform.position;
    }

    void Update()
    {
        if (!hasAuthority) {
            return;
        }
        if (!entity.isSelectable)
            return;
        if (isSelected)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                CmdScrPlayerSetDestination(ray);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y = Screen.height - pos.y;

            isSelected = UnitSelecter.rect.Contains(pos, true);
            selectionSphere.SetActive(isSelected);
        }
    }

    [Command]
    public void CmdScrPlayerSetDestination(Ray ray)
    {
        if (Physics.Raycast(ray, out rh))
        {
            SendMessage("MouseUp", rh, SendMessageOptions.DontRequireReceiver);

            if (rh.collider != null)
            {
                if (rh.collider.CompareTag("Terrain"))
                {
                    target = rh.point;
                    RpcScrPlayerSetDestination(target);
                }
            }                
        }
    }

    [ClientRpc]
    public void RpcScrPlayerSetDestination(Vector3 argPosition)
    {
        nv.SetDestination(argPosition);
    }

}
