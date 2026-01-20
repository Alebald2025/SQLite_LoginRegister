using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private RectTransform[] todosLosPaneles; 
    [SerializeField] private int panelInicial = 0; 
    [SerializeField] private float duracionFade = 0.3f; // Duración del fade in/out
    [SerializeField] private Ease easeIn = Ease.OutQuad; // Easing para entrada
    [SerializeField] private Ease easeOut = Ease.InQuad; // Easing para salida

    private CanvasGroup[] gruposPanel; // Para fade (se crea automáticamente)
    private Sequence secuenciaActual;

    void Start()
    {
        InicializarPaneles();
        MostrarPanel(panelInicial);
    }

    public void MostrarPanel(int indice)
    {
        if (indice < 0 || indice >= todosLosPaneles.Length) return;

        DOTween.Kill(secuenciaActual);
        secuenciaActual = DOTween.Sequence();

        // Fade-out todos los que estén activos (excepto el que queremos mostrar)
        for (int i = 0; i < todosLosPaneles.Length; i++)
        {
            if (i != indice && todosLosPaneles[i].gameObject.activeInHierarchy)
            {
                int idx = i; // capturamos
                secuenciaActual.Join(gruposPanel[idx].DOFade(0f, duracionFade).SetEase(easeOut)
                    .OnComplete(() => {
                        gruposPanel[idx].blocksRaycasts = false;
                        todosLosPaneles[idx].gameObject.SetActive(false);
                    }));
            }
        }

        // Fade-in el seleccionado
        int targetIdx = indice;
        todosLosPaneles[targetIdx].gameObject.SetActive(true);
        gruposPanel[targetIdx].alpha = 0f;
        gruposPanel[targetIdx].blocksRaycasts = false;

        secuenciaActual.Append(gruposPanel[targetIdx].DOFade(1f, duracionFade).SetEase(easeIn)
            .OnComplete(() => {
                gruposPanel[targetIdx].blocksRaycasts = true;
            }));
    }

    public void MostrarPanelPorNombre(string nombrePanel)
    {
        for (int i = 0; i < todosLosPaneles.Length; i++)
        {
            if (todosLosPaneles[i].name == nombrePanel)
            {
                MostrarPanel(i);
                return;
            }
        }
        Debug.LogWarning("Panel '" + nombrePanel + "' no encontrado!");
    }

    private void InicializarPaneles()
    {
        gruposPanel = new CanvasGroup[todosLosPaneles.Length];
        for (int i = 0; i < todosLosPaneles.Length; i++)
        {
            // Agregar CanvasGroup si no existe
            gruposPanel[i] = todosLosPaneles[i].GetComponent<CanvasGroup>();
            if (gruposPanel[i] == null)
                gruposPanel[i] = todosLosPaneles[i].gameObject.AddComponent<CanvasGroup>();

            // Estado inicial: Ocultos
            gruposPanel[i].alpha = 0f;
            gruposPanel[i].blocksRaycasts = false;
            todosLosPaneles[i].gameObject.SetActive(false);
        }
    }
}
