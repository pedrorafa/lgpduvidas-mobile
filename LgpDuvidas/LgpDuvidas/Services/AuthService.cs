﻿using LgpDuvidas.Data;
using LgpDuvidas.Interfaces;
using LgpDuvidas.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LgpDuvidas.Services
{
    public class AuthService : IAuthService
    {
        private IDbContext _dbContext => DependencyService.Get<IDbContext>();
        private readonly HttpClient _client;

        public AuthService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://lgpduvidas.azurewebsites.net/api/")
            };
        }
        public async Task<AuthModel> Register(AuthModel input)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("register", content);

                response.EnsureSuccessStatusCode();
                await response.Content.ReadAsStringAsync();
            }
            catch (Exception err)
            {
            }
            return input;

        }
        public async Task<AuthModel> Login(AuthModel auth)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(auth), Encoding.UTF8, "application/json");
                var response = _client.PostAsync("login", content);
                response.Wait();

                response.Result.EnsureSuccessStatusCode();
                auth.Token = await response.Result.Content.ReadAsStringAsync();
                auth.Token = auth.Token.Replace("\"", "");

                _dbContext.Connection.InsertOrReplace(auth);
            }
            catch (Exception err)
            {
            }
            return auth;
        }
    }
}