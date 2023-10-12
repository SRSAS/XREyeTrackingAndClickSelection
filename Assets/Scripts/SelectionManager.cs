using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Class to manage the interaction between selectable targets and the user input, as well as to differentiate between the many targets
public class SelectionManager : MonoBehaviour
{
    // Selectors
    [SerializeField]
    private EyeHoverer dominantEye;
    [SerializeField]
    private EyeHoverer nonDominantEye;

    [SerializeField]
    private SelectConfirmer confirmer;

    public bool hovering = false;
    public GameObject hovered = null;
    public GameObject lastSelected = null;
    
    public float selectionDisplayTime = 1f;

    public Color defaultColor = Color.white;
    public Color hoverColor = Color.blue;
    public Color selectColor = Color.green;

    public UnityEvent onClick = new();

    // Start is called before the first frame update
    void Start()
    {
        if (confirmer == null)
            confirmer = GetComponent<SelectConfirmer>();

    }

    // Update is called once per frame
    void Update()
    {
        bool clicked = false;
        lock(confirmer.obj)
        {
            clicked = confirmer.selected;
        }
        if (clicked)
        {
            lastSelected = hovered;
            onClick.Invoke();
            Unhover();
        }
        else
        {
            Unhover();
            if (dominantEye.hitSomething)
                Hover(dominantEye.hitObject);
            else if (nonDominantEye.hitSomething)
                Hover(nonDominantEye.hitObject);
        }
    }

    void Hover(GameObject g)
    {
        Selectable s = g.GetComponent<Selectable>();
        if (s != null)
        {
            s.Hover();
            hovering = true;
            hovered = g;
        }
    }
    
    void Unhover()
    {
        if (hovering)
        {
            hovering = false;
            hovered.GetComponent<Selectable>().Unhover();
            hovered = null;
        }
    }
}
