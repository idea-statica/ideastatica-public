interface IKeyValueStorage 
{
    async Task Set(string key, byte[] value);
    async Task<byte[]?> Get(string key);
    async Task<bool> Delete(string key);
    async Task<bool> Exists(string key);
}