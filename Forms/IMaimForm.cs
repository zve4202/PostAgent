namespace PostAgent.forms
{
    public interface IMaimForm
    {
        void BeginSending();
        void FinishSending();
        void ShowMessage(string msgType, string message);
    }
}