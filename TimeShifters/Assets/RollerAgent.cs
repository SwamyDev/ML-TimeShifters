using UnityEngine;

public class RollerAgent : Agent {
	public Transform Target;
	public float Speed = 10;
	
	private Rigidbody body;
	private float previousDistnace = float.MaxValue;
	
	void Start () {
		body = GetComponent<Rigidbody>();
	}

	public override void AgentReset() {
		if (transform.position.y < -1.0f) {
			transform.position = Vector3.zero;
			body.angularVelocity = Vector3.zero;
			body.velocity = Vector3.zero;
		}
		else {
			Target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
		}
	}

	public override void CollectObservations() {
		var relativePosition = Target.position - transform.position;
		AddVectorObs(relativePosition.x / 5);
		AddVectorObs(relativePosition.x / 5);
		
		AddVectorObs((transform.position.x + 5)/5);
		AddVectorObs((transform.position.x - 5)/5);
		AddVectorObs((transform.position.z + 5)/5);
		AddVectorObs((transform.position.z - 5)/5);
		
		AddVectorObs(body.velocity.x/5);
		AddVectorObs(body.velocity.y/5);
	}

	public override void AgentAction(float[] vectorAction, string textAction) {
		var distance = Vector3.Distance(transform.position, Target.position);
		if (distance < 1.42f) {
			Done();
			AddReward(1.0f);
		}

		if (distance < previousDistnace) {
			AddReward(0.1f);
		}
		
		AddReward(-0.05f);

		if (transform.position.y < -1.0f) {
			Done();
			AddReward(-1.0f);
		}

		previousDistnace = distance;
		
		var signal = Vector3.zero;
		signal.x = Mathf.Clamp(vectorAction[0], -1, 1);
		signal.z = Mathf.Clamp(vectorAction[1], -1, 1);
		body.AddForce(signal * Speed);
	}
}
