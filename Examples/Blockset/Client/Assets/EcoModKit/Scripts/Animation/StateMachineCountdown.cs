using UnityEngine;

public class StateMachineCountdown : StateMachineBehaviour
{
    public float min;
    public float max;
    public string parameterName = "Countdown";
    private int parameterID;
    private float countdown;

    private void Awake()
    {
        parameterID = Animator.StringToHash(parameterName);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        countdown = Random.Range(min, max);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        countdown -= Time.deltaTime;
        animator.SetFloat(parameterID, countdown);
    }
}