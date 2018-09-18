﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class turret : MonoBehaviour
{
    public SkinnedMeshRenderer rend;

    public float againstGrav;

    public SteamVR_ActionSet actionSet;

    public SteamVR_Action_Single gripSqueeze;
    public SteamVR_Action_Single pinchSqueeze;


    private Rigidbody turretRigidbody;
    private Interactable interactable;
    
    private void Start()
    {
        turretRigidbody = GetComponent<Rigidbody>();
        interactable = GetComponent<Interactable>();

        interactable.onAttachedToHand += Interactable_onAttachedToHand;
        interactable.onDetachedFromHand += Interactable_onDetachedFromHand;
    }

    private void Interactable_onDetachedFromHand(Hand hand)
    {
        if (hand.otherHand.currentAttachedObject != null && (hand.otherHand.currentAttachedObject.GetComponent<SquishyToy>() != null || hand.otherHand.currentAttachedObject.GetComponent<turret>() != null))
            return;

        actionSet.Deactivate();
    }

    private void Interactable_onAttachedToHand(Hand hand)
    {
        actionSet.ActivatePrimary();
    }

    private void OnDestroy()
    {
        if (interactable == null)
        {
            interactable.onAttachedToHand -= Interactable_onAttachedToHand;
            interactable.onDetachedFromHand -= Interactable_onDetachedFromHand;
        }
    }

    private void Update()
    {
        float grip = 0;
        float pinch = 0;

        if (interactable.attachedToHand)
        {
            grip = gripSqueeze.GetAxis(interactable.attachedToHand.handType);
            pinch = pinchSqueeze.GetAxis(interactable.attachedToHand.handType);
        }

        rend.SetBlendShapeWeight(0, Mathf.Lerp(rend.GetBlendShapeWeight(0), grip * 150, Time.deltaTime * 10));
        rend.SetBlendShapeWeight(1, Mathf.Lerp(rend.GetBlendShapeWeight(1), pinch * 200, Time.deltaTime * 10));
    }

    private void FixedUpdate()
    {
        turretRigidbody.AddForce(-Physics.gravity * againstGrav, ForceMode.Acceleration);
    }
}