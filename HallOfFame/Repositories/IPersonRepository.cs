using HallOfFame.Models;

namespace HallOfFame.Repositories
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetAllAsync();
        Task<Person> GetByIdAsync(long id);
        Task<Person> AddAsync(Person person);
        Task<Person> UpdateAsync(Person person);
        Task<bool> DeleteAsync(long id);
    }
}
