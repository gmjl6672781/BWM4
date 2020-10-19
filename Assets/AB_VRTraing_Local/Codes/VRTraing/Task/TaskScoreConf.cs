public class TaskScoreConf
{
    private string priKey;
    private float traingScore = 5;
    private float examScore = 10;
    public string PriKey { get => priKey; set => priKey = value; }
    public float TraingScore { get => traingScore; set => traingScore = value; }
    public float ExamScore { get => examScore; set => examScore = value; }
}
