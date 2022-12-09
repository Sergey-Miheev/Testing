public class Requests
{
    private string _baseUrl = "http://shop.qatl.ru/";
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
            response = await httpClient.GetAsync($"{_baseUrl}api/products");
        }
        catch { }    
        return response;
    }
    public async Task<HttpResponseMessage?> CreateProduct(JsonContent content)
    {
        HttpResponseMessage? response = null;        
        try
        {
            response = await httpClient.PostAsync($"{_baseUrl}api/addproduct", content);
        }
        catch { }
        return response;
    }
    public async Task<HttpResponseMessage?> DeleteProduct(string idProduct)
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await httpClient.DeleteAsync($"{_baseUrl}api/deleteproduct?id={idProduct}");
        }
        catch { }
        return response;
    }
    public async Task<HttpResponseMessage?> UpdateProduct(JsonContent content)
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await httpClient.PostAsync($"{_baseUrl}api/editproduct", content);
        }
        catch { }
        return response;
    }
}