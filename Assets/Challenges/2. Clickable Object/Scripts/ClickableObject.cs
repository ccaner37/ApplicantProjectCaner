using System;
using UnityEngine;

namespace Challenges._2._Clickable_Object.Scripts
{
    public class InvalidInteractionMethodException : Exception
    {
        private const string MessageWithMethodArgument =
            "Attempted to register to an invalid method of clickable interaction. The ClickableObject '{0}' does not allow interaction of type {1}";
        public InvalidInteractionMethodException(string gameObjectName, ClickableObject.InteractionMethod interactionMethod) : base(string.Format(MessageWithMethodArgument,gameObjectName,interactionMethod))
        {
        }
    }
    [RequireComponent(typeof(Collider))]
    public class ClickableObject : MonoBehaviour, IClickableObject
    {
      
        // Do not remove the provided 3 options, you can add more if you like
        [Flags]
        public enum InteractionMethod
        {
            Tap=2,
            DoubleTap=4,
            TapAndHold=8,
        }
        
        
        /// <summary>
        /// Dont edit
        /// </summary>
        [SerializeField]
        private InteractionMethod allowedInteractionMethods;


        private OnClickableClicked ClickInteraction;


        /// <summary>
        /// Checks if the given interaction method is valid for this clickable object.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public bool IsInteractionMethodValid(InteractionMethod method)
        {
            return allowedInteractionMethods.HasFlag(method);
        }


        /// <summary>
        /// Updates the interaction method of the clickable object. Can contain more than one value due to bitflags
        /// </summary>
        public void SetInteractionMethod(InteractionMethod method)
        {
            allowedInteractionMethods = method;
        }
        
        
        /// <summary>
        /// Will invoke the given callback when the clickable object is interacted with alongside the method of interaction
        /// </summary>
        /// <param name="callback">Function to invoke</param>
        public void RegisterToClickable(OnClickableClicked callback)
        {
            ClickInteraction += callback;
        }


        /// <summary>
        /// Will unregister a previously provided callback
        /// </summary>
        /// <param name="callback">Function previously given</param>
        public void UnregisterFromClickable(OnClickableClicked callback)
        {
            ClickInteraction -= callback;
        }

        /// <summary>
        /// Will invoke the given callback when the clickable object is tapped. 
        /// </summary>
        /// <param name="onTapCallback"></param>
        /// <exception cref="InvalidInteractionMethodException">If tapping is not allowed for this clickable</exception>
        public void RegisterToClickableTap(OnClickableClickedUnspecified onTapCallback) 
        {
            if (!allowedInteractionMethods.HasFlag(InteractionMethod.Tap))
                throw new InvalidInteractionMethodException(transform.name, InteractionMethod.Tap);

            ClickInteraction(this, InteractionMethod.Tap);
        }
        
        /// <summary>
        /// Will invoke the given callback when the clickable object is tapped. 
        /// </summary>
        /// <param name="onTapCallback"></param>
        /// <exception cref="InvalidInteractionMethodException">If double tapping is not allowed for this clickable</exception>
        public void RegisterToClickableDoubleTap(OnClickableClickedUnspecified onTapCallback) 
        {
            if (!allowedInteractionMethods.HasFlag(InteractionMethod.DoubleTap))
                throw new InvalidInteractionMethodException(transform.name, InteractionMethod.DoubleTap);

            ClickInteraction(this, InteractionMethod.DoubleTap);
        }

        public void RegisterToClickableTapAndHold(OnClickableClickedUnspecified onTapCallback)
        {
            if (!allowedInteractionMethods.HasFlag(InteractionMethod.TapAndHold))
            {
                Debug.LogWarning(new InvalidInteractionMethodException(transform.name, InteractionMethod.TapAndHold).Message);
                return;
            }

            // I don't know why but throwing the exception with TapAndHold is crashing Unity.

            //throw new InvalidInteractionMethodException(transform.name, InteractionMethod.TapAndHold);

            ClickInteraction(this, InteractionMethod.TapAndHold);
        }
    }
}
