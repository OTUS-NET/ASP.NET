using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.GrpcHost.Protos;

namespace Pcf.GivingToCustomer.GrpcHost.Services
{
    public class PreferenceGrpcService(IRepository<Preference> repository) : PreferenceGrpc.PreferenceGrpcBase
    {
        private readonly IRepository<Preference> _repository = repository;

        /// <summary>
        /// Получить список предпочтений
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<PreferenceReplyList> GetPreferences(
            Empty request,
            ServerCallContext context)
        {
            var preferences = await _repository.GetAllAsync();

            return new PreferenceReplyList
            {
                Preferences =
                {
                    preferences.Select(x => new PreferenceReply
                    {
                        Id = x.Id.ToString(),
                        Name = x.Name
                    })
                }
            };
        }
    }
}
