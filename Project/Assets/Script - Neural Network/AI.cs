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

    private float[] input = new float[5];
    public NeuralNetwork network;

    public float fitness;
    public bool collided;

    private List<Collider> colliders = new List<Collider>();

    public bool training;
    // PRELOADED MODEL WHEN NO TRAINING //
    public string neuralModelPath = "Assets/NeuralModels/Pre-trained.txt";

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
        for (int i = 0; i < 5; i++)
        {
            RaycastHit hit;
            Vector3 newVector = Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 1, 0)) * transform.right;
            Ray Ray = new Ray(transform.position, newVector);

            if (Physics.Raycast(Ray, out hit, 10, raycastMask)){ input[i] = (10 - hit.distance) / 10; }
            else{ input[i] = 0; }
        }
    }

    // UPDATE NETWORK //
    private void UpdateNetwork()
    {
        if (!training) { network = new NeuralNetwork(new int[3] { 5, 3, 2 }); network.Load(neuralModelPath); }
        float[] output = network.FeedForward(input);
        kart.velocity = new Vector2(output[0] * rotation, output[1] * speed);
    }

    // CHANGE FITNESS //
    public void OnTriggerEnter(Collider other){ 
        if (other.CompareTag("CheckPoint")) {
            Vector3 velocity = transform.rotation * kart.Rigidbody.velocity;
            if (!colliders.Contains(other)) { fitness += 100; colliders.Add(other); }
            else { fitness++; if (velocity.z > 0) { fitness++; } fitness += kart.Rigidbody.velocity.sqrMagnitude; }
        }
        if (other.CompareTag("Track")) { fitness -= 10;  if (training) { collided = true; } } 
    }

    // UPDATE FITNESS //
    public void UpdateFitness(){ network.fitness = fitness; }
}
