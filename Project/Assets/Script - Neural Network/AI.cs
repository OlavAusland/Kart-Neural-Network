using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KartGame.KartSystems;

public class AI : MonoBehaviour
{
    public ArcadeKart kart;
    public float speed;
    public float rotation;
    public LayerMask raycastMask;

    private float[] input = new float[8];
    public NeuralNetwork network;

    public float fitness;
    public bool collided;

    private List<Collider> colliders = new List<Collider>();

    public bool training;
    // PRELOADED MODEL WHEN NO TRAINING //
    public string neuralModelPath = "Assets/NeuralModels/General.txt";

    private float[] visualizeRays = new float[8];

    // UPDATE EVERY FRAME //
    public void Update()
    {
        if (collided) { kart.velocity = Vector2.zero; return; }

        CreateRays();
        UpdateNetwork();
    }

    // RAYS FOR INPUT //
    private void CreateRays()
    {
        for (int i = 0; i < 8; i++)
        {
            RaycastHit hit;
            Vector3 newVector = Quaternion.AngleAxis(i * 45 - 90, transform.up) * transform.forward;
            Ray Ray = new Ray(transform.position, newVector);

            if (Physics.Raycast(Ray, out hit, 10, raycastMask)){ input[i] = (10 - hit.distance) / 10; visualizeRays[i] = hit.distance; }
            else{ input[i] = 0; }
        }
    }

    // UPDATE NETWORK //
    private void UpdateNetwork()
    {
        if (!training) { network = new NeuralNetwork(new int[3] { 8, 3, 3 }); network.Load(neuralModelPath); }
        float[] output = network.FeedForward(input);
        kart.velocity = new Vector2(output[0] * rotation, output[1] * speed * (output[2] * -1));
    }

    // CHANGE FITNESS //
    public void OnTriggerEnter(Collider other){ 
        if (other.CompareTag("CheckPoint")) {
            Vector3 velocity = transform.rotation * kart.Rigidbody.velocity;
            if (!colliders.Contains(other)) { fitness += 100; colliders.Add(other); }
            else { if (velocity.z > 0) { fitness += 10; } fitness += kart.Rigidbody.velocity.sqrMagnitude; }
        }
        if (other.CompareTag("Track")) { fitness -= 10;  if (training) { collided = true; } } 
    }

    // UPDATE FITNESS //
    public void UpdateFitness(){ network.fitness = fitness; }

    public void OnDrawGizmosSelected()
    {
        if (collided) { return; }
        Gizmos.color = Color.green;
        for (int i = 0; i < 8; i++)
        {
            Vector3 newVector = Quaternion.AngleAxis(i * 45 - 90, transform.up) * transform.forward;
            Ray visualizeRay = new Ray(transform.position, newVector);
            Gizmos.DrawRay(visualizeRay.origin, visualizeRay.direction * visualizeRays[i]);
        }
    }
}
