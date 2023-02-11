using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Coroutine? footstepCoroutine = null;
    private CharacterController characterController;

    [SerializeField]
    private Camera activeCamera;

    [SerializeField]
    private WalkSpeed walkSpeed;

    [SerializeField]
    private Vector3 aimDirection = Vector3.forward;

    [SerializeField]
    private float aimTransitionSpeed = 10.0f;

    [SerializeField]
    private Transform weaponPivot;

    [SerializeField]
    private Transform weaponOrigin;

    [SerializeField]
    private Transform weaponBulletSpawner;

    [SerializeField]
    private Transform weaponScopeTransform;

    [SerializeField]
    private Transform weaponRelaxedTransform;

    [SerializeField]
    private AudioSource shotAudioSource;

    [SerializeField]
    private AudioSource footStepAudioSource;

    [SerializeField]
    private GameObject bulletDecalPrefab;

    [SerializeField]
    private Vector3 lastDirectionToAim = Vector3.forward;

    [SerializeField]
    private LayerMask aimLayerMask;

    private bool useLastZoneSetDirections = false;

    private ZoneSet? lastZoneSet;
    private ZoneSet? activeZoneSet;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        var moveInputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (useLastZoneSetDirections && moveInputDirection.sqrMagnitude < float.Epsilon)
        {
            useLastZoneSetDirections = false;
        }

        var (forward, right) = useLastZoneSetDirections
            ? (lastZoneSet?.forward ?? Vector3.forward, lastZoneSet?.right ?? Vector3.right)
            : (activeZoneSet?.forward ?? Vector3.forward, activeZoneSet?.right ?? Vector3.right);

        var finalMoveDirection = forward * moveInputDirection.y + right * moveInputDirection.x;

        characterController.Move(finalMoveDirection * walkSpeed.forwards * walkSpeed.multiplier * Time.deltaTime);

        Vector3? targetWeaponPivotDirection = null;

        if (Input.GetButton("Fire2"))
        {
            var ray = activeCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 100.0f, aimLayerMask.value))
            {
                var pointingDirection = (hit.point - transform.position).normalized;

                aimDirection = Vector3.ProjectOnPlane(pointingDirection, Vector3.up);

                targetWeaponPivotDirection = hit.point - weaponPivot.transform.position;
            }
        }
        else
        {
            aimDirection = lastDirectionToAim;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            shotAudioSource.time = .1f;
            shotAudioSource.volume = UnityEngine.Random.Range(.6f, .7f);
            shotAudioSource.pitch = UnityEngine.Random.Range(.9f, 1.3f);
            shotAudioSource.Play();

            var bulletRay = new Ray(weaponBulletSpawner.position, weaponBulletSpawner.forward);

            if (Physics.Raycast(bulletRay, out var hit))
            {
                var bulletDecal = Instantiate(bulletDecalPrefab);

                bulletDecal.transform.position = hit.point;
                bulletDecal.transform.rotation = Quaternion.LookRotation(hit.normal);

                hit.transform.gameObject.BroadcastMessage("TakeDamage", 1f, SendMessageOptions.DontRequireReceiver);
            }
        }

        var plainLook = new Vector3(aimDirection.x, 0, aimDirection.z);
        var targetRotation = Quaternion.LookRotation(plainLook);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aimTransitionSpeed * Time.deltaTime);

        var targetWeaponPivotRotation = targetWeaponPivotDirection is not null
            ? Quaternion.LookRotation(targetWeaponPivotDirection.Value)
            : transform.rotation;
        weaponPivot.transform.rotation = Quaternion.Slerp(weaponPivot.transform.rotation, targetWeaponPivotRotation, aimTransitionSpeed * Time.deltaTime);

        var targetWeaponOriginTransform = Input.GetButton("Fire2") ? weaponScopeTransform : weaponRelaxedTransform;
        weaponOrigin.position = Vector3.Lerp(weaponOrigin.position, targetWeaponOriginTransform.position, aimTransitionSpeed * Time.deltaTime);
        weaponOrigin.rotation = Quaternion.Slerp(weaponOrigin.rotation, targetWeaponOriginTransform.rotation, aimTransitionSpeed * Time.deltaTime);

        lastDirectionToAim = Input.GetButton("Horizontal") || Input.GetButton("Vertical")
            ? finalMoveDirection
            : aimDirection;

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            if (footstepCoroutine is null)
            {
                footStepAudioSource.Play();

                footstepCoroutine = StartCoroutine(ChangeFootstepPitch());
            }
        }
        else
        {
            if (footstepCoroutine is not null)
            {
                footStepAudioSource.Stop();
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
        }
    }

    private IEnumerator ChangeFootstepPitch()
    {
        yield return new WaitForSeconds(footStepAudioSource.clip.length);

        footStepAudioSource.pitch = UnityEngine.Random.Range(.8f, 1.2f);

        StartCoroutine(ChangeFootstepPitch());
    }

    public void SwitchZoneSet(ZoneSet zoneSet)
    {
        activeCamera = zoneSet.GetCamera();
        lastZoneSet = activeZoneSet;
        activeZoneSet = zoneSet;
        useLastZoneSetDirections = true;
    }
}

[Serializable]
public class WalkSpeed
{
    public float multiplier;
    public float forwards;
    public float backwards;
    public float sideways;
}
