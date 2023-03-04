namespace LoginUser_Intern_test.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){
        }

        public DbSet<User> Users { get; set; }
    }
}
