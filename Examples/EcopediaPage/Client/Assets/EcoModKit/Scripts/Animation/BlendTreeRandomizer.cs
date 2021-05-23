using UnityEngine;

public class BlendTreeRandomizer : StateMachineBehaviour
{
    [Tooltip("Randomizes parameterName to the default blend tree thresholds for clipCount")]
    public int clipCount;
    [Tooltip("Leave empty for equal probability for each clip. List thresholds in ascending order (eg 0.1, 0.9, 1.0)")]
    public float[] probabilityThresholds;
    public string parameterName = "RandFloat";
    private int parameterID = -1;

    private void Awake()
    {
        parameterID = Animator.StringToHash(parameterName);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (probabilityThresholds.Length == clipCount)
        {
            var value = Random.Range(0f, probabilityThresholds[probabilityThresholds.Length - 1]);
            for (int i = 0; i < probabilityThresholds.Length; i++)
                if (value <= probabilityThresholds[i])
                {
                    animator.SetFloat(parameterID, i / (float)(clipCount - 1));
                    return;
                }
        }
        else
            animator.SetFloat(parameterID, Random.Range(0, clipCount) / (float)(clipCount - 1));
    }
}
