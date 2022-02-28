using UnityEngine;

public class Schoonspringer : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] Rigidbody2D rigidBodyPlayer = null;
    [SerializeField] float AngleOffset = -90.0f;
    [SerializeField] Animator animator;
    [SerializeField] float slerpFactorRotate = 3.0f;
    [SerializeField] float slerpFactorMove = 1.0f;

    public bool Jumped = false;

    private float previousFlow;
#pragma warning restore 649

    public void RotateTowardsPointSlerp(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion newRotation = Quaternion.AngleAxis(angle + AngleOffset, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * slerpFactorRotate);
    }

    public void WalkStartAnim()
    {
        if (!animator.GetBool("isWalking"))
        {
            animator.SetBool("isWalking", true);
        }
    }

    public void WalkStopAnim()
    {
        if (animator.GetBool("isWalking"))
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void Jump()
    {
        if (!animator.GetBool("isMakingJump"))
        {
            animator.SetBool("isMakingJump", true);
        }
    }

    public void SetJumpBool()
    {
        Jumped = true;
    }

    public void DoTricks()
    {
        //if (animator.GetBool("isMakingJump") && !animator.GetBool("isDiving"))
        //{
        animator.SetBool("isDiving", true);
        //}
    }

    public void StopMoving()
    {
        rigidBodyPlayer.velocity = Vector2.zero;
        rigidBodyPlayer.angularVelocity = 0.0f;

        if (!animator.GetBool("playerStop"))
        {
            animator.SetBool("playerStop", true);
        }
    }

    public void SlerpToPoint(Transform point)
    {
        transform.position = Vector3.Slerp(transform.position, point.position, Time.deltaTime * slerpFactorMove);
        transform.rotation = Quaternion.Slerp(transform.rotation, point.rotation, Time.deltaTime * slerpFactorRotate);
    }

    public void SetDynamic()
    {
        rigidBodyPlayer.bodyType = RigidbodyType2D.Dynamic;
    }

    public void SetKinematic()
    {
        rigidBodyPlayer.bodyType = RigidbodyType2D.Kinematic;
    }
}
