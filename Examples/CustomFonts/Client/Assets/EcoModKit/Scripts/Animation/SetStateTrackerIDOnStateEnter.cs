using UnityEngine;
using UnityEngine.Animations;

public class SetStateTrackerIDOnStateEnter : StateMachineBehaviour
{
    [Tooltip("Sets 'StateTrackerID' parameter to UniqueID when entering the state (or when entering any state within a sub-state machine)")]
    public int ID;

    public static readonly int PropertyID;

    // Various issues prevent making this script foolproof so we will have to use it in multiple places instead
    //  * When used on a sub-state machine OnStateEnter/Exit get called on every transition within, so
    //    if we use OnStateExit to reset the property it gets reset after the first state in the sub-SM.
    //  * OnStateMachineEnter/Exit work for sub-SMs, but only if all transitions go through Entry/Exit
    //    (so transitions out of a sub-SM using Any State won't trigger the property reset)

    static SetStateTrackerIDOnStateEnter()
    {
        PropertyID = Animator.StringToHash("StateTrackerID");
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(PropertyID, ID);
    }
}
