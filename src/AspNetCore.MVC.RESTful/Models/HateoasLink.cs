namespace AspNetCore.MVC.RESTful.Models
{
    public class HateoasLink
    {

        public string Rel { get; private set; }
        public string Method { get; private set; }
        public string Href { get; private set; }

        public HateoasLink(string rel, string method, string href)
        {
            Rel = rel;
            Method = method;
            Href = href;
        }
    }
}