namespace APITests
{
    public class APITests
    {
        private Requests requests = new Requests();
        private List<string> idProducts;
        private HttpResponseMessage? response;
        public APITests()
        {
            requests = new Requests();
            idProducts = new List<string>();
            response = new HttpResponseMessage();
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
        [Fact]
        async void CheckExistenceOfField_AllProducts_Failed()
        {
            response = requests.GetAllProducts().Result;            
            string? content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<JsonNode>>(content);

            Assert.True(response.IsSuccessStatusCode, "Ошибка сервера");
            //Проверяем, что все продукты содержат нужные поля
            foreach (var product in result)
            {
                CheckExistenceOfFields(product);
            }
        }
        [Fact]
        async void CreateValidProduct_Success()
        {
            response = requests.CreateProduct(TestData.validProduct).Result;

            Assert.True(response.IsSuccessStatusCode, "Ошибка сервера, продукт не создался");

            JsonNode? node = await response.Content.ReadFromJsonAsync<JsonNode>();
            response = requests.GetAllProducts().Result;
            string? content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<JsonNode>>(content);
            JsonNode? finded = result.Find(shopProduct => shopProduct["id"].ToString() == node["id"].ToString());

            // Проверяем равен ли полученный продукт отправленному
            CompareProducts(finded, JsonNode.Parse(TestData.validProductString));

            await requests.DeleteProduct(node["id"].ToString());          
        }
        [Fact]
        async void CreateInvalidProduct_ProductNotCreated()
        {
            response = requests.CreateProduct(TestData.invalidProduct).Result;
            string? contentType = response.Content.Headers.ContentType.ToString();

            //Если проверка не пройдена, значит вернулся id созданного продукта
            Assert.True("application/json" != contentType, "Сервер вернул id созданного продукта");
        }
        [Fact]
        async void CreateEmptyProduct_ProductNotCreated()
        {
            response = requests.CreateProduct(TestData.emptyProduct).Result;
            string? contentType = response.Content.Headers.ContentType.ToString();

            //Eсли проверка не пройдена, значит вернулся json с id созданного продукта
            Assert.True("application/json" != contentType, "Создался пустой продукт");
        }      
        [Fact]
        async void UpdateProduct_Success()
        {
            response = requests.CreateProduct(TestData.validProduct).Result;
            JsonNode? node = await response.Content.ReadFromJsonAsync<JsonNode>();
            response = requests.GetAllProducts().Result;
            string? content = await response.Content.ReadAsStringAsync();
            List<JsonNode>? result = JsonSerializer.Deserialize<List<JsonNode>>(content);
            JsonNode? finded = result.Find(shopProduct => shopProduct["id"].ToString() == node["id"].ToString());
            JsonNode? updateProduct = TestData.updatedProductNode;
            updateProduct["id"] = finded["id"].ToString();

            response = requests.UpdateProduct(TestData.ToJsonContent(updateProduct)).Result;

            Assert.True(response.IsSuccessStatusCode, "Продукт не обновился, ошибка сервера");

            response = requests.GetAllProducts().Result;
            content = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<List<JsonNode>>(content);
            JsonNode? afterUpdate = result.Find(shopProduct => shopProduct["id"].ToString() == updateProduct["id"].ToString());

            //Сравниваем отправленный продукт и полученный от сервера
            CompareProducts(afterUpdate, updateProduct);

            await requests.DeleteProduct(node["id"].ToString());
        }
        [Fact]
        async void UpdateWithInvalidProduct_ProductNotUpdate()
        {
            response = requests.CreateProduct(TestData.validProduct).Result;
            JsonNode? node = await response.Content.ReadFromJsonAsync<JsonNode>();
            response = requests.GetAllProducts().Result;
            string? content = await response.Content.ReadAsStringAsync();
            List<JsonNode>? result = JsonSerializer.Deserialize<List<JsonNode>>(content);
            JsonNode? finded = result.Find(shopProduct => shopProduct["id"].ToString() == node["id"].ToString());
            JsonNode? updateProduct = JsonNode.Parse(TestData.invalidProductString);
            updateProduct["id"] = finded["id"].ToString();

            response = requests.UpdateProduct(TestData.ToJsonContent(updateProduct)).Result;
            string? contentType = response.Content.Headers.ContentType.ToString();

            //Eсли проверка не пройдена, значит вернулся json с id обновлённого продукта
            Assert.True("application/json" != contentType, "Сервер вернул id обновлённого невалидными значениями продукта");

            await requests.DeleteProduct(node["id"].ToString());
        }
        [Fact]
        async void UpdateWithEmptyProduct_ProductNotUpdate()
        {
            response = requests.CreateProduct(TestData.validProduct).Result;
            JsonNode? node = await response.Content.ReadFromJsonAsync<JsonNode>();
            response = requests.GetAllProducts().Result;
            string? content = await response.Content.ReadAsStringAsync();
            List<JsonNode>? result = JsonSerializer.Deserialize<List<JsonNode>>(content);
            JsonNode? finded = result.Find(shopProduct => shopProduct["id"].ToString() == node["id"].ToString());
            JsonNode? emptyProduct = JsonNode.Parse(TestData.emptyProductString);

            response = requests.UpdateProduct(TestData.ToJsonContent(emptyProduct)).Result;
            JsonObject? resp = await response.Content.ReadFromJsonAsync<JsonObject>();

            Assert.True(resp["status"].ToString() == "0", "Продукт обновился пустым объектом");

            await requests.DeleteProduct(node["id"].ToString());
        }
        [Fact]
        async void UpdateWithNonExistenProduct_ProductNotUpdate()
        {
            response = requests.UpdateProduct(TestData.nonexistenProduct).Result;
            JsonObject? resp = await response.Content.ReadFromJsonAsync<JsonObject>();

            Assert.True(resp["status"].ToString() == "0", "Продукт обновился несуществующим объектом");
        }
        [Fact]
        async void DeleteProduct_Success()
        {
            response = requests.CreateProduct(TestData.validProduct).Result;
            JsonNode? node = await response.Content.ReadFromJsonAsync<JsonNode>();

            response = requests.DeleteProduct(node["id"].ToString()).Result;

            Assert.True(response.IsSuccessStatusCode, "Продукт не удалился, ошибка сервера");

            response = requests.GetAllProducts().Result;
            string? content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<JsonNode>>(content);

            JsonNode? finded = result.Find(shopProduct => shopProduct["id"].ToString() == node["id"].ToString());

            Assert.True(finded == null, "Продукт найден, соответственно не удалился");
        }
        [Fact]
        async void DeleteNonExistenProduct_ProductNotDelete()
        {
            response = requests.DeleteProduct(TestData.id).Result;
            JsonObject? resp = await response.Content.ReadFromJsonAsync<JsonObject>();

            Assert.True(resp["status"].ToString() == "0", "Удалился существующий продукт");
        }
        [Fact]
        async void GeneratingTheAliasField_Success()
        {
            JsonObject? node;
            //Создаём 3 продукта
            for (int i = 0; i < 3; i++)
            {
                response = requests.CreateProduct(TestData.validProduct).Result;
                node = await response.Content.ReadFromJsonAsync<JsonObject>();
                idProducts.Add(node["id"].ToString());
            }        
            //Получаем эти 3 продукта от сервера
            response = requests.GetAllProducts().Result;
            List<JsonObject>? content = await response.Content.ReadFromJsonAsync<List<JsonObject>>();
            JsonObject? firstProduct = content.Find(shopProduct => shopProduct["id"].ToString() == idProducts[0].ToString());
            JsonObject? secondProduct = content.Find(shopProduct => shopProduct["id"].ToString() == idProducts[1].ToString());
            JsonObject? thirdProduct = content.Find(shopProduct => shopProduct["id"].ToString() == idProducts[2].ToString());

            Assert.True(firstProduct["alias"].ToString() == "clickbait", "Поле alias сгенерировано неправильно");
            Assert.True(secondProduct["alias"].ToString() == "clickbait-0", "Поле alias сгенерировано неправильно");
            Assert.True(thirdProduct["alias"].ToString() == "clickbait-0-0", "Поле alias сгенерировано неправильно");

            foreach (var id in idProducts)
            {
                await requests.DeleteProduct(id);
            }          
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
                response = requests.CreateProduct(TestData.validProduct).Result;
                node = await response.Content.ReadFromJsonAsync<JsonNode>();
                idProducts.Add(node["id"].ToString());
            }
            //Обновляем эти 3 продукта
            response = requests.GetAllProducts().Result;
            List<JsonNode>? content = await response.Content.ReadFromJsonAsync<List<JsonNode>>();
            JsonNode? firstProduct = content.Find(shopProduct => shopProduct["id"].ToString() == idProducts[0].ToString());            
            firstUpdatedProduct["id"] = firstProduct["id"].ToString();
            await requests.UpdateProduct(TestData.ToJsonContent(firstUpdatedProduct));
            JsonNode? secondProduct = content.Find(shopProduct => shopProduct["id"].ToString() == idProducts[1].ToString());            
            secondUpdatedProduct["id"] = secondProduct["id"].ToString();
            await requests.UpdateProduct(TestData.ToJsonContent(secondUpdatedProduct));
            JsonNode? thirdProduct = content.Find(shopProduct => shopProduct["id"].ToString() == idProducts[2].ToString());           
            thirdUpdatedProduct["id"] = thirdProduct["id"].ToString();
            await requests.UpdateProduct(TestData.ToJsonContent(thirdUpdatedProduct));

            //Получаем эти 3 обновлённых продукта от сервера
            response = requests.GetAllProducts().Result;
            content = await response.Content.ReadFromJsonAsync<List<JsonNode>>();
            firstUpdatedProduct = content.Find(shopProduct => shopProduct["id"].ToString() == idProducts[0].ToString());
            secondUpdatedProduct = content.Find(shopProduct => shopProduct["id"].ToString() == idProducts[1].ToString());
            thirdUpdatedProduct = content.Find(shopProduct => shopProduct["id"].ToString() == idProducts[2].ToString());

            Assert.True(firstUpdatedProduct["alias"].ToString() == "ggwp", "Поле alias сгенерировано неправильно");
            Assert.True(secondUpdatedProduct["alias"].ToString() == $"ggwp-{secondUpdatedProduct["id"]}", "Поле alias сгенерировано неправильно");
            Assert.True(thirdUpdatedProduct["alias"].ToString() == $"ggwp-{thirdUpdatedProduct["id"]}", "Поле alias сгенерировано неправильно");

            foreach (var id in idProducts)
            {
                await requests.DeleteProduct(id);
            }
        }
    }
}