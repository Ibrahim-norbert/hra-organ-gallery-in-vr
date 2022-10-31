using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class Flicker : MonoBehaviour
{
    [SerializeField] private Image image;
    private void OnEnable()
    {
        SceneBuilder.OnSceneBuilt += () =>
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            Destroy(GetComponentInChildren<TMP_Text>().gameObject);
            Destroy(this.gameObject);
        };
    }

    private void Update()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, GetOpacity().Remap(-1, 1, 0, 1));
    }


    private float GetOpacity()
    {
        return Mathf.Sin(Time.time);
    }
}
