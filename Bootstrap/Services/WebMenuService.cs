using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Bootstrap
{
    public class WebMenuService : IWebMenuService
    {
        private readonly IHttpContextAccessor _accessor; // Dependency injection to access the Http context
        string userId;
        string userToken;
        string _menuString;

        bool _root = false;
        int _menuLevel = 0;
        int _sidenav = 0;


        public WebMenuService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }


        public async Task<string> GetWebMenu()
        {
            if (!string.IsNullOrEmpty(_accessor.HttpContext.User.Identity.Name))
            {
                var contextClaims = _accessor.HttpContext.User.Claims;

                // Extract UserId from User Claims
                userId = (contextClaims.SingleOrDefault(val => val.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
                // Extract User token from User Claims
                userToken = (contextClaims.SingleOrDefault(val => val.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication")).Value;
            }

            UserInfoModel _userInfoModel = new UserInfoModel();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            _userInfoModel.UserId = userId;
            //_userInfoModel.UserId = "ef5be01f-cc0d-4e69-b819-b77f8b2d94f3"; //user 1
            //_userInfoModel.UserId = "ecb0bd58-1b77-4db8-b291-d0c9a1fd10fe"; //user 2
            //_userInfoModel.UserId = "7c02fc06-cee9-4a14-937b-35d26dadd54d"; //user 3
            var jsonData = new StringContent(JsonConvert.SerializeObject(_userInfoModel), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("http://localhost:5000/api/getwebusermenu", jsonData);
            response.EnsureSuccessStatusCode();

            var responseData = response.Content.ReadAsStringAsync().Result; ;
            List<WebMenuModel> menu = JsonConvert.DeserializeObject<List<WebMenuModel>>(responseData).OrderBy(ord => ord.ItemId).ToList();
            _menuString = "";
            BuildMenu(menu);

            client.Dispose();

            return _menuString;
        }

        public void BuildMenu(List<WebMenuModel> fullMenu)
        {
            _menuLevel = 1;
            string _dashboardMenu = "<li class='nav-item'><a class='nav-link' href='/Home/Dashboard'><i class='fa fa-fw fa-dashboard'></i><span class='nav-link-text'>Dashboard</span></a></li>";
            _menuString = "<ul class='navbar-nav navbar-sidenav' id='exampleAccordion'>";
            _menuString = _menuString + _dashboardMenu;

            foreach (var menu in fullMenu)
            {
                if (menu.ParentId == 0)
                {
                    _root = true;
                    _menuLevel = _menuLevel + 1;
                    _sidenav = 0;
                    //_sidenav = 1;
                    SubMenu(fullMenu, menu);
                }
            }
            _menuString = _menuString + "</ul>";
        }

        public void SubMenu(List<WebMenuModel> fullMenu, WebMenuModel menu)
        {
            if (_root == true)
            {
                _menuString = _menuString + "<li class='nav-item'><a class='nav-link nav-link-collapse collapsed' data-toggle='collapse' href='#" + _menuLevel + "' data-parent='#exampleAccordion'><i class='fa fa-fw fa-sitemap'></i><span class='nav-link-text'>" + menu.DisplayName + "</span></a>";
		        _root = false; 
            }
            else
            {
                _menuString = _menuString + "<li><a class='nav-link-collapse collapsed' data-toggle='collapse' href='#" + _menuLevel + "'>" + menu.DisplayName + "</a>";
            }

            var subMenus = fullMenu.Where(p => p.ParentId == menu.ItemId);
            string[] sidenavArray = new string[] { "sidnav-first-level", "sidenav-second-level", "sidenav-third-level" };

            if (subMenus.Count() > 0)
            {
                _sidenav = _sidenav + 1;
                if (_sidenav < 3)
                {
                    _menuString = _menuString + "<ul class='" + sidenavArray[_sidenav] + " collapse' id = '" + _menuLevel + "'>";

                    foreach (WebMenuModel p in subMenus)
                    {
                        if (fullMenu.Count(x => x.ParentId == p.ItemId) > 0)
                        {
                            _menuLevel = _menuLevel + 1;
                            SubMenu(fullMenu, p);
                        }
                        else
                        {
                            _menuString = _menuString + "<li><a href='" + "/Home/Tables" + "'>" + p.DisplayName + "</a></li>";
                        }
                    }
                    _menuString = _menuString + "</ul>";
                }
            }
            _menuString = _menuString + "</li>";
        }
    }
}
