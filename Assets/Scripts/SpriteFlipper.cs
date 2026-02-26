using UnityEngine;

public class SpriteFlipper
{
    private bool _isFacingRight;

    public SpriteFlipper(float initialScaleX)
    {
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
            float targetScaleX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector2(shouldFaceRight ? targetScaleX : -targetScaleX, transform.localScale.y);

            _isFacingRight = shouldFaceRight;
        }
    }
}
