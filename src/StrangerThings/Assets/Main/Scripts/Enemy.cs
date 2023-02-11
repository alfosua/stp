using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField]
    private float health = 10.0f;

    [SerializeField]
    private float pursueRange;

    [SerializeField]
    private float pursueSpeed;

    [SerializeField]
    private Player targetPlayer;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var offset = targetPlayer.transform.position - transform.position;
        var sqrDistanceToPlayer = offset.sqrMagnitude;

        if (sqrDistanceToPlayer < pursueRange * pursueRange)
        {
            var moveDirection = offset.normalized;
            characterController.Move(moveDirection * pursueSpeed * Time.deltaTime);
        }

        if (sqrDistanceToPlayer < 1.5 * 1.5)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void TakeDamage(float damage)
    {
        health = health - damage;

        if (health == 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        var lastColor = Gizmos.color;
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, pursueRange);

        Gizmos.color = lastColor;
    }
}
