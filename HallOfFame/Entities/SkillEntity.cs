namespace HallOfFame.Entities
{
    public class SkillEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }
        public ICollection<PersonSkill> PersonSkills { get; set; }
    }
}
