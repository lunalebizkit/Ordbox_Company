
namespace Ordbox.Services.Common
{
    public class IdResponse<T>
    {
        public IdResponse() { }
        public IdResponse(T id)
        {
            Id = id;
        }
        public T Id { get; set; }

        public string IdStr { get => Id.ToString();}

        public bool Isnew()=> object.Equals(Id, default(T));
    }
}
