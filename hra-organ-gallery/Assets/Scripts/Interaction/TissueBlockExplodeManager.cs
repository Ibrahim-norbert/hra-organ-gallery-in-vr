using HRAOrganGallery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TissueBlockExplodeManager : MonoBehaviour
{
    public Vector3 DefaultPosition { get; set; }
    public float ExplodeValue { get; set; }
    private LineRenderer _renderer;
    [SerializeField] private Material lineMaterial;


    private void Awake()
    {
        SetUpLines();

        //subscribe to event when organ is picked and placed
        //OrganCaller.OrganPicked += ActivateLines;
    }

    private void OnDestroy()
    {
        //OrganCaller.OrganPicked -= ActivateLines;
    }

    private void Update()
    {
        _renderer.SetPositions(new Vector3[] { transform.position, DefaultPosition });
    }

    private void ActivateLines()
    {
        _renderer.enabled = true;
        DefaultPosition = transform.position;
    }

    private void SetUpLines()
    {
        _renderer = gameObject.AddComponent<LineRenderer>();
        _renderer.startColor = Color.white;
        _renderer.endColor = Color.white;
        _renderer.startWidth = .001f;
        _renderer.endWidth = .001f;
        _renderer.material = lineMaterial;
        _renderer.enabled = false;
    }

}
