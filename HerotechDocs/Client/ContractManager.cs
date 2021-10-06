using HerotechDocs.Client.Events;
using HerotechDocs.Client.ViewModels;
using HerotechDocs.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace HerotechDocs.Client
{
    public class ContractManager : EntityManager
    {
        public List<Contract> ContractList = new List<Contract>();

        public ContractManager(AuthenticationState authenticationState, NavigationManager navigationManager, HttpClient httpClient, IJSRuntime iJSRuntime)
            : base(authenticationState, navigationManager, httpClient, iJSRuntime)
        {
            var user = authenticationState.User;
            if (!user.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }

            GetContracts();
        }

        public async void GetContracts()
        {
            ContractList = await HttpClient.GetFromJsonAsync<List<Contract>>("api/Contracts");
            UpdateUI();
        }

        public async Task<bool> DeleteContract(Contract contract)
        {
            var response = await HttpClient.DeleteAsync($"api/Contracts/{contract.Id}");
            if (response.IsSuccessStatusCode)
            {
                ContractList.Remove(contract);
                UpdateUI();
                await IJSRuntime.InvokeVoidAsync("CloseModal", "#deleteContractModal");
                return true;
            }
            else
            {
                return false;
            }



        }

        public async Task<bool> AddItemToContract(Item item)
        {
            var response = await HttpClient.PostAsJsonAsync<Item>("api/Items", item);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> AddContract(CreateContractViewModel createContractViewModel)
        {
            //populate a contract instance
            Contract contract = new Contract();
            contract.Date = createContractViewModel.Date;
            contract.Amount = (double)createContractViewModel.Amount;
            contract.Purpose = createContractViewModel.Purpose;
            contract.Sponsor = createContractViewModel.Sponsor;

            //if a file is chosen
            if (createContractViewModel.File != null)
            {
                contract.File = new DatabaseFile();
                contract.File.Name = createContractViewModel.File.Name;
                contract.File.UploadDate = DateTime.Now;

                var stream = createContractViewModel.File.OpenReadStream(createContractViewModel.File.Size);
                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                contract.File.Data = ms.ToArray();
            }

            var response = await HttpClient.PostAsJsonAsync<Contract>("api/Contracts", contract);

            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                var newContract = JsonConvert.DeserializeObject<Contract>(stringResponse);
                ContractList.Add(newContract);
                ContractList.Sort((a, b) => b.Date.CompareTo(a.Date));

                UpdateUI();
                await IJSRuntime.InvokeVoidAsync("CloseModal", "#addContractModal");
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateContract(Contract contract)
        {
            var response = await HttpClient.PutAsJsonAsync<Contract>($"api/Contracts/{contract.Id}", contract);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var toUpdate = ContractList.First(c => c.Id == contract.Id);
            toUpdate = contract;

            await IJSRuntime.InvokeVoidAsync("CloseModal", "#editContractModal");
            return true;
        }

        public async void DownloadContractFile(int? fileId)
        {
            DatabaseFile df = await HttpClient.GetFromJsonAsync<DatabaseFile>($"api/DatabaseFiles/{fileId}");
            if (df != null)
            {
                await IJSRuntime.InvokeVoidAsync("BlazorDownloadFile", df.Name, "text/plain", df.Data);
            }
            else
            {
                await IJSRuntime.InvokeAsync<object>("alert", "Error downloading contract.");
            }
        }

        public async void UpdateContractFile(Contract contract, IBrowserFile file)
        {
            contract.File = new DatabaseFile();
            contract.File.Name = file.Name;
            contract.File.UploadDate = DateTime.Now;

            var stream = file.OpenReadStream(file.Size);
            MemoryStream ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            contract.File.Data = ms.ToArray();


            var response = await HttpClient.PutAsJsonAsync<Contract>($"api/Contracts/{contract.Id}", contract);

            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                var newContract = JsonConvert.DeserializeObject<Contract>(stringResponse);
                contract.DatabaseFileId = newContract.DatabaseFileId;
                UpdateUI();
            }
            else
            {
                await IJSRuntime.InvokeAsync<object>("alert", "Error updating contract.");
            }
        }

    }
}
