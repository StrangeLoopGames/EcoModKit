using UnityEngine;

public class StateMachineRandomRange : StateMachineBehaviour
{
    public float min;
    public float max;
    public string parameterName = "RandFloat";
    private int parameterID;

    private void Awake()
    {
        parameterID = Animator.StringToHash(parameterName);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat(parameterID, Random.Range(min, max));
    }
}