using UnityEngine;
using System.Collections;

public class MudGenerator : MonoBehaviour
{

    #region Fields

    [SerializeField]
    private GameObject mudPrefap;
    [SerializeField]
    private int numbers = 3;
    [SerializeField]
    private float radius = 100;
    [SerializeField]
    private int maxWith = 50;
    [SerializeField]
    private int minWith = 30;
    [SerializeField]
    private int maxHeight = 30;
    [SerializeField]
    private int minHeight = 10; 
    #endregion

    #region Initialization

    void Start()
    {
        Generate(); 
    }

    #endregion

    #region Private Members

    private void Generate()
    {
        for (int i = 0; i < numbers; i++)
        {
            GameObject mud = Instantiate(mudPrefap);
            mud.transform.localPosition = new Vector3(Random.Range(-radius, radius / 2f), 0.55f, Random.Range(-radius, radius / 2f));
            mud.transform.localScale = new Vector3(Random.Range(minWith, maxWith), 0.01f, Random.Range(minHeight, maxHeight));
            mud.transform.localRotation = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
            mud.transform.parent = transform;
        }
    }
    #endregion
}
