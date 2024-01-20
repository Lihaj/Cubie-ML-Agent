using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;

public class AgentMove : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loosMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    private float explorationReward =-0.1f;// Reward for moving towards goal
    private float inactionPenalty=-0.1f; //Penalty fpr staying still

    private Rigidbody rigidbody;

    public override void Initialize()
    {
        rigidbody=GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-8.2f,8.2f), 0.3f, Random.Range(-7.2f,7.2f));
        target.localPosition=new Vector3(Random.Range(-8.2f,8.2f), 0.3f, Random.Range(-7.2f,7.2f));

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
        float moveSpeed = 4f;

        Vector3 moveDelta = new Vector3(moveX, 0.3f, moveY)*Time.deltaTime*moveSpeed;
     

        // // Add Exploration Reward
        // float distanceToGoal=Vector3.Distance(transform.position,target.position);
        // SetReward(distanceToGoal * explorationReward);

        transform.localPosition += moveDelta;

        // // Penalize Inaction
        // if(Mathf.Abs(moveDelta.magnitude)< 0.01f){
        //     SetReward(inactionPenalty);
        // }
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
            SetReward(2f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.gameObject.tag == "Wall")
        {
            SetReward(-1f);
            floorMeshRenderer.material = loosMaterial;
            EndEpisode();
        }
    }
}
