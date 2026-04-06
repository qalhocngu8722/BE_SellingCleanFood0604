namespace Project_Selling_Clean_Food.DTOs
{
    public class DetailIn4User
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; } // 'admin','staff','user'
        public int cart_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;

    }
}
