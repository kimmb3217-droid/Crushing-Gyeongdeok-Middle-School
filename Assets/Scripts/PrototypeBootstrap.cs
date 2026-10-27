using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mobile-first, self-contained Unity prototype. Attach to an empty GameObject.
/// Builds a fictional school-like structure and offers simple arcade weapons.
/// </summary>
public class PrototypeBootstrap : MonoBehaviour
{
    private enum Weapon { Laser, Missile, Shockwave }
    private readonly List<DestructibleBlock> blocks = new List<DestructibleBlock>();
    private Camera gameCamera;
    private Vector2 lastPointerPosition;
    private bool rotating;
    private int shotsFired;
    private Weapon selectedWeapon = Weapon.Laser;
    private GUIStyle titleStyle, bodyStyle, buttonStyle;
    private Vector3 orbitTarget = new Vector3(0f, 4f, 0f);
    private float orbitYaw, orbitPitch = 10f, cameraDistance = 25f;
    private const float DragThreshold = 12f;

    private void Start() { BuildScene(); }

    private void Update()
    {
        if (gameCamera == null) return;
        HandleMouseInput();
        HandleTouchInput();
        if (Input.GetKeyDown(KeyCode.R)) BuildScene();
        UpdateCameraPose();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1)) { lastPointerPosition = Input.mousePosition; rotating = true; }
        if (Input.GetMouseButton(1) && rotating)
        {
            Vector2 current = Input.mousePosition;
            RotateCamera(current - lastPointerPosition);
            lastPointerPosition = current;
        }
        if (Input.GetMouseButtonUp(1)) rotating = false;

        if (Input.GetMouseButtonDown(0) && !IsOverWeaponButtons(Input.mousePosition))
            FireAtScreenPosition(Input.mousePosition);

        cameraDistance = Mathf.Clamp(cameraDistance - Input.mouseScrollDelta.y * 1.5f, 12f, 42f);
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 2)
        {
            Touch a = Input.GetTouch(0), b = Input.GetTouch(1);
            float previous = Vector2.Distance(a.position - a.deltaPosition, b.position - b.deltaPosition);
            float current = Vector2.Distance(a.position, b.position);
            if (previous > 0.01f)
                cameraDistance = Mathf.Clamp(cameraDistance + (previous - current) * 0.035f, 12f, 42f);
            rotating = false;
            return;
        }
        if (Input.touchCount != 1) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            lastPointerPosition = touch.position;
            rotating = false;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            if (!rotating && touch.deltaPosition.magnitude > DragThreshold) rotating = true;
            if (rotating) RotateCamera(touch.deltaPosition);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            if (!rotating && !IsOverWeaponButtons(touch.position))
                FireAtScreenPosition(touch.position);
            rotating = false;
        }
        else if (touch.phase == TouchPhase.Canceled) rotating = false;
    }

    private bool IsOverWeaponButtons(Vector2 screenPosition)
    {
        // GUI coordinates start at top-left. Weapon controls occupy the right edge.
        float guiY = Screen.height - screenPosition.y;
        return screenPosition.x > Screen.width - 190f && guiY > 100f && guiY < Screen.height - 85f;
    }

    private void RotateCamera(Vector2 delta)
    {
        orbitYaw += delta.x * 0.22f;
        orbitPitch = Mathf.Clamp(orbitPitch - delta.y * 0.16f, -5f, 55f);
    }

    private void UpdateCameraPose()
    {
        if (gameCamera == null) return;
        Quaternion rotation = Quaternion.Euler(orbitPitch, orbitYaw, 0f);
        gameCamera.transform.position = orbitTarget + rotation * new Vector3(0f, 0f, -cameraDistance);
        gameCamera.transform.LookAt(orbitTarget);
    }

    private void BuildScene()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
        blocks.Clear();
        shotsFired = 0;
        selectedWeapon = Weapon.Laser;

        GameObject oldCamera = GameObject.Find("Prototype Camera");
        if (oldCamera != null) Destroy(oldCamera);
        GameObject oldLight = GameObject.Find("Prototype Sun");
        if (oldLight != null) Destroy(oldLight);
        GameObject oldGround = GameObject.Find("Prototype Ground");
        if (oldGround != null) Destroy(oldGround);

        orbitYaw = 0f; orbitPitch = 10f; cameraDistance = 25f;
        orbitTarget = new Vector3(0f, 4f, 0f);

        GameObject cameraObject = new GameObject("Prototype Camera");
        gameCamera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        gameCamera.fieldOfView = 55f;
        UpdateCameraPose();

        GameObject lightObject = new GameObject("Prototype Sun");
        Light sun = lightObject.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.25f;
        lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Prototype Ground";
        ground.transform.position = new Vector3(0f, -0.2f, 0f);
        ground.transform.localScale = new Vector3(6f, 1f, 5f);
        ground.GetComponent<Renderer>().material.color = new Color(0.27f, 0.42f, 0.28f);

        CreateSchoolFacade();
    }

    private void CreateSchoolFacade()
    {
        Color brick = new Color(0.62f, 0.32f, 0.24f);
        Color trim = new Color(0.86f, 0.83f, 0.72f);
        Color glass = new Color(0.22f, 0.55f, 0.72f);
        Color roof = new Color(0.22f, 0.25f, 0.29f);
        MakeBlock("Main Building", new Vector3(0f, 4f, 0.8f), new Vector3(14f, 7.5f, 4f), brick, 7);
        MakeBlock("Roof", new Vector3(0f, 7.9f, 0.8f), new Vector3(14.5f, 0.35f, 4.4f), roof, 3);

        for (int floor = 0; floor < 3; floor++)
        {
            float y = 2.2f + floor * 2.15f;
            MakeBlock("Floor Trim", new Vector3(0f, y - 0.65f, -1.35f), new Vector3(14.1f, 0.16f, 0.15f), trim, 2);
            for (int col = 0; col < 7; col++)
            {
                float x = -5.7f + col * 1.9f;
                MakeBlock("Window", new Vector3(x, y, -1.42f), new Vector3(1.05f, 0.95f, 0.12f), glass, 1);
            }
        }
        MakeBlock("Entrance Canopy", new Vector3(0f, 1.9f, -2.1f), new Vector3(3.8f, 0.3f, 1.1f), trim, 2);
        MakeBlock("Entrance Door", new Vector3(0f, 0.9f, -1.42f), new Vector3(1.6f, 1.8f, 0.18f), new Color(0.25f, 0.34f, 0.38f), 2);
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

    private void FireAtScreenPosition(Vector2 screenPosition)
    {
        if (gameCamera == null) return;
        Ray ray = gameCamera.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f)) return;

        shotsFired++;
        switch (selectedWeapon)
        {
            case Weapon.Laser:
                CreateBeam(ray.origin, hit.point, Color.cyan);
                DamageBlock(hit.collider.GetComponent<DestructibleBlock>(), 2, hit.point, ray.direction);
                break;
            case Weapon.Missile:
                CreateBeam(ray.origin, hit.point, new Color(1f, 0.45f, 0.1f));
                DamageNearby(hit.point, 4.5f, 3, ray.direction);
                CreateBurst(hit.point, new Color(1f, 0.35f, 0.05f));
                break;
            case Weapon.Shockwave:
                DamageNearby(hit.point, 7f, 2, ray.direction);
                CreateBurst(hit.point, new Color(0.55f, 0.3f, 1f));
                break;
        }
    }

    private void DamageBlock(DestructibleBlock block, int damage, Vector3 point, Vector3 direction)
    {
        if (block != null) block.TakeDamage(damage, point, direction);
    }

    private void DamageNearby(Vector3 point, float radius, int damage, Vector3 direction)
    {
        Collider[] hits = Physics.OverlapSphere(point, radius);
        foreach (Collider hit in hits)
        {
            DestructibleBlock block = hit.GetComponent<DestructibleBlock>();
            if (block != null) block.TakeDamage(damage, point, direction);
        }
    }

    private void CreateBeam(Vector3 start, Vector3 end, Color color)
    {
        GameObject beam = new GameObject("Weapon Beam");
        LineRenderer line = beam.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.startWidth = 0.08f;
        line.endWidth = 0.02f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = color;
        line.endColor = color;
        Destroy(beam, 0.12f);
    }

    private void CreateBurst(Vector3 position, Color color)
    {
        GameObject burst = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        burst.name = "Impact Effect";
        burst.transform.position = position;
        burst.transform.localScale = Vector3.one * 0.7f;
        Collider collider = burst.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        Renderer renderer = burst.GetComponent<Renderer>();
        renderer.material.color = color;
        Destroy(burst, 0.18f);
    }

    private void OnGUI()
    {
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Clamp(Screen.width / 34, 18, 28), fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Clamp(Screen.width / 70, 12, 18), normal = { textColor = Color.white } };
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = Mathf.Clamp(Screen.width / 60, 14, 20), fontStyle = FontStyle.Bold };
        }

        int remaining = 0;
        foreach (DestructibleBlock block in blocks)
            if (block != null && !block.IsDestroyed) remaining++;

        float scale = Mathf.Clamp(Screen.width / 1000f, 0.7f, 1.25f);
        GUI.Box(new Rect(10, 10, 300 * scale, 100 * scale), GUIContent.none);
        GUI.Label(new Rect(20, 14, 290 * scale, 30 * scale), "경덕중학교 뿌시기", titleStyle);
        GUI.Label(new Rect(20, 48 * scale, 280 * scale, 22 * scale), "남은 블록: " + remaining + " / " + blocks.Count, bodyStyle);
        GUI.Label(new Rect(20, 72 * scale, 280 * scale, 22 * scale), "공격 횟수: " + shotsFired, bodyStyle);

        float buttonWidth = 150 * scale, buttonHeight = 48 * scale, gap = 8 * scale;
        float x = Screen.width - buttonWidth - 12 * scale;
        float y = 125 * scale;
        DrawWeaponButton(new Rect(x, y, buttonWidth, buttonHeight), "레이저", Weapon.Laser);
        DrawWeaponButton(new Rect(x, y + buttonHeight + gap, buttonWidth, buttonHeight), "미사일", Weapon.Missile);
        DrawWeaponButton(new Rect(x, y + (buttonHeight + gap) * 2, buttonWidth, buttonHeight), "충격파", Weapon.Shockwave);

        if (GUI.Button(new Rect(Screen.width - buttonWidth - 12 * scale, Screen.height - 68 * scale, buttonWidth, 52 * scale), "건물 재생성", buttonStyle))
            BuildScene();

        GUI.Label(new Rect(12, Screen.height - 28, Screen.width - 24, 24),
            Application.isMobilePlatform ? "탭: 공격  |  드래그: 회전  |  두 손가락: 줌" : "좌클릭: 공격  |  우클릭 드래그: 회전  |  휠: 줌  |  R: 재생성",
            bodyStyle);
    }

    private void DrawWeaponButton(Rect rect, string label, Weapon weapon)
    {
        bool selected = selectedWeapon == weapon;
        Color previous = GUI.backgroundColor;
        GUI.backgroundColor = selected ? new Color(0.25f, 0.75f, 1f) : new Color(0.25f, 0.28f, 0.34f);
        if (GUI.Button(rect, label, buttonStyle)) selectedWeapon = weapon;
        GUI.backgroundColor = previous;
    }
}

public class DestructibleBlock : MonoBehaviour
{
    private int health = 3;
    public bool IsDestroyed { get; private set; }

    public void SetHealth(int value) { health = Mathf.Max(1, value); }

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
        else transform.localScale *= 0.985f;
    }
}
