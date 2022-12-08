using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace ConfiguratorWeb.App.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IFormFile"/>.
    /// </summary>
    public static class IFormFileExtensions
    {
        /// <summary>
        /// Returns the content of the <paramref name="this"/> as string.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>A string containing the content of the <paramref name="this"/>.</returns>
        public static string ReadAsString(this IFormFile @this)
        {
            string strContent = null;

            if (@this != null)
            {
                using (var objStream = new MemoryStream())
                {
                    @this.CopyTo(objStream);
                    strContent = Encoding.Default.GetString(objStream.ToArray());
                }
            }

            return strContent;
        }
    }
}