using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public int generation;
    public float timeframe;
    public int populationSize;
    public GameObject prefab;
    public Activation activation;

    // INITIALIZE LAYERS: INPUT, HIDDEN & OUTPUT
    public int[] layers = new int[3] { 5, 3, 2 };

    [Range(0f, 1f)] public float mutationChance = 0.01f;

    [Range(0f, 1f)] public float mutationStrength = 0.5f;

    [Range(0.1f, 10f)] public float timeMultiplier = 1f;

    public List<NeuralNetwork> networks = new List<NeuralNetwork>();
    private List<AI> cars;

    public bool LoadBest = false;


    // INITIALIZE //
    public void Start()
    {
        foreach(NeuralNetwork network in networks) { network.activation = activation; }

        if (populationSize % 2 != 0) { populationSize = 50; }

        if (LoadBest) { InvokeRepeating("InitializeNetworks", 0.1f, timeframe);}
        else { InitializeNetworks(); InvokeRepeating("InstantiateAI", 0.1f, timeframe); }
    }

    // INITIALIZE NETWORKS //
    public void InitializeNetworks()
    {
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            if (LoadBest) { net.Load("Assets/NeuralModels/Pre-trained.txt"); }
            networks.Add(net);
        }

        if (LoadBest) { InstantiateAI(); }
    }

    public void InstantiateAI()
    {
        generation++;
        //SET GAME SPEED //
        Time.timeScale = timeMultiplier;
        if (cars != null)
        {
            for (int i = 0; i < cars.Count; i++){ Destroy(cars[i].gameObject); }
                            
            SortNetworks(); //SORT AND MUTATE NETWORKS //
        }

        cars = new List<AI>();
        for (int i = 0; i < populationSize; i++)
        {
            AI car = (Instantiate(prefab, transform.position, Quaternion.identity)).GetComponent<AI>();
            car.network = networks[i];
            cars.Add(car);
        }
        
    }

    // SORT & MUTATE NETWORKS //
    public void SortNetworks()
    {
        for (int i = 0; i < populationSize; i++){ cars[i].UpdateFitness(); }

        networks.Sort();
        networks[populationSize - 1].Save("Assets/NeuralModels/Save.txt");
        for (int i = 0; i < populationSize / 2; i++)
        {
            networks[i] = networks[i + populationSize / 2].copy(new NeuralNetwork(layers));
            networks[i].Mutate((int)(1/ mutationChance), mutationStrength);
        }
    }
}