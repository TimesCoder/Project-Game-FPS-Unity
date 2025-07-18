//SlapChickenGames
//2021
//Modified ThirdPersonCharacter.cs from Unity standard assets to allow for fullbody control 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scgFullBodyController
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class ThirdPersonCharacter : MonoBehaviour
    {
        //IMPORTANT, this script needs to be on the root transform

        [SerializeField] float m_JumpPower = 12f;
        [Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
        [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField] float m_MoveSpeedMultiplier = 1f;
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;
        public float sensitivity;
        Rigidbody m_Rigidbody;
        Animator m_Animator;
        [HideInInspector] public bool m_IsGrounded;
        float m_OrigGroundCheckDistance;
        const float k_Half = 0.5f;
        float m_TurnAmount;
        float m_ForwardAmount;
        Vector3 m_GroundNormal;
        float m_CapsuleHeight;
        Vector3 m_CapsuleCenter;
        CapsuleCollider m_Capsule;
        [HideInInspector] public bool m_Crouching;
        [HideInInspector] public bool m_Sliding;
        public float jumpDamping;
        float maxCamOriginal;
        float minCamOriginal;
        public CameraController camController;
        bool toggle;

        void Start()
        {
            maxCamOriginal = camController.maxPitch;
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }


        public void Move(Vector3 move, bool crouch, bool jump, bool slide, bool vaulting)
        {
            if (ResumeMenuController.isGamePaused) return;
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus(vaulting);
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);

            if (!vaulting)
                m_ForwardAmount = move.z;
            else
                m_ForwardAmount = 0;


            ScaleCapsuleForCrouching(crouch);
            ScaleCapsuleForSliding(slide);
            PreventStandingInLowHeadroom();
        }
        public void updateLate(Vector3 move, bool crouch, bool prone, bool vaulting, bool forwards, bool backwards, bool strafe, float horizontal, float vertical)
        {
            if (ResumeMenuController.isGamePaused) return;
            m_TurnAmount = camController.relativeYaw;
            transform.eulerAngles = new Vector3(0, camController.transform.eulerAngles.y, 0);
            UpdateAnimator(move, crouch, prone, vaulting, forwards, backwards, strafe, horizontal, vertical);
        }

        public void HandleGroundMovement(bool crouch, bool jump, bool slide)
        {
            if (ResumeMenuController.isGamePaused) return;
            if (m_IsGrounded)
            {
                HandleGroundedMovement(crouch, jump, slide);
            }
            else
            {
                HandleAirborneMovement();
            }
        }


        void ScaleCapsuleForCrouching(bool crouch)
        {
            if (m_IsGrounded && crouch)
            {
                if (m_Crouching) return;
                m_Capsule.height = m_Capsule.height / 2f;
                m_Capsule.center = m_Capsule.center / 2f;
                m_Crouching = true;
            }
            else
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                    return;
                }
                m_Capsule.height = m_CapsuleHeight;
                m_Capsule.center = m_CapsuleCenter;
                m_Crouching = false;
            }
        }

        void ScaleCapsuleForSliding(bool slide)
        {
            if (m_IsGrounded && slide)
            {
                if (m_Sliding) return;
                m_Capsule.height = m_Capsule.height / 2f;
                m_Capsule.center = m_Capsule.center / 2f;
                m_Sliding = true;
            }
            else
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Sliding = true;
                    return;
                }
                m_Capsule.height = m_CapsuleHeight;
                m_Capsule.center = m_CapsuleCenter;
                m_Sliding = false;
            }
        }

        void PreventStandingInLowHeadroom()
        {
            // prevent standing up in crouch-only zones
            if (!m_Crouching)
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                }
            }
        }

        public void kick()
        {
            m_Animator.SetTrigger("kick");
        }

        public void UpdateAnimator(Vector3 move, bool crouch, bool prone, bool vaulting, bool forwards, bool backwards, bool strafe, float horizontal, float vertical)
        {
            // update the animator parameters

            if (backwards)
            {
                m_Animator.SetFloat("Forward", m_ForwardAmount * -1, 0.1f, Time.deltaTime);
                m_Animator.SetFloat("animSpeed", -1);
            }
            else if (forwards)
            {
                m_Animator.SetFloat("animSpeed", 1);
                m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            }
            else
            {
                m_Animator.SetFloat("animSpeed", 1);
                m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            }

            if (strafe)
            {
                m_Animator.SetBool("strafe", true);
            }
            else
            {
                m_Animator.SetBool("strafe", false);
            }

            m_Animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);

            if (prone)
            {
                m_Animator.SetBool("prone", true);
                camController.maxPitch = 45;
            }
            else
            {
                m_Animator.SetBool("prone", false);
                camController.maxPitch = maxCamOriginal;
            }

            if (vaulting)
            {
                m_Animator.SetBool("vaulting", true);
            }
            else
            {
                m_Animator.SetBool("vaulting", false);
            }

            m_Animator.SetFloat("Turn", m_TurnAmount * 0.3f, 0.1f, Time.deltaTime);
            m_Animator.SetBool("Crouch", m_Crouching);
            m_Animator.SetBool("slide", m_Sliding);
            m_Animator.SetBool("OnGround", m_IsGrounded);

            if (!m_IsGrounded)
            {
                m_Animator.SetFloat("Jump", m_Rigidbody.linearVelocity.y);
            }

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle =
                Mathf.Repeat(
                    m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
            if (m_IsGrounded)
            {
                m_Animator.SetFloat("JumpLeg", jumpLeg);
            }

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (m_IsGrounded && move.magnitude > 0)
            {
                m_Animator.speed = m_AnimSpeedMultiplier;
            }
            else
            {
                // don't use that while airborne
                m_Animator.speed = 1;
            }
        }


        void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);

            m_GroundCheckDistance = m_Rigidbody.linearVelocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }


        void HandleGroundedMovement(bool crouch, bool jump, bool slide)
        {
            // check whether conditions are right to allow a jump:
            if (jump && !slide && !crouch)
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
                {
                    initiateJump();
                }
                else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("StrafeStanding"))
                {
                    initiateJump();
                }
            }
        }

        void initiateJump()
        {
            // Jump
            m_Rigidbody.linearVelocity = new Vector3(m_Rigidbody.linearVelocity.x * jumpDamping, m_JumpPower, m_Rigidbody.linearVelocity.z * jumpDamping);
            m_IsGrounded = false;
            m_Animator.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }

        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (m_IsGrounded && Time.deltaTime > 0)
            {
                Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = m_Rigidbody.linearVelocity.y;
                m_Rigidbody.linearVelocity = v;
            }
        }


        void CheckGroundStatus(bool vaulting)
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {

                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
                m_Animator.applyRootMotion = true;
            }
            else if (!vaulting)
            {
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
                m_Animator.applyRootMotion = false;
            }
        }
    }
}

