namespace BeinLab.Conf
{
    public class ResultConf
    {
        private string priKey;
        private string userCode;
        private int modelType;
        private string date;
        private string score;
        private string result;

        public string PriKey { get => priKey; set => priKey = value; }
        public string UserCode { get => userCode; set => userCode = value; }
        public int ModelType { get => modelType; set => modelType = value; }
        public string Date { get => date; set => date = value; }
        public string Score { get => score; set => score = value; }
        public string Result { get => result; set => result = value; }
    }
}