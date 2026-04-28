using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PlanTargetSpawner : MonoBehaviour
{
    public GameObject planTargetPrefab;
    public Transform spawnParent; 
    
    private XRRayInteractor rayInteractor; 
    
    void Start()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        if (rayInteractor == null)

        rayInteractor = GetComponentInParent<XRRayInteractor>();    
        }

    // Update is called once per frame
    void Update()
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if(Input.GetMouseButtonDown(0))
            {
                SpawnPlanTarget(hit.point, Quaternion.identity);
            }
        }
    }
    public void SpawnPlanTarget(Vector3 position, Quaternion rotation)
    {
        GameObject instance = Instantiate(planTargetPrefab, position, rotation, spawnParent );
        Debug.Log($"Spawned plan target at {position}");
    }

}
