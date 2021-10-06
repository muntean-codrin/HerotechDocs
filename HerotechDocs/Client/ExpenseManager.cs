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
    public class ExpenseManager : EntityManager
    {
        public List<Expense> ExpenseList = new List<Expense>();
        public List<Expense> ExpensesToShow = new List<Expense>();
        public List<Category> CategoryList = new List<Category>();
        public List<Contract> ContractList = new List<Contract>();

        public DateTime fromDate = DateTime.Now.AddHours(2).AddMonths(-1);
        public DateTime toDate = DateTime.Now.AddHours(2);

        public ExpenseManager(AuthenticationState authenticationState, NavigationManager navigationManager, HttpClient httpClient, IJSRuntime iJSRuntime)
           : base(authenticationState, navigationManager, httpClient, iJSRuntime)
        {
            var user = authenticationState.User;
            if (!user.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }

            GetData();

        }

        private async void GetData()
        {
            CategoryList = await HttpClient.GetFromJsonAsync<List<Category>>("api/Categories");
            ExpenseList = await HttpClient.GetFromJsonAsync<List<Expense>>("api/Expenses");
            ContractList = (await HttpClient.GetFromJsonAsync<List<Contract>>("api/Contracts"))
                                .Where(a => a.Concluded == false).ToList();

            UpdateDate();
        }

        public void UpdateDate()
        {
            ExpensesToShow = ExpenseList.Where(e => e.Date >= fromDate && e.Date <= toDate).ToList();
            UpdateUI();
        }

        public async void DownloadExpenseFile(int? fileId)
        {
            DatabaseFile df = await HttpClient.GetFromJsonAsync<DatabaseFile>($"api/DatabaseFiles/{fileId}");
            if (df != null)
            {
                await IJSRuntime.InvokeVoidAsync("BlazorDownloadFile", df.Name, "text/plain", df.Data);
            }
            else
            {
                await IJSRuntime.InvokeAsync<object>("alert", "Error downloading expense.");
            }
        }

        public async Task<bool> DeleteExpense(Expense expense)
        {
            var response = await HttpClient.DeleteAsync($"api/Expenses/{expense.Id}");
            if (response.IsSuccessStatusCode)
            {
                ExpenseList.Remove(expense);
                ExpensesToShow.Remove(expense);
                UpdateUI();

                await IJSRuntime.InvokeVoidAsync("CloseModal", "#deleteExpenseModal");
                return true;
            }
            return false;
        }

        public async Task<bool> AddExpense(CreateExpenseViewModel createExpenseViewModel)
        {
            //populate a expense instance
            Expense expense = new Expense();
            expense.Date = createExpenseViewModel.Date.Date;
            expense.Supplier = createExpenseViewModel.Supplier;
            expense.Price = (double)createExpenseViewModel.Price;
            expense.Description = createExpenseViewModel.Description;
            expense.CategoryId = createExpenseViewModel.CategoryId;
            expense.ContractId = (int)createExpenseViewModel.ContractId;
            expense.ReceiptId = createExpenseViewModel.ReceiptId;

            //if a file is chosen
            if (createExpenseViewModel.File != null)
            {
                expense.File = new DatabaseFile();
                expense.File.Name = createExpenseViewModel.File.Name;
                expense.File.UploadDate = DateTime.Now;

                var stream = createExpenseViewModel.File.OpenReadStream(createExpenseViewModel.File.Size);
                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                expense.File.Data = ms.ToArray();
            }

            var response = await HttpClient.PostAsJsonAsync<Expense>("api/Expenses", expense);

            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                var newExpense = JsonConvert.DeserializeObject<Expense>(stringResponse);

                ExpenseList.Add(newExpense);
                ExpenseList.Sort((a, b) => b.Date.CompareTo(a.Date));
                UpdateDate();

                await IJSRuntime.InvokeVoidAsync("CloseModal", "#addExpenseModal");
                return true;
            }

            return false;

        }

        public async Task<bool> UpdateExpense(Expense expense)
        {
            var response = await HttpClient.PutAsJsonAsync<Expense>($"api/Expenses/{expense.Id}", expense);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var toUpdate = ExpenseList.First(c => c.Id == expense.Id);
            toUpdate = expense;

            await IJSRuntime.InvokeVoidAsync("CloseModal", "#editExpenseModal");
            return true;
        }

        public async void UpdateExpenseFile(Expense expense, IBrowserFile file)
        {
            expense.File = new DatabaseFile();
            expense.File.Name = file.Name;
            expense.File.UploadDate = DateTime.Now;

            var stream = file.OpenReadStream(file.Size);
            MemoryStream ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            expense.File.Data = ms.ToArray();


            var response = await HttpClient.PutAsJsonAsync<Expense>($"api/Expenses/{expense.Id}", expense);


            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                var newExpense = JsonConvert.DeserializeObject<Expense>(stringResponse);
                expense.DatabaseFileId = newExpense.DatabaseFileId;
                UpdateUI();
            }
            else
            {
                await IJSRuntime.InvokeAsync<object>("alert", "Error updating expense.");
            }
        }
    }
}
