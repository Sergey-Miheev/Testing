using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Json;

namespace APITests
{
    public class APITests : IDisposable
    {
        private List<string> _idProducts;
        private Requests _requests;
        private HttpResponseMessage? _response;
        private TestData _data;
        public APITests()
        {
            _data = new TestData();
            _idProducts = new List<string>();
            _requests = new Requests();
            _response = new HttpResponseMessage();
        }
        public async void Dispose()
        {
            foreach (var id in _idProducts)
            {
                await _requests.DeleteProduct(id);
            }
        }
        private void CheckExistenceOfFields(JsonNode product)
        {
            Assert.True(product["id"] != null, "поле id отсутствует");
            Assert.True(product["category_id"] != null, "поле category_id отсутствует");
            Assert.True(product["title"] != null, "поле title отсутствует");
            Assert.True(product["alias"] != null, "поле alias отсутствует");
            Assert.True(product["content"] != null, "поле content отсутствует");
            Assert.True(product["price"] != null, "поле price отсутствует");
            Assert.True(product["old_price"] != null, "поле old_price отсутствует");
            Assert.True(product["status"] != null, "поле status отсутствует");
            Assert.True(product["keywords"] != null, "поле keywords отсутствует");
            Assert.True(product["description"] != null, "поле description отсутствует");
            Assert.True(product["img"] != null, "поле img отсутствует");
            Assert.True(product["hit"] != null, "поле hit отсутствует");
            Assert.True(product["cat"] != null, "поле cat отсутствует");
        }
        private void CompareProducts(JsonNode source, JsonNode response)
        {
            Assert.True(source["category_id"].ToString() == response["category_id"].ToString(), "category_id исходного товара не совпадает с category_id сервера");
            Assert.True(source["title"].ToString() == response["title"].ToString(), "title исходного товара не совпадает с title сервера");
            Assert.True(source["content"].ToString() == response["content"].ToString(), "content исходного товара не совпадает с content сервера");
            Assert.True(source["price"].ToString() == response["price"].ToString(), "price исходного товара не совпадает с price сервера");
            Assert.True(source["old_price"].ToString() == response["old_price"].ToString(), "old_price исходного товара не совпадает с old_price сервера");
            Assert.True(source["status"].ToString() == response["status"].ToString(), "status исходного товара не совпадает с status сервера");
            Assert.True(source["keywords"].ToString() == response["keywords"].ToString(), "keywords исходного товара не совпадает с keywords сервера");
            Assert.True(source["description"].ToString() == response["description"].ToString(), "description исходного товара не совпадает с description сервера");
            Assert.True(source["hit"].ToString() == response["hit"].ToString(), "hit исходного товара не совпадает с hit сервера");
        }
        private async Task<JsonNode> PreparationBeforeUpdateProduct(JsonNode product)
        {
            _response = _requests.CreateProduct(TestData.validProduct).Result;
            JsonNode? node = await _response.Content.ReadFromJsonAsync<JsonNode>();
            _idProducts.Add(node["id"].ToString());
            _response = _requests.GetAllProducts().Result;
            List<JsonNode>? result = await _response.Content.ReadFromJsonAsync<List<JsonNode>>();
            JsonNode? finded = result.Find(shopProduct => shopProduct["id"].ToString() == node["id"].ToString());
            JsonNode? updateProduct = product;
            updateProduct["id"] = finded["id"].ToString();
            _response = _requests.UpdateProduct(TestData.ToJsonContent(updateProduct)).Result;
            return updateProduct;
        }
        [Fact]
        async void CheckExistenceOfField_AllProducts_Failed()
        {
            _response = _requests.GetAllProducts().Result;
            string? content = await _response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<JsonNode>>(content);

            Assert.True(_response.IsSuccessStatusCode, "Ошибка сервера");
            //Проверяем, что все продукты содержат нужные поля
            foreach (var product in result)
            {
                CheckExistenceOfFields(product);
            }
        }
        [Fact]
        async void CreateValidProduct_Success()
        {
            _response = _requests.CreateProduct(TestData.validProduct).Result;

            Assert.True(_response.IsSuccessStatusCode, "Ошибка сервера, продукт не создался");

            JsonNode? node = await _response.Content.ReadFromJsonAsync<JsonNode>();
            _idProducts.Add(node["id"].ToString());
            _response = _requests.GetAllProducts().Result;
            string? content = await _response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<JsonNode>>(content);
            JsonNode? finded = result.Find(shopProduct => shopProduct["id"].ToString() == node["id"].ToString());

            // Проверяем равен ли полученный продукт отправленному
            CompareProducts(finded, JsonNode.Parse(TestData.validProductString));
        }
        [Fact]
        async void CreateInvalidProduct_ProductNotCreated()
        {
            _response = _requests.CreateProduct(TestData.invalidProduct).Result;
            string? contentType = _response.Content.Headers.ContentType.ToString();

            //Если проверка не пройдена, значит вернулся id созданного продукта
            Assert.True("application/json" != contentType, "Сервер вернул id созданного продукта");
        }
        [Theory]
        [InlineData(TestData.invalidCategoryIdProductString)]
        [InlineData(TestData.invalidStatusProductString)]
        [InlineData(TestData.invalidHitProductString)]
        async void CreateProductWithInvalidField_ProductNotCreated(string invalidProduct)
        {
            _response = _requests.CreateProduct(new StringContent(invalidProduct)).Result;
            JsonObject? content = await _response.Content.ReadFromJsonAsync<JsonObject>();
            _idProducts.Add(content["id"].ToString());

            //Если проверка не пройдена, значит вернулся id созданного продукта
            Assert.True(content["id"] is null, "Сервер вернул id созданного продукта");
        }
        [Fact]
        async void CreateEmptyProduct_ProductNotCreated()
        {
            _response = _requests.CreateProduct(TestData.emptyProduct).Result;
            string? contentType = _response.Content.Headers.ContentType.ToString();

            //Eсли проверка не пройдена, значит вернулся json с id созданного продукта
            Assert.True("application/json" != contentType, "Создался пустой продукт");
        }
        [Fact]
        async void UpdateProduct_Success()
        {
            var updateProduct = PreparationBeforeUpdateProduct(TestData.updatedProductNode).Result;

            Assert.True(_response.IsSuccessStatusCode, "Продукт не обновился, ошибка сервера");

            _response = _requests.GetAllProducts().Result;
            string? content = await _response.Content.ReadAsStringAsync();
            List<JsonNode>? result = JsonSerializer.Deserialize<List<JsonNode>>(content);
            JsonNode? afterUpdate = result.Find(shopProduct => shopProduct["id"].ToString() == updateProduct["id"].ToString());

            //Сравниваем отправленный продукт и полученный от сервера
            CompareProducts(afterUpdate, updateProduct);
        }
        [Fact]
        async void UpdateWithInvalidProduct_ProductNotUpdate()
        {
            var updateProduct = PreparationBeforeUpdateProduct(TestData.invalidProductNode).Result;

            string? contentType = _response.Content.Headers.ContentType.ToString();

            //Eсли проверка не пройдена, значит вернулся json с id обновлённого продукта
            Assert.True("application/json" != contentType, "Сервер вернул id обновлённого невалидными значениями продукта");
        }
        [Theory]
        [InlineData(TestData.invalidCategoryIdProductString)]
        [InlineData(TestData.invalidStatusProductString)]
        [InlineData(TestData.invalidHitProductString)]
        async void UpdateProductWithInvalidField_ProductNotUpdate(string invalidProduct)
        {
            var productNode = JsonNode.Parse(invalidProduct);
            var updateProduct = PreparationBeforeUpdateProduct(productNode).Result;

            JsonNode? content = await _response.Content.ReadFromJsonAsync<JsonNode>();

            //Если проверка не пройдена, значит вернулся id созданного продукта
            Assert.True(content["id"] is null, "Сервер вернул id обновлённого продукта");
        }        
        [Fact]
        async void UpdateWithEmptyProduct_ProductNotUpdate()
        {
            _response = _requests.CreateProduct(TestData.validProduct).Result;
            JsonNode? node = await _response.Content.ReadFromJsonAsync<JsonNode>();
            _idProducts.Add(node["id"].ToString());
            _response = _requests.GetAllProducts().Result;
            List<JsonNode>? result = await _response.Content.ReadFromJsonAsync<List<JsonNode>>();
            JsonNode? finded = result.Find(shopProduct => shopProduct["id"].ToString() == node["id"].ToString());
            JsonNode? emptyProduct = JsonNode.Parse(TestData.emptyProductString);

            _response = _requests.UpdateProduct(TestData.ToJsonContent(emptyProduct)).Result;
            JsonObject? resp = await _response.Content.ReadFromJsonAsync<JsonObject>();

            Assert.True(resp["status"].ToString() == "0", "Продукт обновился пустым объектом");            
        }
        [Fact]
        async void UpdateWithNonExistenProduct_ProductNotUpdate()
        {
            _response = _requests.UpdateProduct(TestData.nonexistenProduct).Result;
            JsonObject? resp = await _response.Content.ReadFromJsonAsync<JsonObject>();

            Assert.True(resp["status"].ToString() == "0", "Продукт обновился несуществующим объектом");
        }
        [Fact]
        async void DeleteProduct_Success()
        {
            _response = _requests.CreateProduct(TestData.validProduct).Result;
            JsonNode? node = await _response.Content.ReadFromJsonAsync<JsonNode>();

            _response = _requests.DeleteProduct(node["id"].ToString()).Result;

            Assert.True(_response.IsSuccessStatusCode, "Продукт не удалился, ошибка сервера");

            _response = _requests.GetAllProducts().Result;
            string? content = await _response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<JsonNode>>(content);

            JsonNode? finded = result.Find(shopProduct => shopProduct["id"].ToString() == node["id"].ToString());

            Assert.True(finded is null, "Продукт найден, соответственно не удалился");
        }
        [Fact]
        async void DeleteNonExistenProduct_ProductNotDelete()
        {
            _response = _requests.DeleteProduct(TestData.id).Result;
            JsonObject? resp = await _response.Content.ReadFromJsonAsync<JsonObject>();

            Assert.True(resp["status"].ToString() == "0", "Удалился существующий продукт");
        }
        [Fact]
        async void GeneratingTheAliasField_Success()
        {
            JsonObject? node;
            //Создаём 3 продукта
            for (int i = 0; i < 3; i++)
            {
                _response = _requests.CreateProduct(TestData.validProduct).Result;
                node = await _response.Content.ReadFromJsonAsync<JsonObject>();
                _idProducts.Add(node["id"].ToString());
            }        
            //Получаем эти 3 продукта от сервера
            _response = _requests.GetAllProducts().Result;
            List<JsonObject>? content = await _response.Content.ReadFromJsonAsync<List<JsonObject>>();
            JsonObject? firstProduct = content.Find(shopProduct => shopProduct["id"].ToString() == _idProducts[0]);
            JsonObject? secondProduct = content.Find(shopProduct => shopProduct["id"].ToString() == _idProducts[1]);
            JsonObject? thirdProduct = content.Find(shopProduct => shopProduct["id"].ToString() == _idProducts[2]);

            Assert.True(firstProduct["alias"].ToString() == "simka", "Поле alias сгенерировано неправильно");
            Assert.True(secondProduct["alias"].ToString() == "simka-0", "Поле alias сгенерировано неправильно");
            Assert.True(thirdProduct["alias"].ToString() == "simka-0-0", "Поле alias сгенерировано неправильно");         
        }
        [Fact]
        async void GeneratingTheAliasField_UpdateProduct_Success()
        {
            JsonNode? node;
            JsonNode? firstUpdatedProduct = TestData.updatedProductNode;
            JsonNode? secondUpdatedProduct = TestData.updatedProductNode;
            JsonNode? thirdUpdatedProduct = TestData.updatedProductNode;
            //Создаём 3 продукта
            for (int i = 0; i < 3; i++)
            {
                _response = _requests.CreateProduct(TestData.validProduct).Result;
                node = await _response.Content.ReadFromJsonAsync<JsonNode>();
                _idProducts.Add(node["id"].ToString());
            }
            //Обновляем эти 3 продукта
            _response = _requests.GetAllProducts().Result;
            List<JsonNode>? content = await _response.Content.ReadFromJsonAsync<List<JsonNode>>();
            JsonNode? firstProduct = content.Find(shopProduct => shopProduct["id"].ToString() == _idProducts[0].ToString());            
            firstUpdatedProduct["id"] = firstProduct["id"].ToString();
            await _requests.UpdateProduct(TestData.ToJsonContent(firstUpdatedProduct));
            JsonNode? secondProduct = content.Find(shopProduct => shopProduct["id"].ToString() == _idProducts[1].ToString());            
            secondUpdatedProduct["id"] = secondProduct["id"].ToString();
            await _requests.UpdateProduct(TestData.ToJsonContent(secondUpdatedProduct));
            JsonNode? thirdProduct = content.Find(shopProduct => shopProduct["id"].ToString() == _idProducts[2].ToString());           
            thirdUpdatedProduct["id"] = thirdProduct["id"].ToString();
            await _requests.UpdateProduct(TestData.ToJsonContent(thirdUpdatedProduct));

            //Получаем эти 3 обновлённых продукта от сервера
            _response = _requests.GetAllProducts().Result;
            content = await _response.Content.ReadFromJsonAsync<List<JsonNode>>();
            firstUpdatedProduct = content.Find(shopProduct => shopProduct["id"].ToString() == _idProducts[0].ToString());
            secondUpdatedProduct = content.Find(shopProduct => shopProduct["id"].ToString() == _idProducts[1].ToString());
            thirdUpdatedProduct = content.Find(shopProduct => shopProduct["id"].ToString() == _idProducts[2].ToString());

            Assert.True(firstUpdatedProduct["alias"].ToString() == "ggwp", "Поле alias сгенерировано неправильно");
            Assert.True(secondUpdatedProduct["alias"].ToString() == $"ggwp-{secondUpdatedProduct["id"]}", "Поле alias сгенерировано неправильно");
            Assert.True(thirdUpdatedProduct["alias"].ToString() == $"ggwp-{thirdUpdatedProduct["id"]}", "Поле alias сгенерировано неправильно");
        }
    }
}