using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EasyWords.Client.Interfaces;
using EasyWords.Client.Models;
using Newtonsoft.Json;

namespace EasyWords.Client
{
    public class QuestionConsumer: IQuestionConsumer
    {

        Configuration configuration;

        public Configuration Configurations
        {
            get
            {
                if (configuration == null)
                {
                    configuration = ConfigurationProvider.ResolveConfiguration();
                }
                return configuration;
            }
        }


        public QuestionConsumer()
        {
        }

        public async Task<QuestionBinding> GetQuestion(int userId,int userClientId,int dictionaryId,int questionTypeId)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            //httpClient.BaseAddress = new Uri(configuration.ApiBaseAddress);
            //httpClient.BaseAddress = new Uri("https://httpbin.org");

            //string requestUri = string.Format("q/GetQuestion/{0}/{1}/{2}/{3}", userClientId, userId, dictionaryId, questionTypeId);


            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, Configurations.ApiBaseAddress+"/q/GetQuestion/1/1/1/1");

            var response =  httpClient.SendAsync(httpRequest).Result;

            if(response.IsSuccessStatusCode)
            {
                string jsonContent = await response.Content.ReadAsStringAsync();
                QuestionBinding questionBinding = JsonConvert.DeserializeObject<QuestionBinding>(jsonContent);

                return questionBinding;
            }
            else
            {
                return null;
            }

        }

        public async Task<AnswerResult> SendAnswer(AnswerModel answer)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, Configurations.ApiBaseAddress + "/q/SaveAnswer");
            var jsonString = JsonConvert.SerializeObject(answer);
            httpRequest.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = httpClient.SendAsync(httpRequest).Result;
            var responseContent = await response.Content.ReadAsStringAsync();
            AnswerResult answerResult = JsonConvert.DeserializeObject<AnswerResult>(responseContent);

            return answerResult;

        }
    }
}
