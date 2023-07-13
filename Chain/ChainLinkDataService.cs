using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace Chain
{
    public class ChainLinkDataService : IChainLinkDataService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IFeatureManagerSnapshot _featureManagerSnapshot;

        private string? connString = string.Empty;

        public ChainLinkDataService(/*ILoggerFactory loggerFactory,*/
            //ILogger<DocumentService> logger,
            IConfiguration configuration,
            IFeatureManagerSnapshot featureManagerSnapshot//,
                                                          //IConfigurationRefresherProvider refresherProvider
            )
        {
            //_logger = loggerFactory.CreateLogger<DocumentService>();
            //_logger = logger;
            _configuration = configuration;
            _featureManagerSnapshot = featureManagerSnapshot;
            //_configurationRefresher = refresherProvider.Refreshers.First();

            // Read configuration data
            connString = _configuration["LiteDB:ConnectionString"];
        }

        LiteDatabase dbClient = null;

        private ILiteCollection<ChainLink> GetCollection()
        {
            dbClient = new LiteDatabase(connString);

            return dbClient.GetCollection<ChainLink>("links");
        }

        public async Task<IEnumerable<ChainLink>> GetAllJobs()
        {
            List<ChainLink> rtnVal = new();
            return rtnVal;
        }

        public async Task<IEnumerable<ChainLink>> Select(string orderBy = "", bool descending = false)
        {
            List<ChainLink> rtnVal = new();

            ILiteCollection<ChainLink> col = GetCollection();

            if(descending)
            {
                foreach (ChainLink item in col.Query()
                    .OrderByDescending(orderBy ?? "Chain")
                    .ToEnumerable())
                {
                    rtnVal.Add(item);
                }
            }
            else
            {
                foreach (ChainLink item in col.Query()
                    .OrderBy(orderBy ?? "Chain")
                    .ToEnumerable())
                {
                    rtnVal.Add(item);
                }
            }

            dbClient.Dispose();

            return rtnVal;
        }

        public async Task<ChainLink> GetChainLink(string JobId)
        {
            ChainLink rtnVal = new();

            ILiteCollection<ChainLink> col = GetCollection();
            foreach (ChainLink item in col.Query()
                .Where(x => x.Id == JobId)
                .ToEnumerable())
            {
                rtnVal = item;
                break;
            }

            dbClient.Dispose();
            return rtnVal;
        }


        public async Task<ChainLink> CreateChainLink(ChainLink document)
        {
            ChainLink? newOne = document;
            string newId = string.Empty;

            try
            {
                var coll = GetCollection();

                try
                {
                    if (string.IsNullOrEmpty(newOne.Id))
                    {
                        newOne.Id = Guid.NewGuid().ToString().Replace("-", "");
                    }
                    //string jsonValue = JsonSerializer.Serialize(newOne);
                }
                catch (Exception ex)
                {
                    //response.StatusCode = HttpStatusCode.BadRequest;
                    ////response.Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(ex.ToString()));
                    //response.Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(ex.Message));
                    //return response;
                }

                newId = newOne.Id == null ? "" : newOne.Id.ToString();

                coll.Insert(newOne);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //response.StatusCode = HttpStatusCode.InternalServerError;
                //response.Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(ex.Message));
                //return response;
            }

            dbClient.Dispose();

            return newOne;
        }

        public async Task<ChainLink> UpdateChainLink(ChainLink document)
        {
            ChainLink? newOne = document;

            ILiteCollection<ChainLink> col = GetCollection();

            col.Update(newOne);

            dbClient.Dispose();

            return newOne;
        }

        public async Task<int> DeleteChainLink(string id)
        {
            int numDeleted = 0;

            ILiteCollection<ChainLink> col = GetCollection();

            col.Delete(id);

            dbClient.Dispose();

            return numDeleted;
        }
    }
}
