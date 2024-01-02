using System;
using System.Linq;
using System.Threading.Tasks;
using HeatKeeper.Abstractions;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Server.Measurements;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Configuration;

namespace HeatKeeper.Server.Export
{
    [Order(1)]
    public class InfluxDbBootStrapper : IBootStrapper
    {
        private readonly IConfiguration _configuration;

        public InfluxDbBootStrapper(IConfiguration configuration)
            => _configuration = configuration;

        public async Task Execute()
        {
            var url = _configuration.GetInfluxDbUrl();
            var key = _configuration.GetInfluxDbApiKey();
            using var client = new InfluxDBClient(_configuration.GetInfluxDbUrl(), _configuration.GetInfluxDbApiKey());
            var bucketApi = client.GetBucketsApi();
            var test = _configuration.GetInfluxDbOrganization();
            var orgId = (await client.GetOrganizationsApi().FindOrganizationsAsync(org: _configuration.GetInfluxDbOrganization())).First().Id;

            await CreateBucket(RetentionPolicy.Hour, seconds: 3600);
            await CreateBucket(RetentionPolicy.Day, seconds: 24 * 3600);
            await CreateBucket(RetentionPolicy.Week, seconds: 7 * 24 * 3600);
            await CreateBucket(RetentionPolicy.None, 0);


            async Task CreateBucket(RetentionPolicy retentionPolicy, long seconds)
            {
                var bucketName = Enum.GetName<RetentionPolicy>(retentionPolicy);
                if (await bucketApi.FindBucketByNameAsync(bucketName) == null)
                {
                    var retentionRules = new BucketRetentionRules(BucketRetentionRules.TypeEnum.Expire, seconds);
                    await client.GetBucketsApi().CreateBucketAsync(bucketName, retentionRules, orgId);
                }
            }
        }


    }
}
