using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform panelRect; 
    [SerializeField] private float duracion = 0.3f;

    public void Expandir()
    {
        Vector2 tamañoFinal = new Vector2(270f, 350f); 
        panelRect.DOSizeDelta(tamañoFinal, duracion)
                 .SetEase(Ease.OutQuad); // Easing suave (opcional: InOutQuad, etc.)
    }

    public void Contraer()
    {
        Vector2 tamañoOriginal = new Vector2(270f, 260f);
        panelRect.DOSizeDelta(tamañoOriginal, duracion)
                 .SetEase(Ease.InQuad);
    }
}
