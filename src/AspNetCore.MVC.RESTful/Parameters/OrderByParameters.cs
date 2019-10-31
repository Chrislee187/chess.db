namespace AspNetCore.MVC.RESTful.Parameters
{
    public class OrderByParameters
    {
        public static OrderByParameters Default { get; } = new OrderByParameters();

        public string Clause { get; set; }
    }
}