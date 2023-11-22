using Integration.Backend;
using Integration.Common;
using RedLockNet.SERedis;

namespace Integration.DistributedSystem
{
    public class ItemIntegrationService
    {
        private readonly RedLockFactory _redLockFactory;
        private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();


        public ItemIntegrationService(RedLockFactory redLockFactory)
        {
            _redLockFactory = redLockFactory;
        }

        public Result SaveItem(string itemContent)
        {

            /*
             *  Notes
             *      - Assuming Redis configurations and Dependency Injection settings have been made.
             *      - Assuming Redis is used instead of ItemIntegrationBackend.
             *      Since ItemIntegrationBackend stores data in InMemory, it should be stored in a common storage space instead
             * */

            using (var redLock = _redLockFactory.CreateLock(itemContent, TimeSpan.FromSeconds(30)))
            {
                if (redLock.IsAcquired)
                {
                    if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
                    {
                        return new Result(false, $"Duplicate item received with content {itemContent}.");
                    }

                    var item = ItemIntegrationBackend.SaveItem(itemContent);

                    return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
                }

                return new Result(false, $"Duplicate item received with content {itemContent}.");
            }
        }

    }
}
