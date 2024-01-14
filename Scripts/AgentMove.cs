using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentMove : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loosMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    private float explorationReward =-0.1f;// Reward for moving towards goal
    private float inactionPenalty=-0.1f; //Penalty fpr staying still

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0f, 0.6f, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];
        float moveSpeed = 2f;

        Vector3 moveDelta = new Vector3(moveX, 0.6f, moveY) * Time.deltaTime * moveSpeed;

        // Add Exploration Reward
        float distanceToGoal=Vector3.Distance(transform.position,target.position);
        SetReward(distanceToGoal * explorationReward);

        transform.localPosition += moveDelta;

        // Penalize Inaction
        if(Mathf.Abs(moveDelta.magnitude)< 0.01f){
            SetReward(inactionPenalty);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goal")
        {
            SetReward(10f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.gameObject.tag == "Wall")
        {
            SetReward(-5f);
            floorMeshRenderer.material = loosMaterial;
            EndEpisode();
        }
    }
}
