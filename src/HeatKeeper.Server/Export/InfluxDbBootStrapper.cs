using System;
using System.Linq;
using System.Threading.Tasks;
using HeatKeeper.Abstractions;
using HeatKeeper.Server.Configuration;
using HeatKeeper.Server.Measurements;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Configuration;
using Vibrant.InfluxDB.Client;

namespace HeatKeeper.Server.Export
{
    public class InfluxDbBootStrapper : IBootStrapper
    {
        private readonly IConfiguration _configuration;

        public InfluxDbBootStrapper(IInfluxClient influxClient, IConfiguration configuration)
            => _configuration = configuration;

        public async Task Execute()
        {
            using var client = new InfluxDBClient(_configuration.GetInfluxDbUrl(), _configuration.GetInfluxDbApiKey());
            var bucketApi = client.GetBucketsApi();
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
