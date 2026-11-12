using UnityEngine;

public class LowerBodyAnimator : MonoBehaviour
{
    private enum MoveState { Idle, Forward, Backward, Crouch, Jump }

    [Header("참조")]
    [SerializeField] private PlayerController2D controller;
    [SerializeField] private PlayerAimController aim;
    [SerializeField] private SpriteRenderer lowerBodyRenderer;

    [Header("기본 프레임")]
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite[] forwardFrames;   // 바라보는 방향으로 전진
    [SerializeField] private Sprite[] backwardFrames;  // 반대 방향으로 후진

    [Header("앉기 / 점프 프레임")]
    [SerializeField] private Sprite[] crouchFrames;
    [Tooltip("[0] 상승 포즈, [1] 하강 포즈. 한 장만 넣으면 그 프레임만 사용")]
    [SerializeField] private Sprite[] jumpFrames;

    [Header("재생 설정")]
    [SerializeField] private float fps = 10f;
    [SerializeField] private float moveThreshold = 0.1f;

    private MoveState currentState = MoveState.Idle;
    private int frameIndex;
    private float timer;

    private void Update()
    {
        MoveState state = DetermineState();

        if (state != currentState)
        {
            currentState = state;
            frameIndex = 0;
            timer = 0f;
        }

        // 점프는 상승/하강 포즈로 고정 표시
        if (state == MoveState.Jump)
        {
            Sprite[] jf = jumpFrames;
            if (jf == null || jf.Length == 0) return;

            bool rising = controller.VerticalVelocity > 0f;
            Sprite s = (jf.Length == 1) ? jf[0] : (rising ? jf[0] : jf[jf.Length - 1]);
            lowerBodyRenderer.sprite = s;
            return;
        }

        Sprite[] frames = GetFrames(state);
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        float frameTime = 1f / fps;
        if (timer >= frameTime)
        {
            timer -= frameTime;
            frameIndex = (frameIndex + 1) % frames.Length;
        }

        lowerBodyRenderer.sprite = frames[frameIndex];
    }

    private MoveState DetermineState()
    {
        // 1순위: 공중 (점프/낙하)
        if (!controller.IsGrounded)
            return MoveState.Jump;

        // 2순위: 앉기
        if (controller.IsCrouching)
            return MoveState.Crouch;

        // 3순위: 이동 / 정지
        float vx = controller.GetComponent<Rigidbody2D>().linearVelocity.x;
        if (Mathf.Abs(vx) < moveThreshold)
            return MoveState.Idle;

        bool movingLeft = vx < 0f;
        bool facingLeft = aim.IsFacingLeft;
        return (movingLeft == facingLeft) ? MoveState.Forward : MoveState.Backward;
    }

    private Sprite[] GetFrames(MoveState state)
    {
        switch (state)
        {
            case MoveState.Forward: return forwardFrames;
            case MoveState.Backward: return backwardFrames;
            case MoveState.Crouch: return crouchFrames;
            default: return idleFrames;
        }
    }
}
