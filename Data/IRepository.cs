namespace SD_340_W22SD_Final_Project_Group6.Data
{
        public interface IRepository<T> where T : class
        {
            // CREATE
            public Task<T> Create(T entity);

            // READ
            public Task<T?> Get(int id);
            public Task<ICollection<T>> GetAll();

            // UPDATE
            public Task<T> Update(T entity);

            // DELETE
            public Task Delete(T entity);
        }

}
