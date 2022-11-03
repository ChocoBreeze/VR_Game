using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using TMPro;

public class LongSword : MonoBehaviour
{

    private Interactable interactable;

    private void OnHandHoverBegin(Hand hand)
    {

    }

    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags &
        (~Hand.AttachmentFlags.SnapOnAttach) &
        (~Hand.AttachmentFlags.DetachOthers);

    private void OnHandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(gameObject);

        if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            hand.HoverLock(interactable);
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
        } 
        else if (isGrabEnding)
        {
            hand.DetachObject(gameObject);
            hand.HoverUnlock(interactable);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
