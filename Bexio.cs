﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using bexio.net.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace bexio.net
{
    public class Bexio
    {
        public string _url;
        public string _apiToken;
        public HttpClient _httpClient = new HttpClient();

        public Bexio(string url, string apiToken) {
            _url = url;
            _apiToken = apiToken;

            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient.BaseAddress = new Uri(url);

            _httpClient.DefaultRequestHeaders.Authorization = GenerateHeader();
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync() {
            var response = await GetAsync("pr_project");
            IEnumerable<Project> list = JsonConvert.DeserializeObject<IEnumerable<Project>>(response);
            return list;

        }

        private AuthenticationHeaderValue GenerateHeader() {
            return new AuthenticationHeaderValue("Bearer", _apiToken);
        }

        private async Task<string> GetAsync(string path)
        {
            UriBuilder uri = new UriBuilder();
            uri.Host = _url;
            uri.Path = path;
            HttpResponseMessage response = await _httpClient.GetAsync(uri.Uri);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string message = String.Format("Get failed. Received HTTP {0}", response.StatusCode);
                throw new ApplicationException(message);
            }

            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        } 

        private async Task<string> PostAsync(Uri url, Dictionary<string, string> body)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(body);

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                throw new ApplicationException(message);
            }

            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        } 
    }
}