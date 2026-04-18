using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Navigate To Target 2D",
        description: "Moves a 2D GameObject toward another GameObject. No NavMesh. No 3D rotation.",
        story: "[Agent] navigates to [Target]",
        category: "Action/Movement",
        id: "navigate_to_target_2d_custom_001")]
    internal partial class NavigateToTarget2DAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        [SerializeReference] public BlackboardVariable<float> Speed = new(3f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new(0.2f);
        [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new("Speed");

        private Animator m_Animator;
        private float m_CurrentSpeed;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
                return Status.Failure;

            m_Animator = Agent.Value.GetComponentInChildren<Animator>();
            UpdateAnimator(0f);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Target.Value == null)
                return Status.Failure;

            Transform agent = Agent.Value.transform;
            Transform target = Target.Value.transform;

            Vector3 pos = agent.position;
            Vector3 targetPos = target.position;

            // SIDE SCROLLER = MOVE X ONLY
            float newX = Mathf.MoveTowards(
                pos.x,
                targetPos.x,
                Speed.Value * Time.deltaTime
            );

            agent.position = new Vector3(newX, pos.y, pos.z);

            // Distance check on X only
            float distance = Mathf.Abs(targetPos.x - agent.position.x);

            // FACE TARGET USING SCALE (NO ROTATION)
            Vector3 scale = agent.localScale;

            if (targetPos.x > pos.x)
                scale.x = Mathf.Abs(scale.x);
            else if (targetPos.x < pos.x)
                scale.x = -Mathf.Abs(scale.x);

            agent.localScale = scale;

            // LOCK ROTATION
            agent.rotation = Quaternion.identity;

            // Speed for animator
            m_CurrentSpeed = Speed.Value;
            UpdateAnimator(m_CurrentSpeed);

            if (distance <= DistanceThreshold.Value)
            {
                UpdateAnimator(0f);
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            UpdateAnimator(0f);
        }

        private void UpdateAnimator(float speed)
        {
            if (m_Animator != null && !string.IsNullOrEmpty(AnimatorSpeedParam.Value))
            {
                m_Animator.SetFloat(AnimatorSpeedParam.Value, speed);
            }
        }
    }
}