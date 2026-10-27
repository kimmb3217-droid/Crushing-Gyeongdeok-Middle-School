using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Self-contained first prototype. Attach to an empty GameObject in a new 3D scene.
/// Creates a stylized school-like building from destructible blocks at runtime.
/// </summary>
public class PrototypeBootstrap : MonoBehaviour
{
    private readonly List<DestructibleBlock> blocks = new List<DestructibleBlock>();
    private Camera gameCamera;
    private float zoom = 0f;
    private int shotsFired;
    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;

    private void Start()
    {
        BuildScene();
    }

    private void Update()
    {
        if (gameCamera == null) return;

        zoom -= Input.mouseScrollDelta.y * 2.0f;
        zoom = Mathf.Clamp(zoom, -2f, 12f);
        gameCamera.transform.position += gameCamera.transform.forward * (Input.mouseScrollDelta.y * 2.0f);
        gameCamera.transform.position = new Vector3(
            gameCamera.transform.position.x,
            Mathf.Max(3f, gameCamera.transform.position.y),
            gameCamera.transform.position.z);

        if (Input.GetMouseButtonDown(0))
            FireAtCursor();

        if (Input.GetKeyDown(KeyCode.R))
            BuildScene();
    }

    private void BuildScene()
    {
        // Remove objects created by the previous run/reset.
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        blocks.Clear();
        shotsFired = 0;

        GameObject oldCamera = GameObject.Find("Prototype Camera");
        if (oldCamera != null) Destroy(oldCamera);
        GameObject oldLight = GameObject.Find("Prototype Sun");
        if (oldLight != null) Destroy(oldLight);
        GameObject oldGround = GameObject.Find("Prototype Ground");
        if (oldGround != null) Destroy(oldGround);

        var cameraObject = new GameObject("Prototype Camera");
        gameCamera = cameraObject.AddComponent<Camera>();
        gameCamera.tag = "MainCamera";
        gameCamera.fieldOfView = 55f;
        cameraObject.transform.position = new Vector3(0f, 8f, -23f);
        cameraObject.transform.LookAt(new Vector3(0f, 4f, 0f));

        var lightObject = new GameObject("Prototype Sun");
        var sun = lightObject.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.25f;
        lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Prototype Ground";
        ground.transform.position = new Vector3(0f, -0.2f, 0f);
        ground.transform.localScale = new Vector3(6f, 1f, 5f);
        ground.GetComponent<Renderer>().material.color = new Color(0.27f, 0.42f, 0.28f);

        CreateSchoolFacade();
        CreateLabel();
    }

    private void CreateSchoolFacade()
    {
        // A stylized three-storey school block. Each facade section is independently destructible.
        Color brick = new Color(0.62f, 0.32f, 0.24f);
        Color trim = new Color(0.86f, 0.83f, 0.72f);
        Color glass = new Color(0.22f, 0.55f, 0.72f);
        Color roof = new Color(0.22f, 0.25f, 0.29f);

        MakeBlock("Main Building", new Vector3(0f, 4.0f, 0.8f), new Vector3(14f, 7.5f, 4f), brick, 7);
        MakeBlock("Roof", new Vector3(0f, 7.9f, 0.8f), new Vector3(14.5f, 0.35f, 4.4f), roof, 3);

        for (int floor = 0; floor < 3; floor++)
        {
            float y = 2.2f + floor * 2.15f;
            MakeBlock("Floor Trim", new Vector3(0f, y - 0.65f, -1.25f), new Vector3(14.1f, 0.16f, 0.15f), trim, 2);

            for (int col = 0; col < 7; col++)
            {
                float x = -5.7f + col * 1.9f;
                MakeBlock("Window", new Vector3(x, y, -1.28f), new Vector3(1.05f, 0.95f, 0.12f), glass, 1);
            }
        }

        // Entrance and sign.
        MakeBlock("Entrance Canopy", new Vector3(0f, 1.9f, -2.1f), new Vector3(3.8f, 0.3f, 1.1f), trim, 2);
        MakeBlock("Entrance Door", new Vector3(0f, 0.9f, -1.32f), new Vector3(1.6f, 1.8f, 0.18f), new Color(0.25f, 0.34f, 0.38f), 2);
        MakeBlock("Left Wing", new Vector3(-7.35f, 3.1f, 0.8f), new Vector3(0.5f, 5.8f, 4f), trim, 4);
        MakeBlock("Right Wing", new Vector3(7.35f, 3.1f, 0.8f), new Vector3(0.5f, 5.8f, 4f), trim, 4);
    }

    private void MakeBlock(string blockName, Vector3 position, Vector3 scale, Color color, int hitPoints)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = blockName;
        block.transform.SetParent(transform, true);
        block.transform.position = position;
        block.transform.localScale = scale;
        block.GetComponent<Renderer>().material.color = color;

        DestructibleBlock destructible = block.AddComponent<DestructibleBlock>();
        destructible.SetHealth(hitPoints);
        blocks.Add(destructible);
    }

    private void FireAtCursor()
    {
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            DestructibleBlock target = hit.collider.GetComponent<DestructibleBlock>();
            if (target != null)
            {
                shotsFired++;
                target.TakeDamage(1, hit.point, ray.direction);
            }
        }
    }

    private void CreateLabel()
    {
        // UI is drawn by OnGUI, so no Canvas setup is required for the first prototype.
    }

    private void OnGUI()
    {
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                normal = { textColor = Color.white }
            };
        }

        int remaining = 0;
        foreach (var block in blocks)
            if (block != null && !block.IsDestroyed) remaining++;

        GUI.Box(new Rect(16, 16, 330, 118), GUIContent.none);
        GUI.Label(new Rect(30, 22, 300, 34), "안동 경덕중학교 뿌시기", titleStyle);
        GUI.Label(new Rect(30, 62, 300, 24), "남은 블록: " + remaining + " / " + blocks.Count, bodyStyle);
        GUI.Label(new Rect(30, 86, 300, 24), "공격 횟수: " + shotsFired, bodyStyle);
        GUI.Label(new Rect(30, 110, 480, 24), "마우스 클릭: 공격  |  휠: 줌  |  R: 재생성", bodyStyle);
        GUI.Label(new Rect(Screen.width - 190, 18, 180, 24), "PROTOTYPE 0.1", bodyStyle);
    }
}

/// <summary>
/// Basic health component for a single building block.
/// When health reaches zero, the block becomes a falling physics object.
/// </summary>
public class DestructibleBlock : MonoBehaviour
{
    private int health = 3;
    public bool IsDestroyed { get; private set; }

    public void SetHealth(int value)
    {
        health = Mathf.Max(1, value);
    }

    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 shotDirection)
    {
        if (IsDestroyed) return;
        health -= amount;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.Lerp(Color.red, renderer.material.color, Mathf.Clamp01(health / 3f));

        if (health <= 0)
        {
            IsDestroyed = true;
            Rigidbody body = gameObject.AddComponent<Rigidbody>();
            body.mass = Mathf.Max(0.5f, transform.localScale.x * transform.localScale.y * transform.localScale.z * 0.08f);
            body.AddExplosionForce(240f, hitPoint - shotDirection * 1.5f, 5f, 0.8f, ForceMode.Impulse);
            body.AddForce(shotDirection * 90f, ForceMode.Impulse);
            Destroy(gameObject, 12f);
        }
        else
        {
            transform.localScale *= 0.985f;
        }
    }
}
