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
    private Rigidbody rigidbody;

    public override void Initialize()
    {
        rigidbody=GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-8.2f,8.2f), 0.5f, Random.Range(-7.2f,7.2f));
        target.localPosition=new Vector3(Random.Range(-8.2f,8.2f), 0.5f, Random.Range(-7.2f,7.2f));

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        // sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveFoward = actions.ContinuousActions[1];
        float moveSpeed = 3f;
        
        rigidbody.MovePosition(transform.position + transform.forward *moveFoward * moveSpeed *Time.deltaTime);
        transform.Rotate(0f,moveRotate* moveSpeed,0f,Space.Self);
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
            AddReward(4f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.gameObject.tag == "Wall")
        {
            AddReward(-1f);
            floorMeshRenderer.material = loosMaterial;
            EndEpisode();
        }
    }
}
