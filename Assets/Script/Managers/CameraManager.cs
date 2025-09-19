using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Enter the Gungeon Style")]
    public float baseFollowDistance = 3f;
    public float maxFollowDistance = 6f;
    public float followSpeed = 8f;
    public bool snapToTarget = false;

    [Header("Mouse/Look Ahead")]
    public bool followMouse = true;
    public float mouseInfluence = 0.4f;
    public float maxMouseDistance = 8f;
    public LayerMask uiLayer = -1; // Pour ignorer l'UI

    [Header("Room Transitions")]
    public bool useRoomBounds = false;
    public Vector2 currentRoomMin = new Vector2(-10, -10);
    public Vector2 currentRoomMax = new Vector2(10, 10);
    public float roomTransitionSpeed = 12f;

    [Header("Effects")]
    public float shakeIntensity = 0f;
    public float shakeDuration = 0f;

    private Player player;
    private Vector3 targetPosition;
    private Vector3 mouseWorldPos;
    private Camera cam;
    private bool isTransitioning = false;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (target != null)
        {
            player = target.GetComponent<Player>();

            // Position initiale
            if (snapToTarget)
            {
                transform.position = target.position + offset;
                targetPosition = transform.position;
            }
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        CalculateTargetPosition();
        MoveCamera();
        HandleShake();
    } 

    void CalculateTargetPosition()
    {
        Vector3 basePosition = target.position;

        // Calcul de la position de base avec le mouvement du joueur
        Vector2 inputDirection = Vector2.zero;
        if (player != null)
        {
            inputDirection = player.GetVelocity().normalized;
        }

        // Distance basée sur l'input (style Enter the Gungeon)
        float currentDistance = Mathf.Lerp(baseFollowDistance, maxFollowDistance,
                                          inputDirection.magnitude);
        Vector3 inputOffset = (Vector3)inputDirection * currentDistance;

        // Position de la souris dans le monde
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y,
                                                           cam.nearClipPlane));
        mouseWorldPos.z = 0;

        // Calcul de l'influence de la souris
        Vector3 mouseOffset = Vector3.zero;
        if (followMouse)
        {
            Vector3 mouseDirection = (mouseWorldPos - target.position).normalized;
            float mouseDistance = Vector3.Distance(mouseWorldPos, target.position);
            mouseDistance = Mathf.Clamp(mouseDistance, 0, maxMouseDistance);

            mouseOffset = mouseDirection * mouseDistance * mouseInfluence;
        }

        // Position cible finale
        targetPosition = basePosition /*+ inputOffset + mouseOffset*/ + offset;

        // Application des limites de room si activées
        if (useRoomBounds)
        {
            // Calcul des limites de caméra basées sur la taille de l'écran
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            float minX = currentRoomMin.x + camWidth;
            float maxX = currentRoomMax.x - camWidth;
            float minY = currentRoomMin.y + camHeight;
            float maxY = currentRoomMax.y - camHeight;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }
    }

    void MoveCamera()
    {
        if (isTransitioning)
        {
            // Mouvement rapide lors des transitions de room
            transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                                                   roomTransitionSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isTransitioning = false;
            }
        }
        else
        {
            // Mouvement normal - direct mais légèrement lissé
            transform.position = Vector3.Lerp(transform.position, targetPosition,
                                            followSpeed * Time.deltaTime);
        }
    }

    void HandleShake()
    {
        if (shakeDuration > 0f)
        {
            Vector3 shakeOffset = Random.insideUnitCircle * shakeIntensity;
            shakeOffset.z = 0;

            transform.position += shakeOffset;

            shakeDuration -= Time.deltaTime;
            shakeIntensity = Mathf.Lerp(shakeIntensity, 0f, Time.deltaTime * 5f);

            if (shakeDuration <= 0f)
            {
                shakeIntensity = 0f;
            }
        }
    }

    // Méthodes publiques pour le gameplay

    public void StartShake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
    }

    public void SnapToTarget()
    {
        if (target != null)
        {
            CalculateTargetPosition();
            transform.position = targetPosition;
        }
    }

    public void SetRoomBounds(Vector2 min, Vector2 max, bool transition = true)
    {
        currentRoomMin = min;
        currentRoomMax = max;
        useRoomBounds = true;

        if (transition)
        {
            isTransitioning = true;
        }
    }

    public void DisableRoomBounds()
    {
        useRoomBounds = false;
    }

    // Transition instantanée vers une nouvelle room
    public void TransitionToRoom(Vector2 roomMin, Vector2 roomMax, bool instant = false)
    {
        SetRoomBounds(roomMin, roomMax, !instant);

        if (instant)
        {
            CalculateTargetPosition();
            transform.position = targetPosition;
        }
    }

    // Désactive temporairement le suivi de la souris (utile pour les cutscenes)
    public void SetMouseFollow(bool enabled)
    {
        followMouse = enabled;
    }

    // Force la caméra à regarder un point spécifique (pour les cutscenes)
    public void FocusOnPoint(Vector3 point, float duration = 1f)
    {
        StartCoroutine(FocusCoroutine(point, duration));
    }

    private System.Collections.IEnumerator FocusCoroutine(Vector3 point, float duration)
    {
        bool originalMouseFollow = followMouse;
        followMouse = false;

        Vector3 startPos = transform.position;
        Vector3 focusPos = point + offset;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, focusPos,
                                            Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        followMouse = originalMouseFollow;
    }

    // Utilitaires
    public Vector3 GetMouseWorldPosition() => mouseWorldPos;
    public bool IsInTransition() => isTransitioning;

    // Debug
    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // Position du joueur
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position, 0.5f);

        // Position cible de la caméra
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(targetPosition, 0.3f);

        // Ligne vers la position cible
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, targetPosition);

        // Zone de suivi de la souris
        if (followMouse)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, maxMouseDistance);
        }

        // Limites de room
        if (useRoomBounds)
        {
            Gizmos.color = Color.magenta;
            Vector3 center = new Vector3((currentRoomMin.x + currentRoomMax.x) / 2f,
                                       (currentRoomMin.y + currentRoomMax.y) / 2f, 0f);
            Vector3 size = new Vector3(currentRoomMax.x - currentRoomMin.x,
                                     currentRoomMax.y - currentRoomMin.y, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}