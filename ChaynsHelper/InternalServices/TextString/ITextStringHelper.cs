using System.Collections.Generic;

namespace ChaynsHelper.InternalServices.TextString
{
    public interface ITextStringHelper
    {
        /// <summary>
        /// get a textstring, providing a dictionary of replacements and a fallback
        /// </summary>
        /// <param name="textString">name of the string</param>
        /// <param name="fallback">fallback if the string doesn't exist or could not be retrieved</param>
        /// <param name="replacements">replacements for variables</param>
        /// <param name="libName">library key if more than one library was initialized</param>
        /// <param name="overridePrefix">override the prefix instead of using the one passed in the beginning</param>
        /// <returns>textstring: string</returns>
        string GetTextString(
            string textString,
            string fallback = "",
            IDictionary<string, string> replacements = null,
            string libName = null,
            bool overridePrefix = false);

        /// <summary>
        /// get a textstring, providing a dictionary of replacements and a fallback
        /// </summary>
        /// <param name="textString">name of the string</param>
        /// <param name="fallback">fallback if the string doesn't exist or could not be retrieved</param>
        /// <param name="replacements">replacements for variables</param>
        /// <param name="libName">library key if more than one library was initialized</param>
        /// <param name="overridePrefix">override the prefix instead of using the one passed in the beginning</param>
        /// <typeparam name="T">The type of the lib key, so enums can be used for this</typeparam>
        /// <returns>textstring: string</returns>
        string GetTextString<T>(
            string textString,
            string fallback = "",
            IDictionary<string, string> replacements = null,
            T libName = default,
            bool overridePrefix = false);
    }
}