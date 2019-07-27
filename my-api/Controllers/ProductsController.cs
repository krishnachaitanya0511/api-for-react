using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using my_api.Models;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Linq;
using System;

namespace my_api.Controllers
{
    public class ProductsController : ApiController
    {
        // GET api/products        
        public string jsonFile = HttpContext.Current.Server.MapPath(@"~/App_Data/products.json");
        
        public IEnumerable<Product> Get()
        {
            var json = File.ReadAllText(jsonFile);
            var productList = new List<Product>();
            try
            {
                var jObject = JObject.Parse(json);

                if (jObject != null)
                {
                    JArray products = (JArray)jObject["products"];
                    if (products != null)
                    {
                        foreach (var item in products)
                        {
                            var prod = new Product();
                            prod.id = (int)item["id"];
                            prod.product = (string)item;
                            prod.description = (string)item["description"];
                            prod.adder = (string)item["adder"];
                            productList.Add(prod);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return productList;
        }

        // GET api/products/5
        public Product Get(int id)
        {
            var json = File.ReadAllText(jsonFile);
            var productList = new List<Product>();
            try
            {
                var jObject = JObject.Parse(json);

                if (jObject != null)
                {
                    JArray products = (JArray)jObject["products"];
                    if (products != null)
                    {
                        foreach (var item in products)
                        {
                            if ((int)item["id"] == id)
                            {
                                var prod = new Product();
                                prod.id = (int)item["id"];
                                prod.product = (string)item["product"];
                                prod.description = (string)item["description"];
                                prod.adder = (string)item["adder"];
                                return prod;
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

        // POST api/products
        public void Post([FromBody]Product Product)
        {
            var newProduct = "{ 'id': " + Product.id + ",  'description': '" + Product.description + "',  'adder': '" + Product.adder + "', 'product': '" + Product.product + "'}";
            try
            {
                var json = File.ReadAllText(jsonFile);
                var jsonObj = JObject.Parse(json);
                var products = jsonObj.GetValue("products") as JArray;
                products.Add(newProduct);
                jsonObj["products"] = products;
                string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj,
                                       Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jsonFile, newJsonResult);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        // PUT api/products/5
        public void Put(int id, [FromBody]Product Product)
        {
            string json = File.ReadAllText(jsonFile);

            try
            {
                var jObject = JObject.Parse(json);
                JArray products = (JArray)jObject["products"];

                foreach (var product in products.Where(obj => obj["id"].Value<int>() == id))
                {
                    product["product"] = !string.IsNullOrEmpty(Product.product) ? Product.product : "";
                    product["adder"] = !string.IsNullOrEmpty(Product.adder) ? Product.adder : "";
                    product["description"] = !string.IsNullOrEmpty(Product.description) ? Product.description : "";
                }

                jObject["products"] = products;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jsonFile, output);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // DELETE api/products/5
        public void Delete(int id)
        {
            var json = File.ReadAllText(jsonFile);
            try
            {
                var jObject = JObject.Parse(json);
                JArray products = (JArray)jObject["products"];
                var product = products.FirstOrDefault(obj => obj["id"].Value<int>() == id);

                products.Remove(product);

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
