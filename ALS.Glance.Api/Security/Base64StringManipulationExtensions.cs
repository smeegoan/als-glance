using System;
using System.Text;

namespace ALS.Glance.Api.Security
{
    /// <summary>
    /// Class with extension methods for Base64 encode/decode operations
    /// </summary>
    public static class Base64StringManipulationExtensions
    {

        /// <summary>
        /// Decodes a base64 ASCII encoded string
        /// </summary>
        /// <param name="valueToDecode">The value to decode</param>
        /// <returns>The decoded string</returns>
        /// <exception cref="DecoderFallbackException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public static string DecodeFromBase64ASCII(this string valueToDecode)
        {
            var encoding = (Encoding)Encoding.ASCII.Clone();
            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;

            return encoding.GetString(Convert.FromBase64String(valueToDecode));
        }

        /// <summary>
        /// Encodes a given string using a base64 ASCII encoder
        /// </summary>
        /// <param name="valueToEncode">The value to encode</param>
        /// <returns>The encoded string</returns>
        /// <exception cref="EncoderFallbackException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string EncodeToBase64ASCII(this string valueToEncode)
        {
            var encoding = (Encoding)Encoding.ASCII.Clone();
            encoding.EncoderFallback = EncoderFallback.ExceptionFallback;

            return Convert.ToBase64String(encoding.GetBytes(valueToEncode));
        }

    }
}