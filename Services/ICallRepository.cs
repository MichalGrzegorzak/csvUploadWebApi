namespace csvUploadServices;

public interface ICallRepository
{
    public string LetsTest();
}

public class CallRepository : ICallRepository
{
    public string LetsTest()
    {
        return "success";
    }
}