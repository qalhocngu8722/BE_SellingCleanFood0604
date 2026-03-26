namespace Project_Selling_Clean_Food.Model
{
    public class cart
    {
        public int? id { get; set; }
        public int user_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
