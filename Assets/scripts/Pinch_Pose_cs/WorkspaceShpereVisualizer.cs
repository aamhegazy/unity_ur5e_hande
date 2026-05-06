using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class WorkspaceSphereVisualizer : MonoBehaviour
{
    [Header("Workspace")]
    public Transform robotBaseLink;
    public float maxReach = 0.85f;

    [Header("Appearance")]
    public Color sphereColor = new Color(0.2f, 0.7f, 1.0f, 0.15f);

    void Start()
    {
        if (robotBaseLink == null)
        {
            Debug.LogError("[WorkspaceSphere] robotBaseLink not assigned");
            return;
        }

        // Use sphere primitive mesh
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GetComponent<MeshFilter>().sharedMesh = sphere.GetComponent<MeshFilter>().sharedMesh;
        Destroy(sphere);

        // Disable collider so raycasts pass through
        if (TryGetComponent(out Collider col)) Destroy(col);

        // Transparent material
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetFloat("_Surface", 1);   // Transparent
        mat.SetFloat("_Blend", 0);     // Alpha blend
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.renderQueue = 3000;
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.color = sphereColor;
        GetComponent<MeshRenderer>().material = mat;

        transform.position = robotBaseLink.position;
        transform.localScale = Vector3.one * (maxReach * 2f);
    }

    void Update()
    {
        if (robotBaseLink != null)
            transform.position = robotBaseLink.position;
    }
}