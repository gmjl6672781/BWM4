using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBGSize : MonoBehaviour
{
    public RectTransform scrollView;
    public RectTransform grid;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Set());
    }

    IEnumerator Set()
    {
        while (grid.rect.width==0|| grid.rect.height == 0)
            yield return 0;
        GetComponent<RectTransform>().sizeDelta = scrollView.sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
