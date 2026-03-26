namespace Project_Selling_Clean_Food.Model
{
    public class users
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; } // 'admin','staff','user'
        public DateTime created_at { get; set; } = DateTime.Now;

    }
}
