namespace Libro.Domain.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string resourceName, string searchKey, string searchValue)
            : base($"No {resourceName} found with {searchKey} = {searchValue}")
        {
        }
    }
}
