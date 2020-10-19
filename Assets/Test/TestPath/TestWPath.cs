using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using UnityEngine;

public class TestWPath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LanguagePackConf lpc = new LanguagePackConf();
        lpc.PriKey = Random.Range(1000,9999).ToString();
        string path = Application.dataPath;
        UnityUtil.WriteXMLData<LanguagePackConf>(path, lpc);
        var list = UnityUtil.ReadXMLData<LanguagePackConf>(path);
        if (list != null && list.Count > 0)
        {
            lpc = list[0];
            if (lpc != null)
            {
                Application.Quit();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
