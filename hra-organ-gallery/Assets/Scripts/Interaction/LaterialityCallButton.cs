using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace HRAOrganGallery
{
    public class LaterialityCallButton : MonoBehaviour, IKeyboardButton<Laterality, Laterality>
    {
        public static event Action<Laterality> OnClick;
        [field: SerializeField] public BoxCollider Collider { get; set; }
        [field: SerializeField] public Laterality Feature { get; set; }
        [field: SerializeField] public Material PressedMaterial { get; set; }
        [field: SerializeField] public Material ReadyMaterial { get; set; }

        [field: SerializeField] public Renderer Renderer { get; set; }

        [SerializeField] private LaterialityCallButton other;
        [SerializeField] private bool _locked = true;
        [SerializeField] private GameObject uIpanel;
        [SerializeField] private XRSimpleInteractable _interactable;


        private void Awake()
        {
            LaterialityCallButton.OnClick += (lat) => { TurnOff(lat); };
            Collider = GetComponent<BoxCollider>();
            ReadyMaterial = GetComponent<Renderer>().material;
            Renderer = GetComponent<Renderer>();

            //set active color if on by default
            //if (OrganCaller.Instance.RequestedLaterality == Feature) ChangeColor(PressedMaterial);
        }


        private void Start()
        {
            SetUpXRInteraction();
        }

        public void SetUpXRInteraction()
        {
            //subscribe to hover event
            _interactable.hoverEntered.AddListener(
                (HoverEnterEventArgs args) =>
                {
                    if (_locked) return;
                    ChangeColor(PressedMaterial);
                    OnClick?.Invoke(Feature);
                }
                );
        }
        private void OnValidate()
        {
            if (_interactable.colliders.Count == 0) _interactable.colliders.Add(GetComponent<BoxCollider>());
        }

        private void Update()
        {
            //MustLock();
            AutoSwitch();
        }

        public void MustLock()
        {
            _locked = !OrganCaller.Instance.TwoSidedOrgans.Contains(OrganCaller.Instance.RequestedOrgan) | OrganCaller.Instance.RequestedOrgan == "";
            uIpanel.SetActive(_locked);
        }

        public void AutoSwitch()
        {
            if (OrganCaller.Instance.RequestedLaterality == Feature) ChangeColor(PressedMaterial); else { ChangeColor(ReadyMaterial); }
        }

        public void TurnOff(Laterality lat)
        {
            if (lat != Feature) ChangeColor(ReadyMaterial);
        }

        private void ChangeColor(Material mat)
        {
            Renderer.material = mat;
        }
    }
}
