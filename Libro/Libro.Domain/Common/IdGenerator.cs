namespace Libro.Domain.Common
{
    public static class IdGenerator
    {
        public static string GenerateTransactionId()
        {
            // Generate a unique transaction ID using a combination of timestamp and UUID
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string uniqueId = Guid.NewGuid().ToString().Replace("-", "");
            string transactionId = $"{timestamp}_{uniqueId}";

            return transactionId;
        }
    }

}
