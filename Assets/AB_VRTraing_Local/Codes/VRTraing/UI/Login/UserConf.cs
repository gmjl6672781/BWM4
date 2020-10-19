namespace BeinLab.Conf
{
    public class UserConf
    {
        private string userName;
        private string passWord;
        private string idNum;
        private string showName;
        private string address;
        private string token;
        public string UserName { get => userName; set => userName = value; }
        public string PassWord { get => passWord; set => passWord = value; }
        public string IdNum { get => idNum; set => idNum = value; }
        public string ShowName { get => showName; set => showName = value; }
        public string Address { get => address; set => address = value; }
        public string Token { get => token; set => token = value; }
    }
}