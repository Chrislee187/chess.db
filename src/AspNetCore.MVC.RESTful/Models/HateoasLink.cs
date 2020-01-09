using System.Diagnostics.CodeAnalysis;
using AspNetCore.MVC.RESTful.Configuration;

namespace AspNetCore.MVC.RESTful.Models
{
    /// <summary>
    /// Model for a Hateoas link
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class HateoasLink
    {
        public static HateoasLink Create(string url) => new HateoasLink(
            HateoasConfig.Relationships.Create,
            "POST",
            url);
        public static HateoasLink Upsert(string url) => new HateoasLink(
            HateoasConfig.Relationships.Upsert,
            "PUT",
            url);
        public static HateoasLink Patch(string url) => new HateoasLink(
            HateoasConfig.Relationships.Patch,
            "PATCH",
            url);
        public static HateoasLink Delete(string url) => new HateoasLink(
            HateoasConfig.Relationships.Delete,
            "DELETE",
            url);
        public static HateoasLink Collection(string url, string rel = null) => new HateoasLink(
            string.IsNullOrWhiteSpace(rel) ? HateoasConfig.Relationships.CurrentPage : rel,
            "GET",
            url);
        public static HateoasLink Get(string url, string rel = null) => new HateoasLink(
            string.IsNullOrWhiteSpace(rel) ? HateoasConfig.Relationships.Self : rel,
            "GET",
            url);

        public string Rel { get; }
        public string Method { get; }
        public string Href { get; }

        public HateoasLink(string rel, string method, string href)
        {
            Rel = rel;
            Method = method;
            Href = href;
        }

    }
}