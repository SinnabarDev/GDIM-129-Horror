using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Patrol 2D",
        description: "Moves a 2D side-scroller enemy between waypoint objects. No NavMesh. No 3D rotation.",
        category: "Action/Movement",
        story: "[Agent] patrols between [Waypoints]",
        id: "2d_patrol_custom_node_001")]
    internal partial class Patrol2DAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;

        [SerializeReference] public BlackboardVariable<float> Speed = new(2f);
        [SerializeReference] public BlackboardVariable<float> ArriveDistance = new(0.1f);
        [SerializeReference] public BlackboardVariable<float> WaitTime = new(1f);

        [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new("Speed");

        private Animator m_Animator;

        [CreateProperty] private int m_CurrentIndex = 0;
        [CreateProperty] private float m_WaitTimer = 0f;
        [CreateProperty] private bool m_Waiting = false;

        protected override Status OnStart()
        {
            if (Agent.Value == null)
            {
                Debug.LogError("Patrol2D: No Agent assigned.");
                return Status.Failure;
            }

            if (Waypoints.Value == null || Waypoints.Value.Count == 0)
            {
                Debug.LogError("Patrol2D: No waypoints assigned.");
                return Status.Failure;
            }

            m_Animator = Agent.Value.GetComponentInChildren<Animator>();
            m_Waiting = false;
            m_WaitTimer = 0f;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Waypoints.Value == null || Waypoints.Value.Count == 0)
                return Status.Failure;

            if (m_Waiting)
            {
                m_WaitTimer -= Time.deltaTime;

                UpdateAnimator(0f);

                if (m_WaitTimer <= 0f)
                {
                    m_Waiting = false;
                    m_CurrentIndex = (m_CurrentIndex + 1) % Waypoints.Value.Count;
                }

                return Status.Running;
            }

            Transform agent = Agent.Value.transform;
            Transform target = Waypoints.Value[m_CurrentIndex].transform;

            Vector3 pos = agent.position;
            Vector3 targetPos = target.position;

            // SIDE SCROLLER = MOVE ON X ONLY
            float newX = Mathf.MoveTowards(
                pos.x,
                targetPos.x,
                Speed.Value * Time.deltaTime
            );

            agent.position = new Vector3(newX, pos.y, pos.z);

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

            UpdateAnimator(Speed.Value);

            if (distance <= ArriveDistance.Value)
            {
                m_Waiting = true;
                m_WaitTimer = WaitTime.Value;
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