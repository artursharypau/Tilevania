using UnityEngine;

namespace Common
{
    public static class LayerMaskProvider
    {
        public static LayerMask Player = LayerMask.GetMask("Player");
        public static LayerMask DeadPlayer = LayerMask.GetMask("DeadPlayer");
        public static LayerMask Bullet = LayerMask.GetMask("Bullet");
        public static LayerMask Climbing = LayerMask.GetMask("Climbing");
        public static LayerMask Hazards = LayerMask.GetMask("Hazards");
        public static LayerMask Enemy = LayerMask.GetMask("Enemy");
        public static LayerMask Ground = LayerMask.GetMask("Ground", "Bouncing");

        public static bool Contains(int objectLayer, params LayerMask[] masks)
        {
            int combinedMask = 0;

            for (int i = 0; i < masks.Length; i++)
            {
                combinedMask |= masks[i].value;
            }

            return (combinedMask & (1 << objectLayer)) != 0;
        }

        public static int MaskToLayer(LayerMask mask)
        {
            int bitmask = mask.value;
            int result = 0;

            while (bitmask > 1)
            {
                bitmask >>= 1;
                ++result;
            }

            return result;
        }
    }
}
