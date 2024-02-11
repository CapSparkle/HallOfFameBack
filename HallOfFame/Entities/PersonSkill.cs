namespace HallOfFame.Entities
{
    public class PersonSkill
    {
        public long PersonId { get; set; }
        public PersonEntity Person { get; set; }
        public long SkillId { get; set; }
        public SkillEntity Skill { get; set; }
    }
}
