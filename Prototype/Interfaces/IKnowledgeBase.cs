using RestSharp;

namespace Prototype.Interfaces
{
    public interface IKnowledgeBase
    {
        IRestResponse Search(string query);
    }
}
