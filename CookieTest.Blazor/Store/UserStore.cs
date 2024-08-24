using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace CookieTest.Blazor.Store
{
    public class UserStore
    {
        private readonly ILocalStorageService _localStorage;
        private const string UserKey = "isAuth";

        public UserStore(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<bool> GetUserAsync()
        {
            return await _localStorage.GetItemAsync<bool>(UserKey); ;
        }

        public async Task SaveUserAsync(bool isUser)
        {
            await _localStorage.SetItemAsync(UserKey, isUser);
        }

        public async Task ClearUserAsync()
        {
            await _localStorage.RemoveItemAsync(UserKey);
        }
    }
}
