using System.Collections.Specialized;
using System.Web;


namespace nbaunderdogleagueAPI.DataAccess.Helpers
{
    public static class HttpExtensions
    {
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            NameValueCollection httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            UriBuilder ub = new(uri) {
                Query = httpValueCollection.ToString()
            };

            return ub.Uri;
        }
    }
}