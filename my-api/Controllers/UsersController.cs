using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using my_api.Models;
using Newtonsoft.Json.Linq;

namespace my_api.Controllers
{
    public class UsersController : ApiController
    {        
        // GET api/users        
        public string jsonFile = HttpContext.Current.Server.MapPath(@"~/App_Data/users.json");
        public string tokenFile = HttpContext.Current.Server.MapPath(@"~/App_Data/token.json");

        public IEnumerable<User> Get()
        {
            var json = File.ReadAllText(jsonFile);
            var userList = new List<User>();
            try
            {
                var jObject = JObject.Parse(json);

                if (jObject != null)
                {
                    JArray users = (JArray)jObject["users"];
                    if (users != null)
                    {
                        foreach (var item in users)
                        {
                            var us = new User();
                            us.id = (int)item["id"];
                            us.email = (string)item["email"];
                            us.username = (string)item["username"];
                            us.password = (string)item["password"];
                            userList.Add(us);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return userList;
        }

        // Post api/GetToken/
        [HttpPost]
        public Token GetToken([FromBody]User User)
        {           
            var json = File.ReadAllText(jsonFile);            
            try
            {
                var jObject = JObject.Parse(json);

                if (jObject != null)
                {
                    JArray users = (JArray)jObject["users"];
                    if (users != null)
                    {
                        foreach (var item in users)
                        {
                            if ((string)item["username"] == User.username && (string)item["password"] == User.password)
                            {
                                var tok = new Token();
                                tok.username = User.username;
                                tok.token = Guid.NewGuid().ToString();
                                var newToken = "{ 'token': " + tok.token + ",  'username': '" + tok.username + "'}";
                                var tokenJson = File.ReadAllText(tokenFile);
                                var jsonObj = JObject.Parse(tokenJson);
                                var tokens = jsonObj.GetValue("tokens") as JArray;
                                tokens.Add(newToken);
                                jsonObj["tokens"] = users;
                                string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj,
                                                       Newtonsoft.Json.Formatting.Indented);
                                File.WriteAllText(jsonFile, newJsonResult);

                                return tok;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return null;
        }

        // Post api/GetToken/
        [HttpPost]
        public bool ValidateToken([FromBody]Token token)
        {
            var json = File.ReadAllText(tokenFile);            
            try
            {
                var jObject = JObject.Parse(json);

                if (jObject != null)
                {
                    JArray tokens = (JArray)jObject["tokens"];
                    if (tokens != null)
                    {
                        foreach (var item in tokens)
                        {
                            if ((string)item["token"] == token.token && (string)item["username"] == token.username)
                            {                                
                                return true;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return false;
        }

        // GET api/users/5
        public User Get(int id)
        {
            var json = File.ReadAllText(jsonFile);
            var userList = new List<User>();
            try
            {
                var jObject = JObject.Parse(json);

                if (jObject != null)
                {
                    JArray users = (JArray)jObject["users"];
                    if (users != null)
                    {
                        foreach (var item in users)
                        {
                            if ((int)item["id"] == id)
                            {
                                var us = new User();
                                us.id = (int)item["id"];
                                us.email = (string)item["email"];
                                us.username = (string)item["username"];
                                us.password = (string)item["password"];
                                return us;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return null;
        }

        // POST api/users
        public void Post([FromBody]User User)
        {
            var newUser = "{ 'id': " + User.id + ",  'email': '" + User.email + "',  'username': '" + User.username + "', 'password': '" + User.password + "'}";
            try
            {
                var json = File.ReadAllText(jsonFile);
                var jsonObj = JObject.Parse(json);
                var users = jsonObj.GetValue("users") as JArray;
                users.Add(newUser);
                jsonObj["users"] = users;
                string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj,
                                       Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jsonFile, newJsonResult);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        // PUT api/users/5
        public void Put(int id, [FromBody]User User)
        {
            string json = File.ReadAllText(jsonFile);

            try
            {
                var jObject = JObject.Parse(json);
                JArray users = (JArray)jObject["users"];

                foreach (var user in users.Where(obj => obj["id"].Value<int>() == id))
                {
                    user["username"] = !string.IsNullOrEmpty(User.username) ? User.username : "";
                    user["email"] = !string.IsNullOrEmpty(User.email) ? User.email : "";
                    user["password"] = !string.IsNullOrEmpty(User.password) ? User.password : "";
                }

                jObject["users"] = users;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jsonFile, output);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // DELETE api/users/5
        public void Delete(int id)
        {
            var json = File.ReadAllText(jsonFile);
            try
            {
                var jObject = JObject.Parse(json);
                JArray users = (JArray)jObject["users"];
                var user = users.FirstOrDefault(obj => obj["id"].Value<int>() == id);

                users.Remove(user);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jsonFile, output);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
