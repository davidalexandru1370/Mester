namespace Registry.Errors.Services
{
    public class InvalidSpecialitiesTypeException : ServiceException
    {
        public List<string> Types { get; }
        public InvalidSpecialitiesTypeException(List<string> types) : base($"Invalid speciality type: {string.Join(",", types)}")
        {
            Types = types;
        }
    }
}
