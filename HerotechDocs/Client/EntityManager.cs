using HerotechDocs.Client.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HerotechDocs.Client
{
    public abstract class EntityManager
    {
        public AuthenticationState AuthenticationState { get; set; }
        public NavigationManager NavigationManager { get; set; }
        public HttpClient HttpClient { get; set; }
        public IJSRuntime IJSRuntime { get; set; }

        public event UIUpdatedEventHandler UIUpdated;

        public EntityManager(AuthenticationState authenticationState, NavigationManager navigationManager, HttpClient httpClient, IJSRuntime iJSRuntime)
        {
            AuthenticationState = authenticationState;
            NavigationManager = navigationManager;
            HttpClient = httpClient;
            IJSRuntime = iJSRuntime;
        }

        public void UpdateUI()
        {
            UIUpdated?.Invoke();
        }
    }
}
