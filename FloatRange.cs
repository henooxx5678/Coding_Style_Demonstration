using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DoubleHeat.Utilites {

    [Serializable]
    public struct FloatRange {

        [SerializeField] float _valueA;
        [SerializeField] float _valueB;


        public float MinValue => Mathf.Min(_valueA, _valueB);
        public float MaxValue => Mathf.Max(_valueA, _valueB);
        public float RangeSize => Mathf.Abs(_valueA - _valueB);


        public FloatRange (float valueA, float valueB, float offset = 0f) {
            _valueA = valueA + offset;
            _valueB = valueB + offset;;
        }

        public FloatRange (FloatRange other, float offset = 0f) : this (other._valueA, other._valueB, offset) {}


        public void Set (float valueA, float valueB) {
            _valueA = valueA;
            _valueB = valueB;
        }

        public void ApplyOffset (float offset) {
            _valueA += offset;
            _valueB += offset;
        }

        public bool IsInRange (float value) {
            return value >= MinValue && value <= MaxValue;
        }

        public float Clamp (float value) {
            return Mathf.Clamp(value, MinValue, MaxValue);
        }

        public float LerpFromMinToMax (float t) {
            return Mathf.Lerp(MinValue, MaxValue, t);
        }

        public float LerpFromMaxToMin (float t) {
            return Mathf.Lerp(MaxValue, MinValue, t);
        }

        /// <summary>
        /// Will return the progress rate value between 0 and 1 if the given value is in range. When "limitInRange" is true, the return value will be clamped between 0 and 1.
        /// </summary>
        public float GetProgressRate (float value, bool limitInRange = true) {
            float progressRate = (value - MinValue) / RangeSize;

            if (limitInRange) {
                progressRate = Mathf.Clamp(progressRate, 0f, 1f);
            }

            return progressRate;
        }

        public float GetRandomInRange () {
            return Random.Range(MinValue, MaxValue);
        }

        /// <summary>
        /// Got 0 if the given value is in range.
        /// </summary>
        public float GetRelativePositionFromNearestEdge (float value) {
            if (value < MinValue) {
                return value - MinValue;
            }
            else if (value > MaxValue) {
                return value - MaxValue;
            }
            return 0f;
        }

        public float GetGap (FloatRange other) {
            return Mathf.Abs(GetSignedGap(other));
        }

        public float GetSignedGap (FloatRange other) {

            float thisMaxToOtherMin = other.MinValue - this.MaxValue;
            if (thisMaxToOtherMin > 0f) {
                return thisMaxToOtherMin;
            }

            float thisMinToOtherMax = other.MaxValue - this.MinValue;
            if (thisMinToOtherMax < 0f) {
                return thisMinToOtherMax;
            }
            return 0f;
        }

        public float[] GetEdgeValues () {
            return new float[2] { MinValue, MaxValue };
        }


        public string ToStringInSimpleFormat (string floatFormat = null, bool reverseOrder = false) {
            string min = MinValue.ToString(floatFormat);
            string max = MaxValue.ToString(floatFormat);
            return !reverseOrder ? $"[{min}, {max}]" : $"[{max}, {min}]";
        }


        public static bool HasIntersection (FloatRange a, FloatRange b) {
            return GetIntersection(a, b, out _);
        }

        public static bool HasIntersection (IEnumerable<FloatRange> ranges) {
            return GetIntersection(ranges, out _);
        }

        public static bool GetIntersection (FloatRange a, FloatRange b, out FloatRange result) {
            float newMinValue = Mathf.Max(a.MinValue, b.MinValue);
            float newMaxValue = Mathf.Min(a.MaxValue, b.MaxValue);
            if (newMinValue > newMaxValue) {
                result = default;
                return false;
            }
            result = new FloatRange(newMinValue, newMaxValue);
            return true;
        }

        public static bool GetIntersection (IEnumerable<FloatRange> ranges, out FloatRange result) {
            if (ranges == null || ranges.Count() == 0) {
                result = default;
                return false;
            }
            float newMinValue = ranges.Max(range => range.MinValue);
            float newMaxValue = ranges.Min(range => range.MaxValue);
            if (newMinValue > newMaxValue) {
                result = default;
                return false;
            }
            result = new FloatRange(newMinValue, newMaxValue);
            return true;
        }



        // == Equals implements ==
        public override bool Equals (object obj) => obj is FloatRange other && this.Equals(other);

        public bool Equals (FloatRange other) => MinValue == other.MinValue && MaxValue == other.MaxValue;

        public override int GetHashCode () => (MinValue, MaxValue).GetHashCode();

        public static bool operator == (FloatRange lhs, FloatRange rhs) => lhs.Equals(rhs);

        public static bool operator != (FloatRange lhs, FloatRange rhs) => !(lhs == rhs);

    }

}
