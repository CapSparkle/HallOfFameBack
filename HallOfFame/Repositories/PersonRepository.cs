using AutoMapper;
using HallOfFame.Data;
using HallOfFame.Entities;
using HallOfFame.Models;
using Microsoft.EntityFrameworkCore;

namespace HallOfFame.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PersonRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            var entities = await _context.Persons.Include(p => p.PersonSkills).ThenInclude(ps => ps.Skill).ToListAsync();
            return _mapper.Map<IEnumerable<Person>>(entities);
        }

        public async Task<Person> GetByIdAsync(long id)
        {
            var entity = await _context.Persons.Include(p => p.PersonSkills).ThenInclude(ps => ps.Skill).FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<Person>(entity);
        }

        public async Task<Person> AddAsync(Person person)
        {
            var entity = _mapper.Map<PersonEntity>(person);
            _context.Persons.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Person>(entity);
        }

        public async Task<Person> UpdateAsync(Person person)
        {
            var entity = await _context.Persons.Include(p => p.PersonSkills).FirstOrDefaultAsync(p => p.Id == person.Id);
            if (entity == null)
            {
                return null;
            }
            _mapper.Map(person, entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Person>(entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.Persons.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            _context.Persons.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
