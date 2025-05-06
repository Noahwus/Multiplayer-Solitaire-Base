using UnityEngine;

//
// If you forget what these look like, you have a cheat sheet in Desmos:
// https://www.desmos.com/calculator/mcoqkg8umh
//
namespace EaseLibrary 
{
    public static class EaseFunctions
    {
        public enum Ease
        {
            Linear = 0,

            InSine,
            OutSine,
            InOutSine,

            InQuadratic,
            OutQuadratic,
            InOutQuadratic,

            InCubic,
            OutCubic,
            InOutCubic,

            InQuartic,
            OutQuartic,
            InOutQuartic,

            InQuintic,
            OutQuintic,
            InOutQuintic,

            InExponential,
            OutExponential,
            InOutExponential,

            InCircular,
            OutCircular,
            InOutCircular,

            InSpring,
            OutSpring,
            InOutSpring,

            InBounce,
            OutBounce,
            InOutBounce,

            InBack,
            OutBack,
            InOutBack,

            InElastic,
            OutElastic,
            InOutElastic,
        }

        public static Vector3 Interpolate(Vector3 start, Vector3 end, float value, Ease ease = Ease.Linear)
        {
            return new Vector3(
                Interpolate(start.x, end.x, value, ease),
                Interpolate(start.y, end.y, value, ease),
                Interpolate(start.z, end.z, value, ease)
            );
        }

        public static Vector2 Interpolate(Vector2 start, Vector2 end, float value, Ease ease = Ease.Linear)
        {
            return new Vector2(
                Interpolate(start.x, end.x, value, ease),
                Interpolate(start.y, end.y, value, ease)
            );
        }

        public static Quaternion Interpolate(Quaternion start, Quaternion end, float value, Ease ease = Ease.Linear)
        {
            return Quaternion.SlerpUnclamped(start, end, Interpolate(0, 1, value, ease));
        }

        public static float Interpolate(float start, float end, float value, Ease ease = Ease.Linear)
        {
            value = Mathf.Clamp01(value);
            var interpolate = ease switch
            {
                Ease.Linear => value,
                Ease.InSine => EaseInSine(value),
                Ease.OutSine => EaseOutSine(value),
                Ease.InOutSine => EaseInOutSine(value),
                Ease.InQuadratic => EaseInQuad(value),
                Ease.OutQuadratic => EaseOutQuad(value),
                Ease.InOutQuadratic => EaseInOutQuad(value),
                Ease.InCubic => EaseInCubic(value),
                Ease.OutCubic => EaseOutCubic(value),
                Ease.InOutCubic => EaseInOutCubic(value),
                Ease.InQuartic => EaseInQuart(value),
                Ease.OutQuartic => EaseOutQuart(value),
                Ease.InOutQuartic => EaseInOutQuart(value),
                Ease.InQuintic => EaseInQuint(value),
                Ease.OutQuintic => EaseOutQuint(value),
                Ease.InOutQuintic => EaseInOutQuint(value),
                Ease.InExponential => EaseInExpo(value),
                Ease.OutExponential => EaseOutExpo(value),
                Ease.InOutExponential => EaseInOutExpo(value),
                Ease.InCircular => EaseInCirc(value),
                Ease.OutCircular => EaseOutCirc(value),
                Ease.InOutCircular => EaseInOutCirc(value),
                Ease.InSpring => EaseInSpring(value),
                Ease.OutSpring => EaseOutSpring(value),
                Ease.InOutSpring => EaseInOutSpring(value),
                Ease.InBounce => EaseInBounce(value),
                Ease.OutBounce => EaseOutBounce(value),
                Ease.InOutBounce => EaseInOutBounce(value),
                Ease.InBack => EaseInBack(value),
                Ease.OutBack => EaseOutBack(value),
                Ease.InOutBack => EaseInOutBack(value),
                Ease.InElastic => EaseInElastic(value),
                Ease.OutElastic => EaseOutElastic(value),
                Ease.InOutElastic => EaseInOutElastic(value),
                _ => value,
            };
            return start + (end - start) * interpolate;
        }

        private static float EaseOutQuad(float value)
        {
            return Mathf.Pow(value, 2);
        }
        private static float EaseInQuad(float value)
        {
            return 1 - Mathf.Pow(value - 1, 2);
        }
        private static float EaseInOutQuad(float value)
        {
            if (2 * value < 1) return EaseOutQuad(value) * 2;
            return EaseInQuad(value) * 2 - 1;
        }

        private static float EaseOutCubic(float value)
        {
            return Mathf.Pow(value, 3);
        }
        private static float EaseInCubic(float value)
        {
            return 1 + Mathf.Pow(value - 1, 3);
        }
        private static float EaseInOutCubic(float value)
        {
            if (2 * value < 1) return EaseOutCubic(value) * 4;
            return EaseInCubic(value) * 4 - 3;
        }

        private static float EaseOutQuart(float value)
        {
            return Mathf.Pow(value, 4);
        }
        private static float EaseInQuart(float value)
        {
            return 1 - Mathf.Pow(value - 1, 4);
        }
        private static float EaseInOutQuart(float value)
        {
            if (2 * value < 1) return EaseOutQuart(value) * 8;
            return EaseInQuart(value) * 8 - 7;
        }

        private static float EaseOutQuint(float value)
        {
            return Mathf.Pow(value, 5);
        }
        private static float EaseInQuint(float value)
        {
            return 1 + Mathf.Pow(value - 1, 5);
        }
        private static float EaseInOutQuint(float value)
        {
            if (2 * value < 1) return EaseOutQuint(value) * 16;
            return EaseInQuint(value) * 16 - 15;
        }

        private static float EaseOutSine(float value)
        {
            return 1 - Mathf.Cos(value * Mathf.PI / 2);
        }
        private static float EaseInSine(float value)
        {
            return Mathf.Sin(value * Mathf.PI / 2);
        }
        private static float EaseInOutSine(float value)
        {
            return (1 - Mathf.Cos(value * Mathf.PI)) / 2;
        }

        private static float EaseOutExpo(float value)
        {
            return Mathf.Pow(2, 10 * (value - 1));
        }
        private static float EaseInExpo(float value)
        {
            return 1 - Mathf.Pow(2, -10 * value);
        }
        private static float EaseInOutExpo(float value)
        {
            value *= 2;
            if (value < 1) return EaseOutExpo(value) / 2f;
            return EaseInExpo(value - 1) / 2f + 0.5f;
        }

        private static float EaseOutCirc(float value)
        {
            return 1 - Mathf.Sqrt(1 - Mathf.Pow(value, 2));
        }
        private static float EaseInCirc(float value)
        {
            return Mathf.Sqrt(1 - Mathf.Pow(value - 1, 2));
        }
        private static float EaseInOutCirc(float value)
        {
            value *= 2f;
            if (value < 1) return EaseOutCirc(value) / 2f;
            return EaseInCirc(value - 1) / 2f + 0.5f;
        }

        private static float EaseOutSpring(float value)
        {
            return 1 - EaseInSpring(1 - value);
        }
        private static float EaseInSpring(float value)
        {
            return (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        }
        private static float EaseInOutSpring(float value)
        {
            value *= 2f;
            if (value < 1) return EaseOutSpring(value) / 2f;
            return EaseInSpring(value - 1) / 2f + 0.5f;
        }
        private static float EaseOutBounce(float value)
        {
            return 1 - EaseInBounce(1 - value);
        }
        private static float EaseInBounce(float value)
        {
            if (value < 8f / 22f)
            {
                return 121f / 16f * Mathf.Pow(value, 2);
            }
            else if (value < 16f / 22f)
            {
                return 121f / 16f * Mathf.Pow(value - 12f / 22f, 2) + 48f / 64f;
            }
            else if (value < 20f / 22f)
            {
                return 121f / 16f * Mathf.Pow(value - 18f / 22f, 2) + 60f / 64f;
            }
            else
            {
                return 121f / 16f * Mathf.Pow(value - 21f / 22f, 2) + 63f / 64f;
            }
        }
        private static float EaseInOutBounce(float value)
        {
            value *= 2f;
            if (value < 1) return EaseOutBounce(value) / 2f;
            else return EaseInBounce(value - 1) / 2f + 0.5f;
        }

        private static float EaseOutBack(float value)
        {
            float s = 1.70158f;
            return Mathf.Pow(value, 2) * ((s + 1) * value - s);
        }
        private static float EaseInBack(float value)
        {
            float s = 1.70158f;
            value--;
            return Mathf.Pow(value, 2) * ((s + 1) * value + s) + 1;
        }
        private static float EaseInOutBack(float value)
        {
            value *= 2f;
            if (value < 1) return EaseOutBack(value) / 2f;
            return EaseInBack(value - 1) / 2f + 0.5f;
        }

        private static float EaseOutElastic(float value)
        {
            return -Mathf.Pow(2, 10 * (value - 1)) * Mathf.Sin((40 * value - 43) * Mathf.PI / 6);
        }
        private static float EaseInElastic(float value)
        {
            return 1 - EaseOutElastic(1 - value);
        }
        private static float EaseInOutElastic(float value)
        {
            value *= 2f;
            if (value < 1) return EaseInElastic(value) / 2f;
            return EaseOutElastic(value - 1) / 2f + 0.5f;
        }
    }

}
