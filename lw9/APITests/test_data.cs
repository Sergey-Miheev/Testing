public class TestData
{
    static public JsonContent ToJsonContent(JsonNode node)
    {
        Object updateProductObject = JsonSerializer.Deserialize<Object>(node);
        JsonContent JsonContentObject = JsonContent.Create(updateProductObject);
        return JsonContentObject;
    }

    static public object validProductObject = new
    {
        category_id = "12",
        title = "simka",
        content = "ccc",
        price = "55",
        old_price = "23",
        status = "1",
        keywords = "yeah",
        description = "tell",
        hit = "0",
    };
    static public string validProductString = "{\"category_id\": \"12\", \"title\": \"simka\", \"content\": \"ccc\", \"price\": \"55\", \"old_price\": \"23\", \"status\": \"1\", \"keywords\": \"yeah\", \"description\": \"tell\", \"hit\": \"0\"}";
    static public JsonContent validProduct = JsonContent.Create(validProductObject);

    static public object validUpdatedProductObject = new
    {
        category_id = "2",
        title = "ggwp",
        content = "ggg",
        price = "300",
        old_price = "300",
        status = "0",
        keywords = "yeah",
        description = "silentium",
        hit = "1",
    };
    static public string validUpdatedProductString = "{\"id\": \"0\", \"category_id\": \"2\", \"title\": \"ggwp\", \"content\": \"ggg\", \"price\": \"300\", \"old_price\": \"300\", \"status\": \"0\", \"keywords\": \"yeah\", \"description\": \"silentium\", \"hit\": \"1\", \"cat\": \"h\", \"img\": \"no\"}";
    static public JsonContent updatedProduct = JsonContent.Create(validUpdatedProductObject);
    static public JsonNode updatedProductNode = JsonNode.Parse(validUpdatedProductString);

    static public object invalidProductObject = new
    {
        category_id = "300",
        title = 44,
        content = 228,
        price = "three hundred",
        old_price = "three hundred",
        status = "228",
        keywords = 100,
        description = 340,
        hit = "-1",
    };
    static public string invalidProductString = "{\"id\": \"0\", \"category_id\": \"300\", \"title\": 44, \"content\": 228, \"price\": \"three hundred\", \"old_price\": \"three hundred\", \"status\": \"228\", \"keywords\": 100, \"description\": 340, \"hit\": \"-1\"}";
    static public JsonContent invalidProduct = JsonContent.Create(invalidProductObject);

    static public object invalidCategoryIdProductObject = new
    {
        category_id = "16",
        title = "clickbait",
        content = "ccc",
        price = "55",
        old_price = "23",
        status = "1",
        keywords = "yeah",
        description = "tell",
        hit = "0",
    };
    static public string invalidCategoryIdProductString = "{\"category_id\": \"16\", \"title\": \"clickbait\", \"content\": \"ccc\", \"price\": \"55\", \"old_price\": \"23\", \"status\": \"1\", \"keywords\": \"yeah\", \"description\": \"tell\", \"hit\": \"0\"}";
    static public JsonContent invalidCategoryIdProductContent = JsonContent.Create(invalidCategoryIdProductObject);

    static public object invalidStatusProductObject = new
    {
        category_id = "12",
        title = "clickbait",
        content = "ccc",
        price = "55",
        old_price = "23",
        status = "2",
        keywords = "yeah",
        description = "tell",
        hit = "0",
    };
    static public string invalidStatusProductString = "{\"category_id\": \"12\", \"title\": \"clickbait\", \"content\": \"ccc\", \"price\": \"55\", \"old_price\": \"23\", \"status\": \"2\", \"keywords\": \"yeah\", \"description\": \"tell\", \"hit\": \"0\"}";
    static public JsonContent invalidStatusProductContent = JsonContent.Create(invalidStatusProductObject);

    static public object invalidHitProductObject = new
    {
        category_id = "12",
        title = "clickbait",
        content = "ccc",
        price = "55",
        old_price = "23",
        status = "1",
        keywords = "yeah",
        description = "tell",
        hit = "2",
    };
    static public string invalidHitProductString = "{\"category_id\": \"12\", \"title\": \"clickbait\", \"content\": \"ccc\", \"price\": \"55\", \"old_price\": \"23\", \"status\": \"1\", \"keywords\": \"yeah\", \"description\": \"tell\", \"hit\": \"2\"}";
    static public JsonContent invalidHitProductContent = JsonContent.Create(invalidHitProductObject);

    static public object emptyProductObject = new
    {
    };
    static public string emptyProductString = "{}";
    static public JsonContent emptyProduct = JsonContent.Create(emptyProductObject);
  
    static public string id = "0";
    static public object nonexistenProductObject = new
    {
        id = id,
        category_id = "3",
        title = "gangsta shit",
        content = "no, haski",
        price = "500",
        old_price = "3",
        status = "0",
        keywords = "yeah",
        description = "chupapi",
        hit = "1",
    };
    static public string nonexistenProductString = "{\"id\": \"0\", \"category_id\": \"3\", \"title\": \"gangsta shit\", \"content\": \"no, haski\", \"price\": \"500\", \"old_price\": \"3\", \"status\": \"0\", \"keywords\": \"yeah\", \"description\": \"chupapi\", \"hit\": \"1\"}";
    static public JsonContent nonexistenProduct = JsonContent.Create(nonexistenProductObject);
}