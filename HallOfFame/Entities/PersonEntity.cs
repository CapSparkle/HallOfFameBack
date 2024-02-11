namespace HallOfFame.Entities
{
    public class PersonEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ICollection<PersonSkill> PersonSkills { get; set; }
    }
}
