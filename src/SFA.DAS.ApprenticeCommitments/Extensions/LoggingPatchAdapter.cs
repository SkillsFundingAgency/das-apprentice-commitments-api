using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace SFA.DAS.ApprenticeCommitments.Extensions
{
    internal class LoggingPatchAdapter : IObjectAdapter
    {
        private readonly ILogger _logger;
        private readonly IContractResolver _contractResolver = new DefaultContractResolver();
        private readonly IAdapterFactory _adapterFactory = new AdapterFactory();

        public LoggingPatchAdapter(ILogger logger) => _logger = logger;

        public void Add(Operation operation, object objectToApplyTo) => Apply(operation, objectToApplyTo);
        public void Copy(Operation operation, object objectToApplyTo) => Apply(operation, objectToApplyTo);
        public void Move(Operation operation, object objectToApplyTo) => Apply(operation, objectToApplyTo);
        public void Remove(Operation operation, object objectToApplyTo) => Apply(operation, objectToApplyTo);
        public void Replace(Operation operation, object objectToApplyTo) => Apply(operation, objectToApplyTo);

        private void Apply(Operation operation, object objectToApplyTo)
        {
            var noDetail = new[] { "/Email", "/LastName", "/FirstName", "/DateOfBirth" };
            var detail = noDetail.Contains(operation.path) ? "" : $"with {operation.value}";

            _logger.LogInformation($"{operation.OperationType} {operation.path} {detail}");

            operation.Apply(
                objectToApplyTo,
                new ObjectAdapter(_contractResolver, null, _adapterFactory));
        }
    }
}