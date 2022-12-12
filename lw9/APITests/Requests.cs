public class Requests
{
    private string _getAllProductsUrl = "http://shop.qatl.ru/api/products";
    private string _createProductUrl = "http://shop.qatl.ru/api/addproduct";
    private string _deleteProductUrl = "http://shop.qatl.ru/api/deleteproduct?id=";
    private string _editProductUrl = "http://shop.qatl.ru/api/editproduct";

    static HttpClient httpClient = new HttpClient();
    public Requests()
    {
        httpClient = new HttpClient();
    }
    public async Task<HttpResponseMessage?> GetAllProducts()
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await httpClient.GetAsync(_getAllProductsUrl);
        }
        catch { }    
        return response;
    }
    public async Task<HttpResponseMessage?> CreateProduct(JsonContent content)
    {
        HttpResponseMessage? response = null;        
        try
        {
            response = await httpClient.PostAsync(_createProductUrl, content);
        }
        catch { }
        return response;
    }
    public async Task<HttpResponseMessage?> CreateProduct(StringContent content)
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await httpClient.PostAsync(_createProductUrl, content);
        }
        catch { }
        return response;
    }
    public async Task<HttpResponseMessage?> DeleteProduct(string idProduct)
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await httpClient.DeleteAsync($"{_deleteProductUrl}{idProduct}");
        }
        catch { }
        return response;
    }
    public async Task<HttpResponseMessage?> UpdateProduct(JsonContent content)
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await httpClient.PostAsync(_editProductUrl, content);
        }
        catch { }
        return response;
    }
    public async Task<HttpResponseMessage?> UpdateProduct(StringContent content)
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await httpClient.PostAsync(_editProductUrl, content);
        }
        catch { }
        return response;
    }
}