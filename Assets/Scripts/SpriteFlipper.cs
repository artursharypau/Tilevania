using UnityEngine;

public class SpriteFlipper
{
    private readonly float _initialAbsScaleX;
    private bool _isFacingRight;

    public SpriteFlipper(float initialScaleX)
    {
        _initialAbsScaleX = Mathf.Abs(initialScaleX);
        _isFacingRight = initialScaleX > 0;
    }

    public void CheckFlip(float moveInputX, Transform transform)
    {
        if (Mathf.Abs(moveInputX) < Mathf.Epsilon)
        {
            return;
        }

        bool shouldFaceRight = moveInputX > Mathf.Epsilon;
        if (shouldFaceRight != _isFacingRight)
        {
            Vector3 scale = transform.localScale;
            scale.x = shouldFaceRight ? _initialAbsScaleX : -_initialAbsScaleX;
            transform.localScale = scale;

            _isFacingRight = shouldFaceRight;
        }
    }
}
