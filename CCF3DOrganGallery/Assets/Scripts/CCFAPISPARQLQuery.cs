using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System.Text;
using System.Linq;
using static Mapping;
using Unity.VisualScripting;
using static CCFAPISPARQLQuery;

public class CCFAPISPARQLQuery : MonoBehaviour
{
    public static CCFAPISPARQLQuery Instance;
    public int ExpectedCellTypes
    {
        get { return _queryResult.triples.Count; }
    }


    public string CellsInSelected
    {
        get
        {
            StringBuilder sb = new StringBuilder();

            foreach (var cell in _queryResult.triples)
            {
                sb.Append(cell.cell_label + "\n");
            }

            sb.Replace("\"\"", ",");
            sb.Replace("\"", "");

            return sb.ToString();
        }
        private set { }
    }
    
    [Header("Data")]
    [SerializeField] private QueryResponse _queryResult = new QueryResponse();

    [Header("Request")]
    [SerializeField] private string _url = "http://grlc.io/api-git/hubmapconsortium/ccf-grlc/subdir/ccf//cells_located_in_as?endpoint=https%3A%2F%2Fccf-api.hubmapconsortium.org%2Fv1%2Fsparql?format=application/json";
    [SerializeField] private SPARQLAPIResponse _apiResponse = new SPARQLAPIResponse();

    [Header("Scene")]
    [SerializeField] private XRRayInteractor _interactor;
    private UnityAction<SelectEnterEventArgs> _selectAction;

    //documentation for SPARQL query through grlc: http://grlc.io/api/hubmapconsortium/ccf-grlc/ccf/#/default/get_cell_by_location
    //url with iri descending colon as query string: http://grlc.io/api-git/hubmapconsortium/ccf-grlc/subdir/ccf//cell_by_location?location=http://purl.obolibrary.org/obo/UBERON_0001158&endpoint=https%3A%2F%2Fccf-api.hubmapconsortium.org%2Fv1%2Fsparql?format=application%2Fjson

    private void OnEnable()
    {
        _selectAction += CheckForCellTypes;

        _interactor.selectEntered.AddListener(
            _selectAction
            );
    }

    public void CheckForCellTypes(SelectEnterEventArgs args)
    {
        GameObject interactable = args.interactableObject.transform.gameObject;

        if (interactable.GetComponent<TissueBlockData>() == null) return;
        string[] iris = interactable.GetComponent<TissueBlockData>().CcfAnnotations;
        _queryResult.triples.Clear();
        for (int i = 0; i < iris.Length; i++)
        {
            List<Cell> result = _apiResponse.triples.Where(n => n.as_iri == iris[i]).ToList();
            _queryResult.triples.AddRange(result);
        }
    }


    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _interactor = GetComponent<XRRayInteractor>();
        await GetAllCellTypes();
    }

    public async Task GetAllCellTypes()
    {
        _apiResponse = await Get(_url);
    }

    public async Task<SPARQLAPIResponse> Get(string url)
    {
        try
        {
            using var www = UnityWebRequest.Get(url);
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);
                await Task.Yield();
            }


            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Failed: {www.error} for {www.url}");

            var text = www.downloadHandler.text;

            _apiResponse = JsonUtility.FromJson<SPARQLAPIResponse>(
                "{ \"triples\":" +
                text
                + "}"
                );

            return _apiResponse;
        }
        catch (Exception ex)
        {
            Debug.LogError($"{nameof(Get)} failed: {ex.Message}");
            return default;
        }
    }

    [Serializable]
    public class QueryResponse
    {
        [SerializeField] public List<Cell> triples = new List<Cell>();
    }

    [Serializable]
    public class SPARQLAPIResponse
    {
        [SerializeField] public Cell[] triples = new Cell[0];
    }

    [Serializable]
    public class Cell
    {
        public string as_iri;
        public string cell_iri;
        public string cell_label;
    }

}
