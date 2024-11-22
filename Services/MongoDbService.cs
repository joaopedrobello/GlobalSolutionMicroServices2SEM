using GlobalSolution.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GlobalSolution.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<ConsumoModel> _consumosCollection;

        public MongoDbService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("MongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("MongoDbSettings:DatabaseName"));
            _consumosCollection = database.GetCollection<ConsumoModel>(configuration.GetValue<string>("MongoDbSettings:CollectionName"));
        }

        public async Task InsertConsumoAsync(ConsumoModel consumo)
        {
            try
            {
                await _consumosCollection.InsertOneAsync(consumo);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao inserir consumo no MongoDB: " + ex.Message);
            }
        }

        public async Task<List<ConsumoModel>> GetConsumosAsync()
        {
            try
            {
                return await _consumosCollection.Find(consumo => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao recuperar consumos do MongoDB: " + ex.Message);
            }
        }
    }
}
