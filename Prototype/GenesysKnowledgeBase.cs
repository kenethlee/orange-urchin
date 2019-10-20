using Newtonsoft.Json;
using Prototype.Interfaces;
using RestSharp;
using System;

namespace Prototype
{
    public class GenesysKnowledgeBase : IKnowledgeBase
    {
        public string KnowledgeBase { get; }
        public string Token { get; }

        public GenesysKnowledgeBase(string token, string knowledgeBase)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            KnowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
        }

        public IRestResponse Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) throw new ArgumentNullException(nameof(query));

            var param = new Parameter {query = query};

            var client = new RestClient($"{_endpoint}/knowledgebases/{KnowledgeBase}/search");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", "514");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Host", "api.genesysappliedresearch.com");
            request.AddHeader("Postman-Token", "4c4fcf5f-46e5-4742-a185-7bf4725502ba,5d77501e-7946-4a68-8c70-d8d558bf1db5");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.18.0");
            request.AddHeader("token", $"{Token}");
            request.AddHeader("organizationid", $"{_orgId}");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", JsonConvert.SerializeObject(param), ParameterType.RequestBody);
            return client.Execute(request);
        }

        private string _endpoint = "https://api.genesysappliedresearch.com/v2/knowledge";
        private string _orgId = "180dba95-1ab6-44b0-9c94-4630e8d280bf";
    }

    internal class Parameter
    {
        public string query { get; set; }
        public int pageSize { get; set; } = 5;
        public int pageNumber { get; set; } = 1;
        public string sortOrder { get; set; } = "string";
        public string sortBy { get; set; } = "string";
        public string languageCode { get; set; } = "en-US";
        public string documentType { get; set; } = "Faq";

    }
}
